namespace LastCommentSelenium
{
    using System;
    using System.Threading;
    using System.Timers;

    public class Program
    {
        private static System.Timers.Timer aliveTimer;

        public static Thread LastCommentThread { get; private set; }
        public static LastComment LastComment { get; private set; }

        public static string Username { get; private set; }
        public static string Password { get; private set; }
        public static string PostUrl { get; private set; }
        public static DateTime Deadline { get; private set; }
        public static bool SpamProtection { get; private set; }

        static void Main(string[] args)
        {

            Console.Write("Post direct URL (e.g. https://www.facebook.com/kronehit/posts/10154953745216701):");
            PostUrl = Console.ReadLine();

            Console.Write("Spam protection that avoids comments if high comment frequency (y/n): ");
            SpamProtection = Console.ReadLine().ToLower() == "y";

            if (SpamProtection)
            {
                Console.WriteLine("Info: Spam protection is on");
            }
            else
            {
                Console.WriteLine("Info: Spam protection is off");
            }

            Console.Write("Facebook Username: ");
            Username = Console.ReadLine();

            while(string.IsNullOrEmpty(Password))
            {
                Console.Write("Facebook Password: ");
                Password = ConsoleEx.HideCharacter();
            }

            DateTime deadline = default(DateTime);
            while (deadline == default(DateTime))
            {
                Console.Write("Deadline (e.g. 24-08-2017 08:00):");
                DateTime.TryParse(Console.ReadLine(), out deadline);
                Deadline = deadline;
            }

            Console.WriteLine("Starting shortly...");

            aliveTimer = new System.Timers.Timer();
            aliveTimer.Interval = 15000;
            aliveTimer.Elapsed += OnTimedEvent;
            aliveTimer.AutoReset = true;
            aliveTimer.Enabled = true;

            while (true)
            { //never exit (window has an exit button)
                Console.ReadLine();
            }
        }

        private static void RunLastCommentRoutineInThread()
        {
            lock (locker)
            {
                try
                {
                    if (LastCommentThread != null)
                    {
                        try
                        {
                            LastComment.Driver.Dispose();
                        }
                        catch (Exception ex)
                        {

                        }

                        Thread.Sleep(4000);

                        LastCommentThread.Abort();
                        LastCommentThread = null;
                        LastComment = null;
                        
                        Thread.Sleep(4000);
                    }
                }
                catch (Exception)
                {
                    //cleanup failed, not too bad...
                }

                LastComment = new LastComment("https://www.facebook.com/kronehit/posts/10154953745216701", Username, Password, new DateTime(2017, 08, 24), SpamProtection);

                LastCommentThread = new Thread(() => { LastComment.Run(); });
                LastCommentThread.Start();
            }
        }

        private static object locker = new object();

        private static void OnTimedEvent(object sender, ElapsedEventArgs e)
        {
            var lastLiveCheckMinutesAgo = DateTime.Now.Subtract(LastComment?.SignOfLiving ?? default(DateTime)).TotalMinutes;
            double maxMinutes = 3;
            
            if(LastComment == null || LastCommentThread == null || !LastCommentThread.IsAlive || lastLiveCheckMinutesAgo > maxMinutes)
            {
                RunLastCommentRoutineInThread();
            }
        }
    }
}
