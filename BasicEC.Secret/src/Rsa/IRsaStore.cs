using System.Collections.Generic;
using System.Security.Cryptography;

namespace BasicEC.Secret.Rsa
{
    public interface IRsaStore
    {
        void ImportKeyToStore(string name, string filePath);
        void GenerateRsaKey(string name, int length);
        IEnumerable<RsaKeyInfo> ListStoredKeys();
        RSACryptoServiceProvider GetKey(string name, bool isPrivate);
        void RemoveKey(string name);
    }
}
