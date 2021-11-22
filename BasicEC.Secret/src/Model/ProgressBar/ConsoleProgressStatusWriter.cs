using System;

namespace BasicEC.Secret.Model.ProgressBar
{
    public sealed class ConsoleProgressStatusWriter : ProgressStatusWriter
    {
        private double _percentageOfProgress;

        public ConsoleProgressStatusWriter(IProgressStatusProvider provider)
            : base(provider)
        {
            Console.CursorVisible = false;
        }

        protected override void WriteProgressStatus(ProgressStatus status)
        {
            var isLast = status.ProcessedTasks == status.TotalTasks;
            var @new = (double)status.ProcessedTasks / status.TotalTasks * 100d;
            if (@new - _percentageOfProgress < 0.1 && !isLast) return;

            _percentageOfProgress = @new;
            var ending = isLast ? '\n' : ' ';
            InteractionService.ShowAtTheBeginning($"Progress: {_percentageOfProgress:F1}%{ending}");
        }
    }
}
