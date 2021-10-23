using CommandLine;

namespace BasicEC.Secret.Model.Commands.Keys
{
    [Verb("ls", HelpText = "List all available keys.")]
    public class ListRsaKeysCommand : CommandBase
    {
    }
}
