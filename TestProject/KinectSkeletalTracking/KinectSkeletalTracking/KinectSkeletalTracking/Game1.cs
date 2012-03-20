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
        Texture2D lineDot;

        Rectangle videoDisplayRectangle;
        Rectangle depthDisplayRectangle;

        SpriteFont messageFont;
        string errorMessage = "";

        byte closestByte;

        Skeleton[] skeletons = null;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            Window.AllowUserResizing = true;
            Window.ClientSizeChanged += new EventHandler<EventArgs>(Window_ClientSizeChanged);
        }

        void Window_ClientSizeChanged(object sender, EventArgs e)
        {
            graphics.PreferredBackBufferWidth = GraphicsDevice.Viewport.Width;
            graphics.PreferredBackBufferHeight = GraphicsDevice.Viewport.Height;
            graphics.ApplyChanges();
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
                //kinect.DepthStream.Enable();
                kinect.SkeletonStream.Enable();
            }
            catch
            {
                errorMessage = "Kinect intialize failed";
                return false;
            }

            kinect.ColorFrameReady += new EventHandler<ColorImageFrameReadyEventArgs>(kinect_ColorFrameReady);
            //kinect.DepthFrameReady += new EventHandler<DepthImageFrameReadyEventArgs>(kinect_DepthFrameReady_Closest);
            kinect.SkeletonFrameReady += new EventHandler<SkeletonFrameReadyEventArgs>(kinect_SkeletonFrameReady);

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

        void kinect_SkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        {
            using (SkeletonFrame frame = e.OpenSkeletonFrame())
            {
                if (frame != null)
                {
                    skeletons = new Skeleton[frame.SkeletonArrayLength];
                    frame.CopySkeletonDataTo(skeletons);
                }
            }
        }

        void kinect_ColorFrameReady(object sender, ColorImageFrameReadyEventArgs e)
        {
            using (ColorImageFrame colorFrame = e.OpenColorImageFrame())
            {
                if (colorFrame == null)
                    return;

                byte[] colorData = new byte[colorFrame.Width * colorFrame.Height * 4];

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

        void kinect_DepthFrameReady_Closest(object sender, DepthImageFrameReadyEventArgs e)
        {
            using (DepthImageFrame depthFrame = e.OpenDepthImageFrame())
            {
                if (depthFrame == null)
                    return;

                short[] depthData = new short[depthFrame.Width * depthFrame.Height];
                byte[] depthBytes = new byte[depthFrame.Width * depthFrame.Height];

                depthFrame.CopyPixelDataTo(depthData);

                for (int i = 0; i < depthData.Length; ++i)
                {
                    int depth = depthData[i] >> 3;
                    if (depth == kinect.DepthStream.UnknownDepth ||
                        depth == kinect.DepthStream.TooFarDepth ||
                        depth == kinect.DepthStream.TooNearDepth)
                    {
                        // Mark as invalid value
                        depthBytes[i] = 255;
                    }
                    else
                    {
                        byte depthByte = (byte)(depth >> 4);
                        depthBytes[i] = depthByte;

                        if (depthByte < closestByte)
                            closestByte = depthByte;
                    }
                }

                Color[] bitmap = new Color[depthFrame.Width * depthFrame.Height];

                for (int i = 0; i < depthBytes.Length; ++i)
                {
                    byte colorValue = (byte)(255 - depthBytes[i]);

                    if (depthBytes[i] == closestByte)
                        bitmap[i] = new Color(colorValue, 0, 0, 255);
                    else
                        bitmap[i] = new Color(colorValue, colorValue, colorValue, 255);
                }

                kinectDepthTexture = new Texture2D(GraphicsDevice, depthFrame.Width, depthFrame.Height);
                kinectDepthTexture.SetData(bitmap);
            }
        }

        void kinect_DepthFrameReady(object sender, DepthImageFrameReadyEventArgs e)
        {
            using (DepthImageFrame depthFrame = e.OpenDepthImageFrame())
            {
                if (depthFrame == null)
                    return;

                short[] depthData = new short[depthFrame.Width * depthFrame.Height];
                
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
                }

                kinectDepthTexture = new Texture2D(GraphicsDevice, depthFrame.Width, depthFrame.Height);
                kinectDepthTexture.SetData(bitmap);
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
            // TODO: Add your initialization logic here.

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
            lineDot = Content.Load<Texture2D>("dot");

            setupKinect();
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
            kinect.Stop();
            kinect.Dispose();
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

            if (Keyboard.GetState().IsKeyDown(Keys.Down))
            {
                kinect.ElevationAngle -= 5;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Up))
            {
                kinect.ElevationAngle += 5;
            }

            videoDisplayRectangle = new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);
            depthDisplayRectangle = new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);

            base.Update(gameTime);
        }

        void drawSkeleton(Skeleton skeleton, Color color)
        {
            // Spine
            drawBone(skeleton.Joints[JointType.Head], skeleton.Joints[JointType.ShoulderCenter], color);
            drawBone(skeleton.Joints[JointType.ShoulderCenter], skeleton.Joints[JointType.Spine], color);

            // Left leg
            drawBone(skeleton.Joints[JointType.Spine], skeleton.Joints[JointType.HipCenter], color);
            drawBone(skeleton.Joints[JointType.HipCenter], skeleton.Joints[JointType.HipLeft], color);
            drawBone(skeleton.Joints[JointType.HipLeft], skeleton.Joints[JointType.KneeLeft], color);
            drawBone(skeleton.Joints[JointType.KneeLeft], skeleton.Joints[JointType.AnkleLeft], color);
            drawBone(skeleton.Joints[JointType.AnkleLeft], skeleton.Joints[JointType.FootLeft], color);

            // Right leg
            drawBone(skeleton.Joints[JointType.HipCenter], skeleton.Joints[JointType.HipRight], color);
            drawBone(skeleton.Joints[JointType.HipRight], skeleton.Joints[JointType.KneeRight], color);
            drawBone(skeleton.Joints[JointType.KneeRight], skeleton.Joints[JointType.AnkleRight], color);
            drawBone(skeleton.Joints[JointType.AnkleRight], skeleton.Joints[JointType.FootRight], color);

            // Left arm
            drawBone(skeleton.Joints[JointType.ShoulderCenter], skeleton.Joints[JointType.ShoulderLeft], color);
            drawBone(skeleton.Joints[JointType.ShoulderLeft], skeleton.Joints[JointType.ElbowLeft], color);
            drawBone(skeleton.Joints[JointType.ElbowLeft], skeleton.Joints[JointType.WristLeft], color);
            drawBone(skeleton.Joints[JointType.WristLeft], skeleton.Joints[JointType.HandLeft], color);

            // Right arm
            drawBone(skeleton.Joints[JointType.ShoulderCenter], skeleton.Joints[JointType.ShoulderRight], color);
            drawBone(skeleton.Joints[JointType.ShoulderRight], skeleton.Joints[JointType.ElbowRight], color);
            drawBone(skeleton.Joints[JointType.ElbowRight], skeleton.Joints[JointType.WristRight], color);
            drawBone(skeleton.Joints[JointType.WristRight], skeleton.Joints[JointType.HandRight], color);
        }

        void drawBone(Joint jointStart, Joint jointEnd, Color color)
        {
            ColorImagePoint jointStartPoint = kinect.MapSkeletonPointToColor(jointStart.Position, ColorImageFormat.RgbResolution640x480Fps30);
            Vector2 jointStartVector = new Vector2(jointStartPoint.X, jointStartPoint.Y);

            ColorImagePoint jointEndPoint = kinect.MapSkeletonPointToColor(jointEnd.Position, ColorImageFormat.RgbResolution640x480Fps30);
            Vector2 jointEndVector = new Vector2(jointEndPoint.X, jointEndPoint.Y);

            drawLine(jointStartVector, jointEndVector, color);
        }

        void drawLine(Vector2 start, Vector2 end, Color color)
        {
            Vector2 difference = end - start;
            Vector2 scale = new Vector2(1.0f, difference.Length() / lineDot.Height);

            float angle = (float)(Math.Atan2(difference.Y, difference.X)) - MathHelper.PiOver2;
            Vector2 origin = new Vector2(0.5f, 0.0f);
            spriteBatch.Draw(lineDot, start, null, color, angle, origin, scale, SpriteEffects.None, 1.0f);
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

            if (kinectDepthTexture != null)
            {
                spriteBatch.Draw(kinectDepthTexture, depthDisplayRectangle, Color.White);
            }

            if (errorMessage.Length > 0)
            {
                spriteBatch.DrawString(messageFont, errorMessage, Vector2.Zero, Color.White);
            }

            if (skeletons != null)
            {
                foreach (Skeleton s in skeletons)
                {
                    if (s.TrackingState == SkeletonTrackingState.Tracked)
                        drawSkeleton(s, Color.White);
                }
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
