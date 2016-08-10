using System;

namespace ObjectPool
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
            aPool.freeObject(ref a);
            //aPool.freeObject(ref a);    //Illegal, repeatedly free one object
            aPool.freeObject(ref b);
            A c = aPool.newObject();
            Console.ReadLine();
        }
    }
}
