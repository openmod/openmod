using System.Runtime.CompilerServices;
using UVector2 = UnityEngine.Vector2;
using UVector3 = UnityEngine.Vector3;
using UVector4 = UnityEngine.Vector4;
using Vector2 = System.Numerics.Vector2;
using Vector3 = System.Numerics.Vector3;
using Vector4 = System.Numerics.Vector4;

namespace OpenMod.UnityEngine.Extensions
{
    public static class VectorConversionExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static UVector2 ToUnityVector(this Vector2 vector)
        {
            return new UVector2(vector.X, vector.Y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 ToSystemVector(this UVector2 vector)
        {
            return new Vector2(vector.x, vector.y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static UVector3 ToUnityVector(this Vector3 vector)
        {
            return new UVector3(vector.X, vector.Y, vector.Z);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 ToSystemVector(this UVector3 vector)
        {
            return new Vector3(vector.x, vector.y, vector.z);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static UVector4 ToUnityVector(this Vector4 vector)
        {
            return new UVector4(vector.X, vector.Y, vector.Z, vector.W);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector4 ToSystemVector(this UVector4 vector)
        {
            return new Vector4(vector.x, vector.y, vector.z, vector.w);
        }
    }
}