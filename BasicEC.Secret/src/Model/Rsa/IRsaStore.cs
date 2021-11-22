using System.Security.Cryptography;

namespace BasicEC.Secret.Model.Rsa
{
    public interface IRsaStore
    {
        void ImportKeyToStore(string name, string filePath);
        void GenerateRsaKey(string name, int length);
        void ListStoredKeys();
        RSACryptoServiceProvider GetKey(string name, bool isPrivate);
        void RemoveKey(string name, bool force);
    }
}
