namespace KTL.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public static class EnumerableExtensions
    {
        public static T GetRandomElement<T>(this IEnumerable<T> enumerable, Random random = null)
        {
            random = random ?? new Random();
            var index = random.Next(0, enumerable.Count());
            return enumerable.ElementAt(index);
        }
    }
}