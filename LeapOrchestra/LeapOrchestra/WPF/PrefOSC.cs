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

    public partial class PrefOSC : Form
    {
        Int32 portbox;
        public event Action<Int32> sendPort;
        public PrefOSC()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            portbox = Convert.ToInt32(textBox2.Text);
            sendPort(portbox);
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
        }

        private void textBox1_TextChanged_1(object sender, EventArgs e)
        {
        
        }

        private void textBox2_TextChanged_1(object sender, EventArgs e)
        {
            
        }
    }
}
