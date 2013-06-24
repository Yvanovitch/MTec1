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

        public Vector (Leap.Vector U) : base(U.x, U.y, U.z)
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

        public void Reset()
        {
            x = 0;
            y = 0;
            z = 0;
        }

        public void Reverse(VectorMath.SelectedCoord select)
        {
            float temp = 0;
            switch (select)
            {
                case VectorMath.SelectedCoord.XY:
                    x = x * (-1);
                    y = y * (-1);
                    break;
                case VectorMath.SelectedCoord.XZ:
                    x = x * (-1);
                    z = z * (-1);
                    break;
                default:
                    z = z * (-1);
                    y = y * (-1);
                    break;
            }
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

        public static float Cos(Vector U, Vector V, SelectedCoord select)
        {
            switch (select)
            {
                case SelectedCoord.XY:
                    return U.x * V.x + U.y * V.y;
                case SelectedCoord.XZ:
                    return U.x * V.x + U.z * V.z;
                default:
                    return U.y * V.y + U.z * V.z;
            }
        }

        public static float CosFromUnstandardized(Vector U, Vector V, SelectedCoord select)
        {
            return Cos(GetNormalized(U), GetNormalized(V), select);
        }

        public static Vector GetNormalized(Vector U)
        {
            float norme = U.Magnitude;
            return new Vector(U.x / norme, U.y / norme, U.z / norme);
        }

        public enum SelectedCoord
        {
            XY, XZ, YZ
        }

        public static Vector GetPositive(Vector U)
        {
            return new Vector(Math.Abs(U.x), Math.Abs(U.y), Math.Abs(U.z));
        }

        public static void ReverseQueue(Queue<Vector> queue, SelectedCoord select)
        {
            foreach (Vector U in queue)
            {
                U.Reverse(select);
            }
        }

        public static Vector GetVector (Leap.Vector vec)
        {
            return new Vector(vec);
        }
    }
}
