using System.Runtime.CompilerServices;
using Quaternion = System.Numerics.Quaternion;
using UQuaternion = UnityEngine.Quaternion;

namespace OpenMod.UnityEngine.Extensions
{
    public static class QuaternionConversionExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static UQuaternion ToUnityQuaternion(this Quaternion quaternion)
        {
            return new UQuaternion(quaternion.X, quaternion.Y, quaternion.Z, quaternion.W);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Quaternion ToSystemQuaternion(this UQuaternion quaternion)
        {
            return new Quaternion(quaternion.x, quaternion.y, quaternion.z, quaternion.w);
        }
    }
}
