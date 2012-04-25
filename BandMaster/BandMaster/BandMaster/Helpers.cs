using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace BandMaster
{

    public class Helpers 
    {
        public static Game Game;

        public static float Clamp(float value, float min, float max)
        {
            return Math.Max(min, Math.Min(max, value));
        }
        public static float Scurve(float from, float to, float var)
        {
            float v = (float)(Math.Cos((Math.PI * var) + Math.PI) * 0.5 + 0.5);
            return from + v * (to - from);
        }
        public static float Lerp(float from, float to, float var)
        {
            return from * (1 - var) + to * var;
        }
        public static float EaseOut(float from, float to, float var)
        {
            return from + (float)Math.Sin((double)var * Math.PI / 2.0)*(to-from);
        }
        public static float EaseIn(float from, float to, float var)
        {
            return from + (float)(Math.Cos(var * Math.PI / 2.0  + Math.PI ) + 1.0) * (to-from); 
        }

        public delegate void SimpleDelegate();
        public delegate float AnimationDelegate(float from, float to, float var);

        public static GameComponent Wait(double seconds, SimpleDelegate then)
        {
            Effector ef = new Effector(0.0f);
            ef.Wait(seconds, then);
            return ef;
        }
    }

    public class Effector : GameComponent
    {
        bool started = true;

        public float Value;

        float from, to;
        double start, time;
        Helpers.SimpleDelegate callback = null;
        Helpers.AnimationDelegate func = null;

        public Effector(float init=0.0f) :
            base(Helpers.Game)
        {
            Value = init;
        }
        public void Animate(float from, float to, double time, Helpers.AnimationDelegate func, Helpers.SimpleDelegate callback)
        {
            this.func = func;
            this.callback = callback;
            this.from = from;
            this.to = to;
            this.time = time;
            this.started = false;
            if (!Game.Components.Contains(this))
                Game.Components.Add(this);
        }


        public void EaseOut(double time = 0.2, float from = 0.0f, float to = 1.0f, Helpers.SimpleDelegate callback = null)
        {
            Animate(from, to, time, Helpers.EaseOut, callback);
        }
        public void EaseIn(double time = 0.2, float from = 0.0f, float to = 1.0f, Helpers.SimpleDelegate callback = null)
        {
            Animate(from, to, time, Helpers.EaseIn, callback);
        }
        public void Lerp(double time = 0.2, float from = 0.0f, float to = 1.0f, Helpers.SimpleDelegate callback = null)
        {
            Animate(from, to, time, Helpers.Lerp, callback);
        }
        public void Scurve(double time = 0.2, float from = 0.0f, float to = 1.0f, Helpers.SimpleDelegate callback = null)
        {
            Animate(from, to, time, Helpers.Scurve, callback);
        }
        public void Wait(double time, Helpers.SimpleDelegate callback = null)
        {
            Animate(Value, Value, time, null, callback);
        }

        public override void Update(GameTime gameTime)
        {
            if (!started)
            {
                started = true;
                start = gameTime.TotalGameTime.TotalSeconds;
            }

            double dt = gameTime.TotalGameTime.TotalSeconds - start;

            if (dt >= time)
            {
                Game.Components.Remove(this);
                Value = to;
                if (callback != null) callback();
            }
            else
            {
                Value = (func == null) ? to : func(from, to, (float)(dt / time));
            }

            base.Update(gameTime);
        }
    }
    
}
