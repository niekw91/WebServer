using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebServer.Utilities.User
{
    class User
    {

        public static IDataReader GetAll()
        {
            return Database.MySQLDatabaseConnection.GetUsers();
        }

        public static void Add(string username, string password, int role)
        {
            string salt = Authentication.Authentication.CreateSalt(5);

            string hash = Authentication.Authentication.HashPasswordWithSalt(password, Encoding.UTF8.GetBytes(salt));

            Database.MySQLDatabaseConnection.AddUser(username, hash, salt, role);
        }

        public static void Edit(int id, string username, string password)
        {
            string salt = Authentication.Authentication.CreateSalt(5);

            string hash = Authentication.Authentication.HashPasswordWithSalt(password, Encoding.UTF8.GetBytes(salt));

            //Database.MySQLDatabaseConnection.AddUser(username, hash, salt, 1);
        }

    }
}
