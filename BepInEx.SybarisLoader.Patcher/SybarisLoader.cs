using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using BepInEx.Logging;
using Mono.Cecil;
using BepInEx.SybarisLoader.Patcher.Util;

namespace BepInEx.SybarisLoader.Patcher
{
    public static class Loader
    {
        private static Dictionary<string, List<MethodInfo>> patchersDictionary = new Dictionary<string, List<MethodInfo>>(StringComparer.InvariantCultureIgnoreCase);

        public static IEnumerable<string> TargetDLLs => patchersDictionary.Select(k => k.Key);

        public static ManualLogSource Log = Logger.CreateLogSource("SybarisLoader");

        public static void Initialize()
        {
            AppDomain.CurrentDomain.AssemblyResolve += ResolvePatchers;

            if (!Directory.Exists(Utils.SybarisDir.Value))
            {
                Log.LogWarning($"{Utils.SybarisDir} does not exist! Creating one...");
                return;
            }

            Log.LogWarning($"Loading patchers from \"{Utils.SybarisDir}\"");

            foreach (string dll in Directory.GetFiles(Utils.SybarisDir.Value, "*.Patcher.dll"))
            {
                Assembly assembly;

                try
                {
                    assembly = Assembly.LoadFile(dll);
                }
                catch (Exception e)
                {
                    Log.LogError($"Failed to load {dll}: {e.Message}");
                    if (e.InnerException != null)
                        Log.LogError($"Inner: {e.Message}");

                    continue;
                }

                Log.LogInfo($"Loaded {dll}");

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

                    Log.LogInfo($"Adding {type.FullName}");

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

            Log.LogInfo($"Patching assembly: {assembly.FullName}");

            string assemblyName = $"{assembly.Name.Name}.dll";

            if (!patchersDictionary.TryGetValue(assemblyName, out List<MethodInfo> jobs))
                return;

            foreach (MethodInfo method in jobs)
            {
                try
                {
                    Log.LogInfo($"Invoking: {method.DeclaringType.FullName}:{method.Name}");
                    method.Invoke(null, new object[] {assembly});
                }
                catch (Exception ex)
                {
                    Log.LogError(ex.ToString());
                }
            }
        }

        public static Assembly ResolvePatchers(object sender, ResolveEventArgs args)
        {
            // Try to resolve from patches directory
            if (Utils.TryResolveDllAssembly(args.Name, Utils.SybarisDir.Value, out Assembly patchAssembly))
                return patchAssembly;
            return null;
        }
    }
}