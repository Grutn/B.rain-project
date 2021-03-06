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


namespace BandMaster.Logic
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class MainMenuMode : Microsoft.Xna.Framework.GameComponent, IMode
    {
        Input.IManageInput input;
        Audio.AudioFx audiofx;

        private SoundEffectInstance ambient;
        Effector ambientVolume = new Effector(1.0f);

        public MainMenuMode(Game game)
            : base(game)
        {
            BandMaster bm = (BandMaster)Game;

            bm.ModeChanged += delegate(object o, EventArgs a)
            {
                if (bm.Mode == this)
                {
                    ambientVolume.Value = 1.0f;
                    ambient = Audio.AudioFx.Play(audiofx.ApplauseBig);

                    Helpers.Wait(1.5, delegate()
                    {
                        input.OnRestart += startGame;
                    });
                }
            };
        }

        void startGame(object o, EventArgs a)
        {
            input.OnRestart -= startGame;
            ambientVolume.Lerp(4.0, 1.0f, 0.0f);
            Helpers.Wait(0.2, delegate()
            {
                ((BandMaster)Game).Mode = ((BandMaster)Game).Tutorial;
            });
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            input = (Input.IManageInput)Game.Services.GetService(typeof(Input.IManageInput));
            audiofx = (Audio.AudioFx)Game.Services.GetService(typeof(Audio.AudioFx));
            base.Initialize();
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            if (ambient != null)
                ambient.Volume = ambientVolume.Value;
            base.Update(gameTime);
        }
    }
}
