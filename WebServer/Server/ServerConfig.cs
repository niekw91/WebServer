﻿using System;
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

        public static readonly string DIR_BROWSING_FILE_PATH = @"\browsing.html";

        public static String Name { get; set; }
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
                XmlNode general = element.SelectSingleNode("general");
                XmlNode server = element.SelectSingleNode("server");
                XmlNode controlServer = element.SelectSingleNode("controlserver");
                // Set properties
                SetProperties(general["name"].InnerText, server["port"].InnerText, server["webroot"].InnerText, server["default-page"].InnerText, 
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
            String name = "WebServer";
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
                // Start writing global configuration
                xmlWriter.WriteStartElement("general");
                xmlWriter.WriteElementString("name", name);
                xmlWriter.WriteEndElement();
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
            SetProperties(name, port, webroot, defaultPage, dirBrowsing, controlPort, controlroot);
        }

        private static void SetProperties(String name, String port, String webroot, String defaultPage, String dirBrowsing, String controlPort, String controlroot)
        {
            Name = name;
            WebPort = Convert.ToInt32(port);
            Webroot = webroot;
            DefaultPage = defaultPage;
            DirectoryBrowsing = Convert.ToBoolean(dirBrowsing);
            ControlPort = Convert.ToInt32(controlPort);
            Controlroot = controlroot;
        }

        internal static void WriteConfig(string webPort, string controlPort, string webRoot, string defaultPage, string dirBrowsing)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(CONFIG_PATH);
            // Get root element
            XmlElement element = doc.DocumentElement;
            // Write elements
            XmlNode server = element.SelectSingleNode("server");
            XmlNode webPortNode = server.SelectSingleNode("port");
            webPortNode.InnerText = webPort;
            XmlNode webRootNode = server.SelectSingleNode("webroot");
            webRootNode.InnerText = webRoot;
            XmlNode defaultPageNode = server.SelectSingleNode("default-page");
            defaultPageNode.InnerText = defaultPage;
            XmlNode dirBrowsingNode = server.SelectSingleNode("dir-browsing");
            dirBrowsingNode.InnerText = dirBrowsing;
            XmlNode controlServer = element.SelectSingleNode("controlserver");
            XmlNode controlPortNode = controlServer.SelectSingleNode("port");
            controlPortNode.InnerText = controlPort;
            // Save config
            doc.Save(CONFIG_PATH);
            // Set properties
            SetProperties(Name, webPort, webRoot, defaultPage, dirBrowsing, controlPort, Controlroot);
        }
    }
}
