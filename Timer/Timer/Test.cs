using System;

namespace Timer
{
    class Test
    {
        static bool test1()
        {
            Console.WriteLine("Time1 - " + DateTime.Now.ToString());
            return true;
        }

        static bool test2()
        {
            Console.WriteLine("Time2 - " + DateTime.Now.ToString());
            return true;
        }

        static void Main()
        {
            Timer timer1 = new Timer(1);
            timer1.triggerHandler += test1;
            Timer timer5 = new Timer(5);
            timer5.triggerHandler += test2;

            while (true)
            {
                double currentTime = (DateTime.Now - Timer.startTime).TotalSeconds;

                while (Timer.scheduler.count > 0)
                {
                    Timer timer = Timer.scheduler.top;
                    if (currentTime < timer.nextTrigger)
                    {
                        break;
                    }

                    timer.OnTrigger(currentTime);
                }
            }
        }
    }
}
