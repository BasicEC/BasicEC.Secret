using System;
using System.Collections.Generic;

namespace BasicEC.Secret.Model
{
    public static class InteractionService
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

        public static void Show(IEnumerable<object> list)
        {
            Console.WriteLine(string.Join('\n', list));
        }

        public static void ShowAtTheBeginning(string str)
        {
            Console.SetCursorPosition(0, Console.CursorTop);
            Console.Write(str);
        }
    }
}
