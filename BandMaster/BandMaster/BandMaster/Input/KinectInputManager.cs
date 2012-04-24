using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Kinect;

namespace BandMaster.Input
{
    public class KinectInputManager : GameComponent, IManageInput
    {
        #region EventHandlers

        public event EventHandler OnPlayerEvent;
        public event EventHandler OnTempoHit;                   // on kinect: detect in kinectinput subclass; on keyboard: some key event
        public event EventHandler OnDynamicHit;                 // same as above

        #endregion

        #region Fields

        private KinectSensor kinect = null;

        // Variables for holding last and current events Skeleton collection
        private Skeleton[] currSkeleton = null;

        private JointType activeHand;
        private JointType offHand;

        private bool isRightHit;

        private bool isReady;

        // Right and Left Threshold for tempo hit
        float right;
        float left;

        private int skeletonIndex;

        private const int numDataPoints = 10;
        
        // Debugging variables
        private KinectDebug debug = null;

        #endregion

        #region Getters and Setters

        public KinectSensor Kinect
        {
            get { return kinect; }
        }

        public Skeleton[] CurrSkeleton
        {
            get { return currSkeleton; }
        }

        public bool IsRightHit
        {
            get { return isRightHit; }
            set { isRightHit = value; }
        }

        public Vector2 CurrActivePos
        {
            get
            {
                ColorImagePoint point = kinect.MapSkeletonPointToColor(currSkeleton[skeletonIndex].Joints[activeHand].Position, ColorImageFormat.RgbResolution640x480Fps30);
                return new Vector2(point.X, point.Y);
            }
        }

        public int RightLine
        {
            get
            {
                SkeletonPoint shoulderRight = currSkeleton[skeletonIndex].Joints[JointType.HipRight].Position;
                SkeletonPoint sPoint = new SkeletonPoint
                {
                    X = right,
                    Y = shoulderRight.Y,
                    Z = shoulderRight.Z
                };

                ColorImagePoint point = kinect.MapSkeletonPointToColor(sPoint, ColorImageFormat.RgbResolution640x480Fps30);
                return point.X;
            }
        }

        public int LeftLine
        {
            get
            {
                SkeletonPoint shoulderRight = currSkeleton[skeletonIndex].Joints[JointType.HipRight].Position;
                SkeletonPoint sPoint = new SkeletonPoint
                {
                    X = left,
                    Y = shoulderRight.Y,
                    Z = shoulderRight.Z
                };

                ColorImagePoint point = kinect.MapSkeletonPointToColor(sPoint, ColorImageFormat.RgbResolution640x480Fps30);
                return point.X;
            }
        }

        public JointType ActiveHand
        {
            get { return activeHand; }
            set
            {
                activeHand = value;
                // Change offhand by checking active hand
                offHand = (activeHand == JointType.HandLeft) ? JointType.HandRight : JointType.HandLeft;
            }
        }

        // TODO: Values for the properties below?
        public bool IsReady
        {
            get { return isReady; }
        }

        public Vector2 RightHand
        {
            get
            {
                SkeletonPoint sp = currSkeleton[skeletonIndex].Joints[activeHand].Position;
                ColorImagePoint cp = kinect.MapSkeletonPointToColor(sp, ColorImageFormat.RgbResolution640x480Fps30);

                return Vector2.Zero;
            }
        }

        public Vector2 LeftHand
        {
            get
            {
                SkeletonPoint sp = currSkeleton[skeletonIndex].Joints[offHand].Position;
                ColorImagePoint cp = kinect.MapSkeletonPointToColor(sp, ColorImageFormat.RgbResolution640x480Fps30);

                return Vector2.Zero;
            }
        }

        public Rectangle Thresholds
        {
            get { return new Rectangle(); }
        }

        // TODO: Values for the properties above?

        #endregion

