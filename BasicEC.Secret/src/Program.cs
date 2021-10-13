using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using BasicEC.Secret.Commands;
using CommandLine;
using Serilog;

namespace BasicEC.Secret
{
    class Program
    {
        public static DirectoryInfo RootDir { get; private set; }

        public static void Main(string[] args)
        {
            var root = new FileInfo(Assembly.GetExecutingAssembly().Location);
            RootDir = root.Directory;
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.Console()
                .WriteTo.File($"{RootDir!.FullName}/logs/log-.txt", rollingInterval: RollingInterval.Month)
                .CreateLogger();

            var commands = new[] { typeof(DecryptCommand), typeof(EncryptCommand), typeof(GenRsaKeysCommand), typeof(ImportRsaKeyCommand) };
            var executors = new[] { new CommandExecutor() };

            try
            {
                Parser.Default.ParseArguments(args, commands).WithParsed<ICommand>(_ => RunCommand(_, executors));
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        private static void RunCommand(ICommand cmd, IEnumerable<ICommandExecutor> executors)
        {
            foreach (var executor in executors)
            {
                try
                {
                    cmd.Apply(executor);
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
                }
            }

            Log.Logger.Warning("Executor for command {Command} not found.", cmd.GetType().FullName);
        }
    }
}
