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
            outputDevice = MidiUtils.ChooseOutputDeviceFromConsole();
            if (outputDevice == null)
            {
                Console.WriteLine("No output devices, so can't run this example.");
                return;
            }
            outputDevice.Open();
        }

        public override void SendBang()
        {
            outputDevice.SendControlChange(Channel.Channel1, Control.SustainPedal, 0);
            outputDevice.SendPitchBend(Channel.Channel1, 8192);
            outputDevice.SendNoteOn(Channel.Channel1, Pitch.C4, 80);
        }

        public override void SendNote(NoteEvent note)
        {
            if (note is NoteOnEvent)
            {
                outputDevice.SendNoteOn((Channel)note.Channel, (Pitch)note.NoteNumber, note.Velocity);
                if (((NoteOnEvent)note).OffEvent == null)
                {
                    Console.WriteLine("Off Event Error");
                }
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

        public override void Close()
        {
            base.Close();
            outputDevice.Close();
        }
    }
}
