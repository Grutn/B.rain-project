using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Kinect;

namespace BandMaster.Input
{
    /// <summary>
    /// Used to define which streams to enable on the Kinect Device.
    /// </summary>
    public enum KinectStreams
    {
        None = 0,
        ColorStream = 1,
        DepthStream = 2,
        SkeletonStream = 4,
    }

     /// <summary>
    /// 
    /// </summary>
    public class KinectInputManager : GameComponent, IManageInput
    {
        public event EventHandler<PlayerEvent> OnPlayerEvent;   // on kinect: all movements of hands; on keyboard: mouse movements
        public event EventHandler OnTempoHit;                   // on kinect: detect in kinectinput subclass; on keyboard: some key event
        public event EventHandler OnDynamicHit;                 // same as above

        private KinectSensor kinect = null;
        private string errorMessage = "";

        private Skeleton[] currSkeleton = null;
        private Skeleton[] lastSkeleton = null;

        private Vector3 lastVelocity;

        private JointType activeHand;
        private JointType offHand;

        public JointType ActiveHand
        {
            set
            {
                activeHand = value;
                // Change offhand by checking active hand
                offHand = (activeHand == JointType.HandLeft) ? JointType.HandRight : JointType.HandLeft;
            }
        }

        public KinectInputManager(Game game) : base(game)
        {
            activeHand = JointType.HandRight;
            offHand = JointType.HandLeft;

            // Check if there is no Kinect Sensor
            if (KinectSensor.KinectSensors.Count == 0)
            {
                throw new NotImplementedException();
                /*
                 * errorMessage = "No Kinects detected";
                return;
                 * */
            }

            // Get a kinect sensor
            kinect = KinectSensor.KinectSensors[0];
            
            kinect.SkeletonStream.Enable();


            kinect.Start();

            /*
            try
            {
                kinect.SkeletonStream.Enable();
                
                // Check if no stream has been started
                if (kinectStreams == KinectStreams.None)
                {
                    errorMessage = "No streams initiated";
                    return;
                }
                // Check if the color stream is to be enabled
                if ((kinectStreams & KinectStreams.ColorStream) != 0)
                {
                    kinect.ColorStream.Enable();
                }
                // Check if the depth stream is to be enabled
                if ((kinectStreams & KinectStreams.DepthStream) != 0)
                {
                    kinect.DepthStream.Enable();
                }
                // Check if the skeleton stream is to be enabled
                if ((kinectStreams & KinectStreams.SkeletonStream) != 0)
                {
                    kinect.SkeletonStream.Enable();
                }
                 * 
            }
            catch
            {
                throw new NotImplementedException();
                
                errorMessage = "Kinect initialize failed";
                return;
            }

            // Try to start the Kinect.
            try
            {
                kinect.Start();
            }
            catch
            {
                throw new NotImplementedException();
                errorMessage = "Camera start failed";
                return;
               
            }
            */
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (kinect != null)
            {
                // Does this work in destructor?
                kinect.Stop();
                kinect.Dispose();
            }
        }

        private void SkeletonStreamEventHandler(object sender, SkeletonFrameReadyEventArgs e)
        {
            using (SkeletonFrame frame = e.OpenSkeletonFrame())
            {
                if (frame != null)
                {
                    // Set current frame to last frame
                    lastSkeleton = currSkeleton;
                    // copy over data from the event arg
                    frame.CopySkeletonDataTo(currSkeleton);

                    // Convert SkeletonPoints to Vector3
                    Vector3 lastActivePos = SkeletonPointToVector3(lastSkeleton[0], activeHand);
                    Vector3 currActivePos = SkeletonPointToVector3(currSkeleton[0], activeHand);

                    Vector3 lastOffPos = SkeletonPointToVector3(lastSkeleton[0], offHand);
                    Vector3 currOffPos = SkeletonPointToVector3(lastSkeleton[0], offHand);

                    // Calculate change from last frame to the current one
                    Vector3 currVelocity = lastActivePos - currActivePos;

                    // Setup values for PlayerEvent
                    Hand hand = (activeHand == JointType.HandRight) ? Hand.Right : Hand.Left;
                    JointType elbow = (hand == Hand.Right) ? JointType.ElbowRight : JointType.ElbowLeft;

                    Vector3 direction = currActivePos - SkeletonPointToVector3(currSkeleton[0], elbow);

                    // Dispatch PlayerEvent for any movement
                    OnPlayerEvent.Invoke(this, new PlayerEvent(hand, direction, currActivePos, currVelocity));
                    
                    // Check for change in velocity direction
                    if ((lastVelocity.X < 0 && currVelocity.X > 0) || (lastVelocity.X > 0 && currVelocity.X < 0))
                    {
                        // Dispatch OnTempoHit event for 
                        OnTempoHit.Invoke(this, new PlayerEvent(hand, direction, currActivePos, currVelocity));
                    }

                    lastVelocity = currVelocity;
                }
            }
        }

        private Vector3 SkeletonPointToVector3(Skeleton skeleton, JointType joint)
        {
            SkeletonPoint sPoint = skeleton.Joints[joint].Position;
            return new Vector3(sPoint.X, sPoint.Y, sPoint.Z);
        }

        /*
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

                    //OnVideoTextureReady.Invoke(this, new VideoTextureReadyEventArgs(texture));
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
         * */
    }
}
