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


namespace BandMaster.Graphics
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class Instrument : Microsoft.Xna.Framework.DrawableGameComponent
    {
        Rectangle _rectangle;
        Texture2D _texture;
        SpriteBatch _spriteBatch;
        

        public Instrument(Game game)
            : base(game)
        {
            // TODO: Construct any child components here
        }

        public Instrument(Game game, String texture, int with, int hight, Rectangle rec)
            : base(game)
        {
            _texture = game.Content.Load<Texture2D>(texture);
            _rectangle = rec;
            _spriteBatch = new SpriteBatch(game.GraphicsDevice);
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
            
            base.Update(gameTime);
        }
        public override void Draw(GameTime gameTime)
        {
            _spriteBatch.Begin();
            _spriteBatch.Draw(_texture, _rectangle, Color.White);
            _spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
