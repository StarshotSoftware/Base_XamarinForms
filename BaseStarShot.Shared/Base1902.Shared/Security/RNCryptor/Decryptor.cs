using System;
using System.IO;
using System.Text;
using Base1902.Security;
#if !NETFX_CORE
using System.Security.Cryptography;
#else
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;
using System.Runtime.InteropServices.WindowsRuntime;
#endif
using System.Collections.Generic;

namespace BaseStarShot.Security.RNCryptor
{
    public class Decryptor : Cryptor, IDecryptor
    {
        public string Decrypt(string encryptedBase64, string password)
        {
            PayloadComponents components = this.unpackEncryptedBase64Data(encryptedBase64);

            if (!this.hmacIsValid(components, password))
            {
                return "";
            }

            byte[] key = this.generateKey(components.salt, password);

            byte[] plaintextBytes = new byte[0];

            switch (this.aesMode)
            {
                case AesMode.CTR:
                    // Yes, we are "encrypting" here.  CTR uses the same code in both directions.
                    plaintextBytes = this.encryptAesCtrLittleEndianNoPadding(components.ciphertext, key, components.iv);
                    break;

                case AesMode.CBC:
                    plaintextBytes = this.decryptAesCbcPkcs7(components.ciphertext, key, components.iv);
                    break;
            }

            return Encoding.UTF8.GetString(plaintextBytes, 0, plaintextBytes.Length);
        }

        private byte[] decryptAesCbcPkcs7(byte[] encrypted, byte[] key, byte[] iv)
        {
#if !NETFX_CORE
#if !WINDOWS_PHONE
            var aes = Aes.Create();
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;
#else
            var aes = new AesManaged();
#endif
            using (aes)
            {
                var decryptor = aes.CreateDecryptor(key, iv);

                string plaintext;
                using (MemoryStream msDecrypt = new MemoryStream(encrypted))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            plaintext = srDecrypt.ReadToEnd();
                        }
                    }
                }
                return TextEncoding.GetBytes(plaintext);
            }
#else
            SymmetricKeyAlgorithmProvider aesKeyProvider = SymmetricKeyAlgorithmProvider.OpenAlgorithm(SymmetricAlgorithmNames.AesCbcPkcs7);
            var cryptographicKey = aesKeyProvider.CreateSymmetricKey(key.AsBuffer());

            return CryptographicEngine.Decrypt(cryptographicKey, encrypted.AsBuffer(), iv.AsBuffer()).ToArray();
#endif
        }

        private PayloadComponents unpackEncryptedBase64Data(string encryptedBase64)
        {
            List<byte> binaryBytes = new List<byte>();
            binaryBytes.AddRange(Convert.FromBase64String(encryptedBase64));

            PayloadComponents components;
            int offset = 0;

            components.schema = binaryBytes.GetRange(0, 1).ToArray();
            offset++;

            this.configureSettings((Schema)binaryBytes[0]);

            components.options = binaryBytes.GetRange(1, 1).ToArray();
            offset++;

            components.salt = binaryBytes.GetRange(offset, Cryptor.saltLength).ToArray();
            offset += components.salt.Length;

            components.hmacSalt = binaryBytes.GetRange(offset, Cryptor.saltLength).ToArray();
            offset += components.hmacSalt.Length;

            components.iv = binaryBytes.GetRange(offset, Cryptor.ivLength).ToArray();
            offset += components.iv.Length;

            components.headerLength = offset;

            components.ciphertext = binaryBytes.GetRange(offset, binaryBytes.Count - Cryptor.hmac_length - components.headerLength).ToArray();
            offset += components.ciphertext.Length;

            components.hmac = binaryBytes.GetRange(offset, Cryptor.hmac_length).ToArray();

            return components;

        }

        private bool hmacIsValid(PayloadComponents components, string password)
        {
            byte[] generatedHmac = this.generateHmac(components, password);

            if (generatedHmac.Length != components.hmac.Length)
            {
                return false;
            }

            for (int i = 0; i < components.hmac.Length; i++)
            {
                if (generatedHmac[i] != components.hmac[i])
                {
                    return false;
                }
            }
            return true;
        }

    }
}

