using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using BepInEx.Configuration;

namespace BepInEx.SybarisLoader.Patcher.Util
{
    public static class Utils
    {
        private static ConfigFile config;

        /// <summary>
        ///     Patches directory for Sybaris.
        /// </summary>
        public static ConfigWrapper<string> SybarisDir { get; }

        static Utils()
        {
            config = new ConfigFile(Path.Combine(Paths.BepInExConfigPath, "SybarisLoader.cfg"), true);
            SybarisDir = config.Wrap("Paths", "SybarisPath",
                                     "Path where Sybaris patchers are located\nPath is relative to game root",
                                     "Sybaris");
        }

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