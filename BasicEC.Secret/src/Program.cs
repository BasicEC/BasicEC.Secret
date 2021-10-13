using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using BasicEC.Secret.Commands;
using BasicEC.Secret.Services;
using CommandLine;
using Serilog;

namespace BasicEC.Secret
{
    class Program
    {
        private const string PathBase = @"/home/basicec/RiderProjects/BasicEC.Secret/";
        private const string PrivateKeyPath = PathBase + "rsa.pem";
        private const string PubKeyPath = PathBase + "pub.pem";

        private const string InputPath = PathBase + "kaizodude-kaizo.gif";
        private const string OutputPath = PathBase + "encrypted.gif";
        private const string OutputDecryptPath = PathBase + "decrypted.gif";

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

            var commands = new[] { typeof(DecryptCommand), typeof(EncryptCommand), typeof(GenRsaKeysCommand) };
            var executors = new[] { new CommandExecutor() };

            try
            {
                Parser.Default.ParseArguments(args, commands).WithParsed<ICommand>(_ => RunCommand(_, executors));
            }
            catch (Exception e)
            {
                Log.Logger.Error(e, "Unexpected error");
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

            Console.WriteLine($"Executor for command {cmd.GetType().FullName} not found.");
        }
    }
}
