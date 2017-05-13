using System;
using System.IO;
using System.Collections.Generic;
using Base1902.Security;
#if !NETFX_CORE
using System.Security.Cryptography;
#else
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;
using System.Runtime.InteropServices.WindowsRuntime;
#endif

namespace BaseStarShot.Security.RNCryptor
{
    public class Encryptor : Cryptor, IEncryptor
    {
        private Schema defaultSchemaVersion = Schema.V2;

        public string Encrypt(string plaintext, string password)
        {
            return this.Encrypt(plaintext, password, this.defaultSchemaVersion);
        }

        public string Encrypt(string plaintext, string password, Schema schemaVersion)
        {
            this.configureSettings(schemaVersion);

#if !NETFX_CORE
            byte[] plaintextBytes = TextEncoding.GetBytes(plaintext);
#else
            byte[] plaintextBytes = CryptographicBuffer.ConvertStringToBinary(plaintext, TextEncoding).ToArray();
#endif
            
            PayloadComponents components = new PayloadComponents();
            components.schema = new byte[] { (byte)schemaVersion };
            components.options = new byte[] { (byte)this.options };
            components.salt = this.generateRandomBytes(Cryptor.saltLength);
            components.hmacSalt = this.generateRandomBytes(Cryptor.saltLength);
            components.iv = this.generateRandomBytes(Cryptor.ivLength);

            byte[] key = this.generateKey(components.salt, password);

            switch (this.aesMode)
            {
                case AesMode.CTR:
                    components.ciphertext = this.encryptAesCtrLittleEndianNoPadding(plaintextBytes, key, components.iv);
                    break;

                case AesMode.CBC:
                    components.ciphertext = this.encryptAesCbcPkcs7(plaintextBytes, key, components.iv);
                    break;
            }

            components.hmac = this.generateHmac(components, password);

            List<byte> binaryBytes = new List<byte>();
            binaryBytes.AddRange(this.assembleHeader(components));
            binaryBytes.AddRange(components.ciphertext);
            binaryBytes.AddRange(components.hmac);

            return Convert.ToBase64String(binaryBytes.ToArray());
        }

        private byte[] encryptAesCbcPkcs7(byte[] plaintext, byte[] key, byte[] iv)
        {
#if !NETFX_CORE
#if !WINDOWS_PHONE
            var aes = Aes.Create();
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;
#else
            var aes = new AesManaged();
#endif
            var encryptor = aes.CreateEncryptor(key, iv);

            byte[] encrypted;
            using (aes)
            {
                using (var ms = new MemoryStream())
                {
                    using (var cs1 = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                    {
                        cs1.Write(plaintext, 0, plaintext.Length);
                    }

                    encrypted = ms.ToArray();
                }
            }

            return encrypted;
#else
            SymmetricKeyAlgorithmProvider aesKeyProvider = SymmetricKeyAlgorithmProvider.OpenAlgorithm(SymmetricAlgorithmNames.AesCbcPkcs7);
            var cryptographicKey = aesKeyProvider.CreateSymmetricKey(key.AsBuffer());

            return CryptographicEngine.Encrypt(cryptographicKey, plaintext.AsBuffer(), iv.AsBuffer()).ToArray();
#endif
        }

        private byte[] generateRandomBytes(int length)
        {
#if !NETFX_CORE
            byte[] randomBytes = new byte[length];
            var rng = new RNGCryptoServiceProvider();
            rng.GetBytes(randomBytes);

            return randomBytes;
#else
            var buffer = CryptographicBuffer.GenerateRandom((uint)length);
            return buffer.ToArray();
#endif
        }
    }
}

