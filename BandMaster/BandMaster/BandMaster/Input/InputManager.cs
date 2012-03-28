using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Kinect;
using Microsoft.Xna.Framework.Graphics;

namespace BandMaster
{
    /// <summary>
    /// 
    /// </summary>
    class InputManager : GameComponent
    {
        // EventHandlers fire after some processing of the raw data from the kinect.
        public event EventHandler<VideoTextureReadyEventArgs> OnVideoTextureReady;
        public event EventHandler<DepthTextureReadyEventArgs> OnDepthTextureReady;
        public event EventHandler<SkeletonTrackingReadyEventArgs> OnSkeletonTrackingReady;
        // EventHandlers for changing 
        public event EventHandler<HandVelocityChangeEventArgs> OnHandVelocityChange;

        KinectManager kinect;

        Skeleton[] currSkeleton = null;
        Skeleton[] lastSkeleton = null;

        public InputManager(Game game) : base(game)
        {
            // 
            kinect = new KinectManager(KinectStreams.ColorStream);
            kinect.AddEventHandler(ColorStreamEventHandler);
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            kinect.Dispose();
        }

        /// <summary>
        /// Sets the RGB data from the Kinect to a texture and fires an
        /// OnVideoTextureReady when it transfered the bitmap to a texture.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ColorStreamEventHandler(object sender, ColorImageFrameReadyEventArgs e)
        {
            using (ColorImageFrame frame = e.OpenColorImageFrame())
            {
                if (frame != null)
                {
                    int length = frame.Width * frame.Height;
                    // Frame Width times FrameHeight times the number of bytes of each pixel, 4.
                    // One byte for Red, Green, Blue and one for Alpha
                    byte[] data = new byte[length * 4];
                    Color[] bitmap = new Color[length];

                    frame.CopyPixelDataTo(data);

                    for (int i = 0, offset = 0; i < length; ++i, offset += 4)
                    {
                        bitmap[i] = new Color(data[offset + 2], data[offset + 1], data[offset + 0], 255);
                    }

                    Texture2D texture = new Texture2D(Game.GraphicsDevice, frame.Width, frame.Height);
                    texture.SetData(bitmap);

                    OnVideoTextureReady.Invoke(this, new VideoTextureReadyEventArgs(texture));
                }
            }
        }

        /// <summary>
        /// Sets the depth data from the Kinect to a texture and
        /// fires an OnDepthTextureReady event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DepthStreamEventHandler(object sender, DepthImageFrameReadyEventArgs e)
        {
            using (DepthImageFrame frame = e.OpenDepthImageFrame())
            {
                int length = frame.Width * frame.Height;
                // The full kinect resolution with the player ID in the 3 least significant bits.
                short[] data = new short[length];
                // A bitmap to transfer the depth data to a texture.
                Color[] bitmap = new Color[length];

                frame.CopyPixelDataTo(data);

                for (int i = 0; i < length; ++i)
                {
                    // Removing the player id and the 4 least significant bits of the depthResolution.
                    int depth = (data[i] >> 3);
                    if (depth == kinect.UnknownDepth)
                    {
                        bitmap[i] = Color.Red;
                    }
                    else if (depth == kinect.TooFarDepth)
                    {
                        bitmap[i] = Color.Blue;
                    }
                    else if (depth == kinect.TooNearDepth)
                    {
                        bitmap[i] = Color.Green;
                    }
                    else
                    {
                        byte depthByte = (byte)(255 - (depth >> 5));
                        bitmap[i] = new Color(depthByte, depthByte, depthByte, 255);
                    }
                }

                Texture2D texture = new Texture2D(Game.GraphicsDevice, frame.Width, frame.Height);
                texture.SetData(bitmap);

                OnDepthTextureReady.Invoke(this, new DepthTextureReadyEventArgs(texture));
            }
        }

        private void SkeletonStreamEventHandler(object sender, SkeletonFrameReadyEventArgs e)
        {
            using (SkeletonFrame frame = e.OpenSkeletonFrame())
            {
                if (frame != null)
                {
                    lastSkeleton = currSkeleton;

                    frame.CopySkeletonDataTo(currSkeleton);

                    OnSkeletonTrackingReady.Invoke(this, new SkeletonTrackingReadyEventArgs(currSkeleton, lastSkeleton));
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SkeletonHandVelocityChangeEventHandler(object sender, SkeletonFrameReadyEventArgs e)
        {
            using (SkeletonFrame frame = e.OpenSkeletonFrame())
            {
                if (frame != null)
                {
                    lastSkeleton = currSkeleton;

                    frame.CopySkeletonDataTo(currSkeleton);

                    Joint lastRightHand = lastSkeleton[0].Joints[JointType.HandRight];
                    Joint currRightHand = currSkeleton[0].Joints[JointType.HandRight];

                    OnHandVelocityChange.Invoke(this, new HandVelocityChangeEventArgs());
                }
            }
        }
    }
}
