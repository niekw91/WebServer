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
        public static string MessagePrefix { get { return "[" + DateTime.Now.ToString() + "]: "; } }

        public static readonly Object lockObject = new Object();

        private LogQueue _logQueue;
        private Thread _logThread;

        public Logger()
        {
            if (!Directory.Exists(Logger.DIR_PATH))
                Directory.CreateDirectory(Logger.DIR_PATH);

            _logQueue = new LogQueue();

            _logThread = new Thread(Run);
        }

        public void Start()
        {
            _logThread.Start();
        }

        public void Run()
        {
            while (true)
            {
                string message = _logQueue.Get();
                if (!string.IsNullOrEmpty(message))
                {
                    WriteToFile(MessagePrefix + message + Environment.NewLine);
                }
            }
        }

        public void WriteMessage(string message)
        {
            _logQueue.Put(message);
        }

        private void WriteToFile(string message)
        {
            lock (lockObject)
            {
                File.AppendAllText(LOGGER_PATH, message);
            }
        }

        //public string Read()
        //{
        //    lock (lockObject)
        //    {
        //        return File.ReadAllText(LOGGER_PATH);
        //    }
        //}
        
    }
}
