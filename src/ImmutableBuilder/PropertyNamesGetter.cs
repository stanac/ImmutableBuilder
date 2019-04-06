using System.Collections.Generic;
using System.Linq;

namespace ImmutableBuilder
{
    internal static class PropertyNamesGetter
    {
        private static readonly IDictionary<string, IReadOnlyList<string>> NamesCache = new Dictionary<string, IReadOnlyList<string>>();
        
        public static IReadOnlyList<string> GetPropertyNames<TModel>()
        {
            string key = typeof(TModel).AssemblyQualifiedName;
            IReadOnlyList<string> names;
            if (!NamesCache.TryGetValue(key, out names))
            {
                names = typeof(TModel)
                    .GetProperties()
                    .Where(x => x.CanRead && x.CanWrite)
                    .Select(x => x.Name)
                    .ToList();

                NamesCache[key] = names;
            }

            return names;
        }

    }
}
