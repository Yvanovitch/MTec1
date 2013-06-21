using System;
using System.Threading;
using Leap;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using LeapOrchestra.SongPlayer;
using Midi;
using Utils.OSC;
using LeapOrchestra.Sensor;

namespace LeapOrchestra
{
    class LeapOrchestra
    {
        [STAThread]
        public static void Main()
        {
        //Sound Management
            SoundManager soundManager = new SoundManager();
            //soundManager.readMidiFile(@"D:\Documents\Cours\Orchestra\Midi\In\z2temple.mid");
            Thread soundManagement = new Thread(soundManager.Process);
            //Lancement du thread de managament
            soundManagement.Start();
            //Receiver receiver = new Receiver();
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
            leapModel.sendBang += soundManager.throwBang;
            //Lancement du Thread du LeapMotion
            Thread leapThread = new Thread(new ParameterizedThreadStart(controller.AddListenerThreadable));
            leapThread.Start(listener);


            SensorManager sensorManager = new SensorManager();
            

        //Application
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Form1 form1 = new Form1();
            form1.sendBang += soundManager.throwBang;
            form1.sendPath += soundManager.readMidiFile;
            form1.sendOutputDevice += soundManager.GetOutputDevice;
            soundManager.sendTempo += form1.GetTempo;
            //form1.prefosc.sendPort += receiver.GetPort;
            Application.Run(form1);

        // Fermeture de tout les threads
            controller.RemoveListener(listener);
            controller.Dispose();
            leapThread.Abort();

            soundManagement.Abort();
            soundManager.Close();
            sensorManager.Close();
        }
    }
}
