using System;
using System.Collections.Generic;

namespace NetCollectionsTests
{
    public class TestHelpers
    {
        public static IEnumerable<byte> CreateRandomValues(bool print = false, int max = 20)
        {
            var rand = new Random();
            int size = rand.Next(2, max);

            var values = new byte[size];
            rand.NextBytes(values);

            if (print)
            {
                Console.Write("Random values: ");
                foreach (var v in values)
                {
                    Console.Write("{0}, ", v);
                }

                Console.WriteLine();
            }

            return values;
        }
    }
}
