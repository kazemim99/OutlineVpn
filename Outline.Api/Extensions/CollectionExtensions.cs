using System;
using System.Collections.Generic;
using System.Linq;

namespace Outline.Api.Extensions
{

    public static class CollectionExtensions
    {
        public static IEnumerable<TSource> DistinctByEx<TSource, TKey>
            (this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            HashSet<TKey> seenKeys = new HashSet<TKey>();
            foreach (TSource element in source)
            {
                if (seenKeys.Add(keySelector(element)))
                {
                    yield return element;
                }
            }
        }
        public static int[] CalculateToArray(this long bookModel)
        {
            var result = new List<int>();

            //result.Add(0);

            string binary = Convert.ToString(bookModel, 2);
            var values = binary.ToArray();

            var arrayList = values.Select(i => int.Parse(i.ToString())).ToArray();
            Array.Reverse(arrayList);

            for (var index = 0; index < arrayList.Length; index++)
            {
                var tt = 2 * arrayList[index];
                var num = Convert.ToInt32(Math.Pow(tt, index) * arrayList[index]);
                if (num != 0)
                {
                    result.Add(num);
                }
            }

            return result.ToArray();
        }
    }
}