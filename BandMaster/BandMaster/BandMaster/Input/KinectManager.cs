using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Kinect;
using Microsoft.Xna.Framework.Media;


namespace BandMaster
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
    /// Kinect Wrapper
    /// </summary>
    public class KinectManager
    {

        #region Fields

        private KinectSensor kinect = null;
        private string errorMessage = "";

        #endregion

        #region Getters and Setters

        /// <summary>
        /// Gets a reference to the kinect sensor.
        /// </summary>
        public KinectSensor KinectSensor
        {
            get { return kinect; }
        }

        /// <summary>
        /// Returns the error message if any
        /// </summary>
        public string ErrorMessage
        {
            get { return errorMessage; }
            protected set { errorMessage = value; }
        }

        public int ColorFrameWidth
        {
            get { return kinect.ColorStream.FrameWidth; }
        }

        public int ColorFrameHeight
        {
            get { return kinect.ColorStream.FrameHeight; }
        }

        public int DepthFrameWidth
        {
            get { return kinect.DepthStream.FrameWidth; }
        }

        public int DepthFrameHeight
        {
            get { return kinect.DepthStream.FrameHeight; }
        }

        public int TooFarDepth
        {
            get { return kinect.DepthStream.TooFarDepth; }
        }

        public int TooNearDepth
        {
            get { return kinect.DepthStream.TooNearDepth; }
        }

        public int UnknownDepth
        {
            get { return kinect.DepthStream.UnknownDepth; }
        }

        #endregion

        /// <summary>
        /// Constructor:
        /// Sets up the Kinect sensor, and initates the streams
        /// given in the KinectStreams argument.
        /// If this constructor is called, Dispose() must be called
        /// or else the Kinect will never be turned off.
        /// </summary>
        /// <param name="kinectStreams"></param>
        public KinectManager(KinectStreams kinectStreams)
        {
            // Check if there is no Kinect Sensor
            if (KinectSensor.KinectSensors.Count == 0)
            {
                errorMessage = "No Kinects detected";
                return;
            }

            // Get a kinect sensor
            kinect = KinectSensor.KinectSensors[0];

            try
            {
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
            }
            catch
            {
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
                errorMessage = "Camera start failed";
                return;
            }
        }

        /// <summary>
        /// Stops the kinect camera and disposes of the resource.
        /// </summary>
        public void Dispose()
        {
            if (kinect != null)
            {
                kinect.Stop();
                kinect.Dispose();
            }
        }

        #region EventHandlers

        // Overloaded methods to add Event Handlers for ColorImage, DepthImage and SkeletonFrame
        public void AddEventHandler(EventHandler<ColorImageFrameReadyEventArgs> handler)
        {
            kinect.ColorFrameReady += handler;
        }

        public void AddEventHandler(EventHandler<DepthImageFrameReadyEventArgs> handler)
        {
            kinect.DepthFrameReady += handler;
        }

        public void AddEventHandler(EventHandler<SkeletonFrameReadyEventArgs> handler)
        {
            kinect.SkeletonFrameReady += handler;
        }

        #endregion
    }
}
