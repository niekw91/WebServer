using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web.Security;
using WebServer.Utilities.Database;

namespace WebServer.Utilities.Authentication
{
    class Authentication
    {
        public static bool Login(string username, string password)
        {
            string salt = MySQLDatabaseConnection.GetUserSalt(username);
            if (salt != null)
            {
                string hash = HashPasswordWithSalt(password, Encoding.UTF8.GetBytes(salt));

                return MySQLDatabaseConnection.GetLoginCredentials(username, hash);
            }
            return false;
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
    }
}
