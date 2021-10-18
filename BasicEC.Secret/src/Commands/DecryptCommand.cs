using BasicEC.Secret.Commands.Keys;
using CommandLine;
using JetBrains.Annotations;

namespace BasicEC.Secret.Commands
{
    [Verb("decrypt", HelpText = "Decrypt file.")]
    public class DecryptCommand : CommandBase
    {
        [Option('o', "out", Required = true, HelpText = "Output file name.")]
        public string Output { get; [UsedImplicitly] set; }

        [Option('f', "file", Required = true, HelpText = "File to decrypt.")]
        public string File { get; [UsedImplicitly] set; }

        [Option('k', "key", Default = GenRsaKeyCommand.DefaultKeyName,
            HelpText = "Name of the key that will be used. (default if not specified)")]
        public string Key { get; [UsedImplicitly] set; }
    }
}
