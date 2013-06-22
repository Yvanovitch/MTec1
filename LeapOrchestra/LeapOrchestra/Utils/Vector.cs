using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Leap;

namespace LeapOrchestra.Utils
{
    class Vector : Leap.Vector
    {
        public Vector (float x, float y, float z) : base(x, y, z)
        {
        }

        public void TakeOff(Vector other)
        {
            this.x -= other.x;
            this.y -= other.y;
            this.z -= other.z;
        }

        public void Divide(float value)
        {
            if (value == 0)
                return;

            x = x / value;
            y = y / value;
            z = z / value;
        }

        public void Multiply(float value)
        {
            x = x * value;
            y = y * value;
            z = z * value;
        }
    }

    class VectorMath
    {
        public static Vector Difference(Vector a, Vector b)
        {
            return new Vector(a.x - b.x, a.y - b.y, a.z - b.z);
        }

        public static Vector Divisor(Vector a, float value)
        {
            if (value == 0)
                return new Vector(0, 0, 0);

            return new Vector(a.x / value, a.y / value, a.z / value);
        }

        public static Vector Average(Queue<Vector> list)
        {
            if (list.Count <= 0)
                return new Vector(0, 0, 0);

            float x = 0, y = 0, z = 0;

            foreach (Vector vec in list)
            {
                x += vec.x;
                y += vec.y;
                z += vec.z;
            }

            x = x / list.Count;
            y = y / list.Count;
            z = z / list.Count;
            return new Vector(x, y, z);

        }
    }
}
