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
        public Hand Hand;
        public Vector3 Direction;
        public Vector3 Position;
        public Vector3 Velocity;
    }

    public interface IManageInput : IGameComponent
    {
        event EventHandler<PlayerEvent> OnPlayerEvent; // on kinect: all movements of hands; on keyboard: mouse movements
        event EventHandler OnTempoHit; // on kinect: detect in kinectinput subclass; on keyboard: some key event
        event EventHandler OnDynamicHit; // same as above
    }
}
