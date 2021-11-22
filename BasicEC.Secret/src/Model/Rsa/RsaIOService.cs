using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using BasicEC.Secret.Data;
using BasicEC.Secret.Model.Commands;
using BasicEC.Secret.Model.Extensions;
using Serilog;

namespace BasicEC.Secret.Model.Rsa
{
    // ReSharper disable once InconsistentNaming
    internal static class RsaIOService
    {
        private const string PrivateKeyFileName = "rsa";
        private const string PublicKeyFileName = "rsa.pub";

        private static readonly Lazy<DirectoryInfo> StoreLazy = new(() =>
        {
            const string rsaStoreEnv = "BASIC_RSA_STORE";
            const string defaultRsaStore = "rsa_store";
            var path = Environment.GetEnvironmentVariable(rsaStoreEnv);
            path ??= Path.Combine(Program.RootDir.FullName, defaultRsaStore);
            var dir = new DirectoryInfo(path);
            if (dir.Exists) return dir;

            Log.Logger.Information("Create rsa store since it doesn't exists");
            dir.Create();

            return dir;
        });

        private static DirectoryInfo Store => StoreLazy.Value;

        public static DirectoryInfo CreateStore(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new CommandException("Name can't be empty");
            }

            var keysDir = new DirectoryInfo(Path.Combine(Store.FullName, name));
            if (!keysDir.Exists)
            {
                Log.Logger.Information("Create store for key {Name}", name);
                keysDir.Create();
            }

            return keysDir;
        }

        public static void ListStoredKeys()
        {
            var keys = new List<RsaKeyStoreInfo>();
            foreach (var inner in Store.EnumerateDirectories())
            {
                keys.Add(new RsaKeyStoreInfo
                {
                    Name = inner.Name,
                    HasPrivateKey = GetKeyFile(inner.Name, true).Exists,
                    HasPublicKey = GetKeyFile(inner.Name, false).Exists,
                });
            }

            InteractionService.Show(keys);
        }

        public static void RemoveKey(string name, bool force)
        {
            var keysDir = new DirectoryInfo(Path.Combine(Store.FullName, name));
            if (!keysDir.Exists)
            {
                Log.Logger.Information("Rsa key ({Name}) not found", name);
                return;
            }

            if (!force && !InteractionService.Confirm($"Are you sure you want remove rsa key {name}"))
            {
                return;
            }

            Directory.Delete(keysDir.FullName, true);
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

            rsa.ImportFromPem(pem);

            return rsa;
        }

        public static FileInfo GetKeyFile(string name, bool isPrivate)
        {
            var fileName = isPrivate ? PrivateKeyFileName : PublicKeyFileName;
            return new FileInfo(Path.Combine(Store.FullName, name, fileName));
        }
    }
}
