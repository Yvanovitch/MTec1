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
        private Queue<Vector> horizontalVelocityBigLine;
        private DateTime lastFrameTime;
        private Vector lastMaxVelocity;
        private Vector lastPosition;
        private Vector lastBeatPosition;
        private SENSOR_TYPE currentSensor;
        private Direction previousDirection;
        private Direction currentDirection;
        private float pointRange = 100;

        public event Action<int> evolvePartCursor;

        public SensorModel()
        {
            tempoList = new Queue<int>();
            velocityLine = new Queue<Vector>();
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
            if (velocityLine.Count > 4)
                velocityLine.Dequeue();


            Vector linearizedVelocity = VectorMath.Average(velocityLine);

            switch (currentDirection)
            {
                case Direction.VerticalDown:
                    if (Math.Abs(linearizedVelocity.y) < (Math.Abs(linearizedVelocity.x) + Math.Abs(linearizedVelocity.z) / 4) &&
                        lastBeatPosition.DistanceTo(position) > pointRange)
                    {
                        currentDirection = Direction.Horizontal1;
                        Console.WriteLine("Temps 2 : velo y " + linearizedVelocity.y + " x " + linearizedVelocity.x + " " + linearizedVelocity.z);
                        VectorMath.ReverseQueue(horizontalVelocityBigLine, VectorMath.SelectedCoord.XZ);
                        lastBeatPosition = position;
                        evolvePartCursor(2);
                    }
                    break;
                case Direction.Horizontal1:
                    horizontalVelocityBigLine.Enqueue(velocity);
                    if (horizontalVelocityBigLine.Count > 30)
                        horizontalVelocityBigLine.Dequeue();

                    Vector averageVelocity = VectorMath.Average(horizontalVelocityBigLine);

                    float cos = VectorMath.CosFromUnstandardized(linearizedVelocity, averageVelocity, VectorMath.SelectedCoord.XZ);
                    if (cos < -0.5 &&
                        lastBeatPosition.DistanceTo(position) > pointRange)
                    {
                        //enregistrer le symétrique de la direction donné par averageVelocity
                        Console.WriteLine("Temps 3 : Cos : " + cos);
                        Console.Beep(440, 20);
                        velocity.Reverse(VectorMath.SelectedCoord.XZ); //On le retourne pour qu'il soit bien au final
                        VectorMath.ReverseQueue(horizontalVelocityBigLine, VectorMath.SelectedCoord.XZ);
                        currentDirection = Direction.Horizontal2;
                        lastBeatPosition = position;
                        evolvePartCursor(3);
                    }
                    break;
                case Direction.Horizontal2:
                    horizontalVelocityBigLine.Enqueue(velocity);
                    if (horizontalVelocityBigLine.Count > 30)
                        horizontalVelocityBigLine.Dequeue();

                    if (Math.Abs(linearizedVelocity.y) > (Math.Abs(linearizedVelocity.x) + Math.Abs(linearizedVelocity.z)) * 4 &&
                        lastBeatPosition.DistanceTo(position) > pointRange)
                    {
                        //Nouvelle mesure : temps 3
                        currentDirection = Direction.VerticalUp;
                        Console.WriteLine("Temps 4 : y" + linearizedVelocity.y + " x " + linearizedVelocity.x + " z " + linearizedVelocity.z);
                        lastBeatPosition = position;
                        evolvePartCursor(4);
                    }
                    break;
                case Direction.VerticalUp:

                    if (linearizedVelocity.y < -0.6 &&
                        lastBeatPosition.DistanceTo(position) > pointRange)
                    {
                        Console.WriteLine("Temps 1 : y :" + linearizedVelocity.y);
                        Console.Beep(440, 20);
                        currentDirection = Direction.VerticalDown;
                        lastBeatPosition = position;
                        evolvePartCursor(1);
                    }
                    break;
            }

            //Console.WriteLine("               positionx y z:" + (int)position.x + " " + (int)position.y + " " + (int)position.z);

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
        LEAP_MOTION,
        KINECT
    }
}
