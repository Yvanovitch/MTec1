using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LeapOrchestra.Utils;

namespace LeapOrchestra.Sensor
{

    class SensorModel
    {
        private Queue<double> bangTimeList;
        private Queue<Vector> velocityLine;
        private Queue<Vector> horizontal1VelocityLine;
        private Queue<Vector> horizontal2VelocityLine;
        private DateTime lastBangTime;
        private DateTime lastFrameTime;
        private Vector lastMaxVelocity;
        private Vector lastPosition;
        private Vector lastBeatPosition;
        private SENSOR_TYPE currentSensor;
        private Direction previousDirection;
        private Direction currentDirection;
        private float pointRange = 200;
        private Boolean hasMiss;


        public event Action<int> evolvePartCursor;
        public event Action<float> SendOrientation;

        public SensorModel()
        {
            bangTimeList = new Queue<double>();
            lastBangTime = DateTime.Now;
            velocityLine = new Queue<Vector>();
            horizontal1VelocityLine = new Queue<Vector>();
            horizontal2VelocityLine = new Queue<Vector>();
            lastPosition = new Vector(0, 0, 0);
            lastBeatPosition = new Vector(0, 0, 0);
            lastMaxVelocity = new Vector(0, 0, 0);
            lastFrameTime = DateTime.Now;
            currentSensor = SENSOR_TYPE.KINECT;
            previousDirection = Direction.VerticalUp;
            currentDirection = Direction.VerticalDown;
            hasMiss = false;
        }

        public void OnFrame(SENSOR_TYPE sensor, float X, float Y, float Z)
        {
            OnFrame(sensor, new Vector(X, Y, Z));
        }

        public void OnFrame(SENSOR_TYPE sensor, Vector position)
        {
            TimeSpan timeBangDifference = DateTime.Now - lastBangTime;
            double timeBangDiff = timeBangDifference.Milliseconds;
            if (timeBangDiff < 250 && currentDirection != Direction.Horizontal1)
            {
                lastFrameTime = DateTime.Now;
                lastPosition = position;
                return;
            }
            else if (timeBangDiff > 930) //On a manqué qqch
            {
                if(!hasMiss) {
                Console.WriteLine("Temps manqué -> On revient au 1");
                    hasMiss = true;
                }
                currentDirection = Direction.VerticalUp;
                lastBeatPosition = new Vector(0, 0, 0);
                velocityLine.Clear();
                velocityLine.Enqueue(new Vector(0, 1000, 0));
                velocityLine.Enqueue(new Vector(0, 1000, 0));
                velocityLine.Enqueue(new Vector(0, 1000, 0));
                velocityLine.Enqueue(new Vector(0, 1000, 0));
            }

            TimeSpan timeDifference = DateTime.Now - lastFrameTime;
            double timeDiff = timeDifference.TotalMilliseconds;

            Vector velocity = VectorMath.Difference(position, lastPosition);
            velocity.Divide((float)(timeDiff / 1000));

            velocityLine.Enqueue(velocity);
            if (velocityLine.Count > 5)
                velocityLine.Dequeue();


            Vector linearizedVelocity = VectorMath.Average(velocityLine);

            switch (currentDirection)
            {
                case Direction.VerticalDown:
                    if (Math.Abs(linearizedVelocity.y) < (Math.Abs(linearizedVelocity.x) + Math.Abs(linearizedVelocity.z) / 1.5) &&
                        lastBeatPosition.DistanceTo(position) > pointRange)
                    {
                        currentDirection = Direction.Horizontal1;
                        //Console.WriteLine("Temps 2 : velo y " + linearizedVelocity.y + " x " + linearizedVelocity.x + " " + linearizedVelocity.z);
                        lastBeatPosition = position;
                        evolvePartCursor(2);
                        lastBangTime = DateTime.Now;
                    }
                    break;
                case Direction.Horizontal1:
                    Vector averageVelocity = VectorMath.Average(horizontal1VelocityLine);

                    float cos = VectorMath.CosFromUnstandardized(linearizedVelocity, averageVelocity, VectorMath.SelectedCoord.XZ);
                    if (cos < -0.3 &&
                        lastBeatPosition.DistanceTo(position) > pointRange)
                    {
                        //enregistrer le symétrique de la direction donné par averageVelocity
                        //Console.WriteLine("Temps 3 : Cos : " + cos + "average :" + averageVelocity);
                        velocity.Reverse(VectorMath.SelectedCoord.XZ); //On le retourne pour qu'il soit bien au final
                        currentDirection = Direction.Horizontal2;
                        lastBeatPosition = position;
                        evolvePartCursor(3);
                        lastBangTime = DateTime.Now;
                        ManageOrientation(averageVelocity); //On met à jour l'orientation
                    }
                    else
                    {
                        horizontal1VelocityLine.Enqueue(velocity);
                        if (horizontal1VelocityLine.Count > 11)
                            horizontal1VelocityLine.Dequeue();
                    }
                    break;
                case Direction.Horizontal2:
                    horizontal2VelocityLine.Enqueue(velocity);
                    if (horizontal2VelocityLine.Count > 10)
                        horizontal2VelocityLine.Dequeue();

                    if (Math.Abs(linearizedVelocity.y) > (Math.Abs(linearizedVelocity.x) + Math.Abs(linearizedVelocity.z)) * 1.5 &&
                        lastBeatPosition.DistanceTo(position) > pointRange)
                    {
                        currentDirection = Direction.VerticalUp;
                        //Console.WriteLine("Temps 4 : y" + linearizedVelocity);
                        lastBeatPosition = position;
                        evolvePartCursor(4);
                        lastBangTime = DateTime.Now;
                    }
                    break;
                case Direction.VerticalUp:

                    if (linearizedVelocity.y < -100 &&
                        lastBeatPosition.DistanceTo(position) > pointRange)
                    {
                        Console.WriteLine("Temps 1 : y :" + linearizedVelocity.y);
                        currentDirection = Direction.VerticalDown;
                        lastBeatPosition = position;
                        evolvePartCursor(1);
                        lastBangTime = DateTime.Now;
                        hasMiss = false;
                    }
                    break;
            }

           // Console.WriteLine("               average :"+VectorMath.Average(horizontalVelocityBigLine));
            

            lastPosition = position;
            lastFrameTime = DateTime.Now;
        }

        private void ManageOrientation(Vector horizontalVelocity)
        {
            Vector normedVelocity = VectorMath.GetNormalized(horizontalVelocity);

            float orientation = 1 - Math.Abs(normedVelocity.x);
            if ( (normedVelocity.z > 0 && normedVelocity.x < 0) ||
                (normedVelocity.z < 0 && normedVelocity.x > 0) )
            {
                orientation = orientation * (-1);
            }

            Console.WriteLine("orientation : "+orientation +" normed : " + normedVelocity);
            SendOrientation(orientation);
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
