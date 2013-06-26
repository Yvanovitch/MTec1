/*
 * Quand on n'a pas chargé de fichier midi, on envoi des bangs
 * On peut choisir entre envoyer les signaux (note ou bang) en OSC ou MIDI
 * */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using NAudio.Midi;
using System.IO;
using Utils.OSC;
using Midi;

namespace LeapOrchestra.SongPlayer
{
    class SoundManager 
    {
        bool interfaceReady;
        public OutputDevice outputDevice;
        public NoteOutputStream noteSender;
        private SEND_MODE playMode;
        private MidiFileReader reader;
        public event Action<int> sendTempo;
        public event Action<string> sendMeasureInfo;
        public OSCReceiver OSCreceiver;
        
        

        enum SEND_MODE{MIDI_BANG, MIDI_NOTE};

        public SoundManager()
        {
            interfaceReady = false;
            noteSender = new MidiOutputStream();
            choosePlayMode();
            playMode = SEND_MODE.MIDI_BANG;
            createOSCReceiver();
           
        }

        public void createOSCReceiver()
        {
            OSCreceiver = new OSCReceiver(8000);
            OSCreceiver.SendBang += this.throwBang;
            OSCreceiver.SendOrientation += this.SetCurrentOrientation;
            OSCreceiver.EvolvePartCursor += this.evolvePartCursor;
        }

        public void readMidiFile(string path)
        {
            if (!File.Exists(path))
            {
                Console.WriteLine("Fichier inexistant");
                playMode = SEND_MODE.MIDI_BANG;
                return;
            }
            
            reader = new MidiFileReader(path);
            reader.SendNote += noteSender.SendNote;
            reader.SendProgramChange += noteSender.SendProgramChange;
            reader.SendEnd += noteSender.SendEnd;
            reader.analyzeProgramChange();
            playMode = SEND_MODE.MIDI_NOTE;
            reader.ChooseChannelOrientation();
            return;
        }

        public int getTempo()
        {
            if (reader == null)
            {
                return 0;
            }
            else
            {
                return (int)reader.Tempo;
            }
        }
        
        public void chooseOutput(NoteOnEvent note)
        {
            //Permettre le changement entre stream Midi et stream OSC
            playMode = SEND_MODE.MIDI_BANG;
        }

        public void setInterfaceReady()
        {
            interfaceReady = true;
        }

        public void choosePlayMode ()
        {
            /*Permettre de choisir entre :
             * Envoyer des bangs dans les temps
             * Envoyer les notes à l'intérieur des temps synchronisées
             */
            //playMode = SEND_MODE.MIDI_NOTE;
        }

        public void throwBang()
        {
            if (playMode == SEND_MODE.MIDI_NOTE)
            {
                if (reader != null)
                {
                    reader.throwBang();
                }
            }
            else
            {
                noteSender.SendBang();
            }
        }

        public void evolvePartCursor(int beatNumber)
        {
            if (playMode == SEND_MODE.MIDI_NOTE)
            {
                if (reader != null)
                {
                    reader.evolvePartCursor(beatNumber);
                    if (interfaceReady)
                    {
                        sendTempo(this.getTempo());
                        sendMeasureInfo(reader.currentMBT());
                    }
                }
            }
            else
            {
                noteSender.SendBang();
            }
        }

        public void SetCurrentOrientation(float value)
        {
            if (reader != null)
                reader.SetCurrentOrientation(value);
        }

        public void Process()
        {
            while (true)
            {
                if (playMode == SEND_MODE.MIDI_NOTE)
                {
                    if (reader != null)
                    {
                        reader.playNote();
                    }
                    Thread.Sleep(8);
                }
                else
                {
                    Thread.Sleep(100);
                }
            }
        }

        //Mis de côté, ne sert à rien
        static void playSong(string path)
        {
            //path = @"D:\Documents\Cours\Orchestra\Wonderful slippery thing.wav");
            
            if (string.IsNullOrEmpty(path))
            {
                Console.WriteLine("Path undefined");
                return;
            }
            
            System.Media.SoundPlayer song = new System.Media.SoundPlayer();
            song.SoundLocation = path;
            song.LoadAsync();
            song.PlayLooping();
            Console.WriteLine("Press Enter to Quit");
            ConsoleKeyInfo KeyInfo = Console.ReadKey();
            while (KeyInfo.Key != ConsoleKey.Enter)
            {
                KeyInfo = Console.ReadKey();
                Thread.Sleep((4 * 60 + 28) * 1000);
            }
            song.Stop();
            return;
        }
       
        public void GetOutputDevice(OutputDevice outputDevice)
        {
            this.outputDevice = outputDevice;
            noteSender.ChangeOutputDevice(outputDevice);
            Console.WriteLine(outputDevice.Name);
        }
        public void Close()
        {
            noteSender.Close();
            //OSCreceiver.Close();
        }
        
    }
}
