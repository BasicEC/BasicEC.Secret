using System.Threading.Tasks;
using BasicEC.Secret.Model.Commands.Keys;
using BasicEC.Secret.Model.ProgressBar;
using BasicEC.Secret.Model.Rsa;
using Microsoft.Extensions.DependencyInjection;

namespace BasicEC.Secret.Model.Commands
{
    public interface ICommandExecutor
    {
        Task ExecuteAsync(ICommand cmd);
    }

    public class CommandExecutor : ICommandExecutor
    {
        private readonly ServiceProvider _serviceProvider;

        public CommandExecutor(ServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task ExecuteAsync(ICommand command)
        {
            var store = _serviceProvider.GetRequiredService<IRsaStore>();
            var statusWriter = _serviceProvider.GetRequiredService<IProgressStatusWriter>();
            switch (command)
            {
                case DecryptCommand cmd:
                {
                    var rsa = store.GetKey(cmd.Key, true);
                    var conveyor = new FileDataConveyor(cmd.File, cmd.Output, rsa.GetEncryptedDataLength(), _ => rsa.Decrypt(_), cmd.Workers);
                    using var _ = conveyor.SubscribeOnProgressStatus(statusWriter);
                    await conveyor.ProcessDataAsync();
                    break;
                }
                case EncryptCommand cmd:
                {
                    var rsa = store.GetKey(cmd.Key, false);
                    var conveyor = new FileDataConveyor(cmd.File, cmd.Output, rsa.GetMaxDataLength(), _ => rsa.Encrypt(_), cmd.Workers);
                    using var _ = conveyor.SubscribeOnProgressStatus(statusWriter);
                    await conveyor.ProcessDataAsync();
                    break;
                }
                case GenRsaKeyCommand cmd:
                {
                    store.GenerateRsaKey(cmd.Name, cmd.Length);
                    break;
                }
                case ImportRsaKeyCommand cmd:
                {
                    store.ImportKeyToStore(cmd.Name, cmd.Input);
                    break;
                }
                case ListRsaKeysCommand _:
                {
                    store.ListStoredKeys();
                    break;
                }
                case RemoveRsaKeyCommand cmd:
                {
                    store.RemoveKey(cmd.Name, cmd.Force);
                    break;
                }
            }
        }
    }
}
