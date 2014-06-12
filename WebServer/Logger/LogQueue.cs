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

         private string[] messages = new string[MAX_QUEUE_COUNT];

        private Semaphore getSem;
        private Semaphore putSem;
        private Semaphore queueSem;

        private int getpos, putpos;
        private int count;

        public LogQueue()
        {
            getSem = new Semaphore(0, MAX_QUEUE_COUNT);
            putSem = new Semaphore(MAX_QUEUE_COUNT, MAX_QUEUE_COUNT);
            queueSem = new Semaphore(1, 1);
            getpos = 0;
            putpos = 0;
            count = 0;
        }

        public string Get()
        {
            getSem.WaitOne(); 
            queueSem.WaitOne();

            string message = messages[getpos];
            getpos = (getpos + 1) % MAX_QUEUE_COUNT;
            count--;

            queueSem.Release();
            putSem.Release();
            return message;
        }

        public void Put(string message)
        {
            putSem.WaitOne();
            queueSem.WaitOne();

            messages[putpos] = message;
            putpos = (putpos + 1) % MAX_QUEUE_COUNT;
            count++;

            queueSem.Release();
            getSem.Release();
        }
    }
}
