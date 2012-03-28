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
    /// This is a game component that implements DrawableGameComponent.
    /// </summary>
    public class Line : Microsoft.Xna.Framework.DrawableGameComponent
    {
        int[] parts;
        Vector2 startpos;
        SpriteBatch LineSpriteBatch;
        Texture2D strait, upp, down;
        int elapsed = 0;
        float offset = 0, CPS = 60, speed = 12;



        public Line(Game game, Vector2 pos)
            : base(game)
        {
            startpos = pos;
            LineSpriteBatch = new SpriteBatch(game.GraphicsDevice);
            upp = game.Content.Load<Texture2D>("upp");           //TODO textures
            strait = game.Content.Load<Texture2D>("strait");
            down = game.Content.Load<Texture2D>("down");
            parts = new int[30];
            for (int i = 0; i < 30; i++)
            {
                parts[i] = 0;
            }
            parts[1] = parts[1] - 20;
            parts[2] = parts[2] - 20;
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
            elapsed = elapsed + gameTime.ElapsedGameTime.Milliseconds;
            if (offset >= strait.Width)
            {
                for (int i = parts.Length; i > 1; i--)
                {
                    parts[i - 1] = parts[i - 2];
                }
                parts[0] = parts[0];
                startpos.X = startpos.X + offset;
                offset = 0;
            }

            else if (elapsed >= 1000 / CPS)
            {
                startpos.X = startpos.X - strait.Width * (speed / 100f);
                offset = offset + strait.Width * (speed / 100f);
                elapsed = 0;
            }
            LineSpriteBatch.Begin();
            for (int i = 1; i < parts.Length; i++)
            {
                if (parts[i - 1] - parts[i] == 0)
                {
                    LineSpriteBatch.Draw(strait, new Rectangle((int)startpos.X - strait.Width * i, (int)startpos.Y + parts[i], strait.Width, strait.Height), Color.White);
                }
                else if (parts[i - 1] - parts[i] > 0)
                {
                    LineSpriteBatch.Draw(down, new Rectangle((int)startpos.X - strait.Width * i, (int)startpos.Y + parts[i], strait.Width, strait.Height), Color.White);
                }
                else
                {
                    LineSpriteBatch.Draw(upp, new Rectangle((int)startpos.X - strait.Width * i, (int)startpos.Y + parts[i], strait.Width, strait.Height), Color.White);
                }

            }
            LineSpriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
