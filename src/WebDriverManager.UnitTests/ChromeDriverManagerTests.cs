using NUnit.Framework;

namespace WebDriverManager.UnitTests
{
    [TestFixture]
    public class ChromeDriverManagerTests
    {
        [Test]
        public void GetDriverManager()
        {
            WebDriverManager manager = WebDriverManager.chromedriver();

            Assert.That(manager, Is.InstanceOf<ChromeDriverManager>());
        }
    }
}
