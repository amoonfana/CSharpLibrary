using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleObjectPool
{
    public class A : IObjPoolItem
    {
        public void onNew(int i)
        {
            Console.WriteLine(i + " onNew");
        }
        public void onFree(int i)
        {
            Console.WriteLine(i + " onFree");
        }
    }

    class Test
    {
        static void Main()
        {
            ObjectPool<A> aPool = new ObjectPool<A>(10);
            A a = aPool.newObject();
            A b = aPool.newObject();
            a = aPool.freeObject(a);
            b = aPool.freeObject(b);
            A c = aPool.newObject();
            Console.ReadLine();
        }
    }
}
