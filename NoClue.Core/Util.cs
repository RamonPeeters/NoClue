using System;
using System.Collections.Generic;

namespace NoClue.Core {
    internal static class Util {
        public static void Shuffle<T>(this IList<T> collection, Random random) {
            int i = collection.Count;
            while (i > 0) {
                i--;
                collection.Swap(i, random.Next(i + 1));
            }
        }

        public static void Swap<T>(this IList<T> collection, int first, int second) {
            T swap = collection[first];
            collection[first] = collection[second];
            collection[second] = swap;
        }
    }
}
