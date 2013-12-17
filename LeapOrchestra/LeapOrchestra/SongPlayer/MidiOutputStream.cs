using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Midi;
using NAudio.Midi;

namespace LeapOrchestra.SongPlayer
{
    class MidiOutputStream : NoteOutputStream
    {
        public OutputDevice outputDevice;

        public MidiOutputStream() : base ()
        {
            if(OutputDevice.InstalledDevices.Count >= 1)
            {
                Console.WriteLine("Sortie(s) midi installée(s) :");
                foreach (OutputDevice outProposed in OutputDevice.InstalledDevices)
                {
                    Console.WriteLine(outProposed.Name);
                }

                //On prend le premier device par default, l'user réglera la sortie dans l'interface graphique (non-implémenté)
                outputDevice = OutputDevice.InstalledDevices[1];
            }

            if (outputDevice == null)
            {
                Console.WriteLine("No output devices, so can't run this example.");
                return;
            }
            outputDevice.Open();
            Console.WriteLine("Midi OutputDevice : " + outputDevice.Name);
        }

        public override void ChangeOutputDevice(OutputDevice outputDevice)
        {
            this.outputDevice.Close();
            this.outputDevice = outputDevice;
        }

        public override void SendBang()
        {
            if (!outputDevice.IsOpen)
            {
                outputDevice.Open();
            }
            outputDevice.SendNoteOn(Channel.Channel1, Pitch.C4, 80);
        }

        public override void SendNote(NoteEvent note)
        {
            if (note is NoteOnEvent)
            {
                outputDevice.SendNoteOn((Channel)note.Channel, (Pitch)note.NoteNumber, note.Velocity);
                /*if (((NoteOnEvent)note).OffEvent == null)
                {
                    Console.WriteLine("Off Event Error");
                }*/
            }
            else
            {
                outputDevice.SendNoteOff((Channel)note.Channel, (Pitch)note.NoteNumber, 80);
            }
        }

        public override void SendProgramChange(int track, int ref_instrument)
        {
            outputDevice.SendProgramChange((Channel)track, (Instrument)ref_instrument);
        }

        public override void SendEnd()
        {
            outputDevice.SilenceAllNotes();
        }

        public override void Close()
        {
            
            base.Close();
            outputDevice.Close();
        }
    }
}
