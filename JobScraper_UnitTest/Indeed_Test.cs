using HtmlAgilityPack;
using Moq;
using NUnit.Framework;

namespace JobScraper_UnitTest
{
    public class Indeed_Test
    {
        [SetUp]
        public void Setup()
        {
        }

        static int[] v = new int[] { 4, 5, 7, 3, 2, 8, 9, 4, 3 };
        [TestCase(3)]
        [TestCaseSource("v")]
        public void Test1(int value)
        {
            Assert.NotNull(value);
        }

        public void GetJobs_From_HtmlPages()
        {
        }
    }
}