using BasicEC.Secret.Commands;
using BasicEC.Secret.Console.Commands.Keys;
using CommandLine;

namespace BasicEC.Secret.Console.Commands
{
    [Verb("decrypt", HelpText = "Decrypt file.")]
    public class DecryptCommand : ConsoleCommandBase, IDecryptCommand
    {
        [Option('o', "out", Required = true, HelpText = "Output file name.")]
        public string Output { get; set; }

        [Option('f', "file", Required = true, HelpText = "File to decrypt.")]
        public string File { get; set; }

        [Option('k', "key", Default = GenRsaKeyCommand.DefaultKeyName, HelpText = "Name of the key that will be used.")]
        public string Key { get; set; }

        [Option('w', "workers", Default = 4, HelpText = "Number threads to perform decryption")]
        public int Workers { get; set; }
    }
}
