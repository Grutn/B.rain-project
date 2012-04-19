using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Kinect;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.IO;

namespace BandMaster.Input
{
    class KinectDebug : DrawableGameComponent
    {
        private readonly KinectInputManager input;
        private Texture2D blank4sqr = null;
        private Texture2D colorImage = null;

        Vector2 activeHand;
        Vector2 offHand;

        public KinectDebug(Game game, KinectInputManager input) : base(game)
        {
            this.input = input;

            DrawOrder = 1120;
        }

        protected override void LoadContent()
        {
            ContentManager content = Game.Content;

            blank4sqr = content.Load<Texture2D>("4sqr");
        }

        public override void Update(GameTime gameTime)
        {
            
        }

        public void OnPlayerData(object sender, PlayerEvent e)
        {
            TextWriter file = new StreamWriter("kinectdata.txt");

            file.WriteLine("Hand: " + e.hand.ToString());
            file.WriteLine("Direction: " + e.direction.ToString());
            file.WriteLine("Position: " + e.position.ToString());
            file.WriteLine("Velocity: " + e.position.ToString());

            file.Close();
        }

        public void  OnColorFrameReady(object sender, ColorImageFrameReadyEventArgs e)
        {
            using (ColorImageFrame frame = e.OpenColorImageFrame())
            {
                if (frame == null)
                {
                    return;
                }

                byte[] data = new byte[frame.Width * frame.Height * frame.BytesPerPixel];
                Color[] bitmap = new Color[frame.Width * frame.Height];
                
                frame.CopyPixelDataTo(data);

                for (int i = 0, offset = 0; i < frame.Height; ++i, offset += frame.BytesPerPixel)
                {
                    bitmap[i] = new Color(data[offset + 2],
                                          data[offset + 1],
                                          data[offset + 0],
                                          255);
                }

                colorImage = new Texture2D(Game.GraphicsDevice, frame.Width, frame.Height);
                colorImage.SetData(bitmap);
            }
        }

        public override void Draw(GameTime gameTime)
        {
            SpriteBatch spriteBatch = (SpriteBatch)Game.Services.GetService(typeof(SpriteBatch));

            spriteBatch.Begin();

            if (colorImage != null)
            {
                int imageWidth = colorImage.Width / 10;
                int imageHeight = colorImage.Height / 10;

                spriteBatch.Draw(colorImage, colorImage.Bounds, null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0f);
            }

            spriteBatch.Draw(blank4sqr, Vector2.Zero, Color.White);

            spriteBatch.End();
        }
    }
}
