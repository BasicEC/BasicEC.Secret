using System.Threading.Tasks;
using BasicEC.Secret.Commands;
using BasicEC.Secret.Commands.Keys;
using BasicEC.Secret.Services.Rsa;

namespace BasicEC.Secret
{
    public interface ICommandExecutor
    {
        Task ExecuteAsync(ICommand cmd);
    }

    public class CommandExecutor : ICommandExecutor
    {
        public async Task ExecuteAsync(ICommand command)
        {
            switch (command)
            {
                case DecryptCommand cmd:
                    await RsaService.DecryptAsync(cmd.Key, cmd.File, cmd.Output, cmd.Workers);
                    break;
                case EncryptCommand cmd:
                    await RsaService.EncryptAsync(cmd.Key, cmd.File, cmd.Output, cmd.Workers);
                    break;
                case GenRsaKeyCommand cmd:
                    RsaService.GenerateRsaKey(cmd.Name, cmd.Length);
                    break;
                case ImportRsaKeyCommand cmd:
                    RsaService.ImportKeyToStore(cmd.Name, cmd.Input);
                    break;
                case ListRsaKeysCommand _:
                    RsaService.ListStoredKeys();
                    break;
                case RemoveRsaKeyCommand cmd:
                    RsaService.RemoveKey(cmd.Name, cmd.Force);
                    break;
            }
        }
    }
}
