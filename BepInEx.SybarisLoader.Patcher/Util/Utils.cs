using System;
using System.IO;
using System.Reflection;

namespace BepInEx.SybarisLoader.Patcher.Util
{
    public static class Utils
    {
        static Utils()
        {
            SybarisDir = Path.GetFullPath($@"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}\..\..\Sybaris\");
        }

        /// <summary>
        ///     Patches directory for UnityPrePatcher.
        /// </summary>
        public static string SybarisDir { get; }

        /// <summary>
        ///     Try to resolve and load the given assembly DLL.
        /// </summary>
        /// <param name="name">Name of the assembly. Follows the format of <see cref="AssemblyName" />.</param>
        /// <param name="directory">Directory to search the assembly from.</param>
        /// <param name="assembly">The loaded assembly.</param>
        /// <returns>True, if the assembly was found and loaded. Otherwise, false.</returns>
        public static bool TryResolveDllAssembly(string name, string directory, out Assembly assembly)
        {
            assembly = null;
            string path = Path.Combine(directory, $"{new AssemblyName(name).Name}.dll");

            if (!File.Exists(path))
                return false;

            try
            {
                assembly = Assembly.LoadFile(path);
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }
    }
}