using System;
using Serilog;

namespace BasicEC.Secret.Model.ProgressBar
{
    public sealed class ConsoleProgressStatusWriter : IProgressStatusWriter
    {
        private double _percentageOfProgress;

        public void OnCompleted() { Console.WriteLine(); }

        public void OnError(Exception error)
        {
            Log.Logger.Error(error, "An error occurred during the execution of the process");
        }

        public void OnNext(ProgressStatus value)
        {
            var isLast = value.ProcessedTasks == value.TotalTasks;
            var @new = (double)value.ProcessedTasks / value.TotalTasks * 100d;
            if (@new - _percentageOfProgress < 0.1 && !isLast) return;

            _percentageOfProgress = @new;
            Console.SetCursorPosition(0, Console.CursorTop);
            Console.Write($"Progress: {_percentageOfProgress:F1}%");
        }
    }
}
