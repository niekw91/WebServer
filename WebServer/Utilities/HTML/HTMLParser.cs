﻿using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebServer.Server;

namespace WebServer.Utilities.HTML
{
    class HTMLParser
    {
        public string ParseHTMLConfig(String path)
        {
            // Load specified HTML document
            var doc = new HtmlDocument();
            doc.Load(path);

            foreach (HtmlNode table in doc.DocumentNode.SelectNodes("//table"))
            {
                foreach (HtmlNode row in table.SelectNodes("tr"))
                {
                    foreach (HtmlNode cell in row.SelectNodes("th|td"))
                    {
                        // Select input field
                        HtmlNode input = cell.SelectSingleNode("input");
                        switch (cell.Id)
                        {
                            case "web-port":
                                input.Attributes["value"].Value = ServerConfig.WebPort.ToString();
                                break;
                            case "control-port":
                                input.Attributes["value"].Value = ServerConfig.ControlPort.ToString();
                                break;
                            case "webroot":
                                input.Attributes["value"].Value = ServerConfig.Webroot.ToString();
                                break;
                            case "default-page":
                                input.Attributes["value"].Value = ServerConfig.DefaultPage.ToString();
                                break;
                            case "dir-browsing":
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
            string result = null;
            using (StringWriter writer = new StringWriter())
            {
                doc.Save(writer);
                result = writer.ToString();
                writer.Flush();
            }
            return result;
        }
    }
}