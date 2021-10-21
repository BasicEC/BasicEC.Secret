using System;
using System.Reflection;
using System.Threading.Tasks;
using CommandLine;

namespace BasicEC.Secret.Services
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class SubVerbsAttribute : Attribute
    {
        public Type[] Types { get; }

        public SubVerbsAttribute(params Type[] types)
        {
            Types = types;
        }
    }

    public static class ParserVerbExtensions
    {
        public static ParserResult<object> ParseVerbs(this Parser parser, ArraySegment<string> args, params Type[] types)
        {
            if (args.Count == 0 || args[0].StartsWith("-"))
            {
                return parser.ParseArguments(args, types);
            }
            var verb = args[0];
            foreach (var type in types)
            {
                var verbAttr = type.GetCustomAttribute<VerbAttribute>();
                if (verbAttr == null || verbAttr.Name != verb)
                {
                    continue;
                }
                if (type.GetCustomAttribute<SubVerbsAttribute>() is {} subAttr)
                {
                    return ParseVerbs(parser, args[1..], subAttr.Types);
                }
            }
            return parser.ParseArguments(args, types);
        }

        public static async Task WithParsedAsync<T>(this ParserResult<object> result, Func<T, Task> action)
        {
            if (result is Parsed<object> { Value: T } parsed)
            {
                await action((T)parsed.Value);
            }
        }
    }
}
