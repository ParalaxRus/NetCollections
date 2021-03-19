using System;
using System.Collections.Generic;

namespace NetCollectionsTests
{
    public class WeightedUri : IComparable<WeightedUri>
    {
        public Uri Host { get; private set; }
        public int Weight { get; private set; }

        public WeightedUri(Uri host, int weight)
        {
            this.Host = host;
            this.Weight = weight;
        }

        public int CompareTo(WeightedUri other)
        {
            if (other == null) 
            {
                return 1;
            }
            return this.Weight.CompareTo(other.Weight);
        }

        public override string ToString()
        {
            return string.Format("Host={0} Weight={1}", this.Host.Host, this.Weight);
        }
    }

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

        public static IEnumerable<WeightedUri> CreateRandomCustomValues(bool print = false, int max = 20)
        {
            var values = TestHelpers.CreateRandomValues(print, max);

            var res = new List<WeightedUri>();
            foreach (var v in values)
            {
                res.Add(new WeightedUri(new Uri(string.Format("https://www.test{0}.com", v)), v));
            }

            return res;
        }
    }
}
