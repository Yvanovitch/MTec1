using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using LeapOrchestra.SongPlayer;
using Midi;
using NAudio.Midi;

namespace LeapOrchestra
{
    public partial class PrefMidi : Form
    {
        public OutputDevice outputDevice;
        public PrefMidi()
        {
            InitializeComponent();
            refreshList();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        
        private void refreshList()
        {
            listBox1.Items.Clear();
            for (int i = 0; i < OutputDevice.InstalledDevices.Count; i++)
            {
                listBox1.Items.Add(OutputDevice.InstalledDevices[i].Name);
            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
