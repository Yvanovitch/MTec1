using System;
using System.Threading;
using Leap;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using LeapOrchestra.SongPlayer;
using Midi;

namespace LeapOrchestra
{
    class LeapOrchestra
    {
        public static void Main()
        {
            /*Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());*/
            // Create a sample listener and controller
            LeapListener listener = new LeapListener();
            LeapController controller = new LeapController();

            //On active le traitement en background
            controller.SetPolicyFlags(Controller.PolicyFlag.POLICYBACKGROUNDFRAMES);

            

            LeapModel leapModel = new LeapModel();
            GesturesModel gesturesModel = new GesturesModel();

            listener.OnFrameRegistered += leapModel.OnFrameRegistered;
            listener.OnGesturesRegistered += gesturesModel.OnGesturesRegistered;

            //Sound Management
            SoundManager soundManager = new SoundManager();
            soundManager.readMidiFile(@"D:\Documents\Cours\Orchestra\Midi\In\link.mid");
            leapModel.sendBang += soundManager.throwBang;

            listener.OnNewFrame += soundManager.Process;

            // Have the sample listener receive events from the controller
            Thread leapThread = new Thread(new ParameterizedThreadStart(controller.AddListenerThreadable));
            leapThread.Start(listener);

            while (true)
            {
                if(Console.KeyAvailable)
                {
                    ConsoleKeyInfo key = Console.ReadKey(true);

                    soundManager.throwBang();
                    Thread.Sleep(10);
                    soundManager.Process();
                    Thread.Sleep(10);
                    soundManager.Process();
                    Thread.Sleep(10);
                    soundManager.Process();
                    Thread.Sleep(10);
                    soundManager.Process();
                    Thread.Sleep(10);
                    soundManager.Process();
                    Thread.Sleep(10);
                    soundManager.Process();
                    Thread.Sleep(10);
                    soundManager.Process();
                }
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
