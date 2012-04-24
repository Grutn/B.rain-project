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

using BandMaster.Input;
namespace BandMaster.Graphics
{

    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class HandVisualizer : Microsoft.Xna.Framework.DrawableGameComponent
    {
        SpriteBatch sprites;
        IManageInput input;

        Texture2D arrowBody, arrowHead, arrowHeadr, circle;

        public HandVisualizer(Game game)
            : base(game)
        {
            DrawOrder = 99;
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            sprites = (SpriteBatch)Game.Services.GetService(typeof(SpriteBatch));
            input = (IManageInput)Game.Services.GetService(typeof(IManageInput));

            arrowBody = Game.Content.Load<Texture2D>("Textures/4sqr");
            arrowHead = Game.Content.Load<Texture2D>("Textures/arrow_head");
            arrowHeadr = Game.Content.Load<Texture2D>("Textures/arrow_head_r");
            circle = Game.Content.Load<Texture2D>("Textures/circle");

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
            sprites.Begin(SpriteSortMode.Immediate, BlendState.Additive);
            {
                Color arrowc = new Color(0.25f, 0.25f, 0.25f);
                // Horizontal Arrow
                {
                    int y = (int)input.RightHand.Y;
                    int l = (int)input.Thresholds.Left;
                    int r = (int)input.Thresholds.Right;

                    Rectangle bodyRect = new Rectangle(l, y - 10, (int)r-l, (int)20);
                    sprites.Draw(arrowBody, bodyRect, arrowc);

                    Rectangle headRect1 = new Rectangle(l - arrowHead.Width, y - arrowHead.Height / 2, (int)arrowHead.Width, (int)arrowHead.Height);
                    sprites.Draw(arrowHead, headRect1, arrowc);

                    Rectangle headRect2 = new Rectangle(r, y - arrowHead.Height / 2, (int)arrowHead.Width, (int)arrowHead.Height);
                    sprites.Draw(arrowHead, headRect2, null, arrowc, 0.0f, Vector2.Zero, SpriteEffects.FlipHorizontally, 0.0f);

                    int x = (int)input.RightHand.X;

                    Rectangle circleRect = new Rectangle(x, y - circle.Height / 2, (int)circle.Width, (int)circle.Height);
                    sprites.Draw(circle, circleRect, Color.White);
                }
                // Vertical arrow
                {
                    int x = (int)input.LeftHand.X;
                    int t = (int)input.Thresholds.Top;
                    int b = (int)input.Thresholds.Bottom;

                    Rectangle bodyRect = new Rectangle(x - 10, t, (int)20, (int)b-t);
                    sprites.Draw(arrowBody, bodyRect, arrowc);

                    Rectangle headRect1 = new Rectangle(x - arrowHead.Width / 2, t - arrowHead.Height, (int)arrowHead.Width, (int)arrowHead.Height);
                    sprites.Draw(arrowHeadr, headRect1, arrowc);

                    Rectangle headRect2 = new Rectangle(x - arrowHead.Width / 2, b, (int)arrowHead.Width, (int)arrowHead.Height);
                    sprites.Draw(arrowHeadr, headRect2, null, arrowc, 0.0f, Vector2.Zero, SpriteEffects.FlipVertically, 0.0f);

                    int y = (int)input.LeftHand.Y;

                    Rectangle circleRect = new Rectangle(x - circle.Width / 2, y, (int)circle.Width, (int)circle.Height);
                    sprites.Draw(circle, circleRect, Color.White);
                }
            }
            sprites.End();
            base.Draw(gameTime);
        }
    }
}