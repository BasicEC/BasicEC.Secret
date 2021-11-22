using System;

namespace BasicEC.Secret.Model.ProgressBar
{
    public abstract class ProgressStatusWriter : IDisposable
    {
        private readonly IDisposable _subscription;

        protected ProgressStatusWriter(IProgressStatusProvider provider)
        {
            _subscription = provider.SubscribeOnProgressMovedForward(WriteProgressStatus);
        }

        protected abstract void WriteProgressStatus(ProgressStatus status);

        public virtual void Dispose() { _subscription?.Dispose(); }
    }
}