        public KinectInputManager(Game game) : base(game)
        {
            // Get a kinect sensor
            kinect = KinectSensor.KinectSensors[0];

            // For non-debug mode comment out the KinectDebug constr below
            debug = new KinectDebug(game, this);

            // Standard setup is right handed controlls
            activeHand = JointType.HandRight;
            offHand = JointType.HandLeft;

            IsRightHit = true;

            skeletonIndex = 0;

            kinect.SkeletonStream.Enable();

            kinect.SkeletonFrameReady += new EventHandler<SkeletonFrameReadyEventArgs>(SkeletonStreamEventHandler);

            if (debug != null)
            {
                kinect.ColorStream.Enable();
                kinect.ColorFrameReady += debug.OnColorFrameReady;

                //OnPlayerEvent += debug.OnPlayerData;

                Game.Components.Add(debug);
            }

            kinect.Start();
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            // If the kinect has been instantiated and started
            // it must be stopped and disposed to shut down the camera
            if (kinect != null)
            {
                kinect.Stop();
                kinect.Dispose();
            }
        }

        public override void Update(GameTime gameTime)
        {
            if (currSkeleton != null && currSkeleton[skeletonIndex].TrackingState != SkeletonTrackingState.Tracked)
            {
                for (int i = 0; i < currSkeleton.Length; ++i)
                {
                    // Are any skeletons being tracked
                    if (currSkeleton[i].TrackingState == SkeletonTrackingState.Tracked)
                    {
                        // Set the tracked skeleton index to the tracked skeleton
                        System.Console.WriteLine("Index {0} Skeleton is tracked", i);
                        skeletonIndex = i;
                        break;
                    }
                }
            }
        }

        private void SkeletonStreamEventHandler(object sender, SkeletonFrameReadyEventArgs e)
        {
            using (SkeletonFrame frame = e.OpenSkeletonFrame())
            {
                if (frame != null)
                {
                    // If skeleton not instantiated
                    if (currSkeleton == null)
                    {
                        currSkeleton = new Skeleton[frame.SkeletonArrayLength];
                    }

                    // Initialize and zero local variables
                    Vector3 currActivePos   = Vector3.Zero;
                    Vector3 currOffPos      = Vector3.Zero;

                    // copy over data from the event arg
                    frame.CopySkeletonDataTo(currSkeleton);

                    //shoulderRight.X + (shoulderRight.X - shoulderCenter.X)
                    Vector3 hipRight = SkeletonPointToVector3(currSkeleton[skeletonIndex], JointType.HipRight);
                    Vector3 hipCenter = SkeletonPointToVector3(currSkeleton[skeletonIndex], JointType.HipCenter);

                    left = hipRight.X + (hipRight.X - hipCenter.X);
                    right = left + 2.0f * (hipRight.X - hipCenter.X);
                    
                    // Current position of active hand and offhand
                    currActivePos = SkeletonPointToVector3(currSkeleton[skeletonIndex], activeHand);
                    currOffPos    = SkeletonPointToVector3(currSkeleton[skeletonIndex], offHand);

                    // Dispatch PlayerEvent for any movement
                    // Check if OnPlayerEvent has value and no point in sending data where position is zero, as this doesn't happen
                    if (currActivePos != Vector3.Zero)
                    {
                        //OnPlayerEvent.Invoke(this, new PlayerEvent());

                        bool isRight =  IsRightHit && currActivePos.X < left;
                        bool isLeft  = !IsRightHit && currActivePos.X > right;

                        if (isRight || isLeft)
                        {
                            IsRightHit = !IsRightHit;
                            // Dispatch OnTempoHit event for 
                            if (OnTempoHit != null)
                            {
                                OnTempoHit.Invoke(this, new PlayerEvent());
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Extracts the joint from a skeleton and converts
        /// the positional data to Vector3
        /// </summary>
        /// <param name="skeleton">Skeleton from which to extract joint</param>
        /// <param name="joint">Joint to convert</param>
        /// <returns></returns>
        private Vector3 SkeletonPointToVector3(Skeleton skeleton, JointType joint)
        {
            SkeletonPoint sPoint = skeleton.Joints[joint].Position;
            return new Vector3(sPoint.X, sPoint.Y, sPoint.Z);
        }
    }
}
