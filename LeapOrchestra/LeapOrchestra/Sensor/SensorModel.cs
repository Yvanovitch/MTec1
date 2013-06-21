using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Leap;

namespace LeapOrchestra.Sensor
{
    
    class SensorModel
    {
        private short SENSOR_NB = 2;
        private double lastAnalysisTime;
        private Queue<int> tempoList;
        private List<DateTime> lastFrameTime;
        private Vector lastKinectPosition;
        private Vector lastKinectVelocity;
        private Vector lastLeapPosition;
        private Vector lastLeapVelocity;
        
        public SensorModel()
        {
            lastAnalysisTime = 0;
            tempoList = new Queue<int>();
            lastKinectPosition = new Vector(0, 0, 0);
            lastKinectVelocity = new Vector(0, 0, 0);
            lastLeapPosition = new Vector(0, 0, 0);
            lastLeapVelocity = new Vector(0, 0, 0);
            for (int i = 0; i < SENSOR_NB; i++)
            {
                //lastFrameTime[SENSOR_TYPE.KINECT] = DateTime.Now;
            }
        }

        public void OnFrame(SENSOR_TYPE sensor, float X, float Y, float Z)
        {
            OnFrame(sensor, new Vector(X, Y, Z));
        }

        public void OnFrame(SENSOR_TYPE sensor, Vector position)
        {
            TimeSpan timeDifference;
            double timeDiff ;
            
            /*switch (sensor )
            {
                case SENSOR_TYPE.KINECT :
                    timeDifference = DateTime.Now - lastKinectFrameTime;
                    timeDiff = timeDifference.TotalMilliseconds;
                    lastKinectVelocity = position.Cross(lastKinectPosition);
                    break;
                default :
                    timeDifference = DateTime.Now - lastLeapFrameTime;
                    timeDiff = timeDifference.TotalMilliseconds;
                    lastLeapVelocity = position.Cross(lastLeapPosition);
                    break;
            }*/

            
            
        }
    }

    enum SENSOR_TYPE
    {
        LEAP_MOTION = 0,
        KINECT = 1
    }
}
