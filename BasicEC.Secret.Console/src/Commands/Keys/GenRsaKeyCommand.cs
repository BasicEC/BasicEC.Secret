using BasicEC.Secret.Commands.Keys;
using CommandLine;

namespace BasicEC.Secret.Console.Commands.Keys
{
    [Verb("gen", HelpText = "Generate rsa key pair.")]
    public class GenRsaKeyCommand : ConsoleCommandBase, IGenRsaKeyCommand
    {
        public const string DefaultKeyName = "default";

        [Option('l', "length", Default = 1024, HelpText = "RSA key length.")]
        public int Length { get; set; }

        [Option('n', "name", Default = DefaultKeyName, HelpText = "Key name.")]
        public string Name { get; set; }
    }
}
