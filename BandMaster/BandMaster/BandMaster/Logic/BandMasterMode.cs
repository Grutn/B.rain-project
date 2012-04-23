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
        }

        protected override void OnEnabledChanged(object sender, EventArgs args)
        {
            base.OnEnabledChanged(sender, args);

            if (Enabled)
            {
                inputManager.OnTempoHit += tempoHit;
                midiPlayer.Tick += onTick;
                midiPlayer.Play();
                tier.Start();                
            }
            else
            {
                inputManager.OnTempoHit -= tempoHit;
                midiPlayer.Tick -= onTick;
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

        /*string[] tempoSplash = new string[] {
            "Er du der?",
            "Zzz..",
            "Raskere",
            "Perfekt tempo!",
            "Litt saktere",
            "Senk tempoet",
            "Alt for raskt!"
        };
        string[] dynamicSplash = new string[] {
            "Uffda",
            "Ok",
            "Bra!",
            "Perfekt!"
        };*/

        private void dynamicHit(object sender, EventArgs e)
        {
            // TODO: calc score ..

            //float score = ;

            //splasher.Write("Dyn "+score, Color.White);
/*            if (veldig bra)
            {
                splasher.Write(dynamicSplash[goodness], Color.White);
            }
            else if (veldig dårlig)
            {
                splasher.Write(dynamicSplash[badness], Color.Red);
            }*/
        }
        private void tempoHit(object sender, EventArgs e)
        {
            float now = 0.001f * tier.ElapsedMilliseconds;

            // spol fram til nextTick
            midiPlayer.Position += ticksToNextHit;
            ticksToNextHit = 960 - 1;

            // set tepo basert på tid siden sist click
            midiPlayer.Continue();
            currentHitTime = now;

            float newTempo = currentHitTime - lastHitTime;
            float v = 0.9f;
            midiPlayer.Tempo = (v * newTempo + (1.0f - v) * lastTempo);

            float idealTempo = 1.0f;
            float tempoDiff = midiPlayer.Tempo - idealTempo;

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
            
            lastTempo = newTempo;
            lastHitTime = currentHitTime;
        }   


        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            // TODO: Add your update code here

            base.Update(gameTime);
        }
    }
}
