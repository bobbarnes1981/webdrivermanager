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

using System;
using System.Collections.Generic;
using System.IO;

namespace WebDriverManager
{
    public static class Extensions
    {
        public static string GetFile(this Uri uri)
        {
            return uri.PathAndQuery;
        }
        public static string SubstringJava(this string str, int startIndex)
        {
            return str.Substring(startIndex);
        }
        public static string SubstringJava(this string str, int startIndex, int endIndex)
        {
            if (endIndex < startIndex)
            {
                throw new NotImplementedException();
            }
            return str.Substring(startIndex, endIndex - startIndex);
        }
        public static void CreateFromStream(this FileInfo fileInfo, Stream source)
        {
            using (Stream stream = fileInfo.OpenWrite())
            {
                source.CopyTo(stream);
            }
        }

        private readonly static DateTime unixEpoch = new DateTime(1970, 1, 1);
        
        public static long UnixTime(this DateTime dateTime)
        {
            return (long)dateTime.Subtract(unixEpoch).TotalMilliseconds;
        }

        public static string FormatUnixTime(this long unixTime, string format)
        {
            return unixEpoch.AddMilliseconds(unixTime).ToString(format);
        }

        public static string ToStringJava<T>(this T[] array)
        {
            return "[" + string.Join(", ", array) + "]";
        }

        public static T[] CopyOf<T>(this T[] source, int newLength)
        {
            T[] newArray = new T[newLength];
            Array.Copy(source, 0, newArray, 0, source.Length);
            return newArray;
        }

        private static Dictionary<Architecture, string> architectureStrings = new Dictionary<Architecture, string>
        {
            { Architecture.DEFAULT, "DEFAULT" },
            { Architecture.X32, "32" },
            { Architecture.X64, "64" },
        };

        public static string GetString(this Architecture architecture)
        {
            return architectureStrings[architecture];
        }
    }
}
