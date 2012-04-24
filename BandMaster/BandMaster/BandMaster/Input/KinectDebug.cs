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

        private TextWriter file; // Recording data

        private float time = -1f; // Impossible value to distinguish from real data

        private bool timestamp = true;
        private bool position = true;

        private bool enableColorImage = false;
        private bool enableLog = false;
        private bool enableEdgeAndHand = false;

        #region Getters and Setters

        public bool EnableColorImage
        {
            get { return enableColorImage; }
            set { enableColorImage = value; }
        }

        public bool EnableLog
        {
            get { return enableLog; }
            set { enableLog = value; }
        }

        public bool EnableEdgeAndHand
        {
            get { return enableEdgeAndHand; }
            set { enableEdgeAndHand = value; }
        }

        public bool EnableTimeStamp
        {
            get { return timestamp; }
            set { timestamp = value; }
        }

        public bool EnablePosition
        {
            get { return position; }
            set { position = value; }
        }

        #endregion

        #region Setup and Initialization

        public KinectDebug(Game game, KinectInputManager input) : base(game)
        {
            this.input = input;
            this.DrawOrder = 1120;
        }

        public override void Initialize()
        {
            if (EnableLog)
            {
                file = new StreamWriter("kinectdata.txt");
                file.WriteLine("Positional data is in meters");
                file.WriteLine("Timestamps are given in ms\n");
            }

            ContentManager content = Game.Content;

            blank4sqr = content.Load<Texture2D>("Textures/4sqr");
        }

        protected override void LoadContent()
        {
        }

        #endregion

        protected override void Dispose(bool disposing)
        {
            if (EnableLog)
            {
                file.Close();
            }
        }        

        public void OnPlayerData(object sender, PlayerEvent e)
        {
            if (EnableLog)
            {
                file.WriteLine("TimeStamp: " + time.ToString());
                file.WriteLine("Hand: " + e.hand.ToString());
                file.WriteLine("Position: " + e.position.ToString());
            }
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

                for (int i = 0, offset = 0; i < (frame.Width * frame.Height); ++i, offset += frame.BytesPerPixel)
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


        public override void Update(GameTime gameTime)
        {
            if (EnableLog)
            {
                time = (float)gameTime.TotalGameTime.TotalMilliseconds;
            }
        }

        public override void Draw(GameTime gameTime)
        {
            if (!EnableColorImage && !EnableEdgeAndHand)
            {
                return;
            }

            SpriteBatch spriteBatch = (SpriteBatch)Game.Services.GetService(typeof(SpriteBatch));

            spriteBatch.Begin();

            if (EnableColorImage && colorImage != null)
            {
                int imageWidth = colorImage.Width;
                int imageHeight = colorImage.Height;

                spriteBatch.Draw(colorImage, Vector2.Zero, null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
            }

            if (enableEdgeAndHand)
            {
                Viewport viewport = Game.GraphicsDevice.Viewport;
            
                spriteBatch.Draw(blank4sqr, input.RightHand, null, Color.Red, 0f, Vector2.Zero, 0.5f, SpriteEffects.None, 0f);
                spriteBatch.Draw(blank4sqr, new Rectangle((int)input.Thresholds.Right, 0, 2, Game.GraphicsDevice.Viewport.Height), Color.White);
                spriteBatch.Draw(blank4sqr, new Rectangle((int)input.Thresholds.Left, 0, 2, Game.GraphicsDevice.Viewport.Height), Color.White);
            }

            spriteBatch.End();
        }
    }
}
