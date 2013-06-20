using System;
using System.Threading;
using Leap;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using LeapOrchestra.SongPlayer;
using Midi;

using LeapOrchestra.Kinect;

namespace LeapOrchestra
{
    class LeapOrchestra
    {
        [STAThread]
        public static void Main()
        {
            

        //Leap Motion
            // Create a sample listener and controller
            LeapListener listener = new LeapListener();
            LeapController controller = new LeapController();
            //On active le traitement en background
            controller.SetPolicyFlags(Controller.PolicyFlag.POLICYBACKGROUNDFRAMES);

            LeapModel leapModel = new LeapModel();
            GesturesModel gesturesModel = new GesturesModel();

            listener.OnFrameRegistered += leapModel.OnFrameRegistered;
            listener.OnGesturesRegistered += gesturesModel.OnGesturesRegistered;

        //Kinect
            KinectController kinectController = new KinectController();

            //Sound Management
            SoundManager soundManager = new SoundManager();
            soundManager.readMidiFile(@"D:\Documents\Cours\Orchestra\Midi\In\z2temple.mid");
            leapModel.sendBang += soundManager.throwBang;


            // Have the listener receive events from the controller in a new thread
            Thread leapThread = new Thread(new ParameterizedThreadStart(controller.AddListenerThreadable));
            leapThread.Start(listener);

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());

            while (true)
            {
                Thread.Sleep(10);
                soundManager.Process();
            }


            // Keep this process running until Enter is pressed
            Console.WriteLine("Press Enter to quit...");
            Console.ReadLine();

            // Remove the sample listener when done
            controller.RemoveListener(listener);
            controller.Dispose();

            soundManager.Close();
        }
    }
}
