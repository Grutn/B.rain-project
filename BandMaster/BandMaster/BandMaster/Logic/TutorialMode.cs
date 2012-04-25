using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;


using BandMaster.State;

namespace BandMaster.Logic
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class TutorialMode : Microsoft.Xna.Framework.GameComponent, IMode
    {
        Graphics.SplashText splasher;
        Input.IManageInput input;

        int rytmer;
        void tellRytmer(Object o, EventArgs a)
        {
            if (++rytmer == 6)
            {
                input.OnTempoHit -= tellRytmer;
                splasher.Write("Fram og tilbake..\n          "+rytmer + " / 6", Color.White);
                Helpers.Wait(1.0, delegate()
                {
                    splasher.Write("Fantastisk!", Color.White);

                    Helpers.Wait(1.0, delegate()
                    {
                        splasher.Write("Med venstre hånd styrer du dynamikken", Color.White, 3.0);
                        Helpers.Wait(8.0, delegate() {
                            splasher.Write("Klar?", Color.White);
                            ((BandMaster)Game).Mode = ((BandMaster)Game).Play;

                        });
                 
                    });
                });
            }
            else
                splasher.Write("Fram og tilbake..\n          " + rytmer + " / 6", Color.White);

        }
        public TutorialMode(Game game)
            : base(game)
        {
            ((BandMaster)Game).ModeChanged += delegate(Object o, EventArgs a)
            {
                if (((BandMaster)Game).Mode == this)
                {
                    Helpers.Wait(4.0, delegate()
                    {
                        splasher.Write("Beveg høyre hånd til siden", Color.White, 3.0f);

                        EventHandler e = null; e = delegate(Object o2, EventArgs a2)
                        {
                            input.OnTempoHit -= e;

                            // TODO: spill lyd
                            splasher.Write("Flott!", Color.White, 1.0f);
                            Helpers.Wait(1.0, delegate()
                            {
                                splasher.Write("Slik holder du rytmen", Color.White, 2.0f);
                                rytmer = 0;
                                input.OnTempoHit += tellRytmer;
                            });
                        };
                        input.OnTempoHit += e;

                    });
                }
            };
        }


        protected override void OnEnabledChanged(object sender, EventArgs args)
        {
            if (Enabled)
            {
                startWaitingForRythm();
            }
            else
            {
            }
        }

        void startWaitingForRythm()
        {
            splasher.Write("Vift med høyre hånd for å styre rytmen", Color.White);

        }
        void startWaitingForDynamics()
        {
            splasher.Write("Vift med venstre hånd for å styre dynamikk", Color.White);

        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            input = (Input.IManageInput)Game.Services.GetService(typeof(Input.IManageInput));
            splasher = (Graphics.SplashText)Game.Services.GetService(typeof(Graphics.SplashText));

            base.Initialize();
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {

            base.Update(gameTime);
        }
    }
}
