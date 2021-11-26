using BasicEC.Secret.Commands.Keys;
using CommandLine;

namespace BasicEC.Secret.Console.Commands.Keys
{
    [Verb("ls", HelpText = "List all available keys.")]
    public class ListRsaKeysCommand : ConsoleCommandBase, IListRsaKeysCommand
    {
    }
}
