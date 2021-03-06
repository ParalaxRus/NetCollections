# PriorityLookupQueue

Example:  

        public void PriorityLookupQueueExample()
        {
            var elements = new List<KeyValuePair<int, Uri>>()
            {
                new KeyValuePair<int, Uri>(3, new Uri("https://www.test3.com")),
                new KeyValuePair<int, Uri>(1, new Uri("https://www.test1.com")),
                new KeyValuePair<int, Uri>(2, new Uri("https://www.test2.com")),
            };

            // Create with original seed of elements
            var queue = new PriorityLookupQueue<int, Uri>(elements.Select(v => v.Key), 
                                                          elements.Select(v => v.Value), 
                                                          PriorityQueueType.Min);

            var existing = new Uri("https://www.test2.com");
            Console.WriteLine("ContainsValue=" + queue.ContainsValue(existing));

            // IEnumerable example
            Console.WriteLine("Enumerating:");
            foreach (var element in queue)
            {
                Console.WriteLine(string.Format("Weight={0} Host={1}", 
                                                element.Key, 
                                                element.Value.Host));
            }

            // Enqueue element with the duplicate min weight
            queue.Enqueue(1, new Uri("https://www.testnonduplicate.com"));

            Console.WriteLine("Count=" + queue.Count + " Empty=" + queue.Empty);

            var lookup = new Uri("https://www.test2.com");
            Console.WriteLine("Priority=" + queue.GetPriority(lookup));
            queue.SetPriority(lookup, 5);
            Console.WriteLine("Priority=" + queue.GetPriority(lookup));

            // Peek top of the queue
            var min = queue.Peek();
            Console.WriteLine(string.Format("Peek: Weight={0} Host={1}", 
                                            min.Key, 
                                            min.Value.Host));

            // Draining queue
            while (!queue.Empty)
            {
                min = queue.Dequeue();

                Console.WriteLine(string.Format("Dequeue: Weight={0} Host={1}", 
                                                min.Key, 
                                                min.Value.Host));
            }

            Console.WriteLine("Count=" + queue.Count + " Empty=" + queue.Empty);
        }

Output:  

        ContainsValue=True
        Enumerating:
        Weight=1 Host=www.test1.com
        Weight=3 Host=www.test3.com
        Weight=2 Host=www.test2.com
        Count=4 Empty=False
        Priority=2
        Priority=5
        Peek: Weight=1 Host=www.test1.com
        Dequeue: Weight=1 Host=www.test1.com
        Dequeue: Weight=1 Host=www.testnonduplicate.com
        Dequeue: Weight=3 Host=www.test3.com
        Dequeue: Weight=5 Host=www.test2.com
        Count=0 Empty=True

