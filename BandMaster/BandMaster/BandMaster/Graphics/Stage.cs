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
        List<Instrument> Band = new List<Instrument>();
        SpriteBatch StageSpriteBatch;
        Hashtable Instruments;
        Line Lines;
        VolumeSnake snake;
        int _hight;

        Texture2D background, stand;
        
        public Stage(Game game)
            : base(game)
        {
            Lines = new Line(game);
            snake = new VolumeSnake(game, new Vector2(120, 50), 100, 100);

        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            StageSpriteBatch = (SpriteBatch)Game.Services.GetService(typeof(SpriteBatch));
            
            background = Game.Content.Load<Texture2D>("Textures/bg"); // NB no file
            stand = Game.Content.Load<Texture2D>("Textures/notestativ");
            Lines.Initialize();
            snake.Initialize();

            //((BandMaster)Game).StateChanged += OnGameStateChanged;
            //((BandMaster)Game).SongChanged += OnSongChanged;

            base.Initialize();
        }


        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            snake.Update(gameTime);

            base.Update(gameTime);
        }

        private void addInstrument(string _instrument, Rectangle rec)
        {
            if (!(Instruments.ContainsKey(_instrument)))
            {
                Instruments.Add(_instrument, new Instrument(Game, _instrument, rec));
            }
            Instrument Current = Instruments[_instrument] as Instrument;
            Current.SetNewBounds(rec);
        }
        /// <summary>
        /// Set a band array based on a BandMaster.State.Song._Instruments Array
        /// </summary>
        /// <param name="_song"></param>
        public void SetBand(State.Song song)
        {
            Band.RemoveRange(0,Band.Count);
            int _width = Game.GraphicsDevice.Viewport.Width/song.Instruments.Length;
            for(int i=0;i<song.Instruments.Length;i++)
            {
                addInstrument(song.Instruments[i], new Rectangle(_width*i, 0, _width, _hight));
                Band.Add(Instruments[song.Instruments[i]] as Instrument);
                
            }
   
        }
        /// <summary>
        /// Simply Draws stage, and every Instrument in a band
        /// </summary>
        /// <param name="gameTime"></param>
        public override void  Draw(GameTime gameTime)
        {
            StageSpriteBatch.Begin();

            // find correct size (keeping aspect ratio) and extra pixels on top (for transition)
            Rectangle dr = Game.GraphicsDevice.Viewport.Bounds;
            Rectangle sr = background.Bounds;
            float aspect = (float)sr.Height/(float)sr.Width;
            dr.Height = (int)(aspect * dr.Width);
            float extraTop = dr.Height - Game.GraphicsDevice.Viewport.Bounds.Height;

            // calculate transition timer
            double introTime = 0.0;
            float fader = Math.Min(Math.Max((float)(gameTime.TotalGameTime.TotalSeconds-introTime)*1.2f, 0.0f), 1.0f);
            fader = (float)Math.Sin((double)fader * Math.PI / 2.0); // ease out

            int bgOffset = (int)(fader * extraTop);
            StageSpriteBatch.Draw(background, new Rectangle(dr.X, dr.Y - bgOffset, dr.Width, dr.Height), null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 1);

            int stOffset = (int)(-400 + fader * (extraTop + 400));
            StageSpriteBatch.Draw(stand, new Rectangle(dr.X,dr.Y - stOffset, dr.Width,dr.Height), null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 1);


            foreach (Instrument _instrument in Band)
            {
                _instrument.Draw(gameTime);
            }
            StageSpriteBatch.End();
            if (Lines != null) Lines.Draw(gameTime);
            if (snake != null) snake.Draw(gameTime);
 	        
            base.Draw(gameTime);
        }
    }
}
