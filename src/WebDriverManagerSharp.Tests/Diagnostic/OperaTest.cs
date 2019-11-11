using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Opera;
using System.IO;

namespace WebDriverManager.Tests.Diagnostic
{
    class OperaTest
    {
        private IWebDriver driver;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            WebDriverManager.operadriver().setup();

            //1. It checks the version of the browser installed in your machine(e.g.Chrome, Firefox).
            //2. It checks the version of the driver(e.g.chromedriver, geckodriver).If unknown, it uses the latest version of the driver.
            //3. It downloads the WebDriver binary if it is not present on the WebDriverManager cache(~/.m2/repository/webdriver by default).
            //4. It exports the proper WebDriver Java environment variables required by Selenium(not done when using WebDriverManager from the CLI or as a Server).
        }

        [SetUp]
        public void SetUp()
        {
            OperaOptions options = new OperaOptions()
            {
                BinaryLocation = Path.Combine(System.Environment.ExpandEnvironmentVariables("%LocalAppData%"), @"Programs\Opera\64.0.3417.92\opera.exe")
            };
            driver = new OperaDriver(options);
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
