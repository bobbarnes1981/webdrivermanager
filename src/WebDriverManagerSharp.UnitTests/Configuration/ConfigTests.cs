/*
 * (C) Copyright 2019 Robert barnes
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *   http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 *
 */

namespace WebDriverManager.UnitTests.Configuration
{
    using Moq;
    using NUnit.Framework;
    using System;
    using System.IO;
    using System.Reflection;
    using System.Text;
    using WebDriverManagerSharp.Configuration;
    using WebDriverManagerSharp.Enums;
    using WebDriverManagerSharp.Exceptions;
    using WebDriverManagerSharp.Logging;
    using WebDriverManagerSharp.Storage;

    [TestFixture]
    public class ConfigTests
    {
        private Mock<ILogger> loggerMock;
        private Mock<ISystemInformation> systemInformationMock;
        private Mock<IFileStorage> fileStorageMock;

        [SetUp]
        public void SetUp()
        {
            loggerMock = new Mock<ILogger>();
            systemInformationMock = new Mock<ISystemInformation>();
            fileStorageMock = new Mock<IFileStorage>();

            fileStorageMock.Setup(x => x.GetCurrentDirectory()).Returns("d:\\my_directory");
        }

        //[TestCase("Properties")]
        [TestCase("TargetPath")]
        //[TestCase("Os")]
        [TestCase("Proxy")]
        [TestCase("ProxyUser")]
        [TestCase("ProxyPass")]
        [TestCase("GitHubTokenName")]
        [TestCase("GitHubTokenSecret")]
        [TestCase("LocalRepositoryUser")]
        [TestCase("LocalRepositoryPassword")]
        [TestCase("BinaryPath")]
        [TestCase("ChromeDriverVersion")]
        [TestCase("ChromeDriverExport")]
        [TestCase("EdgeDriverVersion")]
        [TestCase("EdgeDriverExport")]
        [TestCase("FirefoxDriverVersion")]
        [TestCase("FirefoxDriverExport")]
        [TestCase("InternetExplorerDriverVersion")]
        [TestCase("InternetExplorerDriverExport")]
        [TestCase("OperaDriverVersion")]
        [TestCase("OperaDriverExport")]
        [TestCase("PhantomjsDriverVersion")]
        [TestCase("PhantomjsDriverExport")]
        [TestCase("SeleniumServerStandaloneVersion")]
        public void TestGetConfigStringValueMissingFile(string methodName)
        {
            Config config = new Config(loggerMock.Object, systemInformationMock.Object, fileStorageMock.Object);

            MethodInfo getter = config.GetType().GetMethod("Get" + methodName, BindingFlags.Public | BindingFlags.Instance);
            string val = (string)getter.Invoke(config, new object[] { });

            Assert.That(val, Is.Empty);
        }

