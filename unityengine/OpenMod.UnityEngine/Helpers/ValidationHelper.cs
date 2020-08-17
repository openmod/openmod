using System.Runtime.CompilerServices;
using UVector3 = UnityEngine.Vector3;
using Vector3 = System.Numerics.Vector3;

namespace OpenMod.UnityEngine.Helpers
{
    public static class ValidationHelper
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsValid(float f)
        {
            return !float.IsInfinity(f)
                   && !float.IsNaN(f) 
                   && !float.IsPositiveInfinity(f) 
                   && !float.IsNegativeInfinity(f);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsValid(Vector3 vector)
        {
            return IsValid(vector.X) && IsValid(vector.Y) && IsValid(vector.Z);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsValid(UVector3 vector)
        {
            return IsValid(vector.x) && IsValid(vector.y) && IsValid(vector.z);
        }
    }
}