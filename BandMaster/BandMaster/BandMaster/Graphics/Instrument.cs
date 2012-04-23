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
    /// Exeptions:
    ///     InvalidOperationException
    /// 
    /// </summary>
    public class Instrument : Microsoft.Xna.Framework.DrawableGameComponent
    {

        Texture2D texture;
        SpriteBatch _spriteBatch;
        int _width, _hight, frameWidth, frameHight;


        public Rectangle Bounds;

        public Instrument(Game game)
            : base(game)
        {
            // TODO: Construct any child components here
        }
        /// <summary>
        /// Creates an Instrument in given Rectangele
        /// Texture must have frames structured in a pattern
        /// of Width X Hight
        ///
        /// Exeptions:
        ///     InvalidOperationException
        /// </summary>
        /// <param name="game"></param>
        /// <param name="texture">The texture to load</param>
        /// <param name="width">Number of frames in a texture</param>
        /// <param name="hight">Number of frames in a texture</param>
        /// <param name="rec">Place on the screen where instrument should be drawn</param>
        /*public Instrument(Game game, String textureName, int width, int hight, Rectangle rec)
            : base(game)
        {
            texture = game.Content.Load<Texture2D>(textureName);
            //checks for consistancy between Sheet and given parrameters
            if ((texture.Height % hight != 0) | (texture.Width % width != 0))
                throw new InvalidOperationException("The texture and parrameters do not match");
            //sets rest of variebles
            _rectangle = rec;
            _spriteBatch = (SpriteBatch)Game.Services.GetService(typeof(SpriteBatch));
            _width = width;
            _hight = hight;
            frameWidth = texture.Width / _width;       
            frameHight = texture.Height / _hight;
        }*/
        public Instrument(Game game, String textureName)
            : base(game)
        {
            texture = game.Content.Load<Texture2D>("Textures/"+textureName);
            Bounds = new Rectangle(0, 0, texture.Width, texture.Height);

            _spriteBatch = new SpriteBatch(game.GraphicsDevice);
            _width = 5;
            _hight = 5;
            frameWidth = texture.Width / _width;
            frameHight = texture.Height / _hight;
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
            _spriteBatch.Draw(texture, Bounds,
                /*new Rectangle(frameWidth * horisontalOffset, frameHight * verticallOffset, frameWidth, frameHight),*/
                null,
                Color.White, 0, Vector2.Zero, SpriteEffects.None , 0);
            _spriteBatch.End();
            base.Draw(gameTime);
        }

    }
}
