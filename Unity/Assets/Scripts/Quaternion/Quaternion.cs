using UnityEngine;
using System.Collections;

namespace Utils {

    public class Quaternion {

        #region Overloaded_Operators
        public static Quaternion operator +(Quaternion p, Quaternion q) {
            return new Quaternion(p.X + q.X, p.Y + q.Y, p.Z + q.Z, p.W + q.W);
        }
        public static Quaternion operator -(Quaternion p, Quaternion q) {
            return new Quaternion(p.X - q.X, p.Y - q.Y, p.Z - q.Z, p.W - q.W);
        }
        #endregion

        #region Members
        private float x, y, z, w;
        #endregion

        #region Properties
        public float X {
            get { return x; }
        }

        public float Y {
            get { return y; }
        }

        public float Z {
            get { return z; }
        }

        public float W {
            get { return w; }
        }

        public Quaternion(float x, float y, float z, float w) {
            this.x = x;
            this.y = y;
            this.z = z;
            this.w = w;
        }
        #endregion

        public float Norm() {
            return Mathf.Sqrt(  x * x + y * y + z * z + w * w);
        }

        public Quaternion Negate() {
            return new Quaternion(-x, -y, -z, -w);
        }

        public Quaternion Normalize() {
            float norm = Norm();
            return new Utils.Quaternion(x / norm, y / norm, z / norm, w / norm);
        }

        public static Quaternion Multiply(Quaternion p, Quaternion q) {
            float w = p.W * q.W - p.X * q.X - p.Y * q.Y - p.Z * q.Z;
            float x = p.W * q.X + p.X * q.W + p.Y * q.Z - p.Z * q.Y;
            float y = p.W * q.Y + p.Y * q.W - p.X * q.Z + p.Z * q.X;
            float z = p.W * q.Z + p.Z * q.W + p.X * q.Y - p.Y * q.X;
            return new Utils.Quaternion(w, x, y, z).Normalize();
        }

        public static Quaternion Slerp(Quaternion p, Quaternion q, float t, float theta) {
            // t must always belong to the [0,1] interval
            t = Mathf.Clamp01(t);
            float sinTheta = Mathf.Sin(theta);
            float oMt = 1 - t;
            return new Utils.Quaternion(
                (Mathf.Sin(oMt * theta) * p.X + Mathf.Sin(t * theta) * q.X) / sinTheta,
                (Mathf.Sin(oMt * theta) * p.X + Mathf.Sin(t * theta) * q.Y) / sinTheta,
                (Mathf.Sin(oMt * theta) * p.Z + Mathf.Sin(t * theta) * q.Z) / sinTheta,
                (Mathf.Sin(oMt * theta) * p.W + Mathf.Sin(t * theta) * q.W) / sinTheta);
        }

        public static Quaternion ShortestArc(Quaternion p, Quaternion q) {
            // we will interpolate from p to either q or -q
            float aMinusb = Mathf.Pow( (p - q).Norm() , 2);
            float aPlusb = Mathf.Pow((p + q).Norm(), 2);
            return aPlusb < aMinusb ? q.Negate() : q;
        }

    }

}
