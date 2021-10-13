using CommandLine;
using JetBrains.Annotations;

namespace BasicEC.Secret.Commands
{
    [Verb("import", HelpText = "Import rsa key.")]
    public class ImportRsaKeyCommand : CommandBase
    {
        [Option('f', "file", Required = true, HelpText = "Path to file with rsa key.")]
        public string Input { get; [UsedImplicitly] set; }

        [Option('n', "name", Default = "default", HelpText = "Key name.")]
        public string Name { get; [UsedImplicitly] set; }
    }
}
