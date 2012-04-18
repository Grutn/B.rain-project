using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using BandMaster.State;


namespace BandMaster.Graphics
{
    /// <summary>
    /// This is a game component that implements DrawableGameComponent.
    /// </summary>
    public class Line : Microsoft.Xna.Framework.DrawableGameComponent
    {
        List<int[]> Lines;
        Vector2 startpos;
        SpriteBatch LineSpriteBatch;
        Texture2D strait, upp1, upp2, upp3, down1, down2, down3;
        static Color[] colors={Color.Red, Color.LightBlue, Color.LavenderBlush, Color.MediumPurple, Color.Orange};
        int elapsed = 0;
        float offset = 0, CPS = 60, speed = 0;



        public Line(Game game, Vector2 pos)
            : base(game)
        {
            startpos = pos;
            LineSpriteBatch = (SpriteBatch)Game.Services.GetService(typeof(SpriteBatch));
            upp1 = game.Content.Load<Texture2D>("upp1");
            upp2 = game.Content.Load<Texture2D>("upp2");//TODO textures
            upp3 = game.Content.Load<Texture2D>("upp3");
            strait = game.Content.Load<Texture2D>("strait");
            down1 = game.Content.Load<Texture2D>("down1");
            down2 = game.Content.Load<Texture2D>("down2");
            down3 = game.Content.Load<Texture2D>("down3");
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

        public void ChangeSong(Song _song)
        {
            Lines = _song._Lines;
        }

        public void ChangeSpeed(float increase)
        {
            speed += increase;
        }



        private void DrawLine(GameTime gameTime, int[] parts, Color Col)
        {
            elapsed = elapsed + gameTime.ElapsedGameTime.Milliseconds;
            if (offset >= strait.Width)
            {
                for (int i = parts.Length; i > 1; i--)
                {
                    parts[i - 1] = parts[i - 2];
                }
                parts[0] = parts[0];
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
                switch (parts[i - 1] - parts[i])
                {
                    case 0:
                        LineSpriteBatch.Draw(strait, new Rectangle((int)startpos.X - strait.Width * i, (int)startpos.Y + 5 + parts[i] * strait.Height, strait.Width, strait.Height), Col);
                        break;
                    case 1:
                        LineSpriteBatch.Draw(down1, new Rectangle((int)startpos.X - strait.Width * i, (int)startpos.Y + parts[i] * strait.Height, strait.Width, strait.Height), Col);
                        break;
                    case 2:
                        LineSpriteBatch.Draw(down2, new Rectangle((int)startpos.X - strait.Width * i, (int)startpos.Y + parts[i] * strait.Height, strait.Width, strait.Height), Col);
                        break;
                    case 3:
                        LineSpriteBatch.Draw(down3, new Rectangle((int)startpos.X - strait.Width * i, (int)startpos.Y + parts[i] * strait.Height, strait.Width, strait.Height), Col);
                        break;
                    case -1:
                        LineSpriteBatch.Draw(upp1, new Rectangle((int)startpos.X - strait.Width * i, (int)startpos.Y + parts[i] * strait.Height, strait.Width, strait.Height), Col);
                        break;
                    case -2:
                        LineSpriteBatch.Draw(upp2, new Rectangle((int)startpos.X - strait.Width * i, (int)startpos.Y + parts[i] * strait.Height, strait.Width, strait.Height), Col);
                        break;
                    case -3:
                        LineSpriteBatch.Draw(upp3, new Rectangle((int)startpos.X - strait.Width * i, (int)startpos.Y + parts[i] * strait.Height, strait.Width, strait.Height), Col);
                        break;
                    default:
                        LineSpriteBatch.Draw(strait, new Rectangle((int)startpos.X - strait.Width * i, (int)startpos.Y + 5 + parts[i] * strait.Height, strait.Width, strait.Height), Col);
                        break;
                }

            }
            LineSpriteBatch.End();
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
            for (int i = 0; i < Lines.Count; i++)
            {
                DrawLine(gameTime, Lines.ElementAt(i), colors[i % 5]);
                
            }
            base.Draw(gameTime);
        }
    }
}
