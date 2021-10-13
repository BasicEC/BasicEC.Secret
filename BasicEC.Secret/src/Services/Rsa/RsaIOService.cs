using System.IO;
using System.Security.Cryptography;
using Serilog;

namespace BasicEC.Secret.Services.Rsa
{
    // ReSharper disable once InconsistentNaming
    internal static class RsaIOService
    {
        private const string PrivateKeyFileName = "rsa";
        private const string PublicKeyFileName = "rsa.pub";
        private const string KeysDirName = "rsa_store";

        public static DirectoryInfo CreateStore(string name)
        {
            // todo get rsa_store from Env
            var store = new DirectoryInfo(Path.Combine(Program.RootDir.FullName, KeysDirName));
            if (!store.Exists)
            {
                // todo: catch exceptions (e.g. lack of permissions)
                Log.Information("Create rsa store since it doesn't exists");
                store.Create();
            }

            if (string.IsNullOrWhiteSpace(name))
            {
                throw new CommandException("Name can't be empty");
            }

            var keysDir = new DirectoryInfo(Path.Combine(store.FullName, name));
            if (!keysDir.Exists)
            {
                Log.Information("Create store for key {Name}", name);
                keysDir.Create();
            }

            return keysDir;
        }

        public static RSACryptoServiceProvider ReadKey(string path)
        {
            var file = new FileInfo(path);
            return ReadKey(file);
        }

        public static RSACryptoServiceProvider ReadKey(string name, bool isPrivate)
        {
            var file = GetKeyFile(name, isPrivate);
            return ReadKey(file);
        }

        public static void WriteKey(RSA rsa, FileInfo file, bool @private)
        {
            var pem = @private ? rsa.ExportPrivateKeyAsPem() : rsa.ExportPublicKeyAsPem();
            using var writer = new StreamWriter(file.OpenWrite());
            writer.Write(pem);
        }

        private static RSACryptoServiceProvider ReadKey(FileInfo file)
        {
            file.CheckExists();
            var rsa = new RSACryptoServiceProvider();
            string pem;
            using (var reader = new StreamReader(file.OpenRead()))
            {
                pem = reader.ReadToEnd();
            }

            // todo catch exceptions
            rsa.ImportFromPem(pem);

            return rsa;
        }

        public static FileInfo GetKeyFile(string name, bool isPrivate)
        {
            var fileName = isPrivate ? PrivateKeyFileName : PublicKeyFileName;
            return new FileInfo(Path.Combine(Program.RootDir.FullName, KeysDirName, name, fileName));
        }
    }
}
