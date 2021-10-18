using BasicEC.Secret.Commands;
using BasicEC.Secret.Commands.Keys;
using BasicEC.Secret.Services.Rsa;

namespace BasicEC.Secret
{
    public interface ICommandExecutor
    {
        void Execute(ICommand cmd);
    }

    public class CommandExecutor : ICommandExecutor
    {
        public void Execute(ICommand command)
        {
            switch (command)
            {
                case DecryptCommand cmd:
                    RsaService.Decrypt(cmd.Key, cmd.File, cmd.Output);
                    break;
                case EncryptCommand cmd:
                    RsaService.Encrypt(cmd.Key, cmd.File, cmd.Output);
                    break;
                case GenRsaKeyCommand cmd:
                    RsaService.GenerateRsaKey(cmd.Name, cmd.Length);
                    break;
                case ImportRsaKeyCommand cmd:
                    RsaService.ImportKeyToStore(cmd.Name, cmd.Input);
                    break;
            }
        }
    }
}
