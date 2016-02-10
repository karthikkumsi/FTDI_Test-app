using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ZedGraph;
using System.IO.Ports;
using System.Threading;
using System.Timers;

namespace zedgraph
{
    public partial class Form1 : Form
    {
        
        
        private ZedGraph.ZedGraphControl zedGraphControl1;
        GraphPane myPane ;

       // PointPairList list1 = new PointPairList();
        PointPairList list2 = new PointPairList();
        double i, j;
        LineItem myCurve;
        bool stoplist = false;
        RollingPointPairList list1 = new RollingPointPairList(12000);
        System.Timers.Timer timer1 = new System.Timers.Timer(1000);
                

        public Form1()
        {
            InitializeComponent();

            Run();
        }

        private void Run()
        {
            CreateGraph();
            
                
                Thread update_list_thread1 = new Thread(new ThreadStart(update_list_thread));
                Thread update_list_thread2 = new Thread(new ThreadStart(stop_updating));
                
                //update_list_thread1.Start();
                //update_list_thread2.Start();
                
                
          
            stoplist = true;
           // CreateGraph(zedGraphControl1);
        
        }

        // thread to update the list
        public void update_list_thread()
        {

            while (!stoplist)
            {
                list1.Add(i, j);
                i++;
                j++;
                
            }
            Thread.Sleep(1);

        }

        public void stop_updating()
        {

            stoplist = false;
            Thread.Sleep(100);
            funk();
            stoplist = true;
            Thread.Sleep(1000);
            stoplist = false;
        }

        // SetSize() is separate from Resize() so we can 
        // call it independently from the Form1_Load() method
        // This leaves a 10 px margin around the outside of the control
        // Customize this to fit your needs
        private void SetSize()
        {
            zedGraphControl1.Location = new Point(10, 10);
            // Leave a small margin around the outside of the control
            zedGraphControl1.Size = new Size(ClientRectangle.Width - 20,
                                    ClientRectangle.Height - 20);
        }

        // Respond to form 'Load' event
        private void funk()
        {
            //k = 0;
            myCurve = myPane.AddCurve("Value", list1, Color.Red, SymbolType.None);
        }
        private void CreateGraph()
        {
            

            
            // get a reference to the GraphPane

            myPane = zedGraphControl1.GraphPane;
            // Set the Titles
            myPane.Title.Text = "My Test Graph\n(For CodeProject Sample)";
            myPane.XAxis.Title.Text = "My X Axis";
            myPane.YAxis.Title.Text = "My Y Axis";

            // Make up some data arrays based on the Sine function
            double x, y1, y2;

            // Generate a red curve with diamond
            // symbols, and "Porsche" in the legend
             myCurve = myPane.AddCurve("Porsche",
                  list1, Color.Red, SymbolType.Diamond);
           
            
            timer1.Interval = 100;
            timer1.Enabled = true;
            timer1.Start();
            

            // Tell ZedGraph to refigure the
            // axes since the data have changed
          zedGraphControl1.AxisChange();
            Thread.Sleep(10);
           // zedGraphControl1.Invalidate();
            

        }
    }
}
