using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WebServer.Logger
{
    class Logger
    {
        public static readonly string DIR_PATH = @"logger";
        public static readonly string LOGGER_PATH = @"logger/log.txt";
        public static string MessagePrefix { get { return "[" + DateTime.Now.ToString() + "]: "; } }

        private static object lockObject = new object();

        public Logger()
        {
            if (!Directory.Exists(DIR_PATH))
                Directory.CreateDirectory(DIR_PATH);

            WriteMessage("Logger start");
        }

        public void WriteMessage(string message)
        {
            new Thread(new ParameterizedThreadStart(WriteFunc)).Start(message);
        }

        public static void WriteFunc(object m)
        {
            string message = (string)m;
            lock (lockObject)
            {
                File.AppendAllText(LOGGER_PATH, MessagePrefix + message + Environment.NewLine);
            }
        }

        public void ReadLog(Delegate callback)
        {
            new Thread(new ParameterizedThreadStart(ReadFunc)).Start(callback);
        }

        public static void ReadFunc(object cb)
        {
            Delegate callback = (Delegate)cb;
            lock (lockObject)
            {
                string data = File.ReadAllText(LOGGER_PATH);
                callback.DynamicInvoke(data);
            }
        }
        
    }
}
