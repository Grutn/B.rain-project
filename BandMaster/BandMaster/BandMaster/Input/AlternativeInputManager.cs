﻿using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace BandMaster.Input
{
    public class AlternativeInputManager : GameComponent, IManageInput
    {
        private KeyboardState currState;
        private KeyboardState lastState;

        public event EventHandler<PlayerEvent> OnPlayerEvent; // on kinect: all movements of hands; on keyboard: mouse movements
        public event EventHandler OnTempoHit; // on kinect: detect in kinectinput subclass; on keyboard: some key event
        public event EventHandler OnDynamicHit; // same as above

        public AlternativeInputManager(Game game) : base(game)
        {
        }

        public override void Update(GameTime gameTime)
        {
            lastState = currState;
            currState = Keyboard.GetState();

            if (IsNewKeyPress(Keys.J))
            {
                OnTempoHit(this, null);
            }
            if (IsNewKeyPress(Keys.F))
            {
                OnDynamicHit(this, new PlayerEvent(Hand.Right, Vector3.Zero, Vector3.Zero, Vector3.Zero));
            }
            
        }

        private bool IsNewKeyPress(Keys key)
        {
            return currState.IsKeyDown(key) && lastState.IsKeyDown(key);
        }
    }
}