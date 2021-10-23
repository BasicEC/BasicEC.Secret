using System;
using JetBrains.Annotations;

namespace BasicEC.Secret.Model.Commands
{
    public class CommandException : Exception
    {
        public CommandException([CanBeNull] string message)
            : base(message)
        {
        }
    }
}
