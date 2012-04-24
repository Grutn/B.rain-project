using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Kinect;
using Microsoft.Xna.Framework.Graphics;

namespace BandMaster.Input
{
    public enum Hand
    {
        Left,
        Right
    }

    public class PlayerEvent : EventArgs
    {
        public Hand hand;
        public Vector3 direction;
        public Vector3 position;
        public Vector3 velocity;

        public PlayerEvent() { }

        public PlayerEvent(Hand hand, Vector3 direction, Vector3 position, Vector3 velocity)
        {
            this.hand = hand;
            this.direction = direction;
            this.position = position;
            this.velocity = velocity;
        }
    }

    public interface IManageInput : IGameComponent
    {
        event EventHandler OnPlayerEvent;
        event EventHandler OnTempoHit;                  // on kinect: detect in kinectinput subclass; on keyboard: some key event
        event EventHandler OnDynamicHit;                // same as above

        bool IsReady { get; }

        Vector2 RightHand { get; }
        Vector2 LeftHand { get; }

        Rectangle Thresholds { get; }
    }
}
