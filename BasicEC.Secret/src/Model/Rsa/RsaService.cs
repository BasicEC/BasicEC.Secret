using System.Security.Cryptography;
using System.Threading.Tasks;
using BasicEC.Secret.Model.Commands;
using BasicEC.Secret.Model.ProgressBar;

namespace BasicEC.Secret.Model.Rsa
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

        public static void ListStoredKeys() { RsaIOService.ListStoredKeys(); }

        public static void RemoveKey(string name, bool force) { RsaIOService.RemoveKey(name, force); }

        public static async Task EncryptAsync(string key, string input, string output, int workers)
        {
            var rsa = RsaIOService.ReadKey(key, false);
            var conveyor = new FileDataConveyor(input, output, rsa.GetMaxDataLength(), _ => rsa.Encrypt(_), workers);
            using var writer = new ConsoleProgressStatusWriter(conveyor);
            await conveyor.ProcessDataAsync();
        }

        public static async Task DecryptAsync(string key, string input, string output, int workers)
        {
            var rsa = RsaIOService.ReadKey(key, true);
            var conveyor = new FileDataConveyor(input, output, rsa.GetEncryptedDataLength(), _ => rsa.Decrypt(_), workers);
            using var writer = new ConsoleProgressStatusWriter(conveyor);
            await conveyor.ProcessDataAsync();
        }
    }
}
