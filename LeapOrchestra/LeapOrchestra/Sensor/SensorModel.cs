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
        private Direction currentDirection;
        private Boolean hasMiss;
        private int analysisBeatsNumber;

        public event Action<Vector> sendPosition;
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
            currentDirection = Direction.VerticalDown;
            hasMiss = false;
            BeatsModel();
        }

        public void OnFrame(SENSOR_TYPE sensor, float X, float Y, float Z)
        {
            OnFrame(sensor, new Vector(X, Y, Z));
        }
        public void BeatsModel()
        {
            string entry;
            Boolean choosed = false;
            int result;
            Console.WriteLine("please enter the beat pattern : \n 2 = 2/4 ou 6/8 \n 3 = 3/4 ou 9/8 \n 4 = 4/4 \n");
            while (!choosed)
            {
                entry = Console.ReadLine();
                if (int.TryParse(entry, out result))
                {
                    if (result >= 2 && result <= 4)
                    {
                        choosed = true;
                        analysisBeatsNumber = result;
                    }
                }
            }
        }
        public void OnFrame(SENSOR_TYPE sensor, Vector position)
        {
            TimeSpan timeDifference = DateTime.Now - lastFrameTime;
            double timeDiff = timeDifference.TotalMilliseconds;

            Vector velocity = VectorMath.Difference(position, lastPosition);
            velocity.Divide((float)(timeDiff / 1000));

            lastFrameTime = DateTime.Now;
            lastPosition = position;
            OnFrame(sensor, position, velocity);
        }
        public void Analysis4Beats(SENSOR_TYPE sensor, Vector position, Vector velocity)
        {

            int velocity_base_y = 0;
            int velocity_threshold = 0;
            float pointRange = 90;
            switch (sensor)
            {
                case SENSOR_TYPE.KINECT:
                    velocity_base_y = 3000;
                    velocity_threshold = -200;
                    break;
                default:
                    velocity_base_y = 30;
                    velocity_threshold = -8;
                    pointRange = 90;
                    break;
            }

            TimeSpan timeBangDifference = DateTime.Now - lastBangTime;
            double timeBangDiff = timeBangDifference.Milliseconds;
            if (timeBangDiff < 250 && currentDirection != Direction.Horizontal1)
            {
                return;
            }
            else if (timeBangDiff > 930) //On a manqué qqch
            {
                if (!hasMiss)
                {
                    Console.WriteLine("Temps manqué -> On revient au 1");
                    hasMiss = true;
                }
                currentDirection = Direction.VerticalUp;
                lastBeatPosition = new Vector(0, 0, 0);
                velocityLine.Clear();
                velocityLine.Enqueue(new Vector(0, velocity_base_y, 0));
                velocityLine.Enqueue(new Vector(0, velocity_base_y, 0));
                velocityLine.Enqueue(new Vector(0, velocity_base_y, 0));
                velocityLine.Enqueue(new Vector(0, velocity_base_y, 0));
                velocityLine.Enqueue(new Vector(0, velocity_base_y, 0));
            }

            velocityLine.Enqueue(velocity);
            if (velocityLine.Count > 3)
                velocityLine.Dequeue();

            Vector linearizedVelocity = VectorMath.Average(velocityLine);

            switch (currentDirection)
            {
                case Direction.VerticalDown:
                    if (Math.Abs(linearizedVelocity.y) < (Math.Abs(linearizedVelocity.x) + Math.Abs(linearizedVelocity.z) / 1.2) &&
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
                    if (cos < -0.1 &&
                        timeBangDiff > 220 &&
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
                        if (horizontal1VelocityLine.Count > 13)
                            horizontal1VelocityLine.Dequeue();
                    }
                    break;
                case Direction.Horizontal2:
                    horizontal2VelocityLine.Enqueue(velocity);
                    if (horizontal2VelocityLine.Count > 10)
                        horizontal2VelocityLine.Dequeue();

                    if (linearizedVelocity.y > (Math.Abs(linearizedVelocity.x) + Math.Abs(linearizedVelocity.z)) / 1.2 &&
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
                    if (linearizedVelocity.y < velocity_threshold &&
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
        }

        public void Analysis3Beats(SENSOR_TYPE sensor, Vector position, Vector velocity)
        {
            int velocity_base_y = 0;
            int velocity_threshold = 0;
            float pointRange = 90;
            switch (sensor)
            {
                case SENSOR_TYPE.KINECT:
                    velocity_base_y = 3000;
                    velocity_threshold = -200;
                    break;
                default:
                    velocity_base_y = 30;
                    velocity_threshold = -6;
                    pointRange = 60;
                    break;
            }

            TimeSpan timeBangDifference = DateTime.Now - lastBangTime;
            double timeBangDiff = timeBangDifference.Milliseconds;
            if (timeBangDiff < 250 && currentDirection != Direction.Horizontal1)
            {
                return;
            }
            else if (timeBangDiff > 930) //On a manqué qqch
            {
                if (!hasMiss)
                {
                    Console.WriteLine("Temps manqué -> On revient au 1");
                    hasMiss = true;
                }
                currentDirection = Direction.VerticalDown;
                lastBeatPosition = new Vector(0, 0, 0);
                velocityLine.Clear();
                velocityLine.Enqueue(new Vector(0, velocity_base_y, 0));
                velocityLine.Enqueue(new Vector(0, velocity_base_y, 0));
                velocityLine.Enqueue(new Vector(0, velocity_base_y, 0));
                velocityLine.Enqueue(new Vector(0, velocity_base_y, 0));
                velocityLine.Enqueue(new Vector(0, velocity_base_y, 0));
            }

            velocityLine.Enqueue(velocity);
            if (velocityLine.Count > 3)
                velocityLine.Dequeue();

            Vector linearizedVelocity = VectorMath.Average(velocityLine);

            switch (currentDirection)
            {
                case Direction.VerticalUp:
                    if (linearizedVelocity.y > (Math.Abs(linearizedVelocity.x) + Math.Abs(linearizedVelocity.z) / 1.2) &&
                        lastBeatPosition.DistanceTo(position) > pointRange)
                    {
                        currentDirection = Direction.VerticalDown;
                        //Console.WriteLine("Temps 2 : velo y " + linearizedVelocity.y + " x " + linearizedVelocity.x + " " + linearizedVelocity.z);
                        lastBeatPosition = position;
                        evolvePartCursor(3);
                        lastBangTime = DateTime.Now;
                    }
                    break;
                case Direction.Horizontal2:
                    if (Math.Abs(linearizedVelocity.y) < (Math.Abs(linearizedVelocity.x) + Math.Abs(linearizedVelocity.z)) / 2 &&
                        lastBeatPosition.DistanceTo(position) > pointRange)
                    {
                        currentDirection = Direction.VerticalUp;
                        //Console.WriteLine("Temps 4 : y" + linearizedVelocity);
                        lastBeatPosition = position;
                        evolvePartCursor(2);
                        lastBangTime = DateTime.Now;
                    }
                    break;
                case Direction.VerticalDown:
                    if (linearizedVelocity.y < velocity_threshold &&
                        lastBeatPosition.DistanceTo(position) > pointRange)
                    {
                        Console.WriteLine("Temps 1 : y :" + linearizedVelocity.y);
                        currentDirection = Direction.Horizontal2;
                        lastBeatPosition = position;
                        evolvePartCursor(1);
                        lastBangTime = DateTime.Now;
                        hasMiss = false;
                    }
                    break;
            }
        }

        public void Analysis2Beats(SENSOR_TYPE sensor, Vector position, Vector velocity)
        {
            float pointRange = 90;
            switch (sensor)
            {
                case SENSOR_TYPE.KINECT:
                    break;
                default:
                    pointRange = 60;
                    break;
            }

            TimeSpan timeBangDifference = DateTime.Now - lastBangTime;
            double timeBangDiff = timeBangDifference.Milliseconds;
            if (timeBangDiff < 250 && currentDirection != Direction.Horizontal1)
            {
                return;
            }
            else if (timeBangDiff > 930) //On a manqué qqch
            {
                if (!hasMiss)
                {
                    Console.WriteLine("Temps manqué -> On revient au 1");
                    hasMiss = true;
                }
                currentDirection = Direction.VerticalDown;
                lastBeatPosition = new Vector(0, 0, 0);
            }

            velocityLine.Enqueue(velocity);
            if (velocityLine.Count > 3)
                velocityLine.Dequeue();

            Vector linearizedVelocity = VectorMath.Average(velocityLine);
            Vector averageVelocity;
            float cos;

            switch (currentDirection)
            {
                case Direction.Horizontal1:
                    averageVelocity = VectorMath.Average(horizontal1VelocityLine);
                    cos = VectorMath.CosFromUnstandardized(linearizedVelocity, averageVelocity, VectorMath.SelectedCoord.XYZ);
                    if (cos < -0.2 &&
                        timeBangDiff > 220 &&
                        lastBeatPosition.DistanceTo(position) > pointRange)
                    {
                        //Console.WriteLine("Temps 1 : Cos : " + cos + "average :" + averageVelocity);
                        currentDirection = Direction.Horizontal2;
                        lastBeatPosition = position;
                        evolvePartCursor(1);
                        lastBangTime = DateTime.Now;
                        ManageOrientation(averageVelocity); //On met à jour l'orientation
                    }
                    else
                    {
                        horizontal1VelocityLine.Enqueue(velocity);
                        if (horizontal1VelocityLine.Count > 13)
                            horizontal1VelocityLine.Dequeue();
                    }
                    break;
                default:
                    averageVelocity = VectorMath.Average(horizontal2VelocityLine);
                    cos = VectorMath.CosFromUnstandardized(linearizedVelocity, averageVelocity, VectorMath.SelectedCoord.XYZ);
                    if (cos < -0.2 &&
                        timeBangDiff > 220 &&
                        lastBeatPosition.DistanceTo(position) > pointRange)
                    {
                        currentDirection = Direction.Horizontal1;
                        //Console.WriteLine("Temps 2 : Cos : " + cos + "average :" + averageVelocity);
                        lastBeatPosition = position;
                        evolvePartCursor(2);
                        lastBangTime = DateTime.Now;
                    }
                    else
                    {
                        horizontal2VelocityLine.Enqueue(velocity);
                        if (horizontal2VelocityLine.Count > 13)
                            horizontal2VelocityLine.Dequeue();
                    }
                    break;
            }
        }

        public void OnFrame(SENSOR_TYPE sensor, Vector position, Vector velocity)
        {
            switch (analysisBeatsNumber)
            {
                case 3:
                    Analysis3Beats(sensor, position, velocity);
                    break;
                case 2:
                    Analysis2Beats(sensor, position, velocity);
                    break;
                default:
                    Analysis4Beats(sensor, position, velocity);
                    break;
            }

        }

        private void ManageOrientation(Vector horizontalVelocity)
        {
            Vector normedVelocity = VectorMath.GetNormalized(horizontalVelocity);

            float orientation = 1 - Math.Abs(normedVelocity.x);
            if ((normedVelocity.z > 0 && normedVelocity.x < 0) ||
                (normedVelocity.z < 0 && normedVelocity.x > 0))
            {
                orientation = orientation * (-1);
            }

            Console.WriteLine("orientation : " + orientation + " normed : " + normedVelocity);
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
