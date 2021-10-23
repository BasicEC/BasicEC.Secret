using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using BasicEC.Secret.Model.Commands;
using BasicEC.Secret.Model.Commands.Keys;
using BasicEC.Secret.Model.Extensions;
using CommandLine;
using Serilog;

namespace BasicEC.Secret
{
    internal static class Program
    {
        public static DirectoryInfo RootDir { get; private set; }

        public static async Task Main(string[] args)
        {
            var root = new FileInfo(Assembly.GetExecutingAssembly().Location);
            RootDir = root.Directory;
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.Console()
                .WriteTo.File($"{RootDir!.FullName}/logs/log-.txt", rollingInterval: RollingInterval.Month)
                .CreateLogger();

            var commands = new[]
            {
                typeof(DecryptCommand),
                typeof(EncryptCommand),
                typeof(RsaKeyCommands),
            };
            var executors = new[]
            {
                new CommandExecutor(),
            };

            try
            {
                await Parser.Default.ParseVerbs(args, commands).WithParsedAsync<ICommand>(_ => RunCommandAsync(_, executors));
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        private static async Task RunCommandAsync(ICommand cmd, IEnumerable<ICommandExecutor> executors)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            foreach (var executor in executors)
            {
                try
                {
                    await cmd.ApplyAsync(executor);
                    stopwatch.Stop();
                    Log.Logger.Information("Command executed: {Command}; Time: {Time}ms", cmd.GetType(), stopwatch.ElapsedMilliseconds);
                    return;
                }
                catch (CommandException e)
                {
                    Log.Logger.Error(e.Message);
                    return;
                }
                catch (Exception e)
                {
                    Log.Logger.Error(e, "Unexpected error");
                    return;
                }
            }

            Log.Logger.Warning("Executor for command {Command} not found.", cmd.GetType().FullName);
        }
    }
}
