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
        Texture2D SnakeTexture;
        int _max, _min;
        float speed;
        float[] snake;

        public VolumeSnake(Game game)
            : base(game)
        {
            // TODO: Construct any child components here
        }

        public VolumeSnake(Game game, Vector2 position, int max, int min)
            : base(game)
        {
            SnakeSpriteBatch = new SpriteBatch(game.GraphicsDevice);
            SnakeTexture = game.Content.Load<Texture2D>("Snake");
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
            snake = new float[8];
            for (int i = 0; i < snake.Length; i++)
            {
                snake[i] = _position.X;
            }
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
                //snake[0] = snake[0] + rand.Next(-5, 6);   //other imput here
            }
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            SnakeSpriteBatch.Begin();
            SnakeSpriteBatch.Draw(SnakeTexture,
                new Rectangle((int)(_position.Y), (int)snake[0], SnakeTexture.Width, SnakeTexture.Height),
                SnakeTexture.Bounds,
                Color.White,
                0f,
                new Vector2(SnakeTexture.Width / 2, SnakeTexture.Height / 2), SpriteEffects.None, 1f);
            for (int i = 1; i < snake.Length; i++)
            {
                SnakeSpriteBatch.Draw(SnakeTexture,
                new Rectangle((int)(_position.Y - i * speed), (int)snake[i], SnakeTexture.Width, SnakeTexture.Height),
                SnakeTexture.Bounds,
                Color.White,
                (float)Math.Tan(((snake[i - 1] - snake[i]) / speed)),
                new Vector2(SnakeTexture.Width / 2, SnakeTexture.Height / 2), SpriteEffects.None, 1f);
            }
            SnakeSpriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
