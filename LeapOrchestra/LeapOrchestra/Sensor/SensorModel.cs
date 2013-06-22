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
        private Queue<Vector> verticalVelocityBigLine;
        private Queue<Vector> horizontalVelocityBigLine;
        private DateTime lastFrameTime;
        private Vector lastMaxVelocity;
        private Vector lastPosition;
        private Vector lastBeatPosition;
        private SENSOR_TYPE currentSensor;
        private Direction previousDirection;
        private Direction currentDirection;
        private float pointRange = 100;

        public SensorModel()
        {
            tempoList = new Queue<int>();
            velocityLine = new Queue<Vector>();
            verticalVelocityBigLine = new Queue<Vector>();
            horizontalVelocityBigLine = new Queue<Vector>();
            lastPosition = new Vector(0, 0, 0);
            lastBeatPosition = new Vector(0, 0, 0);
            lastMaxVelocity = new Vector(0, 0, 0);
            lastFrameTime = DateTime.Now;
            currentSensor = SENSOR_TYPE.KINECT;
            previousDirection = Direction.VerticalUp;
            currentDirection = Direction.VerticalDown;
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
            velocity.Divide((float)(timeDiff / 1000));
            velocity.Multiply(30);

            velocityLine.Enqueue(velocity);
            if (velocityLine.Count > 3)
                velocityLine.Dequeue();


            Vector linearizedVelocity = VectorMath.Average(velocityLine);

            if (currentDirection == Direction.VerticalDown)
            {
                verticalVelocityBigLine.Enqueue(velocity);
                if (verticalVelocityBigLine.Count > 30)
                    verticalVelocityBigLine.Dequeue();

                Vector averageVelocity = VectorMath.Average(verticalVelocityBigLine);

                if (Math.Abs(averageVelocity.y) < (Math.Abs(linearizedVelocity.x) + Math.Abs(linearizedVelocity.z)/2) &&
                    lastBeatPosition.DistanceTo(position) > pointRange)
                {
                    //Nouvelle mesure : temps 1
                    currentDirection = Direction.Horizontal1;
                    VectorMath.ReverseQueue(horizontalVelocityBigLine, VectorMath.SelectedCoord.XZ);
                    Console.WriteLine("Temps 1");
                    lastBeatPosition = position;
                }
            }
            else if (currentDirection == Direction.Horizontal1)
            {
                horizontalVelocityBigLine.Enqueue(velocity);
                if (horizontalVelocityBigLine.Count > 30)
                    horizontalVelocityBigLine.Dequeue();

                Vector averageVelocity = VectorMath.Average(horizontalVelocityBigLine);

                float cos = VectorMath.CosFromUnstandardized(linearizedVelocity, averageVelocity, VectorMath.SelectedCoord.XZ);
                if (cos < -0.5 &&
                    lastBeatPosition.DistanceTo(position) > pointRange)
                {
                    //enregistrer le symétrique de la direction donné par averageVelocity
                    Console.WriteLine("Temps 2 : Cos : " + cos);
                    Console.Beep(440, 20);
                    velocity.Reverse(VectorMath.SelectedCoord.XZ); //On le retourne pour qu'il soit bien au final
                    VectorMath.ReverseQueue(horizontalVelocityBigLine, VectorMath.SelectedCoord.XZ);
                    currentDirection = Direction.Horizontal2;
                    lastBeatPosition = position;
                }
            }
            else if (currentDirection == Direction.Horizontal2)
            {
                horizontalVelocityBigLine.Enqueue(velocity);
                if (horizontalVelocityBigLine.Count > 30)
                    horizontalVelocityBigLine.Dequeue();

                Vector averageVelocity = VectorMath.Average(horizontalVelocityBigLine);

                if (Math.Abs(linearizedVelocity.y) > Math.Abs(averageVelocity.x) + Math.Abs(averageVelocity.z) &&
                    lastBeatPosition.DistanceTo(position) > pointRange)
                {
                    //Nouvelle mesure : temps 3
                    currentDirection = Direction.VerticalUp;
                    VectorMath.ReverseQueue(verticalVelocityBigLine, VectorMath.SelectedCoord.XY);
                    Console.WriteLine("Temps 3");
                    lastBeatPosition = position;
                }
            }
            else if (currentDirection == Direction.VerticalUp)
            {
                verticalVelocityBigLine.Enqueue(velocity);
                if (verticalVelocityBigLine.Count > 30)
                    verticalVelocityBigLine.Dequeue();

                Vector averageVelocity = VectorMath.Average(verticalVelocityBigLine);

                float cos = VectorMath.CosFromUnstandardized(linearizedVelocity, averageVelocity, VectorMath.SelectedCoord.XY);
                if (cos < -0.3 &&
                    lastBeatPosition.DistanceTo(position) > pointRange)
                {
                    Console.WriteLine("Temps 4 : Cos : " + cos);
                    Console.Beep(440, 20);
                    velocity.Reverse(VectorMath.SelectedCoord.XY); //On le retourne pour qu'il soit bien au final
                    VectorMath.ReverseQueue(verticalVelocityBigLine, VectorMath.SelectedCoord.XZ);
                    currentDirection = Direction.VerticalDown;
                    lastBeatPosition = position;
                }
            }
            else
                Console.WriteLine("positionx y z:" + (int)position.x + " " + (int)position.y + " " + (int)position.z);


            lastPosition = position;


        }


    }

    enum Direction
    {
        VerticalUp,
        VerticalDown,
        Horizontal1,
        Horizontal2
    }

    enum SENSOR_TYPE
    {
        LEAP_MOTION = 0,
        KINECT = 1
    }
}
