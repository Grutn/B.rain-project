using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;


namespace BandMaster.Graphics
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class Metronum : Microsoft.Xna.Framework.DrawableGameComponent
    {
        Vector2 startpos;
        SpriteBatch MetronumSpriteBatch;
        Texture2D metronumTexture;

        public Metronum(Game game)
            : base(game)
        {
            
        }
        public Metronum(Game game, Vector2 pos)
            : base(game)
        {
            startpos = pos;
            MetronumSpriteBatch = (SpriteBatch)Game.Services.GetService(typeof(SpriteBatch));
            metronumTexture = game.Content.Load<Texture2D>("metronum"); //TODO no texture
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            // TODO: Add your initialization code here

            base.Initialize();
        }

        public void Change(int change)
        {
            startpos.Y = startpos.Y + change;
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
        public override void Draw(GameTime gameTime)
        {
            MetronumSpriteBatch.Begin();
            MetronumSpriteBatch.Draw(metronumTexture,
                new Rectangle((int)startpos.X, (int)startpos.Y, metronumTexture.Width, metronumTexture.Height),
                Color.White);
            MetronumSpriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
