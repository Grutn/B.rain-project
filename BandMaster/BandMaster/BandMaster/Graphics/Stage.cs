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
        List<Instrument> Band;
        SpriteBatch StageSpriteBatch;
        Hashtable Instruments;
        Game _game;
        Line Lines;
        int _hight;
        Texture2D _stageText;

        public Stage(Game game)
            : base(game)
        {
            _game = game;
            StageSpriteBatch = new SpriteBatch(game.GraphicsDevice);
            Game.Services.AddService(typeof(SpriteBatch), StageSpriteBatch);
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            _stageText = _game.Content.Load<Texture2D>("stage");//NB no file

            base.Initialize();
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            // TODO: Add your update code here

            base.Update(gameTime);
        }

        private void addInstrument(string _instrument, Rectangle rec)
        {
            if (!(Instruments.ContainsKey(_instrument)))
            {
                Instruments.Add(_instrument, new Instrument(_game, _instrument, rec));
            }
            Instrument Current = Instruments[_instrument] as Instrument;
            Current.SetNewBounds(rec);
        }
        /// <summary>
        /// Set a band array based on a BandMaster.State.Song._Instruments Array
        /// </summary>
        /// <param name="_song"></param>
        public void SetBand(BandMaster.State.Song _song)
        {
            Band.RemoveRange(0,Band.Count);
            Lines.ChangeSong(_song);
            int _width = _game.GraphicsDevice.Viewport.Width/_song._Instruments.Length;
            for(int i=0;i<_song._Instruments.Length;i++)
            {
                addInstrument(_song._Instruments[i], new Rectangle(_width*i, 0, _width, _hight));
                Band.Add(Instruments[_song._Instruments[i]] as Instrument);
                
            }
   
        }
        /// <summary>
        /// Simply Draws stage, and every Instrument in a band
        /// </summary>
        /// <param name="gameTime"></param>
        public override void  Draw(GameTime gameTime)
        {   
            StageSpriteBatch.Begin();
            
            StageSpriteBatch.Draw(_stageText, _game.GraphicsDevice.Viewport.Bounds, null, Color.White, 0f,
                Vector2.Zero, SpriteEffects.None, 1);
            foreach (Instrument _instrument in Band)
            {
                _instrument.Draw(gameTime);
            }
            
            Lines.Draw(gameTime);
            
            StageSpriteBatch.End();
 	        
            base.Draw(gameTime);
        }
    }
}
