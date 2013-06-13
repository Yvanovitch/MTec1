using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Leap;

namespace LeapOrchestra
{
    class LeapListener : Listener
    {
        private Object thisLock = new Object();

        public event Action<FingerList> OnFingersRegistered;
        public event Action<GestureList> OnGesturesRegistered;

        private void SafeWriteLine(String line)
        {
            lock (thisLock)
            {
                Console.WriteLine(line);
            }
        }

        public override void OnInit(Controller controller)
        {
            SafeWriteLine("Initialized");
        }

        public override void OnConnect(Controller controller)
        {
            SafeWriteLine("Connected");
            controller.EnableGesture(Gesture.GestureType.TYPECIRCLE);
            controller.EnableGesture(Gesture.GestureType.TYPEKEYTAP);
            controller.EnableGesture(Gesture.GestureType.TYPESCREENTAP);
            controller.EnableGesture(Gesture.GestureType.TYPESWIPE);
        }

        public override void OnDisconnect(Controller controller)
        {
            //Note: not dispatched when running in a debugger.
            SafeWriteLine("Disconnected");
        }

        public override void OnExit(Controller controller)
        {
            SafeWriteLine("Exited");
        }

        public override void OnFrame(Controller controller)
        {
            
            // Get the most recent frame and report some basic information
            Frame frame = controller.Frame();

            if (frame.Fingers.Count != 0)
            {
                OnFingersRegistered(frame.Fingers);
                OnGesturesRegistered(frame.Gestures());

                if (!frame.Hands.Empty || !frame.Gestures().Empty)
                {
                    //SafeWriteLine("");
                }
            }
        }
    }
}