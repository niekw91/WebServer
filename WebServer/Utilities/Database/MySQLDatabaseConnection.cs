using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebServer.Utilities.Interfaces;

namespace WebServer.Utilities.Database
{
    class MySQLDatabaseConnection : IDatabaseConnection
    {
        private static readonly string CONNECTION_STRING = "Server=127.0.0.1;Database=webserver;Uid=root;Pwd=;";

        public static string GetUserSalt(string username)
        {
            string salt = null;

            try
            {
                using (MySqlConnection conn = new MySqlConnection(CONNECTION_STRING))
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand("SELECT salt FROM user WHERE username=@username", conn);

                    cmd.Parameters.AddWithValue("@username", username);

                    MySqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                        salt = (string)reader[0];

                    reader.Close();
                }
            }
            catch (MySqlException ex) { Console.WriteLine(ex.Message); }

            return salt;
        }

        public static bool GetLoginCredentials(string username, string password)
        {
            bool success = false;

            try
            {
                using (MySqlConnection conn = new MySqlConnection(CONNECTION_STRING))
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand("SELECT * FROM user WHERE username=@username AND password=@password", conn);

                    cmd.Parameters.AddWithValue("@username", username);
                    cmd.Parameters.AddWithValue("@password", password);

                    MySqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read()) 
                    {
                        if (reader["password"].ToString() == password)
                            success = true;
                    }
                    reader.Close();
                }
            }
            catch (MySqlException ex) { Console.WriteLine(ex.Message); }

            return success;
        }

        public static MySqlDataReader GetUsers()
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(CONNECTION_STRING))
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand("SELECT * FROM user;", conn);

                    return cmd.ExecuteReader();
                    //reader.Close();
                }
            }
            catch (MySqlException ex) { Console.WriteLine(ex.Message); }
            return null;
        }

        public static MySqlDataReader GetUser(int id)
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(CONNECTION_STRING))
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand("SELECT * FROM user WHERE id=@id;", conn);

                    cmd.Parameters.AddWithValue("@id", id);

                    return cmd.ExecuteReader();
                    //reader.Close();
                }
            }
            catch (MySqlException ex) { Console.WriteLine(ex.Message); }
            return null;
        }

        public static void AddUser(string username, string password, string salt)
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(CONNECTION_STRING))
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand("INSERT INTO user (username, password, salt) VALUES(@username, @password, @salt)", conn);

                    cmd.Parameters.AddWithValue("@username", username);
                    cmd.Parameters.AddWithValue("@password", password);
                    cmd.Parameters.AddWithValue("@salt", salt);

                    cmd.ExecuteReader();
                    //reader.Close();
                }
            }
            catch (MySqlException ex) { Console.WriteLine(ex.Message); }
        }

        public static void EditUser()
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(CONNECTION_STRING))
                {
                    conn.Open();
                    // Update query
                    MySqlCommand cmd = new MySqlCommand("INSERT INTO user (username, password, salt) VALUES(@username, @password, @salt)", conn);

                    //cmd.Parameters.AddWithValue("@username", username);
                    //cmd.Parameters.AddWithValue("@password", password);

                    cmd.ExecuteReader();
                    //reader.Close();
                }
            }
            catch (MySqlException ex) { Console.WriteLine(ex.Message); }
        }
    }
}
