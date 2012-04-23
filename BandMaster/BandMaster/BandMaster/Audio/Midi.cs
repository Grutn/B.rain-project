using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Sanford.Multimedia.Midi;

namespace BandMaster.Audio
{
    public class Midi
    {
        // TODO: everything is now in ticks, it should perhaps be hits here 

        public class Player: GameComponent
        {
            private Sequencer sequencer1;
            private OutputDevice outDevice;

            private int outDeviceID = 0;

            public Player(Game game)
                : base(game)
            {
                sequencer1 = new Sanford.Multimedia.Midi.Sequencer();
                sequencer1.clock.Tick += onTick;

                outDevice = new OutputDevice(outDeviceID);

                
                this.sequencer1.PlayingCompleted += new System.EventHandler(this.HandlePlayingCompleted);
                this.sequencer1.ChannelMessagePlayed += new System.EventHandler<Sanford.Multimedia.Midi.ChannelMessageEventArgs>(this.HandleChannelMessagePlayed);
                this.sequencer1.SysExMessagePlayed += new System.EventHandler<Sanford.Multimedia.Midi.SysExMessageEventArgs>(this.HandleSysExMessagePlayed);
                this.sequencer1.Chased += new System.EventHandler<Sanford.Multimedia.Midi.ChasedEventArgs>(this.HandleChased);
                this.sequencer1.Stopped += new System.EventHandler<Sanford.Multimedia.Midi.StoppedEventArgs>(this.HandleStopped);
                

            }


            private void HandleChannelMessagePlayed(object sender, ChannelMessageEventArgs e)
            {
                /*if (closing)
                {
                    return;
                }*/

                if (e.Message.Command == ChannelCommand.NoteOn)
                    NotePlayed((object)(e.Message.MidiChannel), null);

                outDevice.Send(e.Message);
                //pianoControl1.Send(e.Message);
            }

            private void HandleChased(object sender, ChasedEventArgs e)
            {
                foreach (ChannelMessage message in e.Messages)
                {
                    outDevice.Send(message);
                }
            }

            private void HandleSysExMessagePlayed(object sender, SysExMessageEventArgs e)
            {
                //     outDevice.Send(e.Message); Sometimes causes an exception to be thrown because the output device is overloaded.
            }

            private void HandleStopped(object sender, StoppedEventArgs e)
            {
                foreach (ChannelMessage message in e.Messages)
                {
                    outDevice.Send(message);
                    //pianoControl1.Send(message);
                }
            }

            private void HandlePlayingCompleted(object sender, EventArgs e)
            {
                //timer1.Stop();
            }


            private void onTick(object sender, EventArgs args)
            {
                if(Tick != null)
                    Tick(sender, args);
            }

            // you get this every tick
            public event System.EventHandler Tick;

            // you get this everey time a note is played on any instrument, sender is just an Integer containing the instrument index
            public event System.EventHandler NotePlayed;

            // the song playing
            Song _song;
            public Song Song 
            { 
                set 
                {
                    _song = value;
                    this.sequencer1.Position = 0;
                    this.sequencer1.Sequence = value.sequence1;
                }
                get
                {
                    return _song;
                }
            }



            // ticks
            public int Length
            {
                get
                {
                    return sequencer1.Sequence.GetLength();
                }
            }

            // ticks
            public int Position 
            { 
                get 
                { 
                    return sequencer1.Position; 
                } 
                set
                { 
                    sequencer1.Position = value;
                } 
            }

            float idealTempo = 1.0f;
            public float TempoDifference
            {
                get
                {
                    return Tempo - idealTempo;
                }
            }

            // Tempo = secPerHit
            public float Tempo 
            { 
                get 
                { 
                    return sequencer1.secPerHit; 
                } 
                set
                {
                    if (value == 0) return;
                    sequencer1.secPerHit = value; 
                } 
            }

            // 0.0f <= Volume <= 1.0f
            public void SetVolume(int channel, float volume)
            {
                throw new NotImplementedException("SetVolume");
            }

            public void Play()
            {
                sequencer1.Start();
            }
            public void Continue() 
            {
                //sequencer1.Start();
                //sequencer1.Continue();

                if (!sequencer1.clock.IsRunning)
                {
                    sequencer1.clock.Start();
                }
            }
            public void Stop()
            {
                sequencer1.clock.Stop();
            }
            public bool IsRunning { get { return sequencer1.clock.IsRunning; } }

        }

        public class Song
        {
            internal Sequence sequence1;
            public Song()
            {
                this.sequence1 = new Sequence();
                this.sequence1.Format = 1;
            }

            private void onLoad(object sender, EventArgs args)
            {
                List<Track> newTracks = new List<Track>();
                for (int i = 0; i < sequence1.Count; i++)
                {
                    Track t = new Track();
                    foreach (MidiEvent e in sequence1[i].Iterator())
                    {
                        bool discard = false;
                        if (e.MidiMessage.MessageType == MessageType.Channel)
                        {
                            ChannelMessage m = (e.MidiMessage as ChannelMessage);
                            if (m.Command == ChannelCommand.Controller && (m.Data1 == (int)ControllerType.Volume || m.Data1 == (int)ControllerType.VolumeFine))
                            {
                                discard = true;
                            }
                        }
                        if (!discard) t.Insert(e.AbsoluteTicks, e.MidiMessage);
                    }
                    sequence1.Remove(sequence1[i]);
                    /*if (i == 0)*/
                    newTracks.Add(t);
                }
                foreach (Track t in newTracks)
                    sequence1.Add(t);
                foreach (MidiEvent e in sequence1[0].Iterator())
                {
                    if (e.MidiMessage.MessageType == MessageType.Channel)
                    {
                        ChannelMessage m = (e.MidiMessage as ChannelMessage);
                        if (m.Command == ChannelCommand.NoteOn)
                        {
                            int a = e.AbsoluteTicks;
                        }
                    }
                }
                //for (int i=0; i<4; i++)
                //    Sequence[i].Insert(1, new ChannelMessage(ChannelCommand.Controller, i, (int)ControllerType.Volume, 0 ));


                // then we return to loader code
                if (loadComplete != null)
                    loadComplete();
            }

            public delegate void SimpleDelegate();
            SimpleDelegate loadComplete;
            public void LoadAsync(string filename, SimpleDelegate loadComplete )
            {
                this.sequence1.LoadAsync("Content/"+filename);
                this.sequence1.LoadCompleted += onLoad;
                this.loadComplete = loadComplete;

            }
        }
    }

}
