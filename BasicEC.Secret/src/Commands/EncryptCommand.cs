using BasicEC.Secret.Commands.Keys;
using CommandLine;
using JetBrains.Annotations;

namespace BasicEC.Secret.Commands
{
    [Verb("encrypt", HelpText = "Encrypt file.")]
    public class EncryptCommand : CommandBase
    {
        [Option('o', "out", Required = true, HelpText = "Output file name.")]
        public string Output { get; [UsedImplicitly] set; }

        [Option('f', "file", Required = true, HelpText = "File to encrypt.")]
        public string File { get; [UsedImplicitly] set; }

        [Option('k', "key", Default = GenRsaKeyCommand.DefaultKeyName,
            HelpText = "Name of the key that will be used.")]
        public string Key { get; [UsedImplicitly] set; }
    }
}
