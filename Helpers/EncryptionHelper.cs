using System.Security.Cryptography;
using System.Text;
using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
namespace RedBerryCorporate.Helpers
{

        public static class EncryptionHelper
        {
            // -------------------------------
            // 1️⃣ LEGACY PASSWORD (SHA1) - keeps old passwords working
            // -------------------------------
            public static string EncrptPassword(string txtPassword)
            {
                if (string.IsNullOrEmpty(txtPassword))
                    return txtPassword;

                byte[] data = Encoding.Unicode.GetBytes(txtPassword);
                using (SHA1 algorithm = SHA1.Create())
                {
                    byte[] hash = algorithm.ComputeHash(data);
                    return Convert.ToBase64String(hash);
                }
            }

            // -------------------------------
            // 2️⃣ Modern Secure Password Hash (PBKDF2) for new users
            // -------------------------------
            public static string HashPassword(string password)
            {
                if (string.IsNullOrEmpty(password))
                    return password;

                using (var deriveBytes = new Rfc2898DeriveBytes(password, 16, 100_000))
                {
                    byte[] salt = deriveBytes.Salt;
                    byte[] key = deriveBytes.GetBytes(32); // 256-bit key
                    return Convert.ToBase64String(salt) + ":" + Convert.ToBase64String(key);
                }
            }

            public static bool VerifyPassword(string password, string storedHash)
            {
                if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(storedHash))
                    return false;

                var parts = storedHash.Split(':');
                if (parts.Length != 2) return false;

                byte[] salt = Convert.FromBase64String(parts[0]);
                byte[] storedKey = Convert.FromBase64String(parts[1]);

                using (var deriveBytes = new Rfc2898DeriveBytes(password, salt, 100_000))
                {
                    byte[] key = deriveBytes.GetBytes(32);
                    return key.SequenceEqual(storedKey);
                }
            }

            // -------------------------------
            // 3️⃣ AES Encryption/Decryption for sensitive data
            // -------------------------------
            private static readonly string AesKey = "b14ca5898a4e4133bbce2ea2315a1916"; // Must be 32 bytes for AES-256

            public static string EncryptPass(string plainText)
            {
                if (string.IsNullOrEmpty(plainText))
                    return plainText;

                using (Aes aes = Aes.Create())
                {
                    aes.Key = Encoding.UTF8.GetBytes(AesKey);
                    aes.GenerateIV(); // random IV for each encryption

                    using (var encryptor = aes.CreateEncryptor(aes.Key, aes.IV))
                    using (var ms = new MemoryStream())
                    {
                        // prepend IV
                        ms.Write(aes.IV, 0, aes.IV.Length);
                        using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                        using (var sw = new StreamWriter(cs))
                        {
                            sw.Write(plainText);
                        }
                        return Convert.ToBase64String(ms.ToArray());
                    }
                }
            }

            public static string DecryptPass(string encryptedText)
            {
                if (string.IsNullOrEmpty(encryptedText))
                    return encryptedText;

                var fullCipher = Convert.FromBase64String(encryptedText);
                using (Aes aes = Aes.Create())
                {
                    byte[] iv = new byte[16];
                    byte[] cipher = new byte[fullCipher.Length - 16];

                    Array.Copy(fullCipher, 0, iv, 0, iv.Length);
                    Array.Copy(fullCipher, iv.Length, cipher, 0, cipher.Length);

                    aes.Key = Encoding.UTF8.GetBytes(AesKey);
                    aes.IV = iv;

                    using (var decryptor = aes.CreateDecryptor(aes.Key, aes.IV))
                    using (var ms = new MemoryStream(cipher))
                    using (var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                    using (var sr = new StreamReader(cs))
                    {
                        return sr.ReadToEnd();
                    }
                }
            }

            // -------------------------------
            // 4️⃣ Optional SHA256 Hash (for general use)
            // -------------------------------
            public static string Hash(string value)
            {
                if (string.IsNullOrEmpty(value))
                    return value;

                using (var sha256 = SHA256.Create())
                {
                    var bytes = Encoding.UTF8.GetBytes(value);
                    var hashBytes = sha256.ComputeHash(bytes);
                    return Convert.ToBase64String(hashBytes);
                }
            }
        }
    }

