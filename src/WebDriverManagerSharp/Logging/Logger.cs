﻿/*
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

namespace WebDriverManagerSharp.Logging
{
    public class Logger : ILogger
    {
        private readonly Serilog.ILogger logger;

        public Logger(Serilog.ILogger logger)
        {
            this.logger= logger;
        }

        public void Trace(string format, params object[] parameters)
        {
            logger.Verbose(format, parameters);
        }

        public void Warn(string format, params object[] parameters)
        {
            logger.Warning(format, parameters);
        }

        public void Debug(string format, params object[] parameters)
        {
            logger.Debug(format, parameters);
        }

        public void Info(string format, params object[] parameters)
        {
            logger.Information(format, parameters);
        }

        public void Error(string format, params object[] parameters)
        {
            logger.Error(format, parameters);
        }

        public bool IsDebugEnabled()
        {
            return logger.IsEnabled(Serilog.Events.LogEventLevel.Debug);
        }

        public bool IsTraceEnabled()
        {
            return logger.IsEnabled(Serilog.Events.LogEventLevel.Verbose);
        }
    }
}
