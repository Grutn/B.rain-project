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
        static Color[] colors={Color.Red, Color.Blue, Color.Yellow, Color.MediumPurple, Color.Orange};
        int elapsed = 0,width = 0,hight = 0, of= 0;
        float offset = 0, CPS = 60, speed = 1f, scale = 0.3f,scaleY = 0.3f;



        public Line(Game game, Vector2 pos)
            : base(game)
        {
            startpos = pos;
            
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            LineSpriteBatch = (SpriteBatch)Game.Services.GetService(typeof(SpriteBatch));
            
            upp1 = Game.Content.Load<Texture2D>("opp1");
            upp2 = Game.Content.Load<Texture2D>("opp2");//TODO textures
            upp3 = Game.Content.Load<Texture2D>("opp3");
            strait = Game.Content.Load<Texture2D>("strek");
            down1 = Game.Content.Load<Texture2D>("ned1");
            down2 = Game.Content.Load<Texture2D>("ned2");
            down3 = Game.Content.Load<Texture2D>("ned3");
            width = (int)(strait.Width * scaleY);
            hight = (int)(strait.Height * scale);
            base.Initialize();
        }

        public void ChangeSong(Song _song)
        {
            Lines = _song.Lines;
        }

        public void ChangeSpeed(float increase)
        {
            speed += increase;
        }

        private void DrawLine(GameTime gameTime, int[] parts, Color Col)
        {
            elapsed = elapsed + gameTime.ElapsedGameTime.Milliseconds;
            if (elapsed >= 1000 / CPS)
            {
                startpos.X = startpos.X - width * (speed / 100f);
                offset = offset + width * (speed / 100f);
                elapsed = 0;
            }
            LineSpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive);
            int inc = (int)(12 * scale);
            for (int i = 1; i < parts.Length; i++)
            {
                switch (parts[i - 1] - parts[i])
                {
                    case 0:
                        LineSpriteBatch.Draw(strait, new Rectangle((int)startpos.X + width * i, (int)startpos.Y + of + parts[i] * (int)(hight/3f), width, hight), Col);
                        break;
                    case 1:
                        of = inc;
                        LineSpriteBatch.Draw(upp1, new Rectangle((int)startpos.X + width * i, (int)startpos.Y + parts[i-1] * (int)(hight / 3f), width, hight), Col);
                        break;
                    case 2:
                        of = inc;
                        LineSpriteBatch.Draw(upp2, new Rectangle((int)startpos.X + width * i, (int)startpos.Y + parts[i-1] * (int)(hight / 3f), width, hight), Col);
                        break;
                    case 3:
                        of = inc;
                        LineSpriteBatch.Draw(upp3, new Rectangle((int)startpos.X + width * i, (int)startpos.Y + parts[i-1] * (int)(hight / 3f), width, hight), Col);
                        break;
                    case -1:
                        of = 0;
                        LineSpriteBatch.Draw(down1, new Rectangle((int)startpos.X + width * i, (int)startpos.Y + parts[i] * (int)(hight / 3f), width, hight), Col);
                        break;
                    case -2:
                        of = 0;
                        LineSpriteBatch.Draw(down2, new Rectangle((int)startpos.X + width * i, (int)startpos.Y + parts[i] * (int)(hight / 3f), width, hight), Col);
                        break;
                    case -3:
                        of = 0;
                        LineSpriteBatch.Draw(down3, new Rectangle((int)startpos.X + width * i, (int)startpos.Y + parts[i] * (int)(hight / 3f), width, hight), Col);
                        break;
                    default:
                        LineSpriteBatch.Draw(strait, new Rectangle((int)startpos.X + width * i, (int)startpos.Y +of + parts[i] * (int)(hight / 3f), width, hight), Col);
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
