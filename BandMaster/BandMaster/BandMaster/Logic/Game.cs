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

namespace BandMaster
{
    using Audio;
    using Input;


    public interface IMode: IGameComponent
    {

    }

    /// <summary>
    /// This is the main type for your game
    /// </summary>
    /// 
    public class BandMaster : Microsoft.Xna.Framework.Game
    {
        public IMode Play, Pause, Menu;

        // Mode and Song holds the state of the game.
        // ModeChanged and SongChanged are events that the graphics components of the system will listen to.
        // The idea is taht we should not know anything about the graphical representation of the game :)

        private IMode mode;
        public IMode Mode { get { return mode; } set {  mode = value; if (ModeChanged != null) ModeChanged(this, null); } }
        public event EventHandler ModeChanged;

        private State.Song song;
        public State.Song Song { get { return song; } set { song = value; if (SongChanged != null) SongChanged(this, null); } }
        public event EventHandler SongChanged;
        public event EventHandler SongLoaded; 

        public BandMaster()
        {
            Content.RootDirectory = "Content";


            // Services
            
            GraphicsDeviceManager graphics = new GraphicsDeviceManager(this);

            IManageInput inputManager;
            try
            {
                inputManager = new KinectInputManager(this);
            }
            catch (Exception e)
            {
                inputManager = new AlternativeInputManager(this);
            }

            Components.Add(inputManager);
            Services.AddService(typeof(IManageInput), inputManager);

            Midi.Player player = new Midi.Player(this);
            Components.Add(player);
            Services.AddService(typeof(Midi.Player), player);


            // Game modes

            Play = new Logic.BandMasterMode(this);
            Pause = new Logic.PauseMenuMode(this);
            Menu = new Logic.MainMenuMode(this);

            Components.Add(Play);
            Components.Add(Pause);
            Components.Add(Menu);

            
            // Graphics

            Components.Add(new Graphics.Stage(this));
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            SpriteBatch sprites = new SpriteBatch(GraphicsDevice); 
            Services.AddService(typeof(SpriteBatch), sprites);

            base.Initialize();

            // Set initial mode (dette er senere satt fra sangvalg-menyen eller noe)

            Mode = Play; // sender ModeChanged  (bare nyttig for grafikken sin del)
            Song = new State.Song(); // sender SongChanged (listners starter å vise loading) 
            Song.LoadAsync("song.txt", delegate() { if (SongLoaded != null) SongLoaded(this, null); }); // PlayMode lytter på SongLoaded
        }

        void InputOnTextureReady(object sender, VideoTextureReadyEventArgs e)
        {
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {

        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            // TODO: Add your update logic here
            
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            base.Draw(gameTime);
        }
    }
}
