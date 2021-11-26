namespace BasicEC.Secret.Commands.Keys
{
    public interface IRemoveRsaKeyCommand : ICommand
    {
        public string Name { get; set; }
        public bool Force { get; set; }
    }
}
