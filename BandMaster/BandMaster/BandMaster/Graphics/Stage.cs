using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using BandMaster.State;


namespace BandMaster.Graphics
{
    /// <summary>
    /// This is a game component that implements DrawableGameComponent.
    /// </summary>
    public class Stage : Microsoft.Xna.Framework.DrawableGameComponent
    {
        SpriteBatch sprites;
        Audio.Midi.Player midiPlayer;
        Graphics.FlyingNotes notes;
        Audio.AudioFx fx;

        List<Instrument> Band = new List<Instrument>();

        Line line;

        Texture2D background, stand, metronome, metronomeSlider, logo;
        
        public Stage(Game game)
            : base(game)
        {
            line = new Line(game);
            
        }

        double score = 0.0;
        double scoreLP = 0.0;

        Effector stagePos = new Effector(0.0f);
        Effector metronomePos = new Effector(0.0f);
        Effector dinPos = new Effector();
        Effector scorePos = new Effector();
        Effector pressStartAlpha = new Effector();
        Effector logoAlpha = new Effector();

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            sprites = (SpriteBatch)Game.Services.GetService(typeof(SpriteBatch));
            midiPlayer = (Audio.Midi.Player)Game.Services.GetService(typeof(Audio.Midi.Player));
            notes = (FlyingNotes)Game.Services.GetService(typeof(FlyingNotes));
            fx = (Audio.AudioFx)Game.Services.GetService(typeof(Audio.AudioFx));

            background = Game.Content.Load<Texture2D>("Textures/bg"); 
            stand = Game.Content.Load<Texture2D>("Textures/notestativ");
            metronome = Game.Content.Load<Texture2D>("Textures/metronome512");
            metronomeSlider = Game.Content.Load<Texture2D>("Textures/metronome-slider");
            logo = Game.Content.Load<Texture2D>("Textures/logo");

            line.Initialize();

            //((BandMaster)Game).StateChanged += OnGameStateChanged;

            midiPlayer.NotePlayed += delegate(Object o, EventArgs a)
            {
                int instr = (int)o;
                Point c = Band[instr].Bounds.Center;
                notes.Emit(new Vector2(c.X,c.Y - 100));
            };

            BandMaster bm = (BandMaster)Game;

            bm.SongLoaded += delegate(Object s, EventArgs a)
            {
                State.Song song = bm.Song;

                Band.Clear();

                // Create instruments and figure out total width 
                float bandWidth = 0.0f;
                foreach (string instrumentName in song.Instruments)
                {
                    Instrument instrument = new Instrument(Game, instrumentName);
                    Band.Add(instrument);
                    bandWidth += instrument.Bounds.Width;
                }

                // find a suitable scale for our band
                Rectangle stageBounds = new Rectangle(150, 20, Game.GraphicsDevice.Viewport.Width - 450, 500);
                float bandScale = stageBounds.Width / bandWidth;

                // place and scale our band
                int x = 0, y = 100;

                foreach (Instrument instrument in Band)
                {
                    int w = (int)((float)instrument.Bounds.Width * bandScale);
                    int h = (int)((float)instrument.Bounds.Height * bandScale);
                    instrument.Bounds = new Rectangle(stageBounds.X + x, stageBounds.Y + y, w, h);
                    instrument.OffsetY.Value = -800.0f;
                    x += w;

                }

                // Deus ex machina effect
                for (int i = 0; i < Band.Count; i++)
                {
                    Effector inst = Band[i].OffsetY;
                    Helpers.Wait(0.4 * i, delegate() { inst.EaseOut(1.0, -800.0f, 0.0f); });
                }

                // Animate stage
                Helpers.Wait(2.5, delegate()
                {
                    stagePos.EaseOut(1.2);
                    Helpers.Wait(1.0, delegate()
                    {
                        metronomePos.EaseOut(1.2, 0,1, delegate()
                        {
                            line.Alpha.Lerp(0.5, 0.0f, 1.0f);
                        });
                    });
                });
            };

            bm.Player.ScoreChanged += delegate(Object s, EventArgs a)
            {
                score = ((BandMaster)Game).Player.Score;
            };

