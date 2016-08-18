//Gong Xueyuan
//2016/8/11
using System.Collections;
using System.Collections.Generic;

namespace Coroutine
{
    public static class Coroutine
    {
        static List<IEnumerator> routines = new List<IEnumerator>();
        
        public static int Count
        {
            get { return routines.Count; }
        }

        public static void Start(IEnumerator routine)
        {
            routines.Add(routine);
        }

        public static void Stop(IEnumerator routine)
        {
            routines.Remove(routine);
        }

        public static void StopAll()
        {
            routines.Clear();
        }

        public static void Update()
        {
            for (int i = 0; i < routines.Count; ++i)
            {
                if (routines[i].Current is IEnumerator)
                {
                    if (RecursiveMoveNext(routines[i].Current as IEnumerator))
                    {
                        continue;
                    }
                }

                if (!routines[i].MoveNext())
                {
                    routines.RemoveAt(i--);
                }
            }
        }

        static bool RecursiveMoveNext(IEnumerator routine)
        {
            if (routine.Current is IEnumerator)
            {
                if (RecursiveMoveNext(routine.Current as IEnumerator))
                {
                    return true;
                }
            }

            return routine.MoveNext();
        }
    }
}