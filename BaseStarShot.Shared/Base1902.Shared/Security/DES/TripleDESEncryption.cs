using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Base1902.Security;
#if !NETFX_CORE
using System.Security.Cryptography;
#endif

namespace BaseStarShot.Security.DES
{
    public class TripleDESEncryption : IEncryptor, IDecryptor
    {
        public string Encrypt(string plaintext, string password)
        {
            if (string.IsNullOrEmpty(plaintext)) return String.Empty;

#if !NETFX_CORE
            if (plaintext.Length > 0)
            {
                string passphrase = password;
                byte[] results;
                System.Text.UTF8Encoding utf8 = new System.Text.UTF8Encoding();

                // Step 1. We hash the passphrase using MD5
                // We use the MD5 hash generator as the result is a 128 bit byte array
                // which is a valid length for the TripleDES encoder we use below
#if WINDOWS_PHONE
                MD5Managed HashProvider = new MD5Managed();
#else
                MD5CryptoServiceProvider HashProvider = new MD5CryptoServiceProvider();
#endif
                byte[] TDESKey = HashProvider.ComputeHash(utf8.GetBytes(passphrase));

#if WINDOWS_PHONE
                // Step 2. Create a TripleDES cipher utility object and setup encoder.
                var cipher = Org.BouncyCastle.Security.CipherUtilities.GetCipher("DESede/ECB/PKCS7Padding");
                cipher.Init(true, new Org.BouncyCastle.Crypto.Parameters.DesEdeParameters(TDESKey));

                // Step 3. Convert the input string to a byte[]
                byte[] DataToEncrypt = utf8.GetBytes(plaintext);

                // Step 4. Encrypt the string
                results = cipher.DoFinal(DataToEncrypt);

                // Step 5. Return the encrypted string as a base64 encoded string
                return Convert.ToBase64String(results);
#else
                // Step 2. Create a new TripleDESCryptoServiceProvider object
                TripleDESCryptoServiceProvider TDESAlgorithm = new TripleDESCryptoServiceProvider();

                // Step 3. Setup the encoder
                TDESAlgorithm.Key = TDESKey;
                TDESAlgorithm.Mode = CipherMode.ECB;
                TDESAlgorithm.Padding = PaddingMode.PKCS7;

                // Step 4. Convert the input string to a byte[]
                byte[] DataToEncrypt = utf8.GetBytes(plaintext);

                // Step 5. Attempt to encrypt the string
                try
                {
                    ICryptoTransform Encryptor = TDESAlgorithm.CreateEncryptor();
                    results = Encryptor.TransformFinalBlock(DataToEncrypt, 0, DataToEncrypt.Length);
                }
                finally
                {
                    // Clear the TripleDes and Hashprovider services of any sensitive information
                    TDESAlgorithm.Clear();
                    HashProvider.Clear();
                }

                // Step 6. Return the encrypted string as a base64 encoded string
                return Convert.ToBase64String(results);
#endif
            }
            else
                return string.Empty;
#else
            // TODO: Implement TripleDESEncryption.Encrypt.
            return null;
#endif
        }


        public string Decrypt(string plaintext, string password)
        {
#if !NETFX_CORE
            if (plaintext.Length > 0)
            {
                string passphrase = password;
                byte[] results;
                System.Text.UTF8Encoding utf8 = new System.Text.UTF8Encoding();

                // Step 1. We hash the passphrase using MD5
                // We use the MD5 hash generator as the result is a 128 bit byte array
                // which is a valid length for the TripleDES encoder we use below
#if WINDOWS_PHONE
                MD5Managed HashProvider = new MD5Managed();
#else
                MD5CryptoServiceProvider HashProvider = new MD5CryptoServiceProvider();
#endif
                byte[] TDESKey = HashProvider.ComputeHash(utf8.GetBytes(passphrase));

#if WINDOWS_PHONE
                // Step 2. Create a TripleDES cipher utility object and setup decoder.
                var cipher = Org.BouncyCastle.Security.CipherUtilities.GetCipher("DESede/ECB/PKCS7Padding");
                cipher.Init(false, new Org.BouncyCastle.Crypto.Parameters.DesEdeParameters(TDESKey));

                // Step 3. Convert the input string to a byte[]
                byte[] DataToDecrypt = Convert.FromBase64String(plaintext);

                // Step 4. Decrypt the string
                results = cipher.DoFinal(DataToDecrypt);

                // Step 5. Return the decrypted string in UTF8 format
                return utf8.GetString(results, 0, results.Length);
#else
                // Step 2. Create a new TripleDESCryptoServiceProvider object
                TripleDESCryptoServiceProvider TDESAlgorithm = new TripleDESCryptoServiceProvider();

                // Step 3. Setup the decoder
                TDESAlgorithm.Key = TDESKey;
                TDESAlgorithm.Mode = CipherMode.ECB;
                TDESAlgorithm.Padding = PaddingMode.PKCS7;

                // Step 4. Convert the input string to a byte[]
                byte[] DataToDecrypt = Convert.FromBase64String(plaintext);

                // Step 5. Attempt to decrypt the string
                try
                {
                    ICryptoTransform Decryptor = TDESAlgorithm.CreateDecryptor();
                    results = Decryptor.TransformFinalBlock(DataToDecrypt, 0, DataToDecrypt.Length);
                }
                finally
                {
                    // Clear the TripleDes and Hashprovider services of any sensitive information
                    TDESAlgorithm.Clear();
                    HashProvider.Clear();
                }

                // Step 6. Return the decrypted string in UTF8 format
                return utf8.GetString(results);
#endif
            }
            else
                return string.Empty;
#else
            // TODO: Implement TripleDESEncryption.Decrypt.
            return null;
#endif
        }
    }
}
