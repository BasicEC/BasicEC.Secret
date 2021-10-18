using CommandLine;

namespace BasicEC.Secret.Commands.Keys
{
    [Verb("ls", HelpText = "List all available keys.")]
    public class ListRsaKeysCommand : CommandBase
    {
    }
}
