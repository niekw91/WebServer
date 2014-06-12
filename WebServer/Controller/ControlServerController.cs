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
        // Boolean to check if current request is made by an admin
        private bool isAdmin = false;

        public override void Init()
        {
            Console.WriteLine(Request.Url);
            // Get cookie with token from header
            string token = Request.GetHeader("cookie");
            if (token != null)
            {
                // Split the token string and retrieve the value
                token = token.Split('=')[1];
                // Check if current request is by an admin
                isAdmin = Authentication.IsCurrentUserAdmin(token, Server.Roles);
            }

            switch (Request.Url)
            {
                case "/configuration.html":
                    if (token != null && Authentication.IsTokenValid(token))
                    {
                        if (Request.Method == "GET")
                            GetConfiguration();
                        else if (Request.Method == "POST")
                        {
                            Console.WriteLine("Referer: " + Request.GetHeader("referer"));
                            // Check if post is from configuration page
                            if (!String.IsNullOrEmpty(Request.GetHeader("referer")) && !Request.GetHeader("referer").Contains("configuration.html"))
                                GetConfiguration();
                            else
                                PostConfiguration();
                        }
                    }
                    else
                    {
                        Redirect("/index.html");
                    }
                    break;
                case "/index.html":
                    if (Request.Method == "GET")
                    {
                        // Check if user is already logged in, if so redirect to config
                        if (token != null && Authentication.IsTokenValid(token))
                            Redirect("configuration.html");
                    }
                    else if (Request.Method == "POST")
                        Login();
                    break;
                case "/user/index.html":
                    if (isAdmin)
                        GetUserTable();
                    break;
                case "/user/add.html":
                    if (isAdmin)
                    {
                        if (Request.Method == "POST")
                            AddUser();
                    }
                    break;
                case "/logout.html":
                    Logout();
                    break;
            }
        }

        private void Logout()
        {
            Response.AddHeader("Set-Cookie", String.Format("UserID=deleted; expires=Thu, 01 Jan 1970 00:00:00 GMT"));
            Redirect("index.html");
        }

        private void GetConfiguration()
        {
            HTMLParser parser = new HTMLParser();
            string content = parser.ParseHTMLConfig(Response.Path, isAdmin);
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
                    String dirBrowsing = Request.Values.ContainsKey("dir-browsing") ? "true" : "false";
                    // Write config file
                    ServerConfig.WriteConfig(webPort, controlPort, webRoot, defaultPage, dirBrowsing);
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
                    string token = Authentication.Login(username, password, Server);
                    if (token != null)
                    {
                        Console.WriteLine("Success login");
                        Response.Path = ServerConfig.Controlroot+ @"\configuration.html";
                        // Set cookie header
                        Response.AddHeader("Set-Cookie", String.Format("UserID={0}; Max-Age=3600; Version=1", token));
                        // Redirect to config
                        Redirect("configuration.html");
                    }
                    else
                    {
                        Console.WriteLine("Failed login");
                    }
                        
                }
            }
            catch (KeyNotFoundException) { }
        }

        private void Redirect(string location)
        {
            Response.StatusCode = 307;
            Response.StatusMessage = "Redirect";
            Response.AddHeader("Location", location);
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
                int role = Server.Roles["User"];
                if(Server.Roles.ContainsKey(Request.Values["role"]))
                    role = Server.Roles[Request.Values["role"]];

                if (!String.IsNullOrEmpty(username) && !String.IsNullOrEmpty(password))
                {
                    User.Add(username, password, role);

                    Response.StatusCode = 307;
                    Response.StatusMessage = "Redirect";
                    Response.AddHeader("Location", "/user/index.html");
                }
            }
        }
    }
}
