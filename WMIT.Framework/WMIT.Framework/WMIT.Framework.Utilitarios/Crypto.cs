using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace WMIT.Framework.Utilitarios
{
    public sealed class Crypto
    {

        public static byte[] Encrypt(byte[] clearData, byte[] Key, byte[] IV)
        {

            MemoryStream ms = new MemoryStream();

            Aes alg = Aes.Create();

            alg.Key = Key;
            alg.IV = IV;
            CryptoStream cs = new CryptoStream(ms, alg.CreateEncryptor(), CryptoStreamMode.Write);
            cs.Write(clearData, 0, clearData.Length);

            cs.Close();
            byte[] encryptedData = ms.ToArray();

            return encryptedData;
        }

        public static string Encrypt(string clearText, string Password)
        {
            if (string.IsNullOrEmpty(clearText))
                return "";

            byte[] InitialVectorBytes = Encoding.ASCII.GetBytes("OFRna73m*aze01xY");

            byte[] SaltValueBytes = Encoding.ASCII.GetBytes("cryptoSalt");

            byte[] PlainTextBytes = Encoding.UTF8.GetBytes(clearText);

            PasswordDeriveBytes DerivedPassword = new PasswordDeriveBytes(Password, SaltValueBytes, "SHA1", 2);

            byte[] KeyBytes = DerivedPassword.GetBytes(256 / 8);

            RijndaelManaged SymmetricKey = new RijndaelManaged();

            SymmetricKey.Mode = CipherMode.CBC;

            byte[] CipherTextBytes = null;

            using (MemoryStream MemStream = new MemoryStream())
            using (ICryptoTransform Encryptor = SymmetricKey.CreateEncryptor(KeyBytes, InitialVectorBytes))
            using (CryptoStream CryptoStream = new CryptoStream(MemStream, Encryptor, CryptoStreamMode.Write))
            {
                CryptoStream.Write(PlainTextBytes, 0, PlainTextBytes.Length);

                CryptoStream.FlushFinalBlock();

                CipherTextBytes = MemStream.ToArray();
            }

            SymmetricKey.Clear();

            return Convert.ToBase64String(CipherTextBytes);
        }

        public static string UrlEncrypt(string texto, string Password)
        {
            string retorno = Encrypt(texto, Password);
            do
            {
                retorno = retorno.Replace("+", "%2b");
            } while (retorno.IndexOf("+") > -1);

            return System.Web.HttpUtility.UrlEncode(retorno);
        }

        public static byte[] Decrypt(byte[] cipherData, byte[] Key, byte[] IV)
        {
            MemoryStream ms = new MemoryStream();

            Aes alg = Aes.Create();
            alg.Key = Key;
            alg.IV = IV;

            CryptoStream cs = new CryptoStream(ms, alg.CreateDecryptor(), CryptoStreamMode.Write);
            cs.Write(cipherData, 0, cipherData.Length);
            cs.Close();

            byte[] decryptedData = ms.ToArray();

            return decryptedData;
        }

        public static string UrlDecrypt(string hash, string Password)
        {
            do
            {
                hash = hash.Replace("%252b", "%2b");
            } while (hash.IndexOf("%252b") > -1);

            return Decrypt(System.Web.HttpUtility.UrlDecode(hash), Password);
        }

        public static string Decrypt(string hash, string Password)
        {
            if (string.IsNullOrEmpty(hash))
                return "";

            byte[] InitialVectorBytes = Encoding.ASCII.GetBytes("OFRna73m*aze01xY");

            byte[] SaltValueBytes = Encoding.ASCII.GetBytes("cryptoSalt");

            byte[] hashBytes = Convert.FromBase64String(hash);

            PasswordDeriveBytes DerivedPassword = new PasswordDeriveBytes(Password, SaltValueBytes, "SHA1", 2);

            byte[] KeyBytes = DerivedPassword.GetBytes(256 / 8);

            RijndaelManaged SymmetricKey = new RijndaelManaged();

            SymmetricKey.Mode = CipherMode.CBC;

            byte[] PlainTextBytes = new byte[hash.Length];
            int length = 0;

            using (MemoryStream MemStream = new MemoryStream(hashBytes))
            using (ICryptoTransform Encryptor = SymmetricKey.CreateDecryptor(KeyBytes, InitialVectorBytes))
            using (CryptoStream CryptoStream = new CryptoStream(MemStream, Encryptor, CryptoStreamMode.Read))
                length = CryptoStream.Read(PlainTextBytes, 0, hashBytes.Length);

            SymmetricKey.Clear();

            return Encoding.UTF8.GetString(PlainTextBytes, 0, length);
        }

        public static byte[] Decrypt(byte[] cipherData, string Password)
        {
            PasswordDeriveBytes pdb = new PasswordDeriveBytes(Password,
            new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });

            return Decrypt(cipherData, pdb.GetBytes(256), pdb.GetBytes(128));
        }

        public static string BytesToHex(byte[] bytes)
        {
            StringBuilder hexString = new StringBuilder(bytes.Length);
            for (int i = 0; i < bytes.Length; i++)
            {
                hexString.Append(bytes[i].ToString("X2"));
            }
            return hexString.ToString();
        }

        public static string HexByte(byte[] bytes)
        {
            StringBuilder hexString = new StringBuilder(bytes.Length);
            for (int i = 0; i < bytes.Length; i++)
            {
                hexString.Append(bytes[i].ToString("X2"));
            }
            return hexString.ToString();
        }

        public static string RSAEncrypt(string clearText)
        {
            if (string.IsNullOrEmpty(clearText))
                return "";

            byte[] encryptedData;

            using (RSACryptoServiceProvider RSA = new RSACryptoServiceProvider(4096))
            {
                var xml = System.IO.File.ReadAllText(string.Concat(AppDomain.CurrentDomain.BaseDirectory, "RSAKeys.xml"));

                RSA.FromXmlString(xml);

                encryptedData = WMIT.Framework.Utilitarios.Crypto.RSAEncrypt(Encoding.UTF8.GetBytes(clearText), RSA.ExportParameters(false), false);

                RSA.PersistKeyInCsp = false;
            }

            return Convert.ToBase64String(encryptedData);
        }

        public static string RSAUrlEncrypt(string texto)
        {
            string retorno = RSAEncrypt(texto);
            //do
            //{
            //    retorno = retorno.Replace("+", "%2b");
            //} while (retorno.IndexOf("+") > -1);

            return System.Web.HttpUtility.UrlEncode(retorno);
        }

        public static string RSAUrlDecrypt(string hash)
        {
            //do
            //{
            //    hash = hash.Replace("%252b", "%2b");
            //} while (hash.IndexOf("%252b") > -1);

            return RSADecrypt(System.Web.HttpUtility.UrlDecode(hash));
        }

        public static string RSADecrypt(string hash)
        {
            if (string.IsNullOrEmpty(hash))
                return "";

            byte[] decryptedData;

            using (RSACryptoServiceProvider RSA = new RSACryptoServiceProvider(4096))
            {
                var xml = System.IO.File.ReadAllText(string.Concat(AppDomain.CurrentDomain.BaseDirectory, "RSAKeys.xml"));

                RSA.FromXmlString(xml);

                decryptedData = WMIT.Framework.Utilitarios.Crypto.RSADecrypt(Convert.FromBase64String(hash), RSA.ExportParameters(true), false);

                RSA.PersistKeyInCsp = false;
            }

            return Encoding.UTF8.GetString(decryptedData);
        }

        /// <summary>
        /// código extraído da documentação oficial: https://msdn.microsoft.com/en-us/library/system.security.cryptography.rsacryptoserviceprovider(v=vs.110).aspx
        /// </summary>
        /// <param name="DataToEncrypt"></param>
        /// <param name="RSAKeyInfo"></param>
        /// <param name="DoOAEPPadding"></param>
        /// <returns></returns>
        static private byte[] RSAEncrypt(byte[] DataToEncrypt, RSAParameters RSAKeyInfo, bool DoOAEPPadding)
        {
            try
            {
                byte[] encryptedData;
                //Create a new instance of RSACryptoServiceProvider.
                using (RSACryptoServiceProvider RSA = new RSACryptoServiceProvider(4096))
                {

                    //Import the RSA Key information. This only needs
                    //toinclude the public key information.
                    RSA.ImportParameters(RSAKeyInfo);

                    //Encrypt the passed byte array and specify OAEP padding.  
                    //OAEP padding is only available on Microsoft Windows XP or
                    //later.  
                    encryptedData = RSA.Encrypt(DataToEncrypt, DoOAEPPadding);

                    RSA.PersistKeyInCsp = false;
                }
                return encryptedData;
            }
            //Catch and display a CryptographicException  
            //to the console.
            catch (CryptographicException e)
            {
                Console.WriteLine(e.Message);

                return null;
            }

        }

        static private byte[] RSADecrypt(byte[] DataToDecrypt, RSAParameters RSAKeyInfo, bool DoOAEPPadding)
        {
            try
            {
                byte[] decryptedData;
                //Create a new instance of RSACryptoServiceProvider.
                using (RSACryptoServiceProvider RSA = new RSACryptoServiceProvider(4096))
                {
                    //Import the RSA Key information. This needs
                    //to include the private key information.
                    RSA.ImportParameters(RSAKeyInfo);

                    //Decrypt the passed byte array and specify OAEP padding.  
                    //OAEP padding is only available on Microsoft Windows XP or
                    //later.  
                    decryptedData = RSA.Decrypt(DataToDecrypt, DoOAEPPadding);

                    RSA.PersistKeyInCsp = false;
                }
                return decryptedData;
            }
            //Catch and display a CryptographicException  
            //to the console.
            catch (CryptographicException e)
            {
                Console.WriteLine(e.ToString());

                return null;
            }

        }
    }
}
