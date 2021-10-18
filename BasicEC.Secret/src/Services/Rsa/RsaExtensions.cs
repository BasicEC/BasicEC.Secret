using System;
using System.Collections.Generic;
using System.Linq;
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

        public static byte[] Encrypt(this RSACryptoServiceProvider rsa,
                                     byte[] data,
                                     RSAEncryptionPadding padding = null)
        {
            if (rsa == null) throw new ArgumentNullException(nameof(rsa));
            if (data == null) throw new ArgumentNullException(nameof(data));

            padding ??= RSAEncryptionPadding.Pkcs1;
            var batchSize = rsa.KeySize / 8 - PaddingLimitDic[padding];

            return data.Chunks(batchSize).SelectMany(_ => rsa.Encrypt(_, padding)).ToArray();
        }

        public static byte[] Decrypt(this RSACryptoServiceProvider rsa,
                                     byte[] data,
                                     RSAEncryptionPadding padding = null)
        {
            if (rsa == null) throw new ArgumentNullException(nameof(rsa));
            if (data == null) throw new ArgumentNullException(nameof(data));
            if (rsa.PublicOnly) throw new ArgumentException("Cannot decrypt without private key", nameof(rsa));

            padding ??= RSAEncryptionPadding.Pkcs1;
            var batchSize = rsa.KeySize / 8;

            return data.Chunks(batchSize).SelectMany(_ => rsa.Decrypt(_, padding)).ToArray();
        }

        #endregion

        private static IEnumerable<T[]> Chunks<T>(this IReadOnlyCollection<T> values, int chunkSize)
        {
            for (var i = 0; i < values.Count; i += chunkSize)
            {
                var tail = values.Count - i;
                var size = tail > chunkSize ? chunkSize : tail;
                yield return new List<T>(values.Skip(i).Take(size)).ToArray();
            }
        }
    }
}
