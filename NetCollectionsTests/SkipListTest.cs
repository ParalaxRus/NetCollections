using Microsoft.VisualStudio.TestTools.UnitTesting;
using NetCollections;

namespace NetCollectionsTests
{
    [TestClass]
    public class SkipListTests
    {
        [TestMethod]
        public void DefaultCtorShouldCreateEmptyList()
        {
            var list = new SkipList<int>();

            Assert.AreEqual(list.Count, 0);
        }

        [TestMethod]
        public void AddingElementShouldIncreaseCountByOne()
        {
            var list = new SkipList<int>();
            list.Add(1);

            Assert.AreEqual(list.Count, 1);
        }

        [TestMethod]
        public void AddingDuplicateShouldIncreaseCountByOne()
        {
            var list = new SkipList<int>();
            list.Add(1);
            list.Add(1);

            Assert.AreEqual(list.Count, 2);
        }

        [TestMethod]
        public void RemovingElementShouldDecreaseCountByOne()
        {
            var list = new SkipList<int>();
            list.Add(1);
            list.Remove(1);

            Assert.AreEqual(list.Count, 0);
        }

        [TestMethod]
        public void RemovingDuplicateShouldIncreaseCountByOne()
        {
            var list = new SkipList<int>();
            list.Add(1);
            list.Add(1);
            list.Remove(1);

            Assert.AreEqual(list.Count, 1);
        }
    }
}
