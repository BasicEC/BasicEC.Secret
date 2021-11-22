using System;

namespace BasicEC.Secret.Model.ProgressBar
{
    public interface IProgressStatusProvider
    {
        IDisposable SubscribeOnProgressStatus(IObserver<ProgressStatus> observer);
    }
}
