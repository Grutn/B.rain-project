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
using Microsoft.Kinect;

namespace KinectSkeletalTracking
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        KinectSensor kinect;

        Texture2D kinectVideoTexture;
        Texture2D kinectDepthTexture;
        Rectangle videoDisplayRectangle;

        SpriteFont messageFont;
        string errorMessage = "";

        byte[] colorData = null;

        short[] depthData = null;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected bool setupKinect()
        {
            if (KinectSensor.KinectSensors.Count == 0)
            {
                errorMessage = "No Kinects detected";
                return false;
            }

            kinect = KinectSensor.KinectSensors[0];

            try
            {
                kinect.ColorStream.Enable();
                kinect.DepthStream.Enable();
            }
            catch
            {
                errorMessage = "Kinect intialize failed";
                return false;
            }

            kinect.ColorFrameReady += new EventHandler<ColorImageFrameReadyEventArgs>(kinect_ColorFrameReady);
            kinect.DepthFrameReady += new EventHandler<DepthImageFrameReadyEventArgs>(kinect_DepthFrameReady);

            try
            {
                kinect.Start();
            }
            catch
            {
                errorMessage = "Camera start failed";
                return false;
            }
            return true;
        }

        void kinect_ColorFrameReady(object sender, ColorImageFrameReadyEventArgs e)
        {
            using (ColorImageFrame colorFrame = e.OpenColorImageFrame())
            {
                if (colorFrame == null)
                    return;

                if (colorData == null)
                    colorData = new byte[colorFrame.Width * colorFrame.Height * 4];

                colorFrame.CopyPixelDataTo(colorData);

                Color[] bitmap = new Color[colorFrame.Width * colorFrame.Height];

                for (int i = 0, sourceOffset = 0; i < bitmap.Length; ++i, sourceOffset += 4)
                {
                    bitmap[i] = new Color(colorData[sourceOffset + 2],
                                          colorData[sourceOffset + 1],
                                          colorData[sourceOffset + 0],
                                          255);
                }
                kinectVideoTexture = new Texture2D(GraphicsDevice, colorFrame.Width, colorFrame.Height);
                kinectVideoTexture.SetData(bitmap);
            }
        }

        void kinect_DepthFrameReady(object sender, DepthImageFrameReadyEventArgs e)
        {
            using (DepthImageFrame depthFrame = e.OpenDepthImageFrame())
            {
                if (depthFrame == null)
                    return;

                if (depthData == null)
                    depthData = new short[depthFrame.Width * depthFrame.Height];

                depthFrame.CopyPixelDataTo(depthData);

                Color[] bitmap = new Color[depthFrame.Width * depthFrame.Height];

                for (int i = 0; i < bitmap.Length; ++i)
                {
                    // 3 lowest bits are player id, 13 bits of depth data
                    int depth = depthData[i] >> 3;

                    if (depth == kinect.DepthStream.UnknownDepth)
                    {
                        bitmap[i] = Color.Red;
                    }
                    else if (depth == kinect.DepthStream.TooFarDepth)
                    {
                        bitmap[i] = Color.Blue;
                    }
                    else if (depth == kinect.DepthStream.TooNearDepth)
                    {
                        bitmap[i] = Color.Green;
                    }
                    else
                    {
                        byte depthByte = (byte)(255 - (depth >> 5));
                        bitmap[i] = new Color(depthByte, depthByte, depthByte, 255);
                    }

                    kinectDepthTexture = new Texture2D(GraphicsDevice, depthFrame.Width, depthFrame.Height);
                    kinectDepthTexture.SetData(bitmap);
                }
            }
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            videoDisplayRectangle = new Rectangle(0, 0,
                GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            messageFont = Content.Load<SpriteFont>("SpriteFont");

            setupKinect();
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            spriteBatch.Begin();
            if (kinectVideoTexture != null)
            {
                spriteBatch.Draw(kinectVideoTexture, videoDisplayRectangle, Color.White);
            }

            if (errorMessage.Length > 0)
            {
                spriteBatch.DrawString(messageFont, errorMessage, Vector2.Zero, Color.White);
            }
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
