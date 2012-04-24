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



namespace BandMaster.Graphics
{

    public class Line : Microsoft.Xna.Framework.DrawableGameComponent
    {
        SpriteBatch sprites;
        Audio.Midi.Player player;

        Texture2D flare, seperator;

        // settings
        Rectangle bounds = new Rectangle(150, 500, 800, 400);
        float segmentWidth = 200.0f;
        float segmentHeight = 100.0f;

        public Effector Alpha = new Effector();

        public Line(Game game)
            : base(game)
        {
            Visible = false;


            ((BandMaster)Game).SongLoaded += delegate(object a, EventArgs b)
            {
                Visible = true;
            };
        }

        public override void Initialize()
        {
            sprites = (SpriteBatch)Game.Services.GetService(typeof(SpriteBatch));
            player = (Audio.Midi.Player)Game.Services.GetService(typeof(Audio.Midi.Player));

            flare = Game.Content.Load<Texture2D>("Textures/flare1");
            seperator = Game.Content.Load<Texture2D>("Textures/seperator");

        }
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        float evaluateFadeoutFactor(float screenPos)
        {
            if (screenPos < bounds.Left + 50)
                return Math.Max(0.0f, (float)(screenPos - bounds.Left) / 50.0f);
            else if (screenPos > bounds.Right - 50)
                return Math.Max(0.0f, (float)(bounds.Right - screenPos) / 50.0f);
            else
                return 1.0f;
        }

        void evaluateLineEffects(GameTime time, int songTime, Vector2 screenPos, out float offset, out float scale, out float glow)
        {
            offset = 0.0f;

            scale = (float)Math.Pow(Math.Sin((double)screenPos.X * 0.01 - time.TotalGameTime.TotalSeconds * 4.0) * 0.5 + 0.5, 100.0) + 1.0f;
            scale *= evaluateFadeoutFactor(screenPos.X);
            glow = 0.0f;
        }

        void drawSegment(GameTime time, int songTimeFrom, int songTimeTo, Vector2 from, Vector2 to)
        {
            int spriteHeight = flare.Height/4;
            int spriteWidth = flare.Width/4 ;
            int particleDensity = 100;

            float fadeout = evaluateFadeoutFactor(from.X) * 0.5f * Alpha.Value;
            sprites.Draw(seperator, new Rectangle((int)from.X, (int)bounds.Top, seperator.Width/2, (int)segmentHeight), new Color(fadeout,fadeout,fadeout));

            float mid = from.X + (to.X - from.X) * 0.5f;
            fadeout = evaluateFadeoutFactor(mid) * 0.5f * Alpha.Value;
            sprites.Draw(seperator, new Rectangle((int)mid, (int)bounds.Top+20, seperator.Width / 2, (int)segmentHeight-30), new Color(fadeout, fadeout, fadeout));

            for (int i = 0; i < particleDensity; i++)
            {
                float var = (float)i / (float)particleDensity;

                int songTime = (int)Helpers.Lerp(songTimeFrom, songTimeTo, var); 
                Vector2 position;
                position.X = Helpers.Lerp(from.X, to.X, var);
                position.Y = var < 0.5? Helpers.Scurve(from.Y, to.Y, var*2.0f) : to.Y;

                float fxOffset, fxScale, fxGlow;
                evaluateLineEffects(time, songTime, position, out fxOffset, out fxScale, out fxGlow);
                position.X += fxOffset;
                position.X -= fxScale * spriteWidth * 0.5f;
                position.Y -= fxScale * spriteHeight * 0.5f;

                Rectangle rect = new Rectangle((int)position.X, (int)position.Y, (int)(spriteWidth*fxScale), (int)(spriteHeight*fxScale));
                Color col = position.X < bounds.Center.X ? new Color(0.2f * Alpha.Value, 0.2f * Alpha.Value, 0.2f * Alpha.Value) : new Color(Alpha.Value, Alpha.Value, Alpha.Value);
                sprites.Draw(flare, rect, col);//new Color(1.0f - 148.0f/255.0f, 1.0f - 38.0f/255.0f, 1.0f - 11.0f/255.0f) );
            }

        }

