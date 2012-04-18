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


    public class SplashText : Microsoft.Xna.Framework.DrawableGameComponent
    {
        SpriteBatch batch;


        public class TextItem
        {
            public bool Started = false;
            public TimeSpan Time;
            public string Text;
            public Color Color;
        };
        List<TextItem> items = new List<TextItem>();


        public SplashText(Game game)
            : base(game)
        {
            
            // TODO: Construct any child components here
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            batch = (SpriteBatch)Game.Services.GetService(typeof(SpriteBatch));
            this.DrawOrder = 9999;
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
            SpriteFont font = ((BandMaster)Game).SplashFont;

            batch.Begin(SpriteSortMode.Immediate, BlendState.Additive);

            TimeSpan ttl = new TimeSpan(0, 0, 1);
            for (int i = 0; i < items.Count; i++)
            {
                TextItem item = items[i];
                if (!item.Started)
                {
                    item.Started = true;
                    item.Time = gameTime.TotalGameTime;
                }
                else if (gameTime.TotalGameTime - item.Time > ttl)
                    items.RemoveAt(i--);
                else
                {
                    double alpha = 1.0 - (gameTime.TotalGameTime - item.Time).TotalSeconds / ttl.TotalSeconds;
                    System.Console.Out.WriteLine(alpha);
                    Vector2 halfsize = font.MeasureString(item.Text) / 2;
                    Vector2 center = new Vector2(
                        Game.GraphicsDevice.Viewport.Bounds.Center.X,
                        Game.GraphicsDevice.Viewport.Bounds.Center.Y
                        );
                    Color color = item.Color;
                    color.R = (byte)(color.R * alpha);
                    color.G = (byte)(color.G * alpha);
                    color.B = (byte)(color.B * alpha);
                    color.A = (byte)(color.A * alpha);

                    Vector2 offset = new Vector2(0, (float)(1.0-alpha) * -100.0f);
                    //color.A = (byte)(alpha*255.0);
                    batch.DrawString(font, item.Text, center - halfsize + offset, color);
                }
            }

            batch.End();

            base.Draw(gameTime);
        }

        public void Write(string text, Color color)
        {
            TextItem i = new TextItem();
            i.Text = text;
            i.Color = color;
            items.Add(i);
        }
    }
}
