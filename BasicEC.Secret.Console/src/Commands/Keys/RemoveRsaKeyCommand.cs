using BasicEC.Secret.Commands.Keys;
using CommandLine;

namespace BasicEC.Secret.Console.Commands.Keys
{
    [Verb("rm", HelpText = "Remove RSA key.")]
    public class RemoveRsaKeyCommand : ConsoleCommandBase, IRemoveRsaKeyCommand
    {
        [Option('n', "name", Default = "default", HelpText = "Key name.")]
        public string Name { get; set; }

        [Option('f', "force", Default = false, HelpText = "Remove key without confirmation.")]
        public bool Force { get; set; }
    }
}
