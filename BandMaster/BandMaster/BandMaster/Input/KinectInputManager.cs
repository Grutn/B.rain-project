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
    /// 
    /// </summary>
    public class KinectInputManager : GameComponent, IManageInput
    {
        #region EventHandlers

        public event EventHandler<PlayerEvent> OnPlayerEvent;   // on kinect: all movements of hands; on keyboard: mouse movements
        public event EventHandler OnTempoHit;                   // on kinect: detect in kinectinput subclass; on keyboard: some key event
        public event EventHandler OnDynamicHit;                 // same as above

        #endregion

        #region Fields

        private KinectSensor kinect = null;
        private string errorMessage = "";

        // Variables for holding last and current events Skeleton collection
        private Skeleton[] currSkeleton = null;
        private Skeleton[] lastSkeleton = null;

        private Vector3[] activePos;
        private Vector3[] offPos;

        private int posIndex = 0;

        private JointType activeHand;
        private JointType offHand;

        private int skeletonIndex;

        private const int numDataPoints = 10;
        
        // Debug variable for drawing
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

        public Skeleton[] LastSkeleton
        {
            get { return lastSkeleton; }
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

            activePos = new Vector3[numDataPoints];
            offPos = new Vector3[numDataPoints];

            skeletonIndex = 0;

            kinect.SkeletonStream.Enable();

            kinect.SkeletonFrameReady += new EventHandler<SkeletonFrameReadyEventArgs>(SkeletonStreamEventHandler);

            if (debug != null)
            {
                kinect.ColorStream.Enable();
                kinect.ColorFrameReady += debug.OnColorFrameReady;

                OnPlayerEvent += debug.OnPlayerData;

                Game.Components.Add(debug);
            }

            kinect.Start();
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void Dispose(bool disposing)
        {
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
                    if (currSkeleton[i].TrackingState == SkeletonTrackingState.Tracked)
                    {
                        System.Console.WriteLine("Index {0} Skeleton is tracked", i);
                        skeletonIndex = i;
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
                    if (currSkeleton == null)
                    {
                        currSkeleton = new Skeleton[frame.SkeletonArrayLength];
                        lastSkeleton = new Skeleton[frame.SkeletonArrayLength];
                    }

                    // Initialize and zero local variables
                    Vector3 currVelocity    = Vector3.Zero;
                    Vector3 lastActivePos   = Vector3.Zero;
                    Vector3 lastOffPos      = Vector3.Zero;
                    Vector3 currActivePos   = Vector3.Zero;
                    Vector3 currOffPos      = Vector3.Zero;


                    // Set current frame to last frame
                    lastSkeleton = currSkeleton;
                    // copy over data from the event arg
                    frame.CopySkeletonDataTo(currSkeleton);
                    
                    currActivePos = SkeletonPointToVector3(currSkeleton[skeletonIndex], activeHand);
                    currOffPos    = SkeletonPointToVector3(currSkeleton[skeletonIndex], offHand);

                    lastActivePos = SkeletonPointToVector3(lastSkeleton[skeletonIndex], activeHand);
                    lastOffPos    = SkeletonPointToVector3(lastSkeleton[skeletonIndex], offHand);

                    currVelocity  = lastActivePos - currActivePos;

                    // Setup values for PlayerEvent
                    Hand hand = (activeHand == JointType.HandRight) ? Hand.Right : Hand.Left;
                    JointType elbow = (hand == Hand.Right) ? JointType.ElbowRight : JointType.ElbowLeft;

                    Vector3 direction = currActivePos - SkeletonPointToVector3(currSkeleton[skeletonIndex], elbow);

                    // Dispatch PlayerEvent for any movement
                    if (OnPlayerEvent != null)
                    {
                        OnPlayerEvent.Invoke(this, new PlayerEvent(hand, direction, currActivePos, currVelocity));
                    }
                    
                    // Check for change in velocity direction
                    if (true)
                    {
                        // Dispatch OnTempoHit event for 
                        if (OnTempoHit != null)
                        {
                            OnTempoHit.Invoke(this, new PlayerEvent(hand, direction, currActivePos, currVelocity));
                        }
                    }
                }
            }
        }

        private Vector3 SkeletonPointToVector3(Skeleton skeleton, JointType joint)
        {
            SkeletonPoint sPoint = skeleton.Joints[joint].Position;
            return new Vector3(sPoint.X, sPoint.Y, sPoint.Z);
        }
    }
}