            bm.ModeChanged += delegate(object s, EventArgs a)
            {
                if (bm.Mode == bm.Menu)
                {
                    logoAlpha.Value = 0.0f;
                    pressStartAlpha.Value = 0.0f;

                    notes.Black.Value = 1.0f;
                    notes.Black.Lerp(1.0, 1.0f, 0.0f, delegate()
                    {
                        logoAlpha.Lerp(0.5, 0.0f, 1.0f, delegate()
                        {
                            Helpers.Wait(0.5, delegate()
                            {
                                ((Input.IManageInput)Game.Services.GetService(typeof(Input.IManageInput))).OnRestart += animateAwayLogo;
                                pressStartAlpha.Lerp(1.0, 0.0f, 1.0f);
                            });
                        });
                    });
                }
                else if (bm.Mode == bm.HighScore)
                {
                    dinPos.Value = -9999f;
                    scorePos.Value = 9999f;
                    pressStartAlpha.Value = 0.0f;

                    line.Alpha.Lerp(1.0, line.Alpha.Value, 0.0f, delegate()
                    {
                        stagePos.EaseOut(1.2, stagePos.Value, 0);
                        metronomePos.EaseOut(1.2, stagePos.Value, 0);
                        // TODO: spill jubel
                        Helpers.Wait(2.0, delegate()
                        {
                            // Anti-Deus ex machina effect
                            for (int i = 0; i < Band.Count; i++)
                            {
                                Effector inst = Band[i].OffsetY;
                                Helpers.Wait(0.4 * i, delegate() { inst.Lerp(0.6, inst.Value, -700.0f); });
                            }
                            // KAbal-effekt
                            
                            ejaculator = 100;
                            Helpers.SimpleDelegate ejaculate = null;
                            Random rand = new Random();
                            ejaculate = delegate()
                            {
                                Vector2 v;
                                v.X = rand.Next(0, Game.GraphicsDevice.Viewport.Width);
                                v.Y = rand.Next(0, Game.GraphicsDevice.Viewport.Height-200);
                                notes.Emit(v, (float)rand.NextDouble()*1.5f + 0.1f);

                                v.X = rand.Next(0, Game.GraphicsDevice.Viewport.Width);
                                v.Y = rand.Next(0, Game.GraphicsDevice.Viewport.Height - 200);
                                notes.Emit(v, (float)rand.NextDouble() * 1.5f + 0.1f);

                                v.X = rand.Next(0, Game.GraphicsDevice.Viewport.Width);
                                v.Y = rand.Next(0, Game.GraphicsDevice.Viewport.Height - 200);
                                notes.Emit(v, (float)rand.NextDouble() * 1.0f + 0.1f);

                                v.X = rand.Next(0, Game.GraphicsDevice.Viewport.Width);
                                v.Y = rand.Next(0, Game.GraphicsDevice.Viewport.Height - 200);
                                notes.Emit(v, (float)rand.NextDouble() * 1.0f + 0.1f);

                                v.X = rand.Next(0, Game.GraphicsDevice.Viewport.Width);
                                v.Y = rand.Next(0, Game.GraphicsDevice.Viewport.Height - 200);
                                notes.Emit(v, (float)rand.NextDouble() * 1.0f + 0.1f);

                                if (--ejaculator != 0)
                                    Helpers.Wait(0.001, ejaculate);
                                else
                                {
                                    dinPos.EaseOut(0.5, -200, 200, delegate()
                                    {
                                        scorePos.EaseOut(0.5, GraphicsDevice.Viewport.Height, 260, delegate()
                                        {
                                            Helpers.Wait(2.0, delegate()
                                            {
                                                pressStartAlpha.Lerp(1.0, 0.0f, 1.0f);
                                                ((Input.IManageInput)Game.Services.GetService(typeof(Input.IManageInput))).OnRestart += animateAwayScore;
                                            });
                                        });
                                    });

                                }
                            };
                            Helpers.Wait(1.2, delegate() 
                            {
                                notes.White.Lerp(0.1, 0.0f, 1.0f, delegate()
                                {
                                    notes.White.Lerp(0.5, 1.0f, 0.0f);
                                    ejaculate();
                                });
                            });
                        });
                    });
                }
            };

            base.Initialize();
        }

        void animateAwayLogo(object sender, EventArgs a)
        {
            ((Input.IManageInput)Game.Services.GetService(typeof(Input.IManageInput))).OnRestart -= animateAwayLogo;
            logoAlpha.Lerp(0.2, logoAlpha.Value, 0.0f);
            pressStartAlpha.Lerp(0.2, pressStartAlpha.Value, 0.0f);
        }

        void animateAwayScore(object sender, EventArgs a)
        {
            ((Input.IManageInput)Game.Services.GetService(typeof(Input.IManageInput))).OnRestart -= animateAwayScore;
            pressStartAlpha.Lerp(0.2, pressStartAlpha.Value, 0.0f, delegate()
            {
                dinPos.EaseIn(0.2, 200, -200, delegate()
                {
                    scorePos.EaseOut(0.3, 260, GraphicsDevice.Viewport.Height);
                });
            });
        }

        int ejaculator;

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            float l = 0.1f;
            scoreLP = score * l + scoreLP * (1.0f - l);

