using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ZedGraph;

namespace LeapOrchestra
{
    public partial class Graph : Form
    {
        double[] x = new double[100];
        double[] y = new double[100];
        double[] z = new double[100];
        static int mycounter = 0;
        public Graph()
        {
            InitializeComponent();
            timer1.Tick += new EventHandler(timer1_Tick);
            timer1.Interval = 500;
            timer1.Start();
        }

        private void Graph_Load(object sender, EventArgs e)
        {

        }

        private void zedGraphControl1_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Lets generate sine and cosine wave
            

            for (int i = 0; i < x.Length; i++)
            {
                x[i] = mycounter;
                y[i] = mycounter;
                z[i] = i;
            }

            // This is to remove all plots
            zedGraphControl1.GraphPane.CurveList.Clear();

            // GraphPane object holds one or more Curve objects (or plots)
            GraphPane myPane = zedGraphControl1.GraphPane;

            // PointPairList holds the data for plotting, X and Y arrays 
            PointPairList spl1 = new PointPairList(x, y);
            PointPairList spl2 = new PointPairList(x, z);

            // Add cruves to myPane object
            LineItem myCurve1 = myPane.AddCurve("Sine Wave", spl1, Color.Blue, SymbolType.None);
            LineItem myCurve2 = myPane.AddCurve("Cosine Wave", spl2, Color.Red, SymbolType.None);

            myCurve1.Line.Width = 3.0F;
            myCurve2.Line.Width = 3.0F;
            myPane.Title.Text = "Show what you are doing";

            // I add all three functions just to be sure it refreshes the plot.   
            zedGraphControl1.AxisChange();
            zedGraphControl1.Invalidate();
            zedGraphControl1.Refresh();
        }

        private void SetGraph(int counter)
        {
            for (int i = 0; i < x.Length; i++)
            {
                x[i] = i;
                y[i] = Math.Sin(0.3 * counter * x[i]);
                z[i] = Math.Cos(0.3 * counter * x[i]);
            }

            // This is to remove all plots
            zedGraphControl1.GraphPane.CurveList.Clear();

            // GraphPane object holds one or more Curve objects (or plots)
            GraphPane myPane = zedGraphControl1.GraphPane;

            // PointPairList holds the data for plotting, X and Y arrays 
            PointPairList spl1 = new PointPairList(x, y);
            PointPairList spl2 = new PointPairList(x, z);

            // Add cruves to myPane object
            LineItem myCurve1 = myPane.AddCurve("Sine Wave", spl1, Color.Blue, SymbolType.None);
            LineItem myCurve2 = myPane.AddCurve("Cosine Wave", spl2, Color.Red, SymbolType.None);

            myCurve1.Line.Width = 3.0F;
            myCurve2.Line.Width = 3.0F;
            myPane.Title.Text = "Show what you are doing";

            // I add all three functions just to be sure it refreshes the plot.   
            zedGraphControl1.AxisChange();
            zedGraphControl1.Invalidate();
            zedGraphControl1.Refresh();
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            mycounter++;
            if(mycounter==10){
                mycounter = 0;
            }
            SetGraph(mycounter);
            zedGraphControl1.AxisChange();
            zedGraphControl1.Invalidate();
            zedGraphControl1.Refresh();
        }
    }
}
