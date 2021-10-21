using System.Threading.Tasks;

namespace BasicEC.Secret.Commands
{
    public interface ICommand
    {
        Task ApplyAsync(ICommandExecutor executor);
    }

    public class CommandBase : ICommand
    {
        public Task ApplyAsync(ICommandExecutor executor) => executor.ExecuteAsync(this);
    }
}
