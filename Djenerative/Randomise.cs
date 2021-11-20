using System;

namespace Djenerative
{
    internal class Randomise
    {
        private static readonly Random Rand = new();

        public static int Run(int min, int max)
        {
            // +1 so max is inclusive
            return Rand.Next(min, max + 1);
        }
    }
}
