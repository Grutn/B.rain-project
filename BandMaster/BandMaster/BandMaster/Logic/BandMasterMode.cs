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
        private Midi.Player player;
        private IManageInput inputManager;

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
            player = (Midi.Player)Game.Services.GetService(typeof(Midi.Player));
            inputManager = (IManageInput)Game.Services.GetService(typeof(IManageInput));
            
            Enabled = false;

            ((BandMaster)Game).SongChanged += onSongChanged;
            ((BandMaster)Game).SongLoaded += onSongLoaded;

            base.Initialize();
        }


        public void onSongChanged(object sender, EventArgs args)
        {
            if (!Enabled) return;
            Enabled = false;
            player.Play();
            tier.Start();
        }
        public void onSongLoaded(object sender, EventArgs args)
        {
            player.Song = ((BandMaster)Game).Song.Midi;
            player.Play();
            tier.Start();
            Enabled = true;   
        }

        protected override void OnEnabledChanged(object sender, EventArgs args)
        {
            base.OnEnabledChanged(sender, args);

            if (Enabled)
            {
                inputManager.OnTempoHit += tempoHit;
                player.Tick += onTick;
                player.Play();
                tier.Start();                
            }
            else
            {
                inputManager.OnTempoHit -= tempoHit;
                player.Tick -= onTick;
                player.Stop();
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
                player.Stop();
        }

        float lastTempo = 0.0f;

        System.Diagnostics.Stopwatch tier = new System.Diagnostics.Stopwatch();
        private void tempoHit(object sender, EventArgs e)
        {
            float now = 0.001f * tier.ElapsedMilliseconds;

            // spol fram til nextTick
            player.Position += ticksToNextHit;
            ticksToNextHit = 960 - 1;

            // set tepo basert på tid siden sist click
            player.Continue();
            currentHitTime = now;

            float newTempo = currentHitTime - lastHitTime;
            float v = 0.9f;
            player.Tempo = (v * newTempo + (1.0f - v) * lastTempo) / 10000;
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