        //[TestCase("Properties")]
        [TestCase("TargetPath", "wdm.targetPath")]
        //[TestCase("Os")]
        [TestCase("Proxy", "wdm.proxy")]
        [TestCase("ProxyUser", "wdm.proxyUser")]
        [TestCase("ProxyPass", "wdm.proxyPass")]
        [TestCase("GitHubTokenName", "wdm.gitHubTokenName")]
        [TestCase("GitHubTokenSecret", "wdm.gitHubTokenSecret")]
        [TestCase("LocalRepositoryUser", "wdm.localRepositoryUser")]
        [TestCase("LocalRepositoryPassword", "wdm.localRepositoryPassword")]
        [TestCase("BinaryPath", "wdm.binaryPath")]
        [TestCase("ChromeDriverVersion", "wdm.chromeDriverVersion")]
        [TestCase("ChromeDriverExport", "wdm.chromeDriverExport")]
        [TestCase("EdgeDriverVersion", "wdm.edgeDriverVersion")]
        [TestCase("EdgeDriverExport", "wdm.edgeDriverExport")]
        [TestCase("FirefoxDriverVersion", "wdm.geckoDriverVersion")]
        [TestCase("FirefoxDriverExport", "wdm.geckoDriverExport")]
        [TestCase("InternetExplorerDriverVersion", "wdm.internetExplorerDriverVersion")]
        [TestCase("InternetExplorerDriverExport", "wdm.internetExplorerDriverExport")]
        [TestCase("OperaDriverVersion", "wdm.operaDriverVersion")]
        [TestCase("OperaDriverExport", "wdm.operaDriverExport")]
        [TestCase("PhantomjsDriverVersion", "wdm.phantomjsDriverVersion")]
        [TestCase("PhantomjsDriverExport", "wdm.phantomjsDriverExport")]
        [TestCase("SeleniumServerStandaloneVersion", "wdm.seleniumServerStandaloneVersion")]
        public void TestGetSetConfigStringValue(string methodName, string propertyName)
        {
            string guid = Guid.NewGuid().ToString();

            fileStorageMock.Setup(x => x.FileExists("webdrivermanager.properties")).Returns(true);
            fileStorageMock.Setup(x => x.OpenRead("webdrivermanager.properties")).Returns(new MemoryStream(Encoding.ASCII.GetBytes(string.Format("{0}={1}", propertyName, guid))));

            Config config = new Config(loggerMock.Object, systemInformationMock.Object, fileStorageMock.Object);

            MethodInfo getter = config.GetType().GetMethod("Get" + methodName, BindingFlags.Public | BindingFlags.Instance);
            string val = (string)getter.Invoke(config, new object[] { });

            Assert.That(val, Is.EqualTo(guid));

            MethodInfo setter = config.GetType().GetMethod("Set" + methodName, BindingFlags.Public | BindingFlags.Instance);

            string newGuid = Guid.NewGuid().ToString();

            setter.Invoke(config, new object[] { newGuid });

            val = (string)getter.Invoke(config, new object[] { });

            Assert.That(val, Is.EqualTo(newGuid));
        }

        [TestCase("VersionsPropertiesUrl")]
        [TestCase("ChromeDriverUrl")]
        [TestCase("ChromeDriverMirrorUrl")]
        [TestCase("EdgeDriverUrl")]
        [TestCase("FirefoxDriverUrl")]
        [TestCase("FirefoxDriverMirrorUrl")]
        [TestCase("InternetExplorerDriverUrl")]
        [TestCase("OperaDriverUrl")]
        [TestCase("OperaDriverMirrorUrl")]
        [TestCase("PhantomjsDriverUrl")]
        [TestCase("PhantomjsDriverMirrorUrl")]
        [TestCase("SeleniumServerStandaloneUrl")]
        public void TestGetConfigUriValueMissingFile(string methodName)
        {
            Config config = new Config(loggerMock.Object, systemInformationMock.Object, fileStorageMock.Object);

            MethodInfo getter = config.GetType().GetMethod("Get" + methodName, BindingFlags.Public | BindingFlags.Instance);
            TargetInvocationException exception = Assert.Throws<TargetInvocationException>(() => getter.Invoke(config, new object[] { }));

            Assert.That(exception.InnerException, Is.TypeOf<WebDriverManagerException>());
        }

