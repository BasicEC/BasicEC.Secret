using System;
using BasicEC.Secret.ProgressBar;
using Microsoft.Extensions.Logging;

namespace BasicEC.Secret.Console
{
    public sealed class ConsoleProgressStatusWriter : IProgressStatusWriter
    {
        private readonly ILogger _logger;

        public ConsoleProgressStatusWriter(ILoggerProvider loggerProvider)
        {
            _logger = loggerProvider.CreateLogger(nameof(ConsoleProgressStatusWriter));
        }

        private double _percentageOfProgress;

        public void OnCompleted() { System.Console.WriteLine(); }

        public void OnError(Exception error)
        {
            _logger.LogError(error, "An error occurred during the execution of the process");
        }

        public void OnNext(ProgressStatus value)
        {
            var isLast = value.ProcessedTasks == value.TotalTasks;
            var @new = (double)value.ProcessedTasks / value.TotalTasks * 100d;
            if (@new - _percentageOfProgress < 0.1 && !isLast) return;

            _percentageOfProgress = @new;
            System.Console.SetCursorPosition(0, System.Console.CursorTop);
            System.Console.Write($"Progress: {_percentageOfProgress:F1}%");
        }
    }
}
