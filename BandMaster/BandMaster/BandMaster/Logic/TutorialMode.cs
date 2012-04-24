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
    public class TutorialMode : Microsoft.Xna.Framework.GameComponent, IMode
    {
        Graphics.SplashText splasher;

        public TutorialMode(Game game)
            : base(game)
        {
            
        }

        protected override void OnEnabledChanged(object sender, EventArgs args)
        {
            if (Enabled)
            {
                startWaitingForRythm();
            }
            else
            {
            }
        }

        void startWaitingForRythm()
        {
            splasher.Write("Vift med høyre hånd for å styre rytmen", Color.White);

        }
        void startWaitingForDynamics()
        {
            splasher.Write("Vift med venstre hånd for å styre dynamikk", Color.White);

        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            splasher = (Graphics.SplashText)Game.Services.GetService(typeof(Graphics.SplashText));

            base.Initialize();
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {

            base.Update(gameTime);
        }
    }
}
