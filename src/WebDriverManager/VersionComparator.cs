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
     * Version comparator.
     *
     * @author Boni Garcia (boni.gg@gmail.com)
     * @since 2.1.0
     */
    public class VersionComparator : IComparer<string>
    {
        ILogger log = Logger.GetLogger();

        public int Compare(string v1, string v2)
        {
            string[] v1split = v1.Split(new string[] { "\\." }, StringSplitOptions.None);
            string[] v2split = v2.Split(new string[] { "\\." }, StringSplitOptions.None);
            int length = Math.Max(v1split.Length, v2split.Length);
            for (int i = 0; i < length; i++)
            {
                try
                {
                    int v1Part = i < v1split.Length ? int.Parse(v1split[i]) : 0;
                    int v2Part = i < v2split.Length ? int.Parse(v2split[i]) : 0;
                    if (v1Part < v2Part)
                    {
                        return -1;
                    }
                    if (v1Part > v2Part)
                    {
                        return 1;
                    }
                }
                catch (Exception e)
                {
                    log.Trace("Exception comparing {0} with {1} ({2})", v1, v2, e.Message);
                }
            }
            return 0;
        }
    }
}