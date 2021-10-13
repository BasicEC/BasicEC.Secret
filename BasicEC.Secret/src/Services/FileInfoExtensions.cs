using System;
using System.IO;

namespace BasicEC.Secret.Services
{
    public static class FileInfoExtensions
    {
        public static void CheckFileExists(this string file)
        {
            new FileInfo(file).CheckExists();
        }

        public static void CheckExists(this FileInfo file)
        {
            if (file == null)
            {
                throw new ArgumentNullException(nameof(file));
            }

            if (!file.Exists)
            {
                throw new CommandException($"File not found {file.FullName}");
            }
        }
    }
}