        public override void Draw(GameTime gameTime)
        {
            if (!Visible) return;

            int[] parts = ((BandMaster)Game).Song.Lines[0];

            float done = (float)player.Position / (float)player.Length;
            // 0.0 => the beginning, start rendering on center
            // 1.0 => the end, start rendering on center - length_of_line


            float center = bounds.Center.X;
            float lineLength = segmentWidth * parts.Length;
            Vector2 startpos = new Vector2(center - (lineLength * done), bounds.Y+20);

            int currentPart = (int)(done * (float)parts.Length); // this calculation should be moved
            int windowSize = 10;
            int fromPart = Math.Max(1, currentPart - windowSize);
            int toPart = Math.Min(parts.Length, currentPart + windowSize);


            BlendState blend = new BlendState();
            blend.ColorSourceBlend = Blend.Zero;
            blend.ColorDestinationBlend = Blend.InverseSourceColor;
            sprites.Begin(SpriteSortMode.Immediate, blend);

      
            for (int i = fromPart; i < toPart; i++)
            {
                int lastLevel = parts[i - 1];
                int currentLevel = parts[i];
                int delta = currentLevel - lastLevel; // delta c[-3,+3]

                Vector2 from, to;
                from.X = startpos.X + segmentWidth * i;
                from.Y = startpos.Y + (float)lastLevel / 3.0f * segmentHeight;

                to.X = from.X + segmentWidth;
                to.Y = startpos.Y + (float)currentLevel / 3.0f * segmentHeight;

                drawSegment(gameTime, 0, 0, from, to);
            }

            sprites.End();


            base.Draw(gameTime);
        }
    }



    /*
    /// <summary>
    /// This is a game component that implements DrawableGameComponent.
    /// </summary>
    public class Line : Microsoft.Xna.Framework.DrawableGameComponent
    {
        SpriteBatch LineSpriteBatch;
        Audio.Midi.Player player;
        
        Texture2D[] lineSegments;

        static Color[] colors={Color.Red, Color.Blue, Color.Yellow, Color.MediumPurple, Color.Orange};

        public Line(Game game, Vector2 pos)
            : base(game)
        {
            Visible = false;
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            LineSpriteBatch = (SpriteBatch)Game.Services.GetService(typeof(SpriteBatch));
            player = (Audio.Midi.Player)Game.Services.GetService(typeof(Audio.Midi.Player));

            lineSegments = new Texture2D[] {
                Game.Content.Load<Texture2D>("Textures/ned3"),
                Game.Content.Load<Texture2D>("Textures/ned2"),
                Game.Content.Load<Texture2D>("Textures/ned1"),
                Game.Content.Load<Texture2D>("Textures/strek"),
                Game.Content.Load<Texture2D>("Textures/opp1"),
                Game.Content.Load<Texture2D>("Textures/opp2"),
                Game.Content.Load<Texture2D>("Textures/opp3")
            };
            
            ((BandMaster)Game).SongLoaded += delegate (object a, EventArgs b) 
            {
                Visible = true;
            };

            base.Initialize();
        }


        private void DrawLine(int[] parts, Color Col)
        {

            int spriteWidth = (int)(lineSegments[0].Width);
            int spriteHeight = (int)(lineSegments[0].Height);

            float done = (float)player.Position / (float)player.Length;
            // 0.0 => the beginning, start rendering on center
            // 1.0 => the end, start rendering on center - length_of_line

            float center = Game.GraphicsDevice.Viewport.Bounds.Center.X;
            float length_of_line = spriteWidth * parts.Length;
            Vector2 startpos = new Vector2(center - (length_of_line * done), 200.0f);

            LineSpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive);

            for (int i = 1; i < parts.Length; i++)
            {
                int lastLevel = parts[i - 1];
                int currentLevel = parts[i];
                int delta = currentLevel - lastLevel; // delta c[-3,+3]

                int offsetX = spriteWidth * i;
                int offsetY = (delta >= 0 ? lastLevel : currentLevel) * -(spriteHeight - 15) / 4;
                Rectangle rect = new Rectangle((int)startpos.X + offsetX, (int)startpos.Y + offsetY, spriteWidth, spriteHeight);
                Texture2D texture = lineSegments[delta + 3];
                LineSpriteBatch.Draw(texture, rect, Col);

            }
            LineSpriteBatch.End();
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            
            
            // XXX: this should not use game time!

            if (!Visible) return;



            //System.Console.Out.WriteLine(done);

            for (int i = 0; i < ((BandMaster)Game).Song.Lines.Count; i++)
            {
                DrawLine(((BandMaster)Game).Song.Lines[i], colors[i % 5]);
            }

            base.Draw(gameTime);
        }
    }*/
}
