/*
 * (C) Copyright 2018 Boni Garcia (http://bonigarcia.github.io/)
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

namespace WebDriverManagerSharp.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using WebDriverManagerSharp.Logging;

    /**
     * Preferences class.
     *
     * @author Boni Garcia (boni.gg@gmail.com)
     * @since 3.0.0
     */
    public class Preferences : IPreferences
    {
        private const string TTL = "-ttl";

        private readonly ILogger logger;
        private readonly IConfig config;

        private readonly Dictionary<string, string> prefs = new Dictionary<string, string>();

        private readonly string dateFormat = "yyyy-MM-dd HH:mm:ss";

        public Preferences(ILogger logger, IConfig config)
        {
            this.logger = logger;
            this.config = config;
        }

        public string GetValueFromPreferences(string key)
        {
            if (!prefs.ContainsKey(key))
            {
                return null;
            }

            return prefs[key];
        }

        private long getExpirationTimeFromPreferences(string key)
        {
            return long.Parse(prefs[getExpirationKey(key)], CultureInfo.InvariantCulture); //default 0
        }

        public void PutValueInPreferencesIfEmpty(string key, string value)
        {
            if (GetValueFromPreferences(key) == null)
            {
                prefs[key] = value;
                long expirationTime = (long)(DateTime.UtcNow.UnixTime() + TimeSpan.FromSeconds(config.GetTtl()).TotalMilliseconds);
                prefs[getExpirationKey(key)] = expirationTime.ToString(CultureInfo.InvariantCulture);
                if (logger.IsDebugEnabled())
                {
                    logger.Debug("Storing preference {0}={1} (valid until {2})", key, value, formatTime(expirationTime));
                }
            }
        }

        private void clearFromPreferences(string key)
        {
            prefs.Remove(key);
            prefs.Remove(getExpirationKey(key));
        }

        public void Clear()
        {
            try
            {
                logger.Info("Clearing WebDriverManager preferences");
                prefs.Clear();
            }
            catch (Exception e)
            {
                logger.Warn("Exception clearing preferences ({0})", e);
            }
        }

        private bool checkValidity(string key, string value, long expirationTime)
        {
            long now = DateTime.UtcNow.UnixTime();
            bool isValid = value != null && expirationTime != 0 && expirationTime > now;
            if (!isValid)
            {
                string expirationDate = formatTime(expirationTime);
                logger.Debug("Removing preference {0}={1} (expired on {2})", key, value, expirationDate);
                clearFromPreferences(key);
            }

            return isValid;
        }

        private string formatTime(long time)
        {
            return time.FormatUnixTime(dateFormat);
        }

        private static string getExpirationKey(string key)
        {
            return key + TTL;
        }

        public bool CheckKeyInPreferences(string key)
        {
            string valueFromPreferences = GetValueFromPreferences(key);
            bool valueInPreferences = !string.IsNullOrEmpty(valueFromPreferences);
            if (valueInPreferences)
            {
                long expirationTime = getExpirationTimeFromPreferences(key);
                string expirationDate = formatTime(expirationTime);
                valueInPreferences &= checkValidity(key, valueFromPreferences, expirationTime);
                if (valueInPreferences)
                {
                    logger.Debug("Preference {0}={1} (valid until {2})", key, valueFromPreferences, expirationDate);
                }
            }

            return valueInPreferences;
        }
    }
}