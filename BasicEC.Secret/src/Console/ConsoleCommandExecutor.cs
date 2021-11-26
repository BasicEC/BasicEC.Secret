using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BasicEC.Secret.Commands;
using BasicEC.Secret.Commands.Keys;
using BasicEC.Secret.Extensions;
using BasicEC.Secret.Rsa;

namespace BasicEC.Secret.Console
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
                System.Console.CursorVisible = false;
                await DispatchAsync(command);
            }
            finally
            {
                System.Console.CursorVisible = true;
            }
        }

        private async Task DispatchAsync(ICommand command)
        {
            switch (command)
            {
                case IDecryptCommand cmd:
                {
                    var file = cmd.File.CheckFileExists();
                    await using var input = file.OpenRead();
                    await using var output = new FileInfo(cmd.Output).Open(FileMode.Create, FileAccess.Write);
                    await _rsaService.DecryptAsync(cmd.Key, input, output, cmd.Workers);
                    break;
                }
                case IEncryptCommand cmd:
                {
                    var file = cmd.File.CheckFileExists();
                    await using var input = file.OpenRead();
                    await using var output = new FileInfo(cmd.Output).Open(FileMode.Create, FileAccess.Write);
                    await _rsaService.EncryptAsync(cmd.Key, input, output, cmd.Workers);
                    break;
                }
                case IGenRsaKeyCommand cmd:
                {
                    _store.GenerateRsaKey(cmd.Name, cmd.Length);
                    break;
                }
                case IImportRsaKeyCommand cmd:
                {
                    _store.ImportKeyToStore(cmd.Name, cmd.Input);
                    break;
                }
                case IRemoveRsaKeyCommand cmd:
                {
                    if (!cmd.Force && !Confirm($"Are you sure you want remove rsa key {cmd.Name}"))
                    {
                        return;
                    }

                    _store.RemoveKey(cmd.Name);
                    break;
                }
                case IListRsaKeysCommand _:
                {
                    System.Console.WriteLine(string.Join('\n', _store.ListStoredKeys().Select(ShowRsaKeyInfo)));
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

        private static bool Confirm(string question)
        {
            System.Console.WriteLine($"{question} (yes/no)?");
            while (true)
            {
                var answer = System.Console.ReadLine();
                if ("yes".Equals(answer)) return true;
                if ("no".Equals(answer)) return false;
                System.Console.WriteLine("Please type 'yes' or 'no':");
            }
        }
    }
}
