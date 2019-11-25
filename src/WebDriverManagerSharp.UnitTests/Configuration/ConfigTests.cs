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

        [Test]
        public void TestConstructor()
        {
            Assert.Throws<ArgumentNullException>(() => new Config(loggerMock.Object, null, fileStorageMock.Object));
        }

        [Test]
        public void TestFileStorageException()
        {
            fileStorageMock.Setup(x => x.FileExists("webdrivermanager.properties")).Returns(true);
            fileStorageMock.Setup(x => x.OpenRead("webdrivermanager.properties")).Throws(new IOException());

            Config config = new Config(loggerMock.Object, systemInformationMock.Object, fileStorageMock.Object);

            string path = config.GetBinaryPath();

            Assert.That(path, Is.Empty);
            loggerMock.Verify(x => x.Trace("Property {0} not found in {1}", "wdm.binaryPath", It.IsAny<ConfigKey<string>>()), Times.Once);
        }

        [Test]
        public void TestIsExecutableNull()
        {
            FileInfo fileInfo = null;

            Config config = new Config(loggerMock.Object, systemInformationMock.Object, fileStorageMock.Object);

            Assert.Throws<ArgumentNullException>(() => config.IsExecutable(fileInfo));
        }

        [Test]
        public void TestIsExecutableWindowsNotExecutable()
        {
            FileInfo fileInfo = new FileInfo("testfile.doc");

            Config config = new Config(loggerMock.Object, systemInformationMock.Object, fileStorageMock.Object);

            config.SetOs("WIN");

            bool result = config.IsExecutable(fileInfo);

            Assert.That(result, Is.False);
        }

        [Test]
        public void TestIsExecutableWindowsExecutable()
        {
            FileInfo fileInfo = new FileInfo("testfile.exe");

            Config config = new Config(loggerMock.Object, systemInformationMock.Object, fileStorageMock.Object);

            config.SetOs("WIN");

            bool result = config.IsExecutable(fileInfo);

            Assert.That(result, Is.True);
        }

        [Test]
        public void TestIsExecutableLinux()
        {
            FileInfo fileInfo = new FileInfo("testfile");

            Config config = new Config(loggerMock.Object, systemInformationMock.Object, fileStorageMock.Object);

            config.SetOs("LINUX");

            Assert.Throws<NotImplementedException>(() => config.IsExecutable(fileInfo));
        }

        [Test]
        public void TestIsExecutableMac()
        {
            FileInfo fileInfo = new FileInfo("testfile");

            Config config = new Config(loggerMock.Object, systemInformationMock.Object, fileStorageMock.Object);

            config.SetOs("Mac");

            Assert.Throws<NotImplementedException>(() => config.IsExecutable(fileInfo));
        }

        [Test]
        public void TestReset()
        {
            Config config = new Config(loggerMock.Object, systemInformationMock.Object, fileStorageMock.Object);

            Architecture origArch = config.GetArchitecture();

            Architecture newArch = Architecture.X32;
            if (origArch == Architecture.X32)
            {
                newArch = Architecture.X64;
            }

            config.SetArchitecture(newArch);

            Architecture arch = config.GetArchitecture();

            Assert.That(arch, Is.EqualTo(newArch));

            config.Reset();

            arch = config.GetArchitecture();

            Assert.That(arch, Is.EqualTo(origArch));
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

        [Test]
        public void TestGetSetConfigBoolInvalidValue()
        {
            fileStorageMock.Setup(x => x.FileExists("webdrivermanager.properties")).Returns(true);
            fileStorageMock.Setup(x => x.OpenRead("webdrivermanager.properties")).Returns(new MemoryStream(Encoding.ASCII.GetBytes("wdm.forceCache=dave")));

            Config config = new Config(loggerMock.Object, systemInformationMock.Object, fileStorageMock.Object);

            Assert.Throws<WebDriverManagerException>(() => config.IsForceCache());
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

        [TestCase("IgnoreVersions")]
        public void TestGetConfigStringArrayValueMissingFile(string methodName)
        {
            Config config = new Config(loggerMock.Object, systemInformationMock.Object, fileStorageMock.Object);

            MethodInfo getter = config.GetType().GetMethod("Get" + methodName, BindingFlags.Public | BindingFlags.Instance);
            string[] val = (string[])getter.Invoke(config, new object[] { });

            Assert.That(val, Is.EqualTo(new string[0]));
        }

        [TestCase("IgnoreVersions", "wdm.ignoreVersions")]
        public void TestGetSetConfigStringArrayValue(string methodName, string propertyName)
        {
            string[] setVal = new string[] { "a", "b" };

            fileStorageMock.Setup(x => x.FileExists("webdrivermanager.properties")).Returns(true);
            fileStorageMock.Setup(x => x.OpenRead("webdrivermanager.properties")).Returns(new MemoryStream(Encoding.ASCII.GetBytes(string.Format("{0}={1}", propertyName, string.Join(",", setVal).ToLower()))));

            Config config = new Config(loggerMock.Object, systemInformationMock.Object, fileStorageMock.Object);

            MethodInfo getter = config.GetType().GetMethod("Get" + methodName, BindingFlags.Public | BindingFlags.Instance);
            string[] val = (string[])getter.Invoke(config, new object[] { });

            Assert.That(val, Is.EqualTo(setVal));

            MethodInfo setter = config.GetType().GetMethod("Set" + methodName, BindingFlags.Public | BindingFlags.Instance);

            string[] newVal = new string[] { "c", "d" };

            setter.Invoke(config, new object[] { newVal });

            val = (string[])getter.Invoke(config, new object[] { });

            Assert.That(val, Is.EqualTo(newVal));

            newVal = new string[] { "e", "f" };

            setter.Invoke(config, new object[] { newVal });

            val = (string[])getter.Invoke(config, new object[] { });

            Assert.That(val, Is.EqualTo(newVal));
        }

        [Test]
        public void TestGetSetConfigIntInvalidValue()
        {
            fileStorageMock.Setup(x => x.FileExists("webdrivermanager.properties")).Returns(true);
            fileStorageMock.Setup(x => x.OpenRead("webdrivermanager.properties")).Returns(new MemoryStream(Encoding.ASCII.GetBytes("wdm.timeout=dave")));

            Config config = new Config(loggerMock.Object, systemInformationMock.Object, fileStorageMock.Object);

            Assert.Throws<FormatException>(() => config.GetTimeout());
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

            string val = config.GetTargetPath();

            Assert.That(val, Is.EqualTo(Environment.ExpandEnvironmentVariables("%userprofile%") + "\\mydirectory"));
        }

        [Test]
        public void TestGetTargetHomePathLinux()
        {
            systemInformationMock.Setup(x => x.OperatingSystem).Returns(WebDriverManagerSharp.Enums.OperatingSystem.LINUX);

            string setVal = "~\\mydirectory";

            fileStorageMock.Setup(x => x.FileExists("webdrivermanager.properties")).Returns(true);
            fileStorageMock.Setup(x => x.OpenRead("webdrivermanager.properties")).Returns(new MemoryStream(Encoding.ASCII.GetBytes(string.Format("wdm.targetPath={0}", setVal.ToString().ToLower()))));

            Config config = new Config(loggerMock.Object, systemInformationMock.Object, fileStorageMock.Object);

            Assert.Throws<NotImplementedException>(() => config.GetTargetPath());

            //string val = config.GetTargetPath();

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

        [Test]
        public void FailedParse()
        {
            ConfigAccessor config = new ConfigAccessor(loggerMock.Object, systemInformationMock.Object, fileStorageMock.Object);

            WebDriverManagerException exception = Assert.Throws<WebDriverManagerException>(() => config.GetFailedConfigKey());

            Assert.That(exception.Message, Is.EqualTo("Type System.Object cannot be parsed"));
        }

        [Test]
        public void FailedReset()
        {
            ConfigAccessor config = new ConfigAccessor(loggerMock.Object, systemInformationMock.Object, fileStorageMock.Object);

            config.Reset();

            loggerMock.Verify(x => x.Warn("Exception resetting {0}", "failedConfigKey"), Times.Once);
        }

        private class ConfigAccessor : Config
        {
            public ConfigAccessor(ILogger logger, ISystemInformation systemInformation, IFileStorage fileStorage)
                : base(logger, systemInformation, fileStorage)
            {
            }

            private readonly BrokenConfigKey<object> failedConfigKey = new BrokenConfigKey<object>("wdm.failedConfigKey");

            public object GetFailedConfigKey()
            {
                return resolve(failedConfigKey);
            }
        }

        private class BrokenConfigKey<T> : ConfigKey<T>
        {
            public BrokenConfigKey(string name)
                :base(name)
            {
            }

            public new void Reset()
            {
                throw new Exception("Failed!");
            }
        }
    }
}
