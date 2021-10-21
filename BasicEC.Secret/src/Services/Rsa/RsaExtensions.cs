using System;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace BasicEC.Secret.Services.Rsa
{
    internal static class RsaExtensions
    {
        #region Export Utils

        public static string ExportPrivateKeyAsPem(this RSA rsa)
        {
            var key = rsa.ExportRSAPrivateKey();
            var pem = PemEncoding.Write("RSA PRIVATE KEY", key);
            return new string(pem);
        }

        public static string ExportPublicKeyAsPem(this RSA rsa)
        {
            var key = rsa.ExportRSAPublicKey();
            var pem = PemEncoding.Write("RSA PUBLIC KEY", key);
            return new string(pem);
        }

        #endregion

        #region Encrypt/Decrypt Utils

        private static readonly Dictionary<RSAEncryptionPadding, int> PaddingLimitDic =
            new()
            {
                [RSAEncryptionPadding.Pkcs1] = 11,
                [RSAEncryptionPadding.OaepSHA1] = 42,
            };

        public static int GetMaxDataLength(this RSACryptoServiceProvider rsa, RSAEncryptionPadding padding = null)
        {
            padding ??= RSAEncryptionPadding.Pkcs1;
            var batchSize = rsa.KeySize / 8 - PaddingLimitDic[padding];
            return batchSize;
        }

        public static int GetEncryptedDataLength(this RSACryptoServiceProvider rsa, RSAEncryptionPadding padding = null)
        {
            return rsa.KeySize / 8;
        }

        public static byte[] Encrypt(this RSACryptoServiceProvider rsa,
                                     byte[] data,
                                     RSAEncryptionPadding padding = null)
        {
            if (rsa == null) throw new ArgumentNullException(nameof(rsa));
            if (data == null) throw new ArgumentNullException(nameof(data));

            padding ??= RSAEncryptionPadding.Pkcs1;
            return rsa.Encrypt(data, padding);
        }

        public static byte[] Decrypt(this RSACryptoServiceProvider rsa,
                                     byte[] data,
                                     RSAEncryptionPadding padding = null)
        {
            if (rsa == null) throw new ArgumentNullException(nameof(rsa));
            if (data == null) throw new ArgumentNullException(nameof(data));
            if (rsa.PublicOnly) throw new ArgumentException("Cannot decrypt without private key", nameof(rsa));

            padding ??= RSAEncryptionPadding.Pkcs1;

            return rsa.Decrypt(data, padding);
        }

        #endregion
    }
}
