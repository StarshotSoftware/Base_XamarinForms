using System;
using System.IO;
#if NETFX_CORE
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;
using System.Runtime.InteropServices.WindowsRuntime;
#else
using System.Security.Cryptography;
#endif
using System.Text;
using System.Collections.Generic;

namespace BaseStarShot.Security.RNCryptor
{
    public enum Schema : short { V0, V1, V2, V3 };
    public enum AesMode : short { CTR, CBC };
    public enum Pbkdf2Prf : short { SHA1 };
    public enum HmacAlgorithm : short { SHA1, SHA256 };
    public enum Algorithm : short { AES };
    public enum Options : short { V0, V1 };

    public struct PayloadComponents
    {
        public byte[] schema;
        public byte[] options;
        public byte[] salt;
        public byte[] hmacSalt;
        public byte[] iv;
        public int headerLength;
        public byte[] hmac;
        public byte[] ciphertext;
    };

    abstract public class Cryptor
    {
        protected AesMode aesMode;
        protected Options options;
        protected bool hmac_includesHeader;
        protected bool hmac_includesPadding;
        protected HmacAlgorithm hmac_algorithm;

        protected const Algorithm algorithm = Algorithm.AES;
        protected const short saltLength = 8;
        protected const short ivLength = 16;
        protected const Pbkdf2Prf pbkdf2_prf = Pbkdf2Prf.SHA1;
        protected const int pbkdf2_iterations = 10000;
        protected const short pbkdf2_keyLength = 32;
        protected const short hmac_length = 32;

#if NETFX_CORE
        /// <summary>
        ///Gets or sets the Encoding
        /// </summary>
        public BinaryStringEncoding TextEncoding { set; get; }

        public Cryptor()
        {
            // set default encoding for UTF8
            TextEncoding = BinaryStringEncoding.Utf8;
        }
#else
        /// <summary>
        ///Gets or sets the Encoding
        /// </summary>
        public Encoding TextEncoding { set; get; }

        public Cryptor()
        {
            // set default encoding for UTF8
            TextEncoding = Encoding.UTF8;
        }
#endif

        protected void configureSettings(Schema schemaVersion)
        {
            switch (schemaVersion)
            {
                case Schema.V0:
                    aesMode = AesMode.CTR;
                    options = Options.V0;
                    hmac_includesHeader = false;
                    hmac_includesPadding = true;
                    hmac_algorithm = HmacAlgorithm.SHA1;
                    break;

                case Schema.V1:
                    aesMode = AesMode.CBC;
                    options = Options.V1;
                    hmac_includesHeader = false;
                    hmac_includesPadding = false;
                    hmac_algorithm = HmacAlgorithm.SHA256;
                    break;

                case Schema.V2:
                case Schema.V3:
                    aesMode = AesMode.CBC;
                    options = Options.V1;
                    hmac_includesHeader = true;
                    hmac_includesPadding = false;
                    hmac_algorithm = HmacAlgorithm.SHA256;
                    break;
            }
        }

        protected byte[] generateHmac(PayloadComponents components, string password)
        {
            List<byte> hmacMessage = new List<byte>();
            if (this.hmac_includesHeader)
            {
                hmacMessage.AddRange(this.assembleHeader(components));
            }
            hmacMessage.AddRange(components.ciphertext);

            byte[] key = this.generateKey(components.hmacSalt, password);

            List<byte> hmac = new List<byte>();
#if NETFX_CORE
            string hmacAlgo = null;
            switch (this.hmac_algorithm)
            {
                case HmacAlgorithm.SHA1:
                    hmacAlgo = MacAlgorithmNames.HmacSha1;
                    break;
                case HmacAlgorithm.SHA256:
                    hmacAlgo = MacAlgorithmNames.HmacSha256;
                    break;
            }
            var crypt = MacAlgorithmProvider.OpenAlgorithm(hmacAlgo);
            var keyBuffer = key.AsBuffer();
            var cryptographicKey = crypt.CreateKey(keyBuffer);
            hmac.AddRange(CryptographicEngine.Sign(cryptographicKey, hmacMessage.ToArray().AsBuffer()).ToArray());
#else
            HMAC hmacAlgo = null;
            switch (this.hmac_algorithm)
            {
                case HmacAlgorithm.SHA1:
                    hmacAlgo = new HMACSHA1(key);
                    break;

                case HmacAlgorithm.SHA256:
                    hmacAlgo = new HMACSHA256(key);
                    break;
            }
            hmac.AddRange(hmacAlgo.ComputeHash(hmacMessage.ToArray()));
#endif

            if (this.hmac_includesPadding)
            {
                for (int i = hmac.Count; i < Cryptor.hmac_length; i++)
                {
                    hmac.Add(0x00);
                }
            }

            return hmac.ToArray();
        }