        [TestCase("VersionsPropertiesUrl", "wdm.versionsPropertiesUrl")]
        [TestCase("ChromeDriverUrl", "wdm.chromeDriverUrl")]
        [TestCase("ChromeDriverMirrorUrl", "wdm.chromeDriverMirrorUrl")]
        [TestCase("EdgeDriverUrl", "wdm.edgeDriverUrl")]
        [TestCase("FirefoxDriverUrl", "wdm.geckoDriverUrl")]
        [TestCase("FirefoxDriverMirrorUrl", "wdm.geckoDriverMirrorUrl")]
        [TestCase("InternetExplorerDriverUrl", "wdm.internetExplorerDriverUrl")]
        [TestCase("OperaDriverUrl", "wdm.operaDriverUrl")]
        [TestCase("OperaDriverMirrorUrl", "wdm.operaDriverMirrorUrl")]
        [TestCase("PhantomjsDriverUrl", "wdm.phantomjsDriverUrl")]
        [TestCase("PhantomjsDriverMirrorUrl", "wdm.phantomjsDriverMirrorUrl")]
        [TestCase("SeleniumServerStandaloneUrl", "wdm.seleniumServerStandaloneUrl")]
        public void TestGetSetConfigUriValue(string methodName, string propertyName)
        {
            Uri guid = new Uri("http://" + Guid.NewGuid().ToString());

            fileStorageMock.Setup(x => x.FileExists("webdrivermanager.properties")).Returns(true);
            fileStorageMock.Setup(x => x.OpenRead("webdrivermanager.properties")).Returns(new MemoryStream(Encoding.ASCII.GetBytes(string.Format("{0}={1}", propertyName, guid))));

            Config config = new Config(loggerMock.Object, systemInformationMock.Object, fileStorageMock.Object);

            MethodInfo getter = config.GetType().GetMethod("Get" + methodName, BindingFlags.Public | BindingFlags.Instance);
            Uri val = (Uri)getter.Invoke(config, new object[] { });

            Assert.That(val, Is.EqualTo(guid));

            MethodInfo setter = config.GetType().GetMethod("Set" + methodName, BindingFlags.Public | BindingFlags.Instance);

            Uri newGuid = new Uri("http://" + Guid.NewGuid().ToString());

            setter.Invoke(config, new object[] { newGuid });

            val = (Uri)getter.Invoke(config, new object[] { });

            Assert.That(val, Is.EqualTo(newGuid));
        }

        [TestCase("ForceCache")]
        [TestCase("Override")]
        [TestCase("UseMirror")]
        [TestCase("UseBetaVersions")]
        [TestCase("AvoidExport")]
        [TestCase("AvoidOutputTree")]
        [TestCase("AvoidAutoVersion")]
        [TestCase("AvoidAutoReset")]
        [TestCase("AvoidPreferences")]
        [TestCase("VersionsPropertiesOnlineFirst")]
        public void TestGetConfigBoolValueMissingFile(string methodName)
        {
            Config config = new Config(loggerMock.Object, systemInformationMock.Object, fileStorageMock.Object);

            MethodInfo getter = config.GetType().GetMethod("Is" + methodName, BindingFlags.Public | BindingFlags.Instance);
            bool val = (bool)getter.Invoke(config, new object[] { });

            Assert.That(val, Is.False);
        }

        [TestCase("ForceCache", "wdm.forceCache")]
        [TestCase("Override", "wdm.override")]
        [TestCase("UseMirror", "wdm.useMirror")]
        [TestCase("UseBetaVersions", "wdm.useBetaVersions")]
        [TestCase("AvoidExport", "wdm.avoidExport")]
        [TestCase("AvoidOutputTree", "wdm.avoidOutputTree")]
        [TestCase("AvoidAutoVersion", "wdm.avoidAutoVersion")]
        [TestCase("AvoidAutoReset", "wdm.avoidAutoReset")]
        [TestCase("AvoidPreferences", "wdm.avoidPreferences")]
        [TestCase("VersionsPropertiesOnlineFirst", "wdm.versionsPropertiesOnlineFirst")]
        public void TestGetSetConfigBoolValue(string methodName, string propertyName)
        {
            bool setVal = true;

            fileStorageMock.Setup(x => x.FileExists("webdrivermanager.properties")).Returns(true);
            fileStorageMock.Setup(x => x.OpenRead("webdrivermanager.properties")).Returns(new MemoryStream(Encoding.ASCII.GetBytes(string.Format("{0}={1}", propertyName, setVal.ToString().ToLower()))));

            Config config = new Config(loggerMock.Object, systemInformationMock.Object, fileStorageMock.Object);

            MethodInfo getter = config.GetType().GetMethod("Is" + methodName, BindingFlags.Public | BindingFlags.Instance);
            bool val = (bool)getter.Invoke(config, new object[] { });

            Assert.That(val, Is.EqualTo(setVal));

            MethodInfo setter = config.GetType().GetMethod("Set" + methodName, BindingFlags.Public | BindingFlags.Instance);

            bool newVal = true;

            setter.Invoke(config, new object[] { newVal });

            val = (bool)getter.Invoke(config, new object[] { });

            Assert.That(val, Is.EqualTo(newVal));

            newVal = false;

            setter.Invoke(config, new object[] { newVal });

            val = (bool)getter.Invoke(config, new object[] { });

            Assert.That(val, Is.EqualTo(newVal));
        }

