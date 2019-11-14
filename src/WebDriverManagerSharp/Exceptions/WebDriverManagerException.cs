/*
 * (C) Copyright 2017 Boni Garcia (http://bonigarcia.github.io/)
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

namespace WebDriverManagerSharp.Exceptions
{
    using System;
    using System.Runtime.Serialization;

    /**
     * Custom exception for WebDriverManager.
     *
     * @author Boni Garcia (boni.gg@gmail.com)
     * @since 1.7.2
     */
    [Serializable]
    public class WebDriverManagerException : Exception
    {
        public WebDriverManagerException()
        {
        }

        public WebDriverManagerException(string message)
            : base(message)
        {
        }

        public WebDriverManagerException(Exception cause)
            : base(string.Empty, cause)
        {
        }

        public WebDriverManagerException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected WebDriverManagerException(SerializationInfo serializationInfo, StreamingContext streamingContext)
            : base(serializationInfo, streamingContext)
        {
        }
    }
}