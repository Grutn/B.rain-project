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
    public class FlyingNotes : Microsoft.Xna.Framework.DrawableGameComponent
    {
        SpriteBatch sprites;

        Texture2D[] textures;
        Texture2D square;

        class Note
        {
            public Vector2 Position;
            public Vector2 Velocity;
            public Texture2D Texture;
            public double Time;
            public float Scale;
        }

        List<Note> notes = new List<Note>();
        Queue<Note> queue = new Queue<Note>();

        public Effector White = new Effector(0.0f);
        public Effector Black = new Effector(0.0f);

        public FlyingNotes(Game game)
            : base(game)
        {
            DrawOrder = 9;
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            sprites = (SpriteBatch)Game.Services.GetService(typeof(SpriteBatch));

            textures = new Texture2D[] {
                Game.Content.Load<Texture2D>("Textures/note1"),
                Game.Content.Load<Texture2D>("Textures/note2")
            };
            square = Game.Content.Load<Texture2D>("Textures/4sqr");
            base.Initialize();
        }


        public override void Draw(GameTime gameTime)
        {
            BlendState blend = new BlendState();
            blend.ColorBlendFunction = BlendFunction.Add;
            blend.ColorSourceBlend = Blend.One;
            blend.ColorDestinationBlend = Blend.One;

            sprites.Begin(SpriteSortMode.Immediate, blend);
            lock (notes)
            {
                foreach (Note note in notes)
                    sprites.Draw(note.Texture, new Rectangle((int)note.Position.X, (int)note.Position.Y, (int)(note.Scale * (float)note.Texture.Width/8), (int)(note.Scale * (float)note.Texture.Height/8)), new Color((float)note.Time, (float)note.Time, (float)note.Time));
            }

            sprites.Draw(square, new Rectangle(0, 0, Game.GraphicsDevice.Viewport.Width, Game.GraphicsDevice.Viewport.Height), new Color(White.Value, White.Value, White.Value));

            sprites.End();

            blend = new BlendState();
            blend.ColorBlendFunction = BlendFunction.Add;
            blend.ColorSourceBlend = Blend.Zero;
            blend.ColorDestinationBlend = Blend.InverseSourceColor;

            sprites.Begin(SpriteSortMode.Immediate, blend);
            sprites.Draw(square, new Rectangle(0, 0, Game.GraphicsDevice.Viewport.Width, Game.GraphicsDevice.Viewport.Height), new Color(Black.Value, Black.Value, Black.Value));

            sprites.End();

            base.Draw(gameTime);
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            double dt = gameTime.ElapsedGameTime.TotalSeconds;

            for (int i = 0; i < notes.Count; i++)
            {
                Note note = notes[i];
                note.Velocity += new Vector2(0,-0.05f);
                note.Position += Vector2.Multiply(note.Velocity, (float)dt * 100.0f * note.Scale);
                note.Time -= dt;
                if (note.Time <= 0.0)
                    notes.RemoveAt(i--);
            }

            base.Update(gameTime);
        }

        Random rand = new Random();

        public void Emit(Vector2 position, float scale = 1.0f)
        {
            Note n = new Note();
            n.Position = position;
            n.Velocity = new Vector2((float)rand.NextDouble()-0.5f, (float)rand.NextDouble()-0.5f);
            n.Velocity.Normalize();
            n.Texture = textures[rand.Next(0,2)];
            n.Time = 1.0;
            n.Scale = scale;
            lock(notes)
            {
                notes.Add(n);
            }
        }
    }
}