        [TestCase("Timeout")]
        [TestCase("ServerPort")]
        [TestCase("Ttl")]
        public void TestGetConfigIntValueMissingFile(string methodName)
        {
            Config config = new Config(loggerMock.Object, systemInformationMock.Object, fileStorageMock.Object);

            MethodInfo getter = config.GetType().GetMethod("Get" + methodName, BindingFlags.Public | BindingFlags.Instance);
            int val = (int)getter.Invoke(config, new object[] { });

            Assert.That(val, Is.EqualTo(0));
        }

        [TestCase("Timeout", "wdm.timeout")]
        [TestCase("ServerPort", "wdm.serverPort")]
        [TestCase("Ttl", "wdm.ttl")]
        public void TestGetSetConfigIntValue(string methodName, string propertyName)
        {
            int setVal = 1;

            fileStorageMock.Setup(x => x.FileExists("webdrivermanager.properties")).Returns(true);
            fileStorageMock.Setup(x => x.OpenRead("webdrivermanager.properties")).Returns(new MemoryStream(Encoding.ASCII.GetBytes(string.Format("{0}={1}", propertyName, setVal.ToString().ToLower()))));

            Config config = new Config(loggerMock.Object, systemInformationMock.Object, fileStorageMock.Object);

            MethodInfo getter = config.GetType().GetMethod("Get" + methodName, BindingFlags.Public | BindingFlags.Instance);
            int val = (int)getter.Invoke(config, new object[] { });

            Assert.That(val, Is.EqualTo(setVal));

            MethodInfo setter = config.GetType().GetMethod("Set" + methodName, BindingFlags.Public | BindingFlags.Instance);

            int newVal = 2;

            setter.Invoke(config, new object[] { newVal });

            val = (int)getter.Invoke(config, new object[] { });

            Assert.That(val, Is.EqualTo(newVal));

            newVal = 3;

            setter.Invoke(config, new object[] { newVal });

            val = (int)getter.Invoke(config, new object[] { });

            Assert.That(val, Is.EqualTo(newVal));
        }

        [Test]
        public void TestGetTargetDotPathWindows()
        {
            systemInformationMock.Setup(x => x.OperatingSystem).Returns(WebDriverManagerSharp.Enums.OperatingSystem.WIN);

            string setVal = ".";

            fileStorageMock.Setup(x => x.FileExists("webdrivermanager.properties")).Returns(true);
            fileStorageMock.Setup(x => x.OpenRead("webdrivermanager.properties")).Returns(new MemoryStream(Encoding.ASCII.GetBytes(string.Format("wdm.targetPath={0}", setVal.ToString().ToLower()))));
            fileStorageMock.Setup(x => x.GetCurrentDirectory()).Returns("c:\\my_curent_path");

            Config config = new Config(loggerMock.Object, systemInformationMock.Object, fileStorageMock.Object);

            MethodInfo getter = config.GetType().GetMethod("GetTargetPath", BindingFlags.Public | BindingFlags.Instance);
            string val = (string)getter.Invoke(config, new object[] { });

            string path = new FileInfo(Assembly.GetExecutingAssembly().Location).DirectoryName;
            Assert.That(val, Is.EqualTo("c:\\my_curent_path"));
        }

