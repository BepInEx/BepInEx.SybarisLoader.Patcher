using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Mono.Cecil;
using BepInEx.SybarisLoader.Patcher.Util;

namespace BepInEx.SybarisLoader.Patcher
{
    public static class Loader
    {
        private static Dictionary<string, List<MethodInfo>> patchersDictionary;


        private static IEnumerable<string> _targetDLLs = null;

        public static IEnumerable<string> TargetDLLs
        {
            get
            {
                if (_targetDLLs == null)
                {
                    Init();

                    _targetDLLs = patchersDictionary.Keys;
                }

                return _targetDLLs;
            }
        }

        private static void Init()
        {
            Configuration.Init();

            AppDomain.CurrentDomain.AssemblyResolve += ResolvePatchers;

            patchersDictionary = new Dictionary<string, List<MethodInfo>>();

            Logger.Log(LogLevel.Info, $"Loading patchers from \"{Utils.SybarisDir}\"");

            foreach (string dll in Directory.GetFiles(Utils.SybarisDir, "*.Patcher.dll"))
            {
                Assembly assembly;

                try
                {
                    assembly = Assembly.LoadFile(dll);
                }
                catch (Exception e)
                {
                    Logger.Log(LogLevel.Error, $"Failed to load {dll}: {e.Message}");
                    if (e.InnerException != null)
                        Logger.Log(LogLevel.Error, $"Inner: {e.InnerException}");
                    continue;
                }

                Logger.Log(LogLevel.Info, $"Loaded {dll}");

                foreach (Type type in assembly.GetTypes())
                {
                    if (type.IsInterface)
                        continue;

                    FieldInfo targetAssemblyNamesField = type.GetField("TargetAssemblyNames", BindingFlags.Static | BindingFlags.Public);

                    if (targetAssemblyNamesField == null || targetAssemblyNamesField.FieldType != typeof(string[]))
                        continue;

                    MethodInfo patchMethod = type.GetMethod("Patch", BindingFlags.Static | BindingFlags.Public);
                    
                    if (patchMethod == null || patchMethod.ReturnType != typeof(void))
                        continue;


                    ParameterInfo[] parameters = patchMethod.GetParameters();
                    
                    if (parameters.Length != 1 || parameters[0].ParameterType.Name != nameof(AssemblyDefinition))
                        continue;

                    string[] requestedAssemblies = targetAssemblyNamesField.GetValue(null) as string[];

                    if (requestedAssemblies == null || requestedAssemblies.Length == 0)
                        continue;

                    Logger.Log(LogLevel.Info, $"Adding {type.FullName}");

                    foreach (string requestedAssembly in requestedAssemblies)
                    {
                        if (!patchersDictionary.TryGetValue(requestedAssembly, out List<MethodInfo> list))
                        {
                            list = new List<MethodInfo>();
                            patchersDictionary.Add(requestedAssembly, list);
                        }

                        list.Add(patchMethod);
                    }
                }
            }
        }
        
        public static void Patch(AssemblyDefinition assembly)
        {
            Logger.Log(LogLevel.Info, $"Patching assembly: {assembly.FullName}");

            string name = assembly.Name.Name;

            if (!patchersDictionary.Any(x =>
                x.Key.Replace(".dll", "").Equals(name, StringComparison.OrdinalIgnoreCase)))
            {
                return;
            }

            var kv = patchersDictionary.First(x =>
                x.Key.Replace(".dll", "").Equals(name, StringComparison.OrdinalIgnoreCase));
            

            List<MethodInfo> jobs = kv.Value;

            foreach (MethodInfo method in jobs)
            {
                try
                {
                    Logger.Log(LogLevel.Info, $"Invoking: {method.DeclaringType.FullName}:{method.Name}");
                    method.Invoke(null, new object[] {assembly});
                }
                catch (Exception ex)
                {
                    Logger.Log(LogLevel.Error, ex.ToString());
                }
            }
        }

        public static Assembly ResolvePatchers(object sender, ResolveEventArgs args)
        {
            // Try to resolve from patches directory
            if (Utils.TryResolveDllAssembly(args.Name, Utils.SybarisDir, out Assembly patchAssembly))
                return patchAssembly;
            return null;
        }
    }
}