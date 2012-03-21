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
    /// This is a game component that implements IDrawble.
    /// It desplays 2D texure with rate of width*hight frames per second
    /// Author: Dimitry Kongevold (dimitryk@github.com)
    /// 
    /// WARNING: Throws and exeption
    /// 
    /// </summary>
    public class Instrument : Microsoft.Xna.Framework.DrawableGameComponent
    {
        Rectangle _rectangle;
        Texture2D _texture;
        SpriteBatch _spriteBatch;
        int _width, _hight, frameWidth, frameHight;
        

        public Instrument(Game game)
            : base(game)
        {
            // TODO: Construct any child components here
        }

        public Instrument(Game game, String texture, int width, int hight, Rectangle rec)
            : base(game)
        {
            _texture = game.Content.Load<Texture2D>(texture);
            //checks for consistancy between Sheet and given parrameters
            if ((_texture.Height % hight != 0) | (_texture.Width % width != 0))
                throw new InvalidOperationException("The texture and parrameters do not match");
            //sets rest of variebles
            _rectangle = rec;
            _spriteBatch = new SpriteBatch(game.GraphicsDevice);
            _width = width;
            _hight = hight;
            frameWidth = _texture.Width / _width;       
            frameHight = _texture.Height / _hight;
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
            //Tempo can be set here by changing konstant 1000 with desirable variable
            int horisontalOffset = ((int)(gameTime.TotalGameTime.Milliseconds) / (1000 / (_width * _hight))) % _width,
                verticallOffset = (int)(gameTime.TotalGameTime.Milliseconds) / (1000 / _hight);
            _spriteBatch.Begin();
            //_spriteBatch.Draw(_texture, _rectangle, Color.White);
            _spriteBatch.Draw(_texture, _rectangle,
                new Rectangle(frameWidth * horisontalOffset, frameHight * verticallOffset, frameWidth, frameHight),
                Color.White);
            _spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
