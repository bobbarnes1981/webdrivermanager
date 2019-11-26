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
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using WebDriverManagerSharp.Enums;

    public static class Extensions
    {
        private static readonly DateTime unixEpoch = new DateTime(1970, 1, 1);

        private static readonly Dictionary<Architecture, string> architectureStrings = new Dictionary<Architecture, string>
        {
            { Architecture.DEFAULT, "DEFAULT" },
            { Architecture.X32, "32" },
            { Architecture.X64, "64" },
        };

        public static string GetFile(this Uri uri)
        {
            if (uri == null)
            {
                throw new ArgumentNullException(nameof(uri));
            }

            return uri.PathAndQuery;
        }

        public static string SubstringJava(this string str, int startIndex)
        {
            if (str == null)
            {
                throw new ArgumentNullException(nameof(str));
            }

            return str.Substring(startIndex);
        }

        public static string SubstringJava(this string str, int startIndex, int endIndex)
        {
            if (str == null)
            {
                throw new ArgumentNullException(nameof(str));
            }

            if (endIndex > str.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(endIndex));
            }

            if (startIndex < 0 || startIndex > endIndex)
            {
                throw new ArgumentOutOfRangeException(nameof(startIndex));
            }

            return str.Substring(startIndex, endIndex - startIndex);
        }
        
        public static long UnixTime(this DateTime dateTime)
        {
            return (long)dateTime.Subtract(unixEpoch).TotalMilliseconds;
        }

        public static string FormatUnixTime(this long unixTime, string format)
        {
            return unixEpoch.AddMilliseconds(unixTime).ToString(format, CultureInfo.InvariantCulture);
        }

        public static string ToStringJava<T>(this T[] array)
        {
            return "[" + string.Join(", ", array) + "]";
        }

        public static T[] CopyOf<T>(this T[] source, int newLength)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            T[] newArray = new T[newLength];
            Array.Copy(source, 0, newArray, 0, source.Length);
            return newArray;
        }

        public static string GetString(this Architecture architecture)
        {
            return architectureStrings[architecture];
        }

        public static Uri Append(this Uri uri, string uriPart)
        {
            if (uri == null)
            {
                throw new ArgumentNullException(nameof(uri));
            }

            if (uriPart == null)
            {
                throw new ArgumentNullException(nameof(uriPart));
            }

            if (uriPart.StartsWith("/", StringComparison.OrdinalIgnoreCase))
            {
                uriPart = uriPart.Substring(1);
            }

            string uriString = uri.ToString();
            if (!uriString.EndsWith("/", StringComparison.OrdinalIgnoreCase))
            {
                uriString += '/';
            }

            return new Uri(uriString + uriPart);
        }
    }
}
