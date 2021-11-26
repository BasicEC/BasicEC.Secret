using System.IO;
using System.Threading.Tasks;
using BasicEC.Secret.Model.ProgressBar;

namespace BasicEC.Secret.Model.Rsa
{
    public interface IRsaService
    {
        Task DecryptAsync(string key, Stream input, Stream output, int workers);
        Task EncryptAsync(string key, Stream input, Stream output, int workers);
    }

    public class RsaService : IRsaService
    {
        private readonly IRsaStore _store;
        private readonly IProgressStatusWriter _statusWriter;

        public RsaService(IRsaStore store, IProgressStatusWriter statusWriter)
        {
            _store = store;
            _statusWriter = statusWriter;
        }

        public async Task DecryptAsync(string key, Stream input, Stream output, int workers)
        {
            var rsa = _store.GetKey(key, true);
            var conveyor = new StreamDataConveyor(input, output, rsa.GetEncryptedDataLength(), _ => rsa.Decrypt(_), workers);
            using var _ = conveyor.SubscribeOnProgressStatus(_statusWriter);
            await conveyor.ProcessDataAsync();
        }

        public async Task EncryptAsync(string key, Stream input, Stream output, int workers)
        {
            var rsa = _store.GetKey(key, false);
            var conveyor = new StreamDataConveyor(input, output, rsa.GetMaxDataLength(), _ => rsa.Encrypt(_), workers);
            using var _ = conveyor.SubscribeOnProgressStatus(_statusWriter);
            await conveyor.ProcessDataAsync();
        }
    }
}
