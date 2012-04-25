using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;


namespace BandMaster.Audio
{
    class AudioFx : GameComponent
    {
        public SoundEffect ApplauseBig;
        public SoundEffect ApplauseSmall;
        public SoundEffect DrumStick;
        public SoundEffect Tuning;
        public SoundEffect Ambient;

        public AudioFx(Game game) : base(game)
        {
        }

        public static SoundEffectInstance Play(SoundEffect e)
        {
            SoundEffectInstance i = e.CreateInstance();
            i.Play();
            return i;
        }

        public override void Initialize()
        {
            ContentManager content = Game.Content;

            ApplauseBig = content.Load<SoundEffect>("Sounds/applause_big");
            ApplauseSmall = content.Load<SoundEffect>("Sounds/applause_small");
            DrumStick = content.Load<SoundEffect>("Sounds/single_drumstick_hit");
            Tuning = content.Load<SoundEffect>("Sounds/preconcert_tuning");
            Pling = content.Load<SoundEffect>("Sounds/single_drumstick_hit");
        }

        /*
        public class PlayRequest
        {
            public bool started = false;
            public string sound = null;
            public TimeSpan time = TimeSpan.Zero;
            public TimeSpan playTime;
            public TimeSpan fadeTime;
            public float pitch;
            public float volume;
            public SoundEffectInstance instance = null;
        };

        public Dictionary<string, SoundEffect> sounds;

        //private List<SoundEffectInstance> instances;
        public List<PlayRequest> requests;

        string[] load = new string [] {"applause_big", "applause_small"};

        public SoundFx(Game game) : base(game)
        {
            sounds = new Dictionary<string, SoundEffect>();
            requests = new List<PlayRequest>();
            //instances = new List<SoundEffectInstance>();
        }

        public override void Initialize()
        {
            ContentManager content = Game.Content;

            foreach (string s in load)
            {
                sounds[s] = content.Load<SoundEffect>("Sounds/" + s);
            }
        }

        public override void Update(GameTime gameTime)
        {
            for (int i = 0; i < requests.Count; ++i)
            {
                PlayRequest req = requests[i];
                TimeSpan time = gameTime.TotalGameTime - req.time;

                if (!req.started)
                {
                    req.started = true;
                    req.instance.Play();
                    req.time = time;
                }
                else if (req.playTime > TimeSpan.Zero)
                {
                    req.playTime -= time;
                }
                else if (req.fadeTime > TimeSpan.Zero)
                {
                    req.fadeTime -= time;
                    req.instance.Volume = req.volume * req.fadeTime.Seconds;
                }
                else
                {
                    requests.RemoveAt(i--);
                }
            }
        }

        public void AddPlayRequest(string sound, float playTime, float fadeTime, float volume, float pitch)
        {
            PlayRequest req = new PlayRequest();

            req.instance = sounds[sound].CreateInstance();

            req.playTime = TimeSpan.FromSeconds(playTime);
            req.fadeTime = TimeSpan.FromSeconds(fadeTime);
            req.volume = volume;
            req.pitch = pitch;

            requests.Add(req);
        }
        */
    }
}
