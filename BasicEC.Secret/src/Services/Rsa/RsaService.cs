using System.IO;
using System.Security.Cryptography;

namespace BasicEC.Secret.Services.Rsa
{
    public static class RsaService
    {
        public static void ImportKeyToStore(string name, string filePath)
        {
            var rsa = RsaIOService.ReadKey(filePath);
            RsaIOService.CreateStore(name);
            var file = RsaIOService.GetKeyFile(name, !rsa.PublicOnly);

            if (file.Exists)
            {
                throw new CommandException("Key already exists");
            }

            RsaIOService.WriteKey(rsa, file, !rsa.PublicOnly);
        }

        public static void GenerateRsaKey(string name, int length)
        {
            RsaIOService.CreateStore(name);

            var privateKeyFile = RsaIOService.GetKeyFile(name, true);
            var publicKeyFile = RsaIOService.GetKeyFile(name, false);
            if (privateKeyFile.Exists || publicKeyFile.Exists)
            {
                throw new CommandException($"Key with the specified name ({name}) already exists.");
            }

            var rsa = new RSACryptoServiceProvider(length);
            RsaIOService.WriteKey(rsa, privateKeyFile, true);
            RsaIOService.WriteKey(rsa, publicKeyFile, false);
        }

        // todo improve this code
        public static void Encrypt(string key, string fileName, string @out)
        {
            var rsa = RsaIOService.ReadKey(key, false);
            fileName.CheckFileExists();
            var data = File.ReadAllBytes(fileName);
            var encrypted = rsa.Encrypt(data);
            File.WriteAllBytes(@out, encrypted);
        }

        // todo improve this code
        public static void Decrypt(string key, string fileName, string @out)
        {
            var rsa = RsaIOService.ReadKey(key, true);
            fileName.CheckFileExists();
            var encrypted = File.ReadAllBytes(fileName);
            var data = rsa.Decrypt(encrypted);
            File.WriteAllBytes(@out, data);
        }
    }
}
