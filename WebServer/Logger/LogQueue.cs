using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WebServer.Server;

namespace WebServer.Logger
{
    class LogQueue
    {
        public static readonly int MAX_QUEUE_COUNT = 10;

         private string[] buffer = new string[MAX_QUEUE_COUNT];

        private Semaphore getSem;
        private Semaphore putSem;
        private Semaphore queueSem;

        // Twee indexen en de lengte bijhouden.
        // Redundant, maar lekker makkelijk!
        private int getpos, putpos;
        private int count;

        public LogQueue()
        {
            getSem = new Semaphore(0, MAX_QUEUE_COUNT);
            putSem = new Semaphore(MAX_QUEUE_COUNT, MAX_QUEUE_COUNT);
            queueSem = new Semaphore(1, 1);
            // 3x semphore
            // release van de put, release van de get, voor de queue
            getpos = 0;
            putpos = 0;
            count = 0;
        }

        public string Get()
        {
            getSem.WaitOne(); // wacht op put
            queueSem.WaitOne();

            string message = buffer[getpos];
            getpos = (getpos + 1) % MAX_QUEUE_COUNT;
            count--;
            //Console.WriteLine(consumername + ": gets " + box.Id);

            queueSem.Release();
            putSem.Release(); // vrijgeven get
            return message;
        }

        public void Put(string message)
        {
            putSem.WaitOne(); // wachten op get
            queueSem.WaitOne();
            //Console.WriteLine(producername + ": puts " + box.Id);

            buffer[putpos] = message;
            putpos = (putpos + 1) % MAX_QUEUE_COUNT;
            count++;

            queueSem.Release();
            getSem.Release(); // vrijgeven get
        }
    }
}