            base.Update(gameTime);
        }

  

        /// <summary>
        /// Simply Draws stage, and every Instrument in a band
        /// </summary>
        /// <param name="gameTime"></param>
        double scoreFlashTime = 0.0;
        string laststr = "";
        public override void  Draw(GameTime gameTime)
        {
            BandMaster bm = (BandMaster)Game;

            // find correct size (keeping aspect ratio) and extra pixels on top (for transition)
            Rectangle dr = Game.GraphicsDevice.Viewport.Bounds;
            Rectangle sr = background.Bounds;
            float aspect = (float)sr.Height / (float)sr.Width;
            dr.Height = (int)(aspect * dr.Width);
            float extraTop = dr.Height - Game.GraphicsDevice.Viewport.Bounds.Height;

            // calculate transition timer

            int bgOffset = (int)(stagePos.Value * extraTop);

            if (   bm.Mode == bm.Play
                || bm.Mode == bm.Tutorial
                || bm.Mode == bm.Menu
                || bm.Mode == bm.HighScore )
            {
                sprites.Begin();
                {
                    // Background

                    sprites.Draw(background, new Rectangle(dr.X, dr.Y - bgOffset, dr.Width, dr.Height), null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 1);
                }
                sprites.End();
            }

            if (   bm.Mode != bm.Play
                && bm.Mode != bm.HighScore
                && bm.Mode != bm.Menu ) return;

            // Instruments
            foreach (Instrument instrument in Band)
            {
                instrument.Bounds.Y -= bgOffset;
                instrument.Draw(gameTime);
                instrument.Bounds.Y += bgOffset;
            }
            sprites.Begin();
            {
                // Stand

                int stOffset = (int)(-400 + stagePos.Value * (extraTop + 400));
                sprites.Draw(stand, new Rectangle(dr.X, dr.Y - stOffset, dr.Width, dr.Height), null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 1);

                // Metronome

                int metOffset = (int)(800 - metronomePos.Value * 500);
                sprites.Draw(metronome, new Rectangle(Game.GraphicsDevice.Viewport.Width - 256, metOffset, metronome.Width / 2, metronome.Height / 2), null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 1);
                int tempoOff = (int)Helpers.Lerp(30.0f, 260.0f, Helpers.Clamp(midiPlayer.TempoDifference * 0.5f + 0.5f, 0.0f, 1.0f));
                sprites.Draw(metronomeSlider, new Rectangle(Game.GraphicsDevice.Viewport.Width - 256 + 76, metOffset + tempoOff, metronomeSlider.Width / 2, metronomeSlider.Height / 2), null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 1);
            }
            sprites.End();


            sprites.Begin(SpriteSortMode.Immediate, BlendState.Additive);
            {
                // Draw score
                SpriteFont font = bm.SplashFont;
                string str = ((int)(scoreLP * 10.0f)).ToString();
                if (str != laststr)
                    scoreFlashTime = gameTime.TotalGameTime.TotalSeconds;
                float f = 1.0f - (float)Math.Min(0.8, Math.Max(0.0, (gameTime.TotalGameTime.TotalSeconds - scoreFlashTime)));
                sprites.DrawString(font, str, new Vector2(dr.Right - font.MeasureString(str).X - 20, 0), new Color(f, f, f));
                laststr = str;
            }
            sprites.End();

            if (bm.Mode == bm.HighScore)
            {
                sprites.Begin(SpriteSortMode.Immediate, BlendState.Additive);
                {
                    SpriteFont font = bm.SplashFont;
                    String str = "Din score:";
                    sprites.DrawString(font, str, new Vector2(dr.Center.X - font.MeasureString(str).X*0.5f, dinPos.Value), Color.White);

                    font = bm.BigFont;
                    str = ((int)(bm.Player.Score*10.0)).ToString();
                    sprites.DrawString(font, str, new Vector2(dr.Center.X - font.MeasureString(str).X*0.5f, scorePos.Value), Color.White);

                    font = bm.MiniFont;
                    str = "T�rk for � spille igjen";
                    sprites.DrawString(font, str, new Vector2(dr.Center.X - font.MeasureString(str).X*0.5f, dr.Bottom-400), new Color(pressStartAlpha.Value,pressStartAlpha.Value,pressStartAlpha.Value) );
                }
                sprites.End();
            }


            if (bm.Mode == bm.Menu)
            {
                sprites.Begin(SpriteSortMode.Immediate, BlendState.Additive);
                {
                    sprites.Draw(logo, new Rectangle(dr.Center.X - logo.Width/2, 100, logo.Width, logo.Height), new Color(logoAlpha.Value, logoAlpha.Value, logoAlpha.Value));

                    SpriteFont font = bm.MiniFont;
                    String str = "T�rk for � starte spillet";
                    sprites.DrawString(font, str, new Vector2(dr.Center.X - font.MeasureString(str).X * 0.5f, dr.Bottom - 400), new Color(pressStartAlpha.Value, pressStartAlpha.Value, pressStartAlpha.Value));
                }
                sprites.End();
            }

            if (line != null) line.Draw(gameTime);
            //if (snake != null) snake.Draw(gameTime);

            base.Draw(gameTime);
        }
    }
}
