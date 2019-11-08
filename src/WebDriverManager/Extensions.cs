using System;
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
    }
}
