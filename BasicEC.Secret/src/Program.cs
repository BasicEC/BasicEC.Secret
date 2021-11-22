using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reactive.Subjects;
using System.Reflection;
using System.Threading.Tasks;
using BasicEC.Secret.Model.Commands;
using BasicEC.Secret.Model.Commands.Keys;
using BasicEC.Secret.Model.Extensions;
using BasicEC.Secret.Model.ProgressBar;
using BasicEC.Secret.Model.Rsa;
using CommandLine;
using Microsoft.Extensions.DependencyInjection;
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
                .WriteTo.File($"{RootDir!.FullName}/logs/log-.txt", rollingInterval: RollingInterval.Month)
                .CreateLogger();

            var serviceProvider = new ServiceCollection()
                .AddTransient<IProgressStatusWriter, ConsoleProgressStatusWriter>()
                .AddSingleton<IRsaStore, LocalRsaStore>()
                .BuildServiceProvider();

            var executors = new[]
            {
                new CommandExecutor(serviceProvider),
            };

            var commands = new[]
            {
                typeof(DecryptCommand),
                typeof(EncryptCommand),
                typeof(RsaKeyCommands),
            };

            try
            {
                Console.CursorVisible = false;
                await Parser.Default.ParseVerbs(args, commands).WithParsedAsync<ICommand>(_ => RunCommandAsync(_, executors));
            }
            finally
            {
                Console.CursorVisible = true;
                Log.CloseAndFlush();
            }
        }

        private static async Task RunCommandAsync(ICommand cmd, IEnumerable<ICommandExecutor> executors)
        {
            foreach (var executor in executors)
            {
                try
                {
                    await cmd.ApplyAsync(executor);
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

            Log.Logger.Warning("Executor for command {Command} not found.", cmd.GetType().Name);
        }
    }
}
