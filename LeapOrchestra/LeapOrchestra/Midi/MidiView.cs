using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Midi;

namespace LeapOrchestra.Midi
{
    class MidiView
    {
        public OutputDevice outputDevice;

        public void initDevice()
        {
            outputDevice = MidiUtils.ChooseOutputDeviceFromConsole();
            if (outputDevice == null)
            {
                Console.WriteLine("No output devices, so can't run this example.");
                return;
            }
        }

        public void sendBang()
        {
            outputDevice.Open();
            outputDevice.SendControlChange(Channel.Channel1, Control.SustainPedal, 0);
            outputDevice.SendPitchBend(Channel.Channel1, 8192);
            outputDevice.SendNoteOn(Channel.Channel1, Pitch.C4, 80);
            Thread.Sleep(5000);
            outputDevice.Close();
        }
    }
}
