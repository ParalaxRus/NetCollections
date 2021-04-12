# AvlTree

Example:  

        public void AvlTreeExample()
        {
            var elements = new List<WeightedUri>()
            {
                new WeightedUri(new Uri("https://www.test2.com"), 2),
                new WeightedUri(new Uri("https://www.test1.com"), 1),
                new WeightedUri(new Uri("https://www.test3.com"), 3),
                new WeightedUri(new Uri("https://www.test4.com"), 4),
            };

            // Create with original seed of elements
            var tree = new AvlTree<WeightedUri>(elements);
            Console.WriteLine("Count={0} Empty={1} Height={2}", tree.Count, tree.Empty, tree.Height);

            var existing = new WeightedUri(new Uri("https://www.test1.com"), 1);
            Console.WriteLine("Contains={0}", tree.Contains(existing));

            // IEnumerable example
            Console.WriteLine("Enumerating:");
            foreach (var element in tree)
            {
                Console.WriteLine(element);
            }

            // Add duplicate element
            var duplicate = new WeightedUri(new Uri("https://www.test2.com"), 2);
            tree.Add(duplicate);
            Console.WriteLine("Count={0} Empty={1} Height={2}", tree.Count, tree.Empty, tree.Height);

            // Remove existing
            existing = new WeightedUri(new Uri("https://www.test1.com"), 1);
            tree.Remove(existing);
            Console.WriteLine("Contains={0}", tree.Contains(existing));

            // Removing all elements
            var left = new List<WeightedUri>();
            foreach (var element in tree)
            {
                left.Add(element);
            }
            foreach (var element in left)
            {
                tree.Remove(element);
            }
            Console.WriteLine("Count={0} Empty={1} Height={2}", tree.Count, tree.Empty, tree.Height);   
        }

Output:  

        Count=4 Empty=False Height=2
        Contains=(True, 1)
        Enumerating:
        Host=www.test1.com Weight=1
        Host=www.test2.com Weight=2
        Host=www.test3.com Weight=3
        Host=www.test4.com Weight=4
        Count=5 Empty=False Height=2
        Contains=(False, 0)
        Count=0 Empty=True Height=0

