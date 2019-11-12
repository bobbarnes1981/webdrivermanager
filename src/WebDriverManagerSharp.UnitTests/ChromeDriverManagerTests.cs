using NUnit.Framework;

namespace WebDriverManagerSharp.UnitTests
{
    [TestFixture]
    public class ChromeDriverManagerTests
    {
        [Test]
        public void GetDriverManager()
        {
            WebDriverManager manager = WebDriverManager.ChromeDriver();

            Assert.That(manager, Is.InstanceOf<ChromeDriverManager>());
        }
    }
}
