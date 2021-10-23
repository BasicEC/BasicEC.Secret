using BasicEC.Secret.Model.Extensions;
using CommandLine;

namespace BasicEC.Secret.Model.Commands.Keys
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
