using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using BasicEC.Secret.Exceptions;
using BasicEC.Secret.Extensions;
using Microsoft.Extensions.Logging;

namespace BasicEC.Secret.Rsa
{
    public class LocalRsaStore : IRsaStore
    {
        private const string PrivateKeyFileName = "rsa";
        private const string PublicKeyFileName = "rsa.pub";

        private readonly ILogger _logger;

        private readonly Lazy<DirectoryInfo> _storeLazy;

        private DirectoryInfo Store => _storeLazy.Value;

        public LocalRsaStore(ILoggerProvider loggerProvider)
        {
            _logger = loggerProvider.CreateLogger(nameof(LocalRsaStore));
            _storeLazy = new Lazy<DirectoryInfo>(() =>
            {
                const string rsaStoreEnv = "BASIC_RSA_STORE";
                const string defaultRsaStore = "rsa_store";
                var path = Environment.GetEnvironmentVariable(rsaStoreEnv);
                path ??= Path.Combine(AppContext.BaseDirectory, defaultRsaStore);
                var dir = new DirectoryInfo(path);
                if (dir.Exists) return dir;

                _logger.LogInformation("Create rsa store since it doesn't exists");
                dir.Create();

                return dir;
            });
        }

        public void ImportKeyToStore(string name, string filePath)
        {
            var rsa = ReadKey(new FileInfo(filePath));
            GetOrCreateStore(name);
            var file = GetKeyFile(name, !rsa.PublicOnly);

            if (file.Exists)
            {
                throw new CommandException("Key already exists");
            }

            WriteKey(rsa, file, !rsa.PublicOnly);
        }

        public void GenerateRsaKey(string name, int length)
        {
            GetOrCreateStore(name);

            var privateKeyFile = GetKeyFile(name, true);
            var publicKeyFile = GetKeyFile(name, false);
            if (privateKeyFile.Exists || publicKeyFile.Exists)
            {
                throw new CommandException($"Key with the specified name ({name}) already exists.");
            }

            var rsa = new RSACryptoServiceProvider(length);
            WriteKey(rsa, privateKeyFile, true);
            WriteKey(rsa, publicKeyFile, false);
        }

        public IEnumerable<RsaKeyInfo> ListStoredKeys()
        {
            var keys = new List<RsaKeyInfo>();
            foreach (var inner in Store.EnumerateDirectories())
            {
                keys.Add(new RsaKeyInfo
                {
                    Name = inner.Name,
                    HasPrivateKey = GetKeyFile(inner.Name, true).Exists,
                    HasPublicKey = GetKeyFile(inner.Name, false).Exists,
                });
            }

            return keys;
        }

        public RSACryptoServiceProvider GetKey(string name, bool isPrivate)
        {
            var file = GetKeyFile(name, isPrivate);

            file.CheckExists();
            var rsa = new RSACryptoServiceProvider();
            string pem;
            using (var reader = new StreamReader(file.OpenRead()))
            {
                pem = reader.ReadToEnd();
            }

            rsa.ImportFromPem(pem);
            return rsa;
        }

        public void RemoveKey(string name)
        {
            var keysDir = new DirectoryInfo(Path.Combine(Store.FullName, name));
            if (!keysDir.Exists)
            {
                _logger.LogInformation("Rsa key ({Name}) not found", name);
                return;
            }

            Directory.Delete(keysDir.FullName, true);
        }

        private FileInfo GetKeyFile(string name, bool isPrivate)
        {
            var fileName = isPrivate ? PrivateKeyFileName : PublicKeyFileName;
            return new FileInfo(Path.Combine(Store.FullName, name, fileName));
        }

        private DirectoryInfo GetOrCreateStore(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new CommandException("Name can't be empty");
            }

            var keysDir = new DirectoryInfo(Path.Combine(Store.FullName, name));
            if (!keysDir.Exists)
            {
                _logger.LogInformation("Create store for key {Name}", name);
                keysDir.Create();
            }

            return keysDir;
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

            rsa.ImportFromPem(pem);
            return rsa;
        }

        private static void WriteKey(RSA rsa, FileInfo file, bool @private)
        {
            var pem = @private ? rsa.ExportPrivateKeyAsPem() : rsa.ExportPublicKeyAsPem();
            using var writer = new StreamWriter(file.OpenWrite());
            writer.Write(pem);
        }
    }
}
