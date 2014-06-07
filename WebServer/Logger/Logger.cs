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
    class Logger
    {
        public static readonly string DIR_PATH = ServerConfig.Controlroot + @"/logger";
        public static readonly string LOGGER_PATH = ServerConfig.Controlroot + @"/logger/log.txt";
        private static readonly int MAX_QUEUE_COUNT = 10;

        public static string MessagePrefix { get { return "[" + DateTime.Now.ToString() + "]: "; } }

        private static object lockObject = new object();

        private Semaphore _queueSem;

        public Logger()
        {
            _queueSem = new Semaphore(0, MAX_QUEUE_COUNT);

            if (!Directory.Exists(DIR_PATH))
                Directory.CreateDirectory(DIR_PATH);

            WriteMessage("Logger start");
        }

        public void WriteMessage(string message)
        {
            new Thread(new ParameterizedThreadStart(WriteFunc)).Start(message);
        }

        private void WriteFunc(object m)
        {
            //_queueSem.WaitOne();
            string message = (string)m;
            lock (lockObject)
            {
                File.AppendAllText(LOGGER_PATH, MessagePrefix + message + Environment.NewLine);
            }
            //_queueSem.Release();
        }

        public void ReadLog(Delegate callback)
        {
            new Thread(new ParameterizedThreadStart(ReadFunc)).Start(callback);
        }

        private void ReadFunc(object cb)
        {
            //_queueSem.WaitOne();
            Delegate callback = (Delegate)cb;
            lock (lockObject)
            {
                string data = File.ReadAllText(LOGGER_PATH);
                callback.DynamicInvoke(data);
            }
            //_queueSem.Release();
        }
        
    }
}
