﻿using System;
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
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
            // Create a sample listener and controller
            LeapListener listener = new LeapListener();
            Controller controller = new Controller();

            //On active le traitement en background
            controller.SetPolicyFlags(Controller.PolicyFlag.POLICYBACKGROUNDFRAMES);

            

            LeapModel leapModel = new LeapModel();
            GesturesModel gesturesModel = new GesturesModel();

            listener.OnFrameRegistered += leapModel.OnFrameRegistered;
            listener.OnGesturesRegistered += gesturesModel.OnGesturesRegistered;

            

            SoundManager soundManager = new SoundManager();
            soundManager.readMidiFile(@"D:\Documents\Cours\Orchestra\Midi\In\link.mid");
            leapModel.sendBang += soundManager.throwBang;

            listener.OnNewFrame += soundManager.Process;

            // Have the sample listener receive events from the controller
            controller.AddListener(listener);


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
