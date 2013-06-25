﻿using System;
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
using ZedGraph;
using LeapOrchestra.Utils;


namespace LeapOrchestra
{

    public partial class Form1 : Form
    {
        Vector position;
        double[] x = new double[100];
        double[] y = new double[100];
        double[] z = new double[100];
        static int mycounter = 0;
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
            timer1.Tick += new EventHandler(timer1_Tick);
            timer1.Interval = 500;
            timer1.Start();
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

        private void button2_Click(object sender, EventArgs e)
        {
            Graph graph = new Graph();
            graph.Show();
        }
        public void GetVector(Vector position)
        {
            this.position = position;
        }
        private void SetGraph(Vector position)
        {
            if (position == null)
            {
                return;
            }
            for (int i = 0; i < x.Length; i++)
            {
                x[i] = position.x;
                y[i] = position.y;
                z[i] = 0;
            }

            // This is to remove all plots
            zedGraphControl1.GraphPane.CurveList.Clear();

            // GraphPane object holds one or more Curve objects (or plots)
            GraphPane myPane = zedGraphControl1.GraphPane;

            // PointPairList holds the data for plotting, X and Y arrays 
            PointPairList spl1 = new PointPairList(x, y);
            PointPairList spl2 = new PointPairList(x, z);

            // Add cruves to myPane object
            LineItem myCurve1 = myPane.AddCurve("position", spl1, Color.Blue, SymbolType.None);
            LineItem myCurve2 = myPane.AddCurve("Cosine Wave", spl2, Color.Red, SymbolType.None);

            myCurve1.Line.Width = 3.0F;
            myCurve2.Line.Width = 3.0F;
            myPane.Title.Text = "Position";
            myPane.XAxis.Scale.Min = -1000;
            myPane.XAxis.Scale.Max = -1000;
            myPane.YAxis.Scale.Min = -1000;
            myPane.YAxis.Scale.Max = -1000;
            // I add all three functions just to be sure it refreshes the plot.   
            zedGraphControl1.AxisChange();
            zedGraphControl1.Invalidate();
            zedGraphControl1.Refresh();
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            //SetLabel(label5, position.x);
            //SetLabel(label6, position.y);
            //SetLabel(label7, position.z);
            //SetLabel(label1, tempo);
            SetGraph(position);
            zedGraphControl1.AxisChange();
            zedGraphControl1.Invalidate();
            zedGraphControl1.Refresh();
        }
        private void SetLabel(System.Windows.Forms.Label label, float number)
        {
            label.Text = number.ToString();
        }
        private void zedGraphControl1_Load(object sender, EventArgs e)
        {

        }

        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }
    }
}

