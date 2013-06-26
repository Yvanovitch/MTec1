using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Leap;
using System.Threading;

namespace LeapOrchestra.Sensor
{
    class SensorManager
    {
        public SensorModel sensorModel;
        public KinectController kinectController;
        private Thread leapThread;
        private LeapController leapController;
        private LeapListener listener;

        public SensorManager()
        {
            //Kinect : Crée automatiquement un nouveau Thread
            kinectController = new KinectController();
            sensorModel = new SensorModel();
            kinectController.OnFrameEvent += sensorModel.OnFrame;
            LeapStarter();
        }

        public void LeapStarter()
        {
            //Leap Motion
            // Create a sample listener and controller
            listener = new LeapListener();
            leapController = new LeapController();
            //On active le traitement en background
            leapController.SetPolicyFlags(Controller.PolicyFlag.POLICYBACKGROUNDFRAMES);

            //LeapModel leapModel = new LeapModel();
            GesturesModel gesturesModel = new GesturesModel();

            listener.OnSensor += sensorModel.OnFrame;
            listener.OnGesturesRegistered += gesturesModel.OnGesturesRegistered;
            //Lancement du Thread du LeapMotion
            leapThread = new Thread(new ParameterizedThreadStart(leapController.AddListenerThreadable));
            leapThread.Start(listener);
        }

        public void Close()
        {
            leapController.RemoveListener(listener);
            leapController.Dispose();
            leapThread.Abort();
            leapThread.Abort();
            kinectController.Close();
        }
    }
}
