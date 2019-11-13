namespace WebDriverManagerSharp.UnitTests
{
    using NUnit.Framework;

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
