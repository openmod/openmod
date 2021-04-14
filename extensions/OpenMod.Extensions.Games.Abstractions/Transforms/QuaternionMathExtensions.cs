using System;
using System.Numerics;

namespace OpenMod.Extensions.Games.Abstractions.Transforms
{
    public static class QuaternionMathExtensions
    {
        private const float c_RadiansToDegrees = (float)(180 / Math.PI);
        private const float c_DegreesToRadians = (float)(Math.PI / 180);

        private static void CleanUpAngles(this ref Vector3 vector)
        {
            while (vector.X >= 360)
                vector.X -= 360;

            while (vector.X < 0)
                vector.X += 360;

            while (vector.Y >= 360)
                vector.Y -= 360;

            while (vector.Y < 0)
                vector.Y += 360;

            while (vector.Z >= 360)
                vector.Z -= 360;

            while (vector.Z < 0)
                vector.Z += 360;
        }

        /// <summary>
        /// Converts a quaternion to its euler angles where X is pitch, Y is yaw, and Z is roll.
        /// </summary>
        /// <param name="quaternion">The quaternion to convert.</param>
        /// <returns>Euler angles of the quaternion.</returns>
        public static Vector3 ToEulerAngles(this Quaternion quaternion)
        {
            var q = quaternion;

            Vector3 euler;

            var unit = (q.X * q.X) + (q.Y * q.Y) + (q.Z * q.Z) + (q.W * q.W);

            var test = q.X * q.W - q.Y * q.Z;

            if (test > 0.4995f * unit) // singularity at north pole
            {
                euler.X = (float)Math.PI / 2;
                euler.Y = 2f * (float)Math.Atan2(q.Y, q.X);
                euler.Z = 0;
            }
            else if (test < -0.4995f * unit) // singularity at south pole
            {
                euler.X = -(float)Math.PI / 2;
                euler.Y = -2f * (float)Math.Atan2(q.Y, q.X);
                euler.Z = 0;
            }
            else // no singularity
            {
                euler.X = (float)Math.Asin(2f * (q.W * q.X - q.Y * q.Z));
                euler.Y = (float)Math.Atan2(2f * q.W * q.Y + 2f * q.Z * q.X, 1 - 2f * (q.X * q.X + q.Y * q.Y));
                euler.Z = (float)Math.Atan2(2f * q.W * q.Z + 2f * q.X * q.Y, 1 - 2f * (q.Z * q.Z + q.X * q.X));
            }

            euler *= c_RadiansToDegrees;

            CleanUpAngles(ref euler);

            return euler;
        }


        /// <summary>
        /// Converts euler angles to its quaternion where where X is pitch, Y is yaw, and Z is roll.
        /// </summary>
        /// <param name="eulerAngles"></param>
        /// <returns></returns>
        public static Quaternion ToQuaternion(this Vector3 eulerAngles)
        {
            var xOver2 = eulerAngles.X * c_DegreesToRadians * 0.5f;
            var yOver2 = eulerAngles.Y * c_DegreesToRadians * 0.5f;
            var zOver2 = eulerAngles.Z * c_DegreesToRadians * 0.5f;

            var sinXOver2 = (float)Math.Sin(xOver2);
            var cosXOver2 = (float)Math.Cos(xOver2);
            var sinYOver2 = (float)Math.Sin(yOver2);
            var cosYOver2 = (float)Math.Cos(yOver2);
            var sinZOver2 = (float)Math.Sin(zOver2);
            var cosZOver2 = (float)Math.Cos(zOver2);
            
            return new Quaternion()
            {
                X = cosYOver2 * sinXOver2 * cosZOver2 + sinYOver2 * cosXOver2 * sinZOver2,
                Y = sinYOver2 * cosXOver2 * cosZOver2 - cosYOver2 * sinXOver2 * sinZOver2,
                Z = cosYOver2 * cosXOver2 * sinZOver2 - sinYOver2 * sinXOver2 * cosZOver2,
                W = cosYOver2 * cosXOver2 * cosZOver2 + sinYOver2 * sinXOver2 * sinZOver2
            };
        }

        /// <summary>
        /// Rotates this point by the given rotation.
        /// </summary>
        /// <param name="point">The point to rotate.</param>
        /// <param name="rotation">The rotation to apply.</param>
        /// <returns>The rotated point.</returns>
        public static Vector3 Rotate(this Vector3 point, Quaternion rotation)
        {
            var x = rotation.X * 2f;
            var y = rotation.Y * 2f;
            var z = rotation.Z * 2f;
            var xx = rotation.X * x;
            var yy = rotation.Y * y;
            var zz = rotation.Z * z;
            var xy = rotation.X * y;
            var xz = rotation.X * z;
            var yz = rotation.Y * z;
            var wx = rotation.W * x;
            var wy = rotation.W * y;
            var wz = rotation.W * z;

            Vector3 result;
            result.X = (1f - (yy + zz)) * point.X + (xy - wz) * point.Y + (xz + wy) * point.Z;
            result.Y = (xy + wz) * point.X + (1f - (xx + zz)) * point.Y + (yz - wx) * point.Z;
            result.Z = (xz - wy) * point.X + (yz + wx) * point.Y + (1f - (xx + yy)) * point.Z;
            return result;
        }
    }
}
