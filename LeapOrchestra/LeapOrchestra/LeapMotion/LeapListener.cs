using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Leap;
using LeapOrchestra.Utils;

namespace LeapOrchestra.Sensor
{
    class LeapListener : Listener
    {
        private Object thisLock = new Object();

        public event Action<SENSOR_TYPE, Utils.Vector, Utils.Vector> OnSensor;
        public event Action<Frame> OnFrameRegistered;
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
            SafeWriteLine("Leap Initialized");
        }

        public override void OnConnect(Controller controller)
        {
            SafeWriteLine("Leap Connected");
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
                //OnFrameRegistered(frame);
                
                //OnGesturesRegistered(frame.Gestures());

                if (!frame.Hands.Empty || !frame.Gestures().Empty)
                {
                    OnSensor(SENSOR_TYPE.LEAP_MOTION, VectorMath.GetVector(frame.Hands[0].PalmPosition),
                        VectorMath.GetVector(frame.Hands[0].PalmVelocity));
                    //SafeWriteLine("");
                }
            }

        }

        public override void OnFocusLost(Controller arg0)
        {
            base.OnFocusLost(arg0);
            SafeWriteLine("Focus lost");
        }
    }
}