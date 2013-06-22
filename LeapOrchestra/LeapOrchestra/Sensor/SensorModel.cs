using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LeapOrchestra.Utils;

namespace LeapOrchestra.Sensor
{
    
    class SensorModel
    {
        private Queue<int> tempoList;
        private Queue<Vector> velocityLine;
        private DateTime lastFrameTime;
        private Vector lastVelocity;
        private Vector lastPosition;
        private SENSOR_TYPE currentSensor;
        
        public SensorModel()
        {
            tempoList = new Queue<int>();
            velocityLine = new Queue<Vector>();
            lastPosition = new Vector(0, 0, 0);
            lastVelocity = new Vector(0, 0, 0);
            lastFrameTime = DateTime.Now;
            currentSensor = SENSOR_TYPE.KINECT;
        }

        public void OnFrame(SENSOR_TYPE sensor, float X, float Y, float Z)
        {
            OnFrame(sensor, new Vector(X, Y, Z));
        }

        public void OnFrame(SENSOR_TYPE sensor, Vector position)
        {
            TimeSpan timeDifference = DateTime.Now - lastFrameTime;
            double timeDiff = timeDifference.TotalMilliseconds;

            Vector velocity = VectorMath.Difference(position, lastPosition);
            velocity.Divide((float) (timeDiff/1000) );
            velocity.Multiply(10);

            velocityLine.Enqueue(velocity);
            if (velocityLine.Count > 4)
                velocityLine.Dequeue();

            Console.WriteLine("velocity x:" + (int)VectorMath.Average(velocityLine).x);


            lastPosition = position;
            
            
        }

        
    }

    enum SENSOR_TYPE
    {
        LEAP_MOTION = 0,
        KINECT = 1
    }
}
