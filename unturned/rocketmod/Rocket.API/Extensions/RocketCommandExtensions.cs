using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rocket.API.Extensions
{
    public static class RocketCommandExtensions
    {
        public static string GetStringParameter(this string[] array, int index)
        {
            return (array.Length <= index || String.IsNullOrEmpty(array[index])) ? null : array[index];
        }

        public static int? GetInt32Parameter(this string[] array, int index)
        {
            int output;
            return (array.Length <= index || !Int32.TryParse(array[index].ToString(), out output)) ? null : (int?)output;
        }

        public static uint? GetUInt32Parameter(this string[] array, int index)
        {
            uint output;
            return (array.Length <= index || !uint.TryParse(array[index].ToString(), out output)) ? null : (uint?)output;
        }

        public static byte? GetByteParameter(this string[] array, int index)
        {
            byte output;
            return (array.Length <= index || !Byte.TryParse(array[index].ToString(), out output)) ? null : (byte?)output;
        }

        public static ushort? GetUInt16Parameter(this string[] array, int index)
        {
            ushort output;
            return (array.Length <= index || !UInt16.TryParse(array[index].ToString(), out output)) ? null : (ushort?)output;
        }

        public static float? GetFloatParameter(this string[] array, int index)
        {
            float output;
            return (array.Length <= index || !float.TryParse(array[index].ToString(), out output)) ? null : (float?)output;
        }

        public static string GetParameterString(this string[] array, int startingIndex = 0)
        {
            if (array.Length - startingIndex <= 0) return null;
            return string.Join(" ", array.ToList().GetRange(startingIndex, array.Length - startingIndex).ToArray());
        }

    }
}
