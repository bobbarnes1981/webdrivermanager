using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;

namespace WebDriverManagerSharp.Tests.Diagnostic
{
    class FirefoxTest
    {
        private IWebDriver driver;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            WebDriverManager.FirefoxDriver().Setup();

            //1. It checks the version of the browser installed in your machine(e.g.Chrome, Firefox).
            //2. It checks the version of the driver(e.g.chromedriver, geckodriver).If unknown, it uses the latest version of the driver.
            //3. It downloads the WebDriver binary if it is not present on the WebDriverManager cache(~/.m2/repository/webdriver by default).
            //4. It exports the proper WebDriver Java environment variables required by Selenium(not done when using WebDriverManagerSharp from the CLI or as a Server).
        }

        [SetUp]
        public void SetUp()
        {
            driver = new FirefoxDriver();
        }

        [TearDown]
        public void TearDown()
        {
            if (driver != null)
            {
                driver.Quit();
            }
        }

        [Test]
        public void Test()
        {
            driver.Url = "http://www.google.co.uk";
            driver.Navigate();
        }
    }
}
