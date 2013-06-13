using System;
using System.Threading;
using Leap;

namespace LeapOrchestra
{
    class LeapOrchestra
    {
        public static void Main()
        {
            // Create a sample listener and controller
            LeapListener listener = new LeapListener();
            Controller controller = new Controller();
            // Have the sample listener receive events from the controller

            LeapModel leapModel = new LeapModel();
            GesturesModel gesturesModel = new GesturesModel();

            listener.OnFrameRegistered += leapModel.OnFrameRegistered;
            listener.OnGesturesRegistered += gesturesModel.OnGesturesRegistered;

            controller.AddListener(listener);

            // Keep this process running until Enter is pressed
            Console.WriteLine("Press Enter to quit...");
            Console.ReadLine();

            // Remove the sample listener when done
            controller.RemoveListener(listener);
            controller.Dispose();
        }
    }
}
