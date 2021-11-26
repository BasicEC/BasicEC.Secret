using System;

namespace BasicEC.Secret.Exceptions
{
    public class CommandException : Exception
    {
        public CommandException(string message)
            : base(message)
        {
        }
    }
}
