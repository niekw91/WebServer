using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebServer.Utilities.Interfaces;

namespace WebServer.Utilities.Database
{
    class MySQLDatabaseConnection : IDatabaseConnection
    {
        private static readonly string CONNECTION_STRING = "Server=databases.aii.avans.nl;Database=nwmwille_db;Uid=nwmwille;Pwd=stoeptegel;";

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

        public static Dictionary<String, String> GetLoginCredentials(string username, string password)
        {
            Dictionary<String, String> credentials = new Dictionary<String, String>();
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
                        credentials.Add("id", reader["id"].ToString());
                        credentials.Add("username", reader["username"].ToString());
                        credentials.Add("password", reader["password"].ToString());
                        credentials.Add("salt", reader["salt"].ToString());
                        credentials.Add("role_id", reader["role_id"].ToString());
                        credentials.Add("token", reader["token"].ToString());
                    }
                    reader.Close();
                }
            }
            catch (MySqlException ex) { Console.WriteLine(ex.Message); }

            return credentials;
        }

        public static IDataReader GetUsers()
        {
            MySqlDataReader reader;
            try
            {
                using (MySqlConnection conn = new MySqlConnection(CONNECTION_STRING))
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand("SELECT * FROM user;", conn);

                    reader = cmd.ExecuteReader();
                    var dt = new DataTable();
                    dt.Load(reader);
                    return dt.CreateDataReader();
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

        public static void SetUserToken(string token, string id)
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(CONNECTION_STRING))
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand("UPDATE user SET token=@token WHERE id=@id", conn);

                    cmd.Parameters.AddWithValue("@token", token);
                    cmd.Parameters.AddWithValue("@id", id);

                    cmd.ExecuteNonQuery();
                }
            }
            catch (MySqlException ex) { Console.WriteLine(ex.Message); }
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

        public static DataTable GetRoles()
        {
            DataTable dt = new DataTable();
            try
            {
                using (MySqlConnection conn = new MySqlConnection(CONNECTION_STRING))
                {
                    conn.Open();

                    MySqlCommand cmd = new MySqlCommand("SELECT * FROM role;", conn);

                    dt.Load(cmd.ExecuteReader());
                }
            }
            catch (MySqlException ex) { Console.WriteLine(ex.Message); }

            return dt;
        }
    }
}
