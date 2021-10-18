using CommandLine;

namespace BasicEC.Secret.Commands.Keys
{
    [Verb("rm", HelpText = "Remove RSA key.")]
    public class RemoveRsaKeyCommand : CommandBase
    {
        [Option('n', "name", Default = "default", HelpText = "Key name.")]
        public string Name { get; set; }

        [Option('f', "force", Default = false, HelpText = "Remove key without confirmation.")]
        public bool Force { get; set; }
    }
}
