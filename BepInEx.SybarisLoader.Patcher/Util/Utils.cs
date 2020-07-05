using System;
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
        public static ConfigEntry<string> SybarisDir { get; }
        
        public static ConfigEntry<string> ResolveManagedFolder { get; }

        static Utils()
        {
            config = new ConfigFile(Path.Combine(Paths.ConfigPath, "SybarisLoader.cfg"), true);
            SybarisDir = config.Bind("Paths", "SybarisPath",
                            "Sybaris",
                                     "Path where Sybaris patchers are located\nPath is relative to game root");
            
            ResolveManagedFolder = config.Bind("Paths", "ResolveManagedDir",
                string.Empty,
                "Additional path to resolve (needed for example by Sybaris 1)");
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