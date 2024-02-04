using System;

namespace ReportCenterSystem
{
    public static class ArrayExt
    {
        public static void IncrementAdd<T>(this T[] array, T item)
        {
            int len = array.Length;
            Array.Resize(ref array, len + 1);
            array[len] = item;
        }
    }
}