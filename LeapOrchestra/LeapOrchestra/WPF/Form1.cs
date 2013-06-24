using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using LeapOrchestra.SongPlayer;
using System.Runtime.InteropServices;
using Midi;
using NAudio.Midi;


namespace LeapOrchestra
{

    public partial class Form1 : Form
    {
        TextBox mid;
        String nameFile;
        int tempo;
        public OutputDevice outputdevice;
        public event Action<OutputDevice> sendOutputDevice;
        public event Action<string> sendPath;
        public event Action sendBang;
        public PrefOSC prefosc;
        public PrefMidi prefmidi;

        public Form1()
        {
            InitializeComponent();
            prefosc = new PrefOSC();
            prefmidi = new PrefMidi();
            prefmidi.sendDevice += this.GetOutputDevice;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
        }
        public void GetTempo(int tempo)
        {
            this.tempo = tempo;
            //label1.Text = "Tempo 120";//"Tempo : " + this.tempo.ToString();
        }

        public void GetOutputDevice(OutputDevice outputdevice)
        {
            this.outputdevice = outputdevice;
            sendOutputDevice(outputdevice);
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void configurationToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void outputToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog oFD = new OpenFileDialog();
            oFD.InitialDirectory = "c:\\";
            oFD.Filter = "Fichiers Midi (*.mid)|*.mid|Tous les fichiers (*.*)|*.*";
            oFD.RestoreDirectory = true;

            if (oFD.ShowDialog() == DialogResult.OK)
            {
                nameFile = oFD.FileName;
                //MessageBox.Show(oFD.FileName);
                sendPath(nameFile);
            }
        }


        private void midiToolStripMenuItem_Click(object sender, EventArgs e)
        {
            prefmidi.ShowDialog();

        }

        private void oSCToolStripMenuItem_Click(object sender, EventArgs e)
        {
            prefosc.ShowDialog();
        }
        private void inputToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PrefInput prefinp = new PrefInput();
            prefinp.ShowDialog();
        }

        private void fileToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            sendBang();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }
}

