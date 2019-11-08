using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace WebDriverManager
{
    public class IllegalStateException : Exception
    {
        public IllegalStateException(string message)
            : base(message)
        {
        }
        public IllegalStateException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
    public class Arrays
    {
        public static string ToString(object[] array)
        {
            return "[" + string.Join(", ", array) + "]";
        }
        public static T[] CopyOf<T>(T[] source, int newLength)
        {
            T[] newArray = new T[newLength];
            Array.Copy(source, 0, newArray, 0, source.Length);
            return newArray;
        }
    }
    public class Files
    {
        public static string Separator
        {
            get
            {
                return "/";
            }
        }
    }

    public class Date
    {
        private static DateTime unixEpoch = new DateTime(1970, 1, 1);
        public static long UnixTime()
        {
            return (long)DateTime.UtcNow.Subtract(unixEpoch).TotalMilliseconds;
        }
        public static string FormatUnixTime(long unixTime, string format)
        {
            return unixEpoch.AddMilliseconds(unixTime).ToString(format);
        }
    }
    public class Javalin
    {
        public static Javalin Create()
        {
            return new Javalin();
        }
        public Javalin Start(int port)
        {
            return this;
        }
        public void Get(string route, Handler handler)
        {

        }
    }
    public delegate void Handler(Context context);

    public class Context
    {
        private Response res;
        public string Method()
        {
            throw new NotImplementedException();
        }
        public string Path()
        {
            throw new NotImplementedException();
        }
        public Dictionary<string, List<string>> QueryParamMap()
        {
            throw new NotImplementedException();
        }
        public Response Res
        {
            get
            {
                return res;
            }
        }
        public void Result(Stream stream)
        {

        }
    }
    public class Response
    {
        public void SetHeader(string name, string value)
        {

        }
    }
    public class SYSTEM
    {
        public static void SetProperty(string property, string value)
        {
            throw new NotImplementedException();
        }
        public static string GetProperty(string property)
        {
            throw new NotImplementedException();
        }
        public static void ClearProperty(string property)
        {
            throw new NotImplementedException();
        }
        public static Stream getResourceAsStream(string path)
        {
            throw new NotImplementedException();
        }
    }
}
