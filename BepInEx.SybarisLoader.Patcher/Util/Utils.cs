using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace BepInEx.SybarisLoader.Patcher.Util
{
    public static class Utils
    {
        /// <summary>
        ///     Patches directory for Sybaris.
        /// </summary>
        public static string SybarisDir { get; }
            = // Another solution would be to use native GetModuleFileName, since we're running in the game's process anyway
            Path.GetFullPath(Path.Combine(Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName),
                                          Config.GetEntry("sybaris-patches-location", "Sybaris", "org.bepinex.patchers.sybarisloader")));

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