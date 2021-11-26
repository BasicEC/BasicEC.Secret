namespace BasicEC.Secret.Commands.Keys
{
    public interface IImportRsaKeyCommand : ICommand
    {
        public string Input { get; set; }
        public string Name { get; set; }
    }
}