        [Test]
        public void TestGetTargetHomePathWindows()
        {
            systemInformationMock.Setup(x => x.OperatingSystem).Returns(WebDriverManagerSharp.Enums.OperatingSystem.WIN);

            string setVal = "~\\mydirectory";

            fileStorageMock.Setup(x => x.FileExists("webdrivermanager.properties")).Returns(true);
            fileStorageMock.Setup(x => x.OpenRead("webdrivermanager.properties")).Returns(new MemoryStream(Encoding.ASCII.GetBytes(string.Format("wdm.targetPath={0}", setVal.ToString().ToLower()))));

            Config config = new Config(loggerMock.Object, systemInformationMock.Object, fileStorageMock.Object);

            MethodInfo getter = config.GetType().GetMethod("GetTargetPath", BindingFlags.Public | BindingFlags.Instance);
            string val = (string)getter.Invoke(config, new object[] { });

            Assert.That(val, Is.EqualTo(Environment.ExpandEnvironmentVariables("%userprofile%") + "\\mydirectory"));
        }

        [Test]
        [Ignore("not implemented")]
        public void TestGetTargetHomePathLinux()
        {
            systemInformationMock.Setup(x => x.OperatingSystem).Returns(WebDriverManagerSharp.Enums.OperatingSystem.LINUX);

            string setVal = "~\\mydirectory";

            fileStorageMock.Setup(x => x.FileExists("webdrivermanager.properties")).Returns(true);
            fileStorageMock.Setup(x => x.OpenRead("webdrivermanager.properties")).Returns(new MemoryStream(Encoding.ASCII.GetBytes(string.Format("wdm.targetPath={0}", setVal.ToString().ToLower()))));

            Config config = new Config(loggerMock.Object, systemInformationMock.Object, fileStorageMock.Object);

            MethodInfo getter = config.GetType().GetMethod("GetTargetPath", BindingFlags.Public | BindingFlags.Instance);
            string val = (string)getter.Invoke(config, new object[] { });

            //Assert.That(val, Is.EqualTo(Environment.ExpandEnvironmentVariables("%userprofile%") + "\\mydirectory"));
        }

        [Test]
        public void TestGetProperties()
        {
            Config config = new Config(loggerMock.Object, systemInformationMock.Object, fileStorageMock.Object);

            string properties = config.GetProperties();

            Assert.That(properties, Is.EqualTo("webdrivermanager.properties"));
        }

        [Test]
        public void TestSetProperties()
        {
            Config config = new Config(loggerMock.Object, systemInformationMock.Object, fileStorageMock.Object);

            string properties = config.GetProperties();

            Assert.That(properties, Is.EqualTo("webdrivermanager.properties"));

            config.SetProperties("alternative.properties");

            properties = config.GetProperties();

            Assert.That(properties, Is.EqualTo("alternative.properties"));
        }

        [Test]
        public void TestGetArchitecture()
        {
            systemInformationMock.Setup(x => x.Architecture).Returns(Architecture.X32);

            Config config = new Config(loggerMock.Object, systemInformationMock.Object, fileStorageMock.Object);

            Architecture arch = config.GetArchitecture();

            Assert.That(arch, Is.EqualTo(Architecture.X32));
        }

        [Test]
        public void TestGetOs()
        {
            systemInformationMock.Setup(x => x.OperatingSystem).Returns(WebDriverManagerSharp.Enums.OperatingSystem.LINUX);

            Config config = new Config(loggerMock.Object, systemInformationMock.Object, fileStorageMock.Object);

            string os = config.GetOs();

            Assert.That(os, Is.EqualTo(WebDriverManagerSharp.Enums.OperatingSystem.LINUX.ToString()));
        }
    }
}
