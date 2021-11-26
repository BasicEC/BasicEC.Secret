using System.Collections.Generic;
using System.Linq;

namespace BasicEC.Secret.Extensions
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<T[]> Chunks<T>(this IReadOnlyCollection<T> values, int chunkSize)
        {
            for (var i = 0; i < values.Count; i += chunkSize)
            {
                var tail = values.Count - i;
                var size = tail > chunkSize ? chunkSize : tail;
                yield return new List<T>(values.Skip(i).Take(size)).ToArray();
            }
        }
    }
}