        protected byte[] assembleHeader(PayloadComponents components)
        {
            List<byte> headerBytes = new List<byte>();
            headerBytes.AddRange(components.schema);
            headerBytes.AddRange(components.options);
            headerBytes.AddRange(components.salt);
            headerBytes.AddRange(components.hmacSalt);
            headerBytes.AddRange(components.iv);

            return headerBytes.ToArray();
        }

        protected byte[] generateKey(byte[] salt, string password)
        {
#if !NETFX_CORE
            var pbkdf2 = new Rfc2898DeriveBytes(password, salt, Cryptor.pbkdf2_iterations);
            return pbkdf2.GetBytes(Cryptor.pbkdf2_keyLength);
#else
            KeyDerivationAlgorithmProvider keyDerivationProvider = KeyDerivationAlgorithmProvider.OpenAlgorithm(KeyDerivationAlgorithmNames.Pbkdf2Sha1);
            KeyDerivationParameters pbkdf2Parms = KeyDerivationParameters.BuildForPbkdf2(salt.AsBuffer(), Cryptor.pbkdf2_iterations);

            var pwBuffer = CryptographicBuffer.ConvertStringToBinary(password, TextEncoding);
            CryptographicKey keyOriginal = keyDerivationProvider.CreateKey(pwBuffer);
            var keyMaterial = CryptographicEngine.DeriveKeyMaterial(keyOriginal, pbkdf2Parms, (uint)Cryptor.pbkdf2_keyLength);
            return keyMaterial.ToArray();
#endif
        }

        protected byte[] encryptAesCtrLittleEndianNoPadding(byte[] plaintextBytes, byte[] key, byte[] iv)
        {
            byte[] counter = this.computeAesCtrLittleEndianCounter(plaintextBytes.Length, iv);
            byte[] encrypted = this.encryptAesEcbNoPadding(counter, key);
            return this.bitwiseXOR(plaintextBytes, encrypted);
        }

        private byte[] computeAesCtrLittleEndianCounter(int payloadLength, byte[] iv)
        {
            byte[] incrementedIv = new byte[iv.Length];
            iv.CopyTo(incrementedIv, 0);

            int blockCount = (int)Math.Ceiling((decimal)payloadLength / (decimal)iv.Length);

            List<byte> counter = new List<byte>();

            for (int i = 0; i < blockCount; ++i)
            {
                counter.AddRange(incrementedIv);

                // Yes, the next line only ever increments the first character
                // of the counter string, ignoring overflow conditions.  This
                // matches CommonCrypto's behavior!
                incrementedIv[0]++;
            }

            return counter.ToArray();
        }

        private byte[] encryptAesEcbNoPadding(byte[] plaintext, byte[] key)
        {
#if !NETFX_CORE
            byte[] encrypted;

#if !WINDOWS_PHONE
            var aes = Aes.Create();
            aes.Mode = CipherMode.ECB;
            aes.Padding = PaddingMode.None;
#else
            var aes = new AesManaged();
#endif
            using (aes)
            {
                var encryptor = aes.CreateEncryptor(key, null);

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
            SymmetricKeyAlgorithmProvider aesKeyProvider = SymmetricKeyAlgorithmProvider.OpenAlgorithm(SymmetricAlgorithmNames.AesEcb);
            var cryptographicKey = aesKeyProvider.CreateSymmetricKey(key.AsBuffer());
            
            return CryptographicEngine.Encrypt(cryptographicKey, plaintext.AsBuffer(), null).ToArray();
#endif
        }

        private byte[] bitwiseXOR(byte[] first, byte[] second)
        {
            byte[] output = new byte[first.Length];
            ulong klen = (ulong)second.Length;
            ulong vlen = (ulong)first.Length;
            ulong k = 0;
            ulong v = 0;
            for (; v < vlen; v++)
            {
                output[v] = (byte)(first[v] ^ second[k]);
                k = (++k < klen ? k : 0);
            }
            return output;
        }

        protected string hex_encode(byte[] input)
        {
            string hex = "";
            foreach (byte c in input)
            {
                hex += String.Format("{0:x2}", c);
            }
            return hex;
        }

    }
}

