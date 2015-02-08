using System;
using System.Collections.Generic;

namespace PAT.Common.Classes.LTL2DRA
{
    public class Ultility
    {
        public static void resizeExact<T>(List<T> list, int newSize)
        {
            list.Clear();
            for (int i = 0; i < newSize; i++)
            {
                list.Add(default(T));
            }
        }

        public static void resize<T>(List<T> list,  int newSize)
        {
            for(int i = list.Count; i <= newSize; i++)
            {
                list.Add(default(T));
            }       
        }

        public static void resize<T>(List<T> list, int newSize, T obj)
        {
            for (int i = list.Count; i <= newSize; i++)
            {
                list.Add(obj);
            }
        }

        public static void swap<T>(T a, T b)
        {

            T tmp = a;
            a = b;
            b = tmp;

        }

        public static bool? NullCheck(object one, object other)
        {
            if (one == null && other == null)
            {
                return true;
            }

            if (one == null || other == null)
            {
                return false;
            }

            return null;
        }

    }
}
