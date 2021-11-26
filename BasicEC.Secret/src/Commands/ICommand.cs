using System.Threading.Tasks;
using BasicEC.Secret.Console;

namespace BasicEC.Secret.Commands
{
    public interface ICommand
    {
        Task ApplyAsync(ICommandExecutor executor);
    }
}
