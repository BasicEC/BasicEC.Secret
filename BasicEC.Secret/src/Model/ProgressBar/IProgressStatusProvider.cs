using System;

namespace BasicEC.Secret.Model.ProgressBar
{
    public interface IProgressStatusProvider
    {
        IDisposable SubscribeOnProgressMovedForward(Action<ProgressStatus> action);
    }
}
