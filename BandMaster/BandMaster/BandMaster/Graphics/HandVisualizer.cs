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

        Texture2D arrowBody, arrowHead, circle;

        public HandVisualizer(Game game)
            : base(game)
        {

        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            sprites = (SpriteBatch)Game.Services.GetService(typeof(SpriteBatch));
            input = (IManageInput)Game.Services.GetService(typeof(IManageInput));

            arrowBody = Game.Content.Load<Texture2D>("Textures/arrow_body");
            arrowHead = Game.Content.Load<Texture2D>("Textures/arrow_head");
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
                /*
                // Horizontal Arrow
                {
                    int y = (int)input.RightHand.Y;
                    int l = (int)input.Thresholds.Left;
                    int r = (int)input.Thresholds.Right;

                    Rectangle bodyRect = new Rectangle(l, y, (int)r-l, (int)arrowBody.Height);
                    sprites.Draw(arrowBody, bodyRect, Color.White);

                    Rectangle headRect1 = new Rectangle(l, y, (int)arrowHead.Width, (int)arrowHead.Height);
                    sprites.Draw(arrowHead, headRect1, Color.White);

                    Rectangle headRect2 = new Rectangle(r, y, (int)arrowHead.Width, (int)arrowHead.Height);
                    sprites.Draw(arrowHead, headRect2, Color.White);

                    int x = (int)input.RightHand.X;

                    Rectangle circleRect = new Rectangle(x, y, (int)circle.Width, (int)circle.Height);
                    sprites.Draw(circle, circleRect, Color.White);
                }
                // Vertical arrow
                {
                    int x = (int)input.LeftHand.X;
                    int t = (int)input.Thresholds.Top;
                    int b = (int)input.Thresholds.Bottom;

                    Rectangle bodyRect = new Rectangle(x, t, (int))arrowBody.Height, (int)b-t);
                    sprites.Draw(arrowBody, bodyRect, Color.White);

                    Rectangle headRect1 = new Rectangle(x, t, (int)arrowHead.Width, (int)arrowHead.Height);
                    sprites.Draw(arrowHead, headRect1, Color.White);

                    Rectangle headRect2 = new Rectangle(x, b, (int)arrowHead.Width, (int)arrowHead.Height);
                    sprites.Draw(arrowHead, headRect2, Color.White);

                    int y = (int)input.LeftHand.Y;

                    Rectangle circleRect = new Rectangle(x, y, (int)circle.Width, (int)circle.Height);
                    sprites.Draw(circle, circleRect, Color.White);
                }*/
            }
            sprites.End();
            base.Draw(gameTime);
        }
    }
}
