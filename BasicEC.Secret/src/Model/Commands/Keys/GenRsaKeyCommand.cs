using CommandLine;
using JetBrains.Annotations;

namespace BasicEC.Secret.Model.Commands.Keys
{
    [Verb("gen", HelpText = "Generate rsa key pair.")]
    public class GenRsaKeyCommand : CommandBase
    {
        public const string DefaultKeyName = "default";

        [Option('l', "length", Default = 1024, HelpText = "RSA key length.")]
        public int Length { get; [UsedImplicitly] set; }

        [Option('n', "name", Default = DefaultKeyName, HelpText = "Key name.")]
        public string Name { get; [UsedImplicitly] set; }
    }
}