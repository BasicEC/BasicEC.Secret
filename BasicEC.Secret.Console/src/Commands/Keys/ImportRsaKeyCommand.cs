using BasicEC.Secret.Commands.Keys;
using CommandLine;

namespace BasicEC.Secret.Console.Commands.Keys
{
    [Verb("import", HelpText = "Import rsa key.")]
    public class ImportRsaKeyCommand : ConsoleCommandBase, IImportRsaKeyCommand
    {
        [Option('f', "file", Required = true, HelpText = "Path to file with rsa key.")]
        public string Input { get; set; }

        [Option('n', "name", Default = "default", HelpText = "Key name.")]
        public string Name { get; set; }
    }
}
