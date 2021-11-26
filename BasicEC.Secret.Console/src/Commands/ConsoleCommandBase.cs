using System.Threading.Tasks;
using BasicEC.Secret.Commands;

namespace BasicEC.Secret.Console.Commands
{
    public class ConsoleCommandBase : ICommand
    {
        public Task ApplyAsync(ICommandExecutor executor) => executor.ExecuteAsync(this);
    }
}
