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
    public class HighScoreMode : Microsoft.Xna.Framework.GameComponent, IMode
    {
        Audio.AudioFx fx;
        Input.IManageInput input;

        SoundEffectInstance applause;
        Effector applauseVolume = new Effector(1.0f);

        public HighScoreMode(Game game)
            : base(game)
        {
            // TODO: Construct any child components here

            BandMaster gm = (BandMaster)Game;
            gm.ModeChanged += delegate(object o, EventArgs a)
            {
                if (gm.Mode == this)
                {
                    applauseVolume.Value = 1.0f;
                    applause = fx.ApplauseBig.CreateInstance();
                    applause.Play();
                    Helpers.Wait(1.0, delegate()
                    {
                        Helpers.Wait(4.0, delegate()
                        {
                            input.StartPressed += restart;
                        });
                    });
                }
            };
        }

        void restart(object o, EventArgs e)
        {
            input.StartPressed -= restart;
            applauseVolume.Lerp(1.0, 1.0f, 0.0f, delegate()
            {
                ((BandMaster)Game).Restart();
            });
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            fx = (Audio.AudioFx)Game.Services.GetService(typeof(Audio.AudioFx));
            input = (Input.IManageInput)Game.Services.GetService(typeof(Input.IManageInput));

            base.Initialize();
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            if(applause != null) applause.Volume = applauseVolume.Value;
            base.Update(gameTime);
        }
    }
}
