using System;
using System.Timers;

namespace ABConsole
{
    class Program
    {
        private static Timer timer;
        private static System.Threading.Timer aTimer;

        private static int startSeconds = 190;
        private static int endSeconds = 86400;

        private static double spanSeconds;

        static void Main(string[] args)
        {
            spanSeconds = startSeconds;

            timer = new Timer(10000);
            //timer.Elapsed += OnTimedEvent;
            //timer.AutoReset = true;
            //timer.Enabled = true;


            var autoEvent = new System.Threading.AutoResetEvent(false);
            aTimer = new System.Threading.Timer(TimerCallback, autoEvent, 0, 1000);
            autoEvent.WaitOne();
            aTimer.Dispose();
            Console.WriteLine("\nDestroying timer.");

            Console.ReadLine();

            timer.Stop();
            timer.Dispose();
        }

        private static void TimerCallback(object state)
        {
            System.Threading.AutoResetEvent autoEvent = (System.Threading.AutoResetEvent)state;

            // Каждый вызов таймера прибавляем к прошедшему времени 10 секунд. 
            spanSeconds += 10;

            if (spanSeconds == 300) // Ускоряем
            {
                aTimer.Change(0, 500);
            }

            if (spanSeconds == 600) // Ускоряем
            {
                aTimer.Change(0, 1);
            }

            if (spanSeconds >= 1000/*endSeconds*/)
            {
                autoEvent.Set();
            }

            DisplayTime();
        }

        private static void OnTimedEvent(object sender, ElapsedEventArgs e)
        {
            // Каждый вызов таймера прибавляем к прошедшему времени 10 секунд. 
            spanSeconds += 10;

            if (spanSeconds == 100) // Ускоряем в 10 раз
            {
                timer.Stop();
                timer.Interval = 1000;//Convert.ToInt32(numericUpDown1.Value);
                timer.Start();
            }
            else if (spanSeconds == 200) // Ускоряем в 100 раз
            {
                timer.Stop();
                timer.Interval = 100;
                timer.Start();
            }
            else { }

            if (spanSeconds >= endSeconds)
            {
                timer.Stop();
            }

            DisplayTime();
        }

        private static void DisplayTime()
        {
            TimeSpan t = TimeSpan.FromSeconds(spanSeconds);

            string timeNow = string.Format("{0:D2}h:{1:D2}m:{2:D2}s",
                                            t.Hours,
                                            t.Minutes,
                                            t.Seconds);

            Console.WriteLine(timeNow);
        }
    }
}
