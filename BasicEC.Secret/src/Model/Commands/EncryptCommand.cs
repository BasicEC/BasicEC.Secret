using BasicEC.Secret.Model.Commands.Keys;
using CommandLine;
using JetBrains.Annotations;

namespace BasicEC.Secret.Model.Commands
{
    [Verb("encrypt", HelpText = "Encrypt file.")]
    public class EncryptCommand : CommandBase
    {
        [Option('o', "out", Required = true, HelpText = "Output file name.")]
        public string Output { get; [UsedImplicitly] set; }

        [Option('f', "file", Required = true, HelpText = "File to encrypt.")]
        public string File { get; [UsedImplicitly] set; }

        [Option('k', "key", Default = GenRsaKeyCommand.DefaultKeyName, HelpText = "Name of the key that will be used.")]
        public string Key { get; [UsedImplicitly] set; }

        [Option('w', "workers", Default = 4, HelpText = "Number threads to perform encryption")]
        public int Workers { get; [UsedImplicitly] set; }
    }
}
