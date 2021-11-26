using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BasicEC.Secret.Model.Commands.Keys;
using BasicEC.Secret.Model.Extensions;
using BasicEC.Secret.Model.Rsa;

namespace BasicEC.Secret.Model.Commands
{
    public interface ICommandExecutor
    {
        Task ExecuteAsync(ICommand cmd);
    }

    public class ConsoleCommandExecutor : ICommandExecutor
    {
        private readonly IRsaStore _store;
        private readonly IRsaService _rsaService;

        public ConsoleCommandExecutor(IRsaStore store, IRsaService rsaService)
        {
            _store = store;
            _rsaService = rsaService;
        }

        public async Task ExecuteAsync(ICommand command)
        {
            try
            {
                Console.CursorVisible = false;
                await DispatchAsync(command);
            }
            finally
            {
                Console.CursorVisible = true;
            }
        }

        private async Task DispatchAsync(ICommand command)
        {
            switch (command)
            {
                case DecryptCommand cmd:
                {
                    var file = cmd.File.CheckFileExists();
                    await using var input = file.OpenRead();
                    await using var output = new FileInfo(cmd.Output).Open(FileMode.Create, FileAccess.Write);
                    await _rsaService.DecryptAsync(cmd.Key, input, output, cmd.Workers);
                    break;
                }
                case EncryptCommand cmd:
                {
                    var file = cmd.File.CheckFileExists();
                    await using var input = file.OpenRead();
                    await using var output = new FileInfo(cmd.Output).Open(FileMode.Create, FileAccess.Write);
                    await _rsaService.EncryptAsync(cmd.Key, input, output, cmd.Workers);
                    break;
                }
                case GenRsaKeyCommand cmd:
                {
                    _store.GenerateRsaKey(cmd.Name, cmd.Length);
                    break;
                }
                case ImportRsaKeyCommand cmd:
                {
                    _store.ImportKeyToStore(cmd.Name, cmd.Input);
                    break;
                }
                case RemoveRsaKeyCommand cmd:
                {
                    _store.RemoveKey(cmd.Name, cmd.Force);
                    break;
                }
                case ListRsaKeysCommand _:
                {
                    Console.WriteLine(string.Join('\n', _store.ListStoredKeys().Select(ShowRsaKeyInfo)));
                    break;
                }
            }
        }

        private static string ShowRsaKeyInfo(RsaKeyInfo info)
        {
            var builder = new StringBuilder();
            builder.Append(info.Name);
            if (!info.HasPrivateKey && !info.HasPublicKey)
            {
                builder.Append(": key files are missing");
            }
            else if (!info.HasPrivateKey)
            {
                builder.Append(": public key only");
            }
            else if (!info.HasPublicKey)
            {
                builder.Append(": private key only");
            }

            return builder.ToString();
        }
    }
}
