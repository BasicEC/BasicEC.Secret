namespace BasicEC.Secret.Commands
{
    public interface ICommand
    {
        void Apply(ICommandExecutor executor);
    }

    public class CommandBase : ICommand
    {
        public void Apply(ICommandExecutor executor) => executor.Execute(this);
    }
}
