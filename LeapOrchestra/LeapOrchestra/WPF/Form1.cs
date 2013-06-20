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


namespace LeapOrchestra
{

    public partial class Form1 : Form
    {
        TextBox mid;
        String nameFile;
        int tempo;
        public event Action<string> sendPath;

        public Form1()
        {
            InitializeComponent();
            
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        public void GetTempo(int tempo)
        {
            this.tempo = tempo;
            label1.Text = tempo.ToString();
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
                //sm.readMidiFile(nameFile);
                /*try
                {
                    StreamReader sr = new StreamReader(nameFile);
                    mid.Text = sr.ReadToEnd();
                    sr.Close();
                }
                catch (Exception maieuh)
                {
                    mid.Text = "Impossible de lire ce fichier : ";
                    mid.Text = maieuh.Message;
                }*/
            }
        }


        private void midiToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PrefMidi prefmidi = new PrefMidi();
            prefmidi.ShowDialog();
        }

        private void oSCToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PrefOSC prefosc = new PrefOSC();
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

    }
}
