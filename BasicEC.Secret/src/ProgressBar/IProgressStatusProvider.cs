using System;

namespace BasicEC.Secret.ProgressBar
{
    public interface IProgressStatusProvider
    {
        IDisposable SubscribeOnProgressStatus(IObserver<ProgressStatus> observer);
    }
}
