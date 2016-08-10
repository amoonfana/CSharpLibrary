//Gong Xueyuan
//2016/8/8
using System;

namespace Timer
{
    public class Timer : IComparable<Timer>
    {
        #region Time Scheduler
        //public static int currentIndex = 0;
        public static KBinaryHeap<Timer> scheduler = new KBinaryHeap<Timer>();
        [ThreadStatic]
        public static DateTime startTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        #endregion

        #region Timer
        public delegate bool TriggerHandler();

        public TriggerHandler triggerHandler;
        //public int id;
        public double waitTime;
        public double nextTrigger;
        //public int repeatCount;

        public Timer(double _waitTime)
        {
            //id = ++currentIndex;
            waitTime = _waitTime;
            nextTrigger = (DateTime.Now - startTime).TotalSeconds + waitTime;
            //repeatCount = 0;

            scheduler.Add(this);
        }

        public int CompareTo(Timer timer)
        {
            if (nextTrigger > timer.nextTrigger) { return 1; }
            else if (nextTrigger < timer.nextTrigger) { return -1; }
            else{ return 0; }
        }

        public void OnTrigger(double currentTime)
        {
            //repeatCount += 1;
            scheduler.RemoveTop();

            bool isReschedule = triggerHandler.Invoke();

            if(!isReschedule){ return; }

            nextTrigger = currentTime + waitTime;

            scheduler.Add(this);
        }
        #endregion
    }
}
