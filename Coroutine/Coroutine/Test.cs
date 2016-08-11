using System;
using System.Collections;
using System.Collections.Generic;

namespace Coroutine
{
    class Test
    {
        #region test coroutine
        static IEnumerator WaitForSeconds(double seconds)
        {
            DateTime startedWaiting = DateTime.UtcNow;
            while ((DateTime.UtcNow - startedWaiting).TotalSeconds < seconds)
            {
                yield return true;
            }
        }

        static IEnumerator testYield()
        {
            Console.WriteLine("NestedCoroutine1, wait 1 sec (literally)");
            yield return WaitForSeconds(1);
            Console.WriteLine("Now wait 2 more.");
            yield return WaitForSeconds(2);
            Console.WriteLine("All done!");
        }

        static void Main(string[] args)
        {
            Coroutine.Start(testYield());

            while (Coroutine.Count > 0)
            {
                Coroutine.Update();
            }

            Console.Read();
        }
        #endregion
    }
}
