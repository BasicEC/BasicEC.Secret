namespace BasicEC.Secret.Commands
{
    public interface IEncryptCommand : ICommand
    {
        public string Output { get; set; }
        public string File { get; set; }
        public string Key { get; set; }
        public int Workers { get; set; }
    }
}
