using HtmlAgilityPack;
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
        public string ParseHTMLConfig(String path, bool isAdmin)
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
                        // If user is administrator, enable all fields
                        if (isAdmin && input != null && input.Attributes["disabled"] != null) input.Attributes["disabled"].Remove();
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
                                if (!ServerConfig.DirectoryBrowsing) input.Attributes["checked"].Remove();
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
            if (!isAdmin)
            {
                HtmlNode userLink = doc.DocumentNode.SelectSingleNode("//a[@id='user-link']");
                userLink.Remove();
                HtmlNode submitButton = doc.DocumentNode.SelectSingleNode("//input[@id='submit-button']");
                submitButton.Remove();
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

        public string ParseUserTableHTML(string path)
        {
            var reader = Database.MySQLDatabaseConnection.GetUsers();

            var doc = new HtmlDocument();
            doc.Load(path);

            foreach (HtmlNode table in doc.DocumentNode.SelectNodes("//table[@id='user-table']"))
            {
                StringBuilder rows = new StringBuilder("<thead><tr><th>Username</th></tr></thead>");
                rows.Append("<tbody>");
                if (reader != null)
                {
                    while (reader.Read())
                    {
                        rows.Append("<tr>");
                        rows.Append("<td>" + reader["username"] + "</td>");
                        rows.Append("<td><a href='/user/edit.html?id=" + reader["id"] + "'></a><a href='/user/delete.html?id=" + reader["id"] + "'></a></td>");
                        rows.Append("</tr>");
                    }
                }
                rows.Append("</tbody>");
                table.InnerHtml = rows.ToString();
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

        public string ParseDirectoryBrowsingHTML(string path, string[] dirs, string[] files)
        {
            var doc = new HtmlDocument();
            doc.Load(path);

            HtmlNode fileList = doc.DocumentNode.SelectSingleNode("//ul[@id='list']");

            StringBuilder listHTML = new StringBuilder();
            string listItemFormat = "<li>{0} <a href='{1}'>{2}</a></li>";

            foreach (string dir in dirs)
            {
                listHTML.Append(String.Format(listItemFormat, "D >", dir+"/", dir));
            }
            foreach (string file in files)
            {
                listHTML.Append(String.Format(listItemFormat, "F >", file, file));
            }

            fileList.InnerHtml = listHTML.ToString();

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
