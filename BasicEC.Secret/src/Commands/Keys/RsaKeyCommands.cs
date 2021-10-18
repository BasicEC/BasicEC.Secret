using BasicEC.Secret.Services;
using CommandLine;

namespace BasicEC.Secret.Commands.Keys
{
    [Verb("key", HelpText = "Key management commands.")]
    [SubVerbs(
        typeof(ImportRsaKeyCommand),
        typeof(GenRsaKeyCommand),
        typeof(ListRsaKeysCommand),
        typeof(RemoveRsaKeyCommand)
    )]
    public class RsaKeyCommands
    {
    }
}
