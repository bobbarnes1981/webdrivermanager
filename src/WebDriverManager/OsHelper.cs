using System.Runtime.InteropServices;

namespace WebDriverManager
{
    class OsHelper
    {
        public static string OsName()
        {
            return RuntimeInformation.OSDescription;
        }
        public static bool IsWindows()
        {
            return RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
        }
        public static bool IsLinux()
        {
            return RuntimeInformation.IsOSPlatform(OSPlatform.Linux);
        }
        public static bool IsMac()
        {
            return RuntimeInformation.IsOSPlatform(OSPlatform.OSX);
        }
    }
}
