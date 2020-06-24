using System;
using System.Collections.Generic;
using System.Linq;

namespace FluentBehaviorTree
{
    public enum Status
    {
        RUNNING = 0,
        SUCCESS = 1,
        FAILURE = 2,
        ERROR = 3
    }

    // https://stackoverflow.com/questions/129389/how-do-you-do-a-deep-copy-of-an-object-in-net-c-specifically/11308879#11308879

    public static class Extensions
    {
        public static IList<T> Clone<T>(this IList<T> list) where T : ICloneable
        {
            return list.Select(item => (T)item.Clone()).ToList();
        }

        public static void Shuffle<T>(this IList<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = Utilities.RandomSystem.Rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
    }

}

namespace FluentBehaviorTree.Utilities
{
    public static class RandomSystem
    {
        public static Random Rng = new Random();

        public static void Seed(int seed)
        {
            Rng = new Random(seed);
        }
    }
}
