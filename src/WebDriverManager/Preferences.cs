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

using System;
using System.Collections.Generic;

namespace WebDriverManager
{

    /**
     * Preferences class.
     *
     * @author Boni Garcia (boni.gg@gmail.com)
     * @since 3.0.0
     */
    public class Preferences
    {

        ILogger log = Logger.GetLogger();

        static string TTL = "-ttl";

        //java.util.prefs.Preferences prefs = userNodeForPackage(WebDriverManager.class);
        Dictionary<string, string> prefs = new Dictionary<string, string>();

        string dateFormat = "yyyy-MM-dd HH:mm:ss";
        Config config;

        public Preferences(Config config)
        {
            this.config = config;
        }

        public string getValueFromPreferences(string key)
        {
            if (!prefs.ContainsKey(key))
            {
                return null;
            }
            return prefs[key];
        }

        private long getExpirationTimeFromPreferences(string key)
        {
            return long.Parse(prefs[getExpirationKey(key)]); //default 0
        }

        public void putValueInPreferencesIfEmpty(string key, string value)
        {
            if (getValueFromPreferences(key) == null)
            {
                prefs[key] = value;
                long expirationTime = (long)(DateTime.UtcNow.UnixTime() + TimeSpan.FromSeconds(config.getTtl()).TotalMilliseconds);
                prefs[getExpirationKey(key)] = expirationTime.ToString();
                if (log.IsDebugEnabled())
                {
                    log.Debug("Storing preference {0}={1} (valid until {2})", key, value, formatTime(expirationTime));
                }
            }
        }

        private void clearFromPreferences(string key)
        {
            prefs.Remove(key);
            prefs.Remove(getExpirationKey(key));
        }

        public void clear()
        {
            try
            {
                log.Info("Clearing WebDriverManager preferences");
                prefs.Clear();
            }
            //catch (BackingStoreException e)
            catch (System.Exception e)
            {
                log.Warn("Exception clearing preferences", e);
            }
        }

        private bool checkValidity(string key, string value, long expirationTime)
        {
            long now = DateTime.UtcNow.UnixTime();
            bool isValid = value != null && expirationTime != 0 && expirationTime > now;
            if (!isValid)
            {
                string expirationDate = formatTime(expirationTime);
                log.Debug("Removing preference {0}={1} (expired on {2})", key, value, expirationDate);
                clearFromPreferences(key);
            }
            return isValid;
        }

        private string formatTime(long time)
        {
            return time.FormatUnixTime(dateFormat);
        }

        private string getExpirationKey(string key)
        {
            return key + TTL;
        }

        public bool checkKeyInPreferences(string key)
        {
            string valueFromPreferences = getValueFromPreferences(key);
            bool valueInPreferences = !string.IsNullOrEmpty(valueFromPreferences);
            if (valueInPreferences)
            {
                long expirationTime = getExpirationTimeFromPreferences(key);
                string expirationDate = formatTime(expirationTime);
                valueInPreferences &= checkValidity(key, valueFromPreferences, expirationTime);
                if (valueInPreferences)
                {
                    log.Debug("Preference {0}={1} (valid until {2})", key, valueFromPreferences, expirationDate);
                }
            }
            return valueInPreferences;
        }

    }
}