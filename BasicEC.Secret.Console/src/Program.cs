using System;
using System.Threading.Tasks;
using BasicEC.Secret.Commands;
using BasicEC.Secret.Console.Commands;
using BasicEC.Secret.Console.Commands.Keys;
using BasicEC.Secret.Exceptions;
using BasicEC.Secret.ProgressBar;
using BasicEC.Secret.Rsa;
using CommandLine;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace BasicEC.Secret.Console
{
    internal static class Program
    {
        public static async Task Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Warning()
                .WriteTo.File($"{AppContext.BaseDirectory}/logs/log-.txt", rollingInterval: RollingInterval.Month)
                .CreateLogger();

            var serviceProvider = new ServiceCollection()
                .AddTransient<IProgressStatusWriter, ConsoleProgressStatusWriter>()
                .AddSingleton<IRsaStore, LocalRsaStore>()
                .AddSingleton<IRsaService, RsaService>()
                .AddSingleton<ICommandExecutor, ConsoleCommandExecutor>()
                .BuildServiceProvider();

            var commands = new[]
            {
                typeof(DecryptCommand),
                typeof(EncryptCommand),
                typeof(RsaKeyCommands),
            };

            try
            {
                await Parser.Default.ParseVerbs(args, commands)
                    .WithParsedAsync<ICommand>(_ => RunCommandAsync(_, serviceProvider));
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        private static async Task RunCommandAsync(ICommand cmd, IServiceProvider serviceProvider)
        {
            try
            {
                var executor = serviceProvider.GetRequiredService<ICommandExecutor>();
                await cmd.ApplyAsync(executor);
            }
            catch (CommandException e)
            {
                Log.Logger.Error(e.Message);
            }
            catch (Exception e)
            {
                Log.Logger.Error(e, "Unexpected error");
            }
        }
    }
}
