using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

using BandMaster.Audio;
using BandMaster.Input;
namespace BandMaster.Logic
{


    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class BandMasterMode : Microsoft.Xna.Framework.GameComponent, IMode
    {
        private Midi.Player midiPlayer;
        private IManageInput inputManager;
        private Graphics.SplashText splasher; 
        
        public BandMasterMode(Game game)
            : base(game)
        {
            // TODO: Construct any child components here
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            midiPlayer = (Midi.Player)Game.Services.GetService(typeof(Midi.Player));
            inputManager = (IManageInput)Game.Services.GetService(typeof(IManageInput));
            splasher = (Graphics.SplashText)Game.Services.GetService(typeof(Graphics.SplashText));

            Enabled = false;

            ((BandMaster)Game).SongChanged += onSongChanged;
            ((BandMaster)Game).SongLoaded += onSongLoaded;

            for (int i = 0; i < PlayerDynamics.Length; i++)
                PlayerDynamics[i] = -1.0f;

                base.Initialize();
        }

        public void onSongChanged(object sender, EventArgs args)
        {
            splasher.Write("Laster..", Color.White);
            if (!Enabled) return;
            Enabled = false;
            midiPlayer.Play();
            tier.Start();
        }
        public void onSongLoaded(object sender, EventArgs args)
        {
            midiPlayer.Song = ((BandMaster)Game).Song.Midi;
            
            Helpers.Wait(6.0, delegate()
            {
                splasher.Write("3", Color.White);
                Helpers.Wait(1.0, delegate()
                {
                    splasher.Write("2", Color.White);
                    Helpers.Wait(1.0, delegate()
                    {
                        splasher.Write("1", Color.White);
                        Helpers.Wait(1.0, delegate()
                        {
                            splasher.Write("Start!", Color.White);
                            midiPlayer.Play();
                            tier.Start();
                            Enabled = true;
                        });
                    });
                });
            });
        }

        protected override void OnEnabledChanged(object sender, EventArgs args)
        {
            base.OnEnabledChanged(sender, args);

            if (Enabled)
            {
                inputManager.OnTempoHit += tempoHit;
                midiPlayer.Tick += onTick;
                midiPlayer.Tick += updateDynamicLine;
                midiPlayer.Play();
                tier.Start();                
            }
            else
            {
                inputManager.OnTempoHit -= tempoHit;
                midiPlayer.Tick -= onTick;
                midiPlayer.Tick -= updateDynamicLine;
                midiPlayer.Stop();
                tier.Stop();
            }
        
        }

        float lastHitTime = -1.0f;
        float currentHitTime = 0.0f;
        int ticksToNextHit = 960;

        void onTick(object sender, EventArgs e)
        {
            ticksToNextHit--;

            if (ticksToNextHit == 1)
                midiPlayer.Stop();
        }

        float lastTempo = 0.0f;
        Random rand = new Random();
        System.Diagnostics.Stopwatch tier = new System.Diagnostics.Stopwatch();

        /*string[] dynamicSplash = new string[] {
            "Uffda",
            "Ok",
            "Bra!",
            "Perfekt!"
        };*/


        public float[] PlayerDynamics = new float[800];
        public int PlayerDynamicsEnd = 0;

        public float GetCorrectDynamics(int time)
        {
            int[] parts = ((BandMaster)Game).Song.Lines[0];
            double done = Helpers.Clamp((float)((double)time / (double)midiPlayer.Length), 0.0f, 0.9999f); // prosent av sangen hvor vi er
            int currentPart = (int)Math.Floor(done * (double)parts.Length); // part-indexen for den parten vi er i
            double partLength = (double)midiPlayer.Length / (double)parts.Length; // hvor mange ticks har man per part
            double start = ((double)currentPart * partLength); // prosent av sangen hvor denne parten starter

            //int ticksPerPart = (960 * 4);
            int ticksPerPart = (int)Math.Floor((float)midiPlayer.Length / (float) parts.Length);
            int currentPartStartTicks = (int)Math.Floor((double)time / (double)ticksPerPart) * ticksPerPart;
            double segmentDone = (double)(time - currentPartStartTicks) / (double)ticksPerPart; 

            double lastHeight = (double)parts[currentPart == 0 ? currentPart : currentPart - 1] / 2.0f;
            int p = parts[currentPart];
            double currentHeight = ((double)p / 2.0f);
            return segmentDone<0.5 ? Helpers.Scurve((float)lastHeight, (float)currentHeight, (float)segmentDone*2.0f) : (float) currentHeight;
        }

        int dynamic = 50;
        private void updateDynamicLine(object sender, EventArgs e)
        {
            if (--dynamic != 0) return;
            dynamic = 10; // do every 100th call
            
            Rectangle r = inputManager.Thresholds;
            float y = Helpers.Clamp(((float)inputManager.LeftHand.Y - (float)r.Top)/(float)r.Height, 0.0f,1.0f);

            PlayerDynamicsEnd++;
            if (PlayerDynamicsEnd >= PlayerDynamics.Length) PlayerDynamicsEnd = 0;
            int insert = PlayerDynamicsEnd + 1;
            if (insert >= PlayerDynamics.Length) insert = 0;
            PlayerDynamics[insert] = y;

            if ( Math.Abs(y - GetCorrectDynamics(midiPlayer.Position)) < 0.1f)
            {
                Player player = ((BandMaster)Game).Player;
                player.Score = player.Score + 0.02f;
            }
            // TODO: evnt skriv noe om en veeeldig lavpassa score hvis indeks av hvor bra det er har endra seg
        }

        


        private void tempoHit(object sender, EventArgs e)
        {
            
            // spol fram til nextTick
            midiPlayer.Position += ticksToNextHit;
            for (int i = 0; i < ticksToNextHit; i++)
                updateDynamicLine(this, null);
            ticksToNextHit = 960 - 1;

            // set tepo basert på tid siden sist click
            midiPlayer.Continue();

            float now = 0.001f * tier.ElapsedMilliseconds;
            midiPlayer.Tempo = now - lastHitTime;
            lastHitTime = now;

            float tempoDiff = midiPlayer.TempoDifference;
            if (-0.05f < tempoDiff && tempoDiff <= 0.05f)
                splasher.Write("Perfekt tempo!", Color.White);
            else if (-0.7f < tempoDiff && tempoDiff <= -0.2f)
                splasher.Write("Saktere!", Color.White);
            else if (-2.0f < tempoDiff && tempoDiff <= -0.7f)
                splasher.Write("Mye saktere!", Color.White);
            else if ( 0.2f < tempoDiff && tempoDiff <= 0.7f)
                splasher.Write("Raskere!", Color.White);
            else if ( 0.7f < tempoDiff && tempoDiff <= 2.0f)
                splasher.Write("Mye raskere!", Color.White);

            Player player = ((BandMaster)Game).Player;
            player.Score = player.Score + (1.0 - Math.Abs(tempoDiff));

        }   


        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            float now = 0.001f * tier.ElapsedMilliseconds;

            if (!midiPlayer.IsRunning)
                midiPlayer.Tempo = now - lastHitTime;

            base.Update(gameTime);
        }
    }
}
