using System;
using JetBrains.Annotations;

namespace BasicEC.Secret
{
    public class CommandException : Exception
    {
        public CommandException([CanBeNull] string message)
            : base(message)
        {
        }
    }
}
