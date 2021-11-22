using System;

namespace BasicEC.Secret.Model.ProgressBar
{
    public interface IProgressStatusWriter : IObserver<ProgressStatus>
    {
    }
}
