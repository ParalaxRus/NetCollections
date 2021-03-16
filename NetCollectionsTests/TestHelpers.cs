using System;
using System.Collections.Generic;

namespace NetCollectionsTests
{
    public class TestHelpers
    {
        public static IEnumerable<byte> CreateRandomValues(int max = 20)
        {
            var rand = new Random();
            int size = rand.Next(2, max);

            var values = new byte[size];
            rand.NextBytes(values);

            return values;
        }
    }
}
