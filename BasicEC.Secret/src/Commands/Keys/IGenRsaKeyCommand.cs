namespace BasicEC.Secret.Commands.Keys
{
    public interface IGenRsaKeyCommand : ICommand
    {
        public int Length { get; set; }
        public string Name { get; set; }
    }
}
