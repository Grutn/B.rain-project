using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Kinect;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BandMaster
{
    public class VideoTextureReadyEventArgs : EventArgs
    {
        Texture2D videoTexture;

        public Texture2D VideoTexture
        {
            get { return videoTexture; }
        }

        public VideoTextureReadyEventArgs(Texture2D videoTexture)
        {
            this.videoTexture = videoTexture;
        }
    }

    public class DepthTextureReadyEventArgs : EventArgs
    {
        Texture2D depthTexture;

        public Texture2D DepthTexture
        {
            get { return depthTexture; }
        }

        public DepthTextureReadyEventArgs(Texture2D depthTexture)
        {
            this.depthTexture = depthTexture;
        }
    }

    public class SkeletonTrackingReadyEventArgs : EventArgs
    {
        Skeleton[] current;
        Skeleton[] last;

        /// <summary>
        /// Returns the skeleton data for the current frame.
        /// </summary>
        public Skeleton[] Current
        {
            get { return current; }
        }

        /// <summary>
        /// Returns the skeleton data for the last frame.
        /// </summary>
        public Skeleton[] Last
        {
            get { return last; }
        }

        public SkeletonTrackingReadyEventArgs(Skeleton[] current, Skeleton[] last)
        {
            this.current = current;
            this.last = last;
        }
    }

    public class HandVelocityChangeEventArgs : EventArgs
    {
        Vector3 position;
        Vector3 velocity;

        public Vector3 Position
        {
            get { return position; }
        }

        public Vector3 Velocity
        {
            get { return velocity; }
        }

        public HandVelocityChangeEventArgs(Vector3 position, Vector3 velocity)
        {
            this.position = position;
            this.velocity = velocity;
        }
    }
}
