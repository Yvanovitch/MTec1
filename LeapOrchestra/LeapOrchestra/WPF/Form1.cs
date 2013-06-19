using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace LeapOrchestra
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void configurationToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void outputToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
           
            if (MessageBox.Show("Ouvrir un Fichier", "Open a File", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                Form1 ff = new Form1();
            }
            else
            {
                this.Close();
            }
        }


        private void midiToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //MidiPref pref = new MidiPref();
        }

        private void oSCToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //OSCPref pref = new OSCPref();
        }
    }
}
