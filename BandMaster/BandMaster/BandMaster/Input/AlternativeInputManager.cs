using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace BandMaster.Input
{
    public class AlternativeInputManager : GameComponent, IManageInput
    {
        public event EventHandler OnTempoHit; // on kinect: detect in kinectinput subclass; on keyboard: some key event
        public event EventHandler OnRestart;

        private KeyboardState currState;
        private KeyboardState lastState;

        // TODO: Standard values for the properties below?
        public bool IsReady
        {
            get { return true; }
        }
        
        Vector2 pos = new Vector2();

        public Vector2 RightHand
        {
            get { return new Vector2(pos.X, pos.Y); }
        }

        public Vector2 LeftHand
        {
            get { return new Vector2(pos.X - 200.0f, pos.Y); }
        }

        public Rectangle Thresholds
        {
            get { return new Rectangle(500,100,400,400); }
        }
        // TODO: Standard value for the properties above?

        public AlternativeInputManager(Game game) : base(game)
        {
        }

        public override void Update(GameTime gameTime)
        {
            MouseState m = Mouse.GetState();
            pos.X = (float)m.X;
            pos.Y = (float)m.Y;


            lastState = currState;
            currState = Keyboard.GetState();

            if (IsNewKeyPress(Keys.J))
            {
                if (OnTempoHit != null)
                {
                    OnTempoHit(this, null);
                }
            }

            if (IsNewKeyPress(Keys.Space))
            {
                if (OnRestart != null)
                {
                    OnRestart(this, null);
                }
            }
            
        }

        private bool IsNewKeyPress(Keys key)
        {
            return currState.IsKeyDown(key) && (!lastState.IsKeyDown(key));
        }
    }
}
