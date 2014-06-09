using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebServer.Utilities.Authentication;
using WebServer.Utilities.HTML;
using WebServer.Utilities.User;

namespace WebServer.Server
{
    class ControlServerController : BaseController
    {
        public override void Init()
        {
            Console.WriteLine(Request.Url);
            switch (Request.Url)
            {
                case "/configuration.html":
                    if (Request.Method == "GET")
                        GetConfiguration();
                    else if (Request.Method == "POST") {
                        Console.WriteLine("Referer: " + Request.GetHeader("referer"));
                        // Dirty check of de post niet van ergens anders komt
                        if (!String.IsNullOrEmpty(Request.GetHeader("referer")) && !Request.GetHeader("referer").Contains("configuration.html"))
                            GetConfiguration();
                        else
                            PostConfiguration();
                    }
                    break;
                case "/index.html":
                    if (Request.Method == "POST")
                        Login();
                    break;
                case "/user/index.html":
                    GetUserTable();
                    break;
                case "/user/add.html":
                    if (Request.Method == "POST")
                    {
                        AddUser();
                    }
                    break;
                case "/user/edit.html":
                    if (Request.Method == "GET")
                        GetEditUser();
                    else if (Request.Method == "POST")
                        PostEditUser();
                    break;
            }
        }

        private void GetConfiguration()
        {
            HTMLParser parser = new HTMLParser();
            string content = parser.ParseHTMLConfig(Response.Path);
            Response.Content = Encoding.UTF8.GetBytes(content);
            Response.AddHeader("Content-Length", Response.Content.Length.ToString());
        }

        private void PostConfiguration()
        {
            try
            {
                if (Request.Values.Count > 0)
                {
                    String webPort = Request.Values["web-port"];
                    String controlPort = Request.Values["control-port"];
                    String webRoot = Request.Values["webroot"];
                    String defaultPage = Request.Values["default-page"];
                    // Write config file
                    ServerConfig.WriteConfig(webPort, controlPort, webRoot, defaultPage);
                    // Restart server
                    Server.RestartId = Request.Id;
                    // Set response
                    GetConfiguration();
                }
            }
            catch (KeyNotFoundException) { }
        }

        private void Login()
        {
            try
            {
                if (Request.Values.Count > 0)
                {
                    String username = Request.Values["username"];
                    String password = Request.Values["password"];
                    // Check if login is valid
                    bool loggedIn = Authentication.Login(username, password);
                    if (loggedIn)
                    {
                        Console.WriteLine("Success login");
                        Response.Path = ServerConfig.Controlroot+ @"\configuration.html";
                        GetConfiguration();
                    }
                    else
                    {
                        Console.WriteLine("Failed login");
                        //Response.Path = ServerConfig.Controlroot+@"\configuration.html";
                        //GetConfiguration();
                        Response.StatusCode = 307;
                        Response.StatusMessage = "Redirect";
                        Response.AddHeader("Location", "configuration.html");
                        //Response.AddHeader("X-Hello", "Hello");
                    }
                        
                }
            }
            catch (KeyNotFoundException) { }
        }

        private void GetUserTable()
        {
            HTMLParser parser = new HTMLParser();
            string content = parser.ParseUserTableHTML(Response.Path);
            if (content != null)
            {
                Response.Content = Encoding.UTF8.GetBytes(content);
                Response.AddHeader("Content-Length", Response.Content.Length.ToString());
            }
        }

        private void AddUser()
        {
            if (Request.Values.Count > 0)
            {
                string username = Request.Values["username"];
                string password = Request.Values["password"];

                if (!String.IsNullOrEmpty(username) && !String.IsNullOrEmpty(password))
                {
                    User.Add(username, password);

                    Response.StatusCode = 307;
                    Response.StatusMessage = "Redirect";
                    Response.AddHeader("Location", "/user/index.html");
                }
            }
        }

        private void PostEditUser()
        {
            if (Request.Values.Count > 0)
            {
                int id = Convert.ToInt32(Request.Values["id"]);
                string username = Request.Values["username"];
                string password = Request.Values["password"];

                if (!String.IsNullOrEmpty(username) && !String.IsNullOrEmpty(password))
                {
                    User.Edit(id, username, password);

                    Response.StatusCode = 307;
                    Response.StatusMessage = "Redirect";
                    Response.AddHeader("Location", "/user/index.html");
                }
            }
        }

        private void GetEditUser()
        {
            HTMLParser parser = new HTMLParser();
            Response.Path = Response.Path.Replace("add", "edit");
            string content = parser.ParseEditUserHTML(Response.Path, 0);
            if (content != null)
            {
                Response.Content = Encoding.UTF8.GetBytes(content);
                Response.AddHeader("Content-Length", Response.Content.Length.ToString());
            }
        }
    }
}
