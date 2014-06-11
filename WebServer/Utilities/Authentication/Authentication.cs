using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Security;
using WebServer.Utilities.Database;

namespace WebServer.Utilities.Authentication
{
    class Authentication
    {
        public static string Login(string username, string password, Server.Server server)
        {
            string salt = MySQLDatabaseConnection.GetUserSalt(username);
            if (salt != null)
            {
                string hash = HashPasswordWithSalt(password, Encoding.UTF8.GetBytes(salt));
                Dictionary<String, String> credentials = MySQLDatabaseConnection.GetLoginCredentials(username, hash);
                if (credentials["password"] == hash)
                {
                    // Generate token
                    string token = GenerateToken();
                    MySQLDatabaseConnection.SetUserToken(token, credentials["id"].ToString());
                    return token;
                }
            }
            return null;
        }

        public static string CreateSalt(int size)
        {
            // Generate a random number 
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            byte[] buff = new byte[size];
            rng.GetBytes(buff);
            // Return a Base64 string of the random number
            return Convert.ToBase64String(buff);
        }

        public static string HashPasswordWithSalt(string plainText, byte[] salt)
        {
            // Convert the plain string password into bytes
            byte[] plainTextBytes = UnicodeEncoding.Unicode.GetBytes(plainText);
            // Append salt to password before hashing
            byte[] combinedBytes = new byte[plainTextBytes.Length + salt.Length];
            System.Buffer.BlockCopy(plainTextBytes, 0, combinedBytes, 0, plainTextBytes.Length);
            System.Buffer.BlockCopy(salt, 0, combinedBytes, plainTextBytes.Length, salt.Length);
            // Create hash
            System.Security.Cryptography.HashAlgorithm hashAlgo = new System.Security.Cryptography.SHA256Managed();
            byte[] hash = hashAlgo.ComputeHash(combinedBytes);

            return Convert.ToBase64String(hash);
        }

        public static string GenerateToken()
        {
            byte[] time = BitConverter.GetBytes(DateTime.UtcNow.ToBinary());
            byte[] key = Guid.NewGuid().ToByteArray();
            return Convert.ToBase64String(time.Concat(key).ToArray());
        }

        public static bool IsTokenValid(string token)
        {
            byte[] data = Convert.FromBase64String(token);
            DateTime when = DateTime.FromBinary(BitConverter.ToInt64(data, 0));
            if (when < DateTime.UtcNow.AddHours(-24))
            {
                return false;
            }
            return true;
        }

        public static Dictionary<String, int> GetRoles()
        {
            DataTable dt = MySQLDatabaseConnection.GetRoles();
            Dictionary<String, int> roles = new Dictionary<string, int>();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                roles.Add(dt.Rows[i]["role"].ToString(), (int)dt.Rows[i]["id"]);
            }
            return roles;
        }

        public static bool IsCurrentUserAdmin(string token, Dictionary<String, int> roles)
        {
            if (token != null)
            {
                Dictionary<String, String> credentials = MySQLDatabaseConnection.GetUser(token);
                if (credentials.Count > 0 && credentials["role_id"] == roles["Administrator"].ToString())
                {
                    return true;
                }
            }
            return false;
        }
    }
}
