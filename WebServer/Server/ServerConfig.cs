using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using WebServer.Utilities;

namespace WebServer.Server
{
    class ServerConfig
    {
        private static readonly String CONFIG_PATH = @"config\serverconfig.xml";

        public static int WebPort { get; set; }
        public static String Webroot { get; set; }
        public static String DefaultPage { get; set; }
        public static bool DirectoryBrowsing { get; set; }

        public static int ControlPort { get; set; }
        public static String Controlroot { get; set; }

        public static void Init()
        {
            if (!File.Exists(CONFIG_PATH))
            {
                CreateConfig();
            }
            else
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(CONFIG_PATH);
                // Get root element
                XmlElement element = doc.DocumentElement;
                // Get elements
                XmlNode server = element.SelectSingleNode("server");
                XmlNode controlServer = element.SelectSingleNode("controlserver");
                // Set properties
                SetProperties(server["port"].InnerText, server["webroot"].InnerText, server["default-page"].InnerText, 
                    server["dir-browsing"].InnerText, controlServer["port"].InnerText, controlServer["root"].InnerText);
            }
        }

        private static void CreateConfig()
        {
            // Defaults
            String webroot = @"webroot\www";
            String port = "8080";
            String dirBrowsing = "false";
            String defaultPage = "index.html;index.htm";
            String controlPort = "8081";
            String controlroot = @"controlroot\www";
            // Create config directory
            Directory.CreateDirectory("config");
            // Create xml writer settings
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.IndentChars = "\t";
            using (XmlWriter xmlWriter = XmlWriter.Create(CONFIG_PATH, settings))
            {
                // Start root
                xmlWriter.WriteStartDocument();
                xmlWriter.WriteStartElement("config");
                // Start writing server configuration
                xmlWriter.WriteStartElement("server");
                xmlWriter.WriteElementString("port", port);
                xmlWriter.WriteElementString("webroot", webroot);
                xmlWriter.WriteElementString("default-page", defaultPage);
                xmlWriter.WriteElementString("dir-browsing", dirBrowsing);
                xmlWriter.WriteEndElement();
                // Start writing control server configuration
                xmlWriter.WriteStartElement("controlserver");
                xmlWriter.WriteElementString("port", controlPort);
                xmlWriter.WriteElementString("root", controlroot);
                xmlWriter.WriteEndElement();
                // End root
                xmlWriter.WriteEndElement();
                xmlWriter.Flush();
            }
            // Set properties
            SetProperties(port, webroot, defaultPage, dirBrowsing, controlPort, controlroot);
        }

        private static void SetProperties(String port, String webroot, String defaultPage, String dirBrowsing, String controlPort, String controlroot)
        {
            WebPort = Convert.ToInt32(port);
            Webroot = webroot;
            DefaultPage = defaultPage;
            DirectoryBrowsing = Convert.ToBoolean(dirBrowsing);
            ControlPort = Convert.ToInt32(controlPort);
            Controlroot = controlroot;
        }
    }
}
