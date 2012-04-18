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
    public class VolumeSnake : Microsoft.Xna.Framework.DrawableGameComponent
    {
        SpriteBatch SnakeSpriteBatch;
        Vector2 _position;
        Texture2D snakeTexture, snakeTextureBody;
        int _max, _min;
        float speed=5;
        float[] snake;

        public VolumeSnake(Game game)
            : base(game)
        {
            
        }

        public VolumeSnake(Game game, Vector2 position, int max, int min)
            : base(game)
        {
            
            _position = position;
            _max = max;
            _min = min;

        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            snake = new float[6];
            for (int i = 0; i < snake.Length; i++)
            {
                snake[i] = _position.X;
            }
            SnakeSpriteBatch = (SpriteBatch)Game.Services.GetService(typeof(SpriteBatch));
            snakeTexture = Game.Content.Load<Texture2D>("snakeTry");
            snakeTextureBody = Game.Content.Load<Texture2D>("snakeBody");
            base.Initialize();
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            if (gameTime.TotalGameTime.Milliseconds % 100 > 70)
            {
                for (int i = snake.Length; i > 1; i--)
                {
                    snake[i - 1] = snake[i - 2];
                }
                //snake[0] = snake[0] + rand.Next(-5, 6);   //TODO other imput here
            }
            base.Update(gameTime);
        }

        public void changeVolume(float inc)
        {
            if (_position.Y + inc < _max && _position.Y + inc < _min) _position.Y += inc;
        }


        public override void Draw(GameTime gameTime)
        {
            Color farge = new Color(255f, 0f, 0f, 255);

            SnakeSpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive);
            SnakeSpriteBatch.Draw(snakeTexture,
                new Rectangle((int)(_position.Y), (int)snake[0], snakeTexture.Width, snakeTexture.Height),
                snakeTexture.Bounds,
                Color.Red,
                0f,
                new Vector2(snakeTexture.Width / 2, snakeTexture.Height / 2), SpriteEffects.None, 1f);
            for (int i = 1; i < snake.Length; i++)
            {
                SnakeSpriteBatch.Draw(snakeTextureBody,
                new Rectangle((int)(_position.Y - i * speed), (int)snake[i], snakeTextureBody.Width, snakeTextureBody.Height),
                snakeTextureBody.Bounds,
                farge,
                (float)Math.Tan(((snake[i - 1] - snake[i]) / speed)),
                new Vector2(snakeTextureBody.Width / 2, snakeTextureBody.Height / 2), SpriteEffects.None, 1f);
                farge.A = (byte)(255 / i);
            }
            SnakeSpriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
