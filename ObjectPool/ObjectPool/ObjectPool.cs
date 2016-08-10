using System;
using System.Collections.Generic;

namespace ObjectPool
{
    public interface IObjPoolItem
    {
        void onNew(int i);
        void onFree(int i);
    }

    public class ObjectPool<T> where T : IObjPoolItem, new()
    {
        private Stack<T> objects;

        public ObjectPool()
        {
            objects = new Stack<T>();
        }

        public ObjectPool(int n)
        {
            objects = new Stack<T>(n);
        }

        public T newObject()
        {
            T t;

            if(objects.Count > 0){ t = objects.Pop(); }
            else{ t = new T(); }

            t.onNew(objects.Count);
            return t;
        }

        public void freeObject(ref T t)
        {
            t.onFree(objects.Count);
            objects.Push(t);
            t = default(T);
        }
    }
}
