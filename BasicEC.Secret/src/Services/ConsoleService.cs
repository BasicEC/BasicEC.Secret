using System;
using System.Collections.Generic;

namespace BasicEC.Secret.Services
{
    public static class ConsoleService
    {
        public static bool Confirm(string question)
        {
            Console.WriteLine($"{question} (yes/no)?");
            while (true)
            {
                var answer = Console.ReadLine();
                if ("yes".Equals(answer)) return true;
                if ("no".Equals(answer)) return false;
                Console.WriteLine("Please type 'yes' or 'no':");
            }
        }

        public static void Write(IEnumerable<object> list)
        {
            Console.WriteLine(string.Join('\n', list));
        }
    }
}
