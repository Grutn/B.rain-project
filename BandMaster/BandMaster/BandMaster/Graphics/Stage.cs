using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;


namespace BandMaster.Graphics
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class Stage : Microsoft.Xna.Framework.DrawableGameComponent
    {   
        List<Instrument> Band;
        SpriteBatch StageSpriteBatch;
        Hashtable Instruments;
        Game _game;

        public Stage(Game game)
            : base(game)
        {
            _game = game;
            StageSpriteBatch = new SpriteBatch(game.GraphicsDevice);
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            // TODO: Add your initialization code here

            base.Initialize();
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


        private void addInstrument(string _instrument)
        {
            if (!(Instruments.ContainsKey(_instrument)))
            {
                Instruments.Add(_instrument, new Instrument(_game));//better creater
            }
        }

        /* fiks it later
        public void SetBand(Object something)
        {
            Band.RemoveRange(0,Band.Count);
   
        }
        */
        public override void  Draw(GameTime gameTime)
        {   
            StageSpriteBatch.Begin();
            foreach (Instrument _instrument in Band)
            {
                if (_instrument.isVisible) _instrument.Draw(gameTime);
            }
            StageSpriteBatch.End();
 	        base.Draw(gameTime);
        }
    }
}
