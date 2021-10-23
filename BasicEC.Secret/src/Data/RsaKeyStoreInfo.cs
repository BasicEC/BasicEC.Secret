using System.Text;

namespace BasicEC.Secret.Data
{
    public class RsaKeyStoreInfo
    {
        public string Name { get; init; }

        public bool HasPrivateKey { get; init; }

        public bool HasPublicKey { get; init; }

        public override string ToString()
        {
            var builder = new StringBuilder();
            builder.Append(Name);
            if (!HasPrivateKey && !HasPublicKey)
            {
                builder.Append(": key files are missing");
            }
            else if (!HasPrivateKey)
            {
                builder.Append(": public key only");
            }
            else if (!HasPublicKey)
            {
                builder.Append(": private key only");
            }

            return builder.ToString();
        }
    }
}
