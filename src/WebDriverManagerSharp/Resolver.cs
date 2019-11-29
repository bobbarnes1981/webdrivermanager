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

namespace WebDriverManagerSharp
{
    using Autofac;
    using Autofac.Core;
    using Serilog;
    using System;
    using WebDriverManagerSharp.Configuration;
    using WebDriverManagerSharp.Logging;
    using WebDriverManagerSharp.Managers;
    using WebDriverManagerSharp.Processes;
    using WebDriverManagerSharp.Storage;
    using WebDriverManagerSharp.Web;

    public static class Resolver
    {
        public static Action<ContainerBuilder> OverriddenRegistrations;

        public static T Resolve<T>(params Parameter[] parameters)
        {
            ContainerBuilder builder = new ContainerBuilder();

            builder.RegisterType<Config>().As<IConfig>().SingleInstance();
            builder.RegisterType<Shell>().As<IShell>();
            builder.RegisterType<Logger>().As<Logging.ILogger>();
            builder.RegisterType<SystemInformation>().As<ISystemInformation>();
            builder.RegisterType<FileStorage>().As<IFileStorage>();
            builder.RegisterType<Preferences>().As<IPreferences>();
            builder.RegisterType<Downloader>().As<IDownloader>();
            builder.RegisterType<HttpClient>().As<IHttpClient>();
            builder.RegisterType<Process>().As<IProcess>();

            builder.RegisterType<ProcessBuilder>().As<IProcessBuilder>();

            builder.RegisterType<HttpClientHelper>().As<IHttpClientHelper>().InstancePerDependency();

            builder.RegisterType<ChromeDriverManager>();
            builder.RegisterType<EdgeDriverManager>();
            builder.RegisterType<FirefoxDriverManager>();
            builder.RegisterType<InternetExplorerDriverManager>();
            builder.RegisterType<OperaDriverManager>();
            builder.RegisterType<PhantomJsDriverManager>();
            builder.RegisterType<SeleniumServerStandaloneManager>();
            builder.RegisterType<VoidDriverManager>();

            builder.RegisterInstance(new LoggerConfiguration().MinimumLevel.Verbose().WriteTo.Console().CreateLogger()).As<Serilog.ILogger>();

            if (OverriddenRegistrations != null)
            {
                OverriddenRegistrations(builder);
            }

            IContainer container = builder.Build();

            return container.Resolve<T>(parameters);
        }
    }
}
