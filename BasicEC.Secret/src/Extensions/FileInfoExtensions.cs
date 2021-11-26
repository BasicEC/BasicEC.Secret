using System;
using System.IO;
using BasicEC.Secret.Commands;
using BasicEC.Secret.Exceptions;

namespace BasicEC.Secret.Extensions
{
    public static class FileInfoExtensions
    {
        public static FileInfo CheckFileExists(this string file)
        {
            return new FileInfo(file).CheckExists();
        }

        public static FileInfo CheckExists(this FileInfo file)
        {
            if (file == null)
            {
                throw new ArgumentNullException(nameof(file));
            }

            if (!file.Exists)
            {
                throw new CommandException($"File not found {file.FullName}");
            }

            return file;
        }
    }
}
