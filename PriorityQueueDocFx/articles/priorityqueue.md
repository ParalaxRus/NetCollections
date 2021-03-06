# PriorityQueue

Example:  

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

        public void PriorityQueueExample()
        {
            var elements = new List<WeightedUri>()
            {
                new WeightedUri(new Uri("https://www.test2.com"), 2),
                new WeightedUri(new Uri("https://www.test1.com"), 1),
                new WeightedUri(new Uri("https://www.test3.com"), 3),
            };

            // Create with original seed of elements
            var queue = new PriorityQueue<WeightedUri>(elements, PriorityQueueType.Max);

            var existing = new WeightedUri(new Uri("https://www.test1.com"), 1);
            Console.WriteLine("Contains=" + queue.Contains(existing));

            // IEnumerable example
            Console.WriteLine("Enumerating:");
            foreach (var element in queue)
            {
                Console.WriteLine(element);
            }

            // Enqueue element with the duplicate max weight
            queue.Enqueue(new WeightedUri(new Uri("https://www.testduplicate.com"), 3));

            Console.WriteLine("Count=" + queue.Count + " Empty=" + queue.Empty);

            // Peek top of the queue
            Console.WriteLine("Peek: " + queue.Peek());

            // Draining queue
            while (!queue.Empty)
            {
                Console.WriteLine("Dequeue: " + queue.Dequeue());
            }

            Console.WriteLine("Count=" + queue.Count + " Empty=" + queue.Empty);
        }

Output:  

        Contains=True
        Enumerating:
        Host=www.test3.com Weight=3
        Host=www.test1.com Weight=1
        Host=www.test2.com Weight=2
        Count=4 Empty=False
        Peek: Host=www.test3.com Weight=3
        Dequeue: Host=www.test3.com Weight=3
        Dequeue: Host=www.testduplicate.com Weight=3
        Dequeue: Host=www.test2.com Weight=2
        Dequeue: Host=www.test1.com Weight=1
        Count=0 Empty=True

