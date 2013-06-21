using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LeapOrchestra.Sensor
{
    class SensorManager
    {
        SensorModel sensorModel;
        KinectController kinectController;

        public SensorManager()
        {
            //Kinect : Crée automatiquement un nouveau Thread
            kinectController = new KinectController();
            sensorModel = new SensorModel();
            kinectController.OnFrameEvent += sensorModel.OnFrame;
        }

        public void Close()
        {
            kinectController.Close();
        }
    }
}
