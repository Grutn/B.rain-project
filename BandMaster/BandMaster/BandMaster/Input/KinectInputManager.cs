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
        private Skeleton[] skeleton = null;

        private JointType activeHand;
        private JointType offHand;

        private bool isRightHit = true;

        private bool isReady = false;

        // Right and Left Threshold for tempo hit
        private float right;
        private float left;

        private int colorImageWidth = 640;
        private int colorImageHeight = 480;

        private int skeletonIndex;
        
        // Debugging variables
        private KinectDebug debug = null;

        #endregion

        #region Getters and Setters

        public KinectSensor Kinect
        {
            get { return kinect; }
        }

        public bool IsRightHit
        {
            get { return isRightHit; }
            set { isRightHit = value; }
        }

        /// <summary>
        /// Used to change tracking of the active hand between right and left hand.
        /// Automatically changes the offhand to the opposit of the active hand.
        /// </summary>
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

        // Only used in this class
        private ColorImageFormat ColorImageFormat
        {
            get { return ColorImageFormat.RgbResolution640x480Fps30; }
        }

        // Only used in this class
        private Skeleton Skeleton
        {
            get { return skeleton[skeletonIndex]; }
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
                // Get 3D position of joint.
                SkeletonPoint sp = Skeleton.Joints[activeHand].Position;

                // Project the joint onto the color image (there's only a color image when the debug variable is set).
                ColorImagePoint cp = kinect.MapSkeletonPointToColor(sp, ColorImageFormat);

                // Get the size of the viewport.
                Viewport viewport = Game.GraphicsDevice.Viewport;

                // Scale from the color image to viewport.
                return new Vector2(cp.X * ((float)viewport.Width / (float)colorImageWidth), cp.Y * ((float)viewport.Height / (float)colorImageHeight));
            }
        }

        public Vector2 LeftHand
        {
            get
            {
                // Get 3D position of joint.
                SkeletonPoint sp = Skeleton.Joints[offHand].Position;

                // Project the joint onto the color image (there's only a color image when the debug variable is set).
                ColorImagePoint cp = kinect.MapSkeletonPointToColor(sp, ColorImageFormat);

                // Get the size of the viewport.
                Viewport viewport = Game.GraphicsDevice.Viewport;

                // Scale from color image to the viewport.
                return new Vector2(cp.X * ((float)viewport.Width / (float)colorImageWidth), cp.Y * ((float)viewport.Height / (float)colorImageHeight));
            }
        }

        public Rectangle Thresholds
        {
            get 
            {
                Vector3 va = SkeletonPointToVector3(Skeleton, JointType.HipRight);

                // SkeletonPoint for the left detection edge
                SkeletonPoint spLeft = new SkeletonPoint
                {
                    X = left,
                    Y = va.Y,
                    Z = va.Z
                };

                // SkeletonPoint for the right detection edge
                SkeletonPoint spRight = new SkeletonPoint
                {
                    X = right,
                    Y = va.Y,
                    Z = va.Z
                };
                
                ColorImagePoint cpl = kinect.MapSkeletonPointToColor(spLeft, ColorImageFormat);
                ColorImagePoint cpr = kinect.MapSkeletonPointToColor(spRight, ColorImageFormat);
                ColorImagePoint cph = kinect.MapSkeletonPointToColor(Skeleton.Joints[JointType.ShoulderCenter].Position, ColorImageFormat);
                ColorImagePoint cpb = kinect.MapSkeletonPointToColor(Skeleton.Joints[JointType.HipCenter].Position, ColorImageFormat);

                Viewport viewport = Game.GraphicsDevice.Viewport;
                // Be sure to keep all decimals
                float xScale = (float)viewport.Width / (float)colorImageWidth;
                float yScale = (float)viewport.Height / (float)colorImageHeight;

                return new Rectangle((int)(cpl.X * xScale), (int)(cph.Y * yScale), (int)((cpr.X - cpl.X) * xScale), (int)((cpl.Y - cph.Y) * yScale));
            }
        }

        // TODO: Values for the properties above?

        #endregion

        /// <summary>
        /// Constructor for the KinectInputManager. The constructor doesn't 
        /// do any exception handling so the attempting to create a 
        /// KinectInputManager object should be wrapped in a try catch block.
        /// </summary>
        /// <param name="game"></param>
        public KinectInputManager(Game game) : base(game)
        {
            // Get a kinect sensor
            kinect = KinectSensor.KinectSensors[0];

            // For non-debug mode comment out the KinectDebug constr below
            debug = new KinectDebug(game, this);

            // Standard setup is right handed controlls
            activeHand = JointType.HandRight;
            offHand = JointType.HandLeft;

            // Start at the first skeleton in the device's skeleton collection
            skeletonIndex = 0;

            // Tell the kinect to start tracking people
            kinect.SkeletonStream.Enable();

            // Add an event handler to transform skeleton into controller outputs
            kinect.SkeletonFrameReady += new EventHandler<SkeletonFrameReadyEventArgs>(SkeletonStreamEventHandler);

            if (debug != null)
            {
                // Comment out code to remove debug visuals
                // debug.EnableColorImage = true;
                // debug.EnableLog = true;
                debug.EnableEdgeAndHand = true;

                if (debug.EnableColorImage)
                {
                    kinect.ColorStream.Enable();
                    kinect.ColorFrameReady += debug.OnColorFrameReady;
                }
                if (debug.EnableLog)
                {
                    //OnPlayerEvent += debug.OnPlayerData;
                }

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
            // Loop through skeleton collection to see if any skeleton is tracked.
            // App chooses skeleton, so picks a "random" skeleton when it begins tracking a person.
            if (skeleton != null && skeleton[skeletonIndex].TrackingState != SkeletonTrackingState.Tracked)
            {
                for (int i = 0; i < skeleton.Length; ++i)
                {
                    // Are any skeletons being tracked
                    if (skeleton[i].TrackingState == SkeletonTrackingState.Tracked)
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
                    if (skeleton == null)
                    {
                        skeleton = new Skeleton[frame.SkeletonArrayLength];
                    }

                    // Initialize and zero local variables
                    Vector3 activePos   = Vector3.Zero;
                    Vector3 offPos      = Vector3.Zero;

                    // copy over data from the event arg
                    frame.CopySkeletonDataTo(skeleton);

                    // The KinectInputManager has data to poll
                    isReady = true;

                    //shoulderRight.X + (shoulderRight.X - shoulderCenter.X)
                    Vector3 hipRight = SkeletonPointToVector3(Skeleton, JointType.HipRight);
                    Vector3 hipCenter = SkeletonPointToVector3(Skeleton, JointType.HipCenter);

                    // Left and Right edge detection for OnTempoHit
                    left = hipRight.X + (hipRight.X - hipCenter.X);
                    right = left + 1.8f * (hipRight.X - hipCenter.X);
                    
                    // Current position of active hand and offhand
                    activePos = SkeletonPointToVector3(Skeleton, activeHand);
                    offPos    = SkeletonPointToVector3(Skeleton, offHand);

                    // If currActivePos is a zero vector the kinect has no data on the active hand
                    if (activePos != Vector3.Zero)
                    {
                        bool isRight =  IsRightHit && activePos.X < left;
                        bool isLeft  = !IsRightHit && activePos.X > right;

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

                    if (offPos != Vector3.Zero)
                    {
                        // TODO: Dynamic controlls
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
