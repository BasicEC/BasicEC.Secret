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
using Microsoft.Extensions.Logging;
using Serilog;

namespace BasicEC.Secret.Console
{
    internal static class Program
    {
        public static async Task Main(string[] args)
        {
            using var logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.File($"{AppContext.BaseDirectory}/logs/log-.txt", rollingInterval: RollingInterval.Month)
                .WriteTo.Console()
                .CreateLogger();

            var serviceProvider = new ServiceCollection()
                .AddTransient<IProgressStatusWriter, ConsoleProgressStatusWriter>()
                .AddSingleton<IRsaStore, LocalRsaStore>()
                .AddSingleton<IRsaService, RsaService>()
                .AddSingleton<ICommandExecutor, ConsoleCommandExecutor>()
                .AddLogging(builder =>
                {
                    builder.ClearProviders();
                    // ReSharper disable once AccessToDisposedClosure
                    builder.AddSerilog(logger);
                })
                .BuildServiceProvider();

            var commands = new[]
            {
                typeof(DecryptCommand),
                typeof(EncryptCommand),
                typeof(RsaKeyCommands),
            };

            await Parser.Default.ParseVerbs(args, commands)
                .WithParsedAsync<ICommand>(_ => RunCommandAsync(_, serviceProvider));
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
                System.Console.WriteLine(e.Message);
            }
            catch (Exception e)
            {
                System.Console.WriteLine($"Unexpected error: {e.Message}\nSee logs for more information");
            }
        }
    }
}
