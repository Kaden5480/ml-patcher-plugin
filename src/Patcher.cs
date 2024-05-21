using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

using MelonLoader;
using Mono.Cecil;
using MonoMod.Utils;

[assembly: MelonInfo(typeof(MLPatcherPlugin.Patcher), "MLPatcherPlugin", "0.1.0", "Kaden5480")]

namespace MLPatcherPlugin {
    public class Patcher : MelonPlugin {
        /**
         * <summary>
         * The directory patched assemblies will be stored in.
         * </summary>
         */
        private string patcherDir;

        /**
         * <summary>
         * Constructs an instance of Patcher.
         * </summary>
         */
        public Patcher() {
            patcherDir = null;
        }

        /**
         * <summary>
         * Gets the Patcher class within an assembly, if found
         * </summary>
         * <param name="assembly">The assembly to search</param>
         * <returns>The patcher class, if found, or null</returns>
         */
        public Type GetPatcherClass(Assembly assembly) {
            foreach (Type t in assembly.GetTypes()) {
                if (t.Name.Equals("Patcher") == true) {
                    return t;
                }
            }

            return null;
        }

        /**
         * <summary>
         * Gets the DLLs which a given patcher class targets.
         * </summary>
         * <param name="patcher">The patcher class to get targeted DLLs for</param>
         * <returns>An enumerator of all targeted DLLs</returns>
         */
        public IEnumerable<string> GetTargetDLLs(Type patcher) {
            MethodInfo getTargetDLLs = patcher.FindMethod("get_TargetDLLs");

            if (getTargetDLLs == null || getTargetDLLs.IsPublic == false
                    || getTargetDLLs.IsStatic == false) {
                yield break;
            }

            if (getTargetDLLs.ReturnType != typeof(IEnumerable<string>)) {
                yield break;
            }

            ParameterInfo[] args = getTargetDLLs.GetParameters();

            if (args.Length != 0) {
                yield break;
            }

            IEnumerable<string> targetDLLs = (IEnumerable<string>)
                getTargetDLLs.Invoke(null, new object[] {});

            foreach (string dll in targetDLLs) {
                yield return dll;
            }
        }

        /**
         * <summary>
         * Checks if a specified patcher targets a given DLL.
         * </summary>
         * <param name="patcher">The patcher to check</param>
         * <param name="dll">The DLL to check</param>
         * <returns>Whether the patcher targets the given DLL</returns>
         */
        public bool TargetsDLL(Type patcher, string dll) {
            foreach (string name in GetTargetDLLs(patcher)) {
                if (name.Equals(dll) == true) {
                    return true;
                }
            }

            return false;
        }

        /**
         * <summary>
         * Gets the patch method for a specific patcher.
         * </summary>
         * <param name="patcher">The patcher to get the patch method for</param>
         * <returns>The patch method, if found, or null</returns>
         */
        public MethodInfo GetPatchMethod(Type patcher) {
            MethodInfo patch = patcher.FindMethod("Patch");

            if (patch == null || patch.IsPublic == false || patch.IsStatic == false) {
                return null;
            }

            if (patch.ReturnType != typeof(void)) {
                return null;
            }

            ParameterInfo[] args = patch.GetParameters();

            if (args.Length != 1) {
                return null;
            }

            if (args[0].ParameterType != typeof(AssemblyDefinition)) {
                return null;
            }

            return patch;
        }

        /**
         * <summary>
         * Patches a DLL using all applicable patchers.
         * <param name="dll">The DLL to patch</param>
         * </summary>
         */
        public void PatchDLL(FileInfo dll) {
            bool wasPatched = false;
            AssemblyDefinition dllAssembly = AssemblyDefinition.ReadAssembly(dll.FullName);

            foreach (MelonAssembly assembly in MelonAssembly.LoadedAssemblies) {
                Type patcher = GetPatcherClass(assembly.Assembly);

                if (patcher == null) {
                    continue;
                }

                if (TargetsDLL(patcher, dll.Name) == false) {
                    continue;
                }

                MethodInfo patch = GetPatchMethod(patcher);

                if (patch == null) {
                    continue;
                }

                patch.Invoke(null, new object[] { dllAssembly });
                wasPatched = true;
            }

            if (wasPatched == false) {
                return;
            }

            string patchedPath = $"{patcherDir}/Patched-{dll.Name}";

            dllAssembly.Write(patchedPath);
            dllAssembly.Dispose();

            Console.WriteLine($"Loading patched DLL: {dll.Name}");
            Assembly.LoadFile(patchedPath);
        }

        /**
         * <summary>
         * Patches all assemblies in a game's Managed directory.
         * </summary>
         */
        public void PatchAll() {
            DirectoryInfo gameDir = new DirectoryInfo(".");
            DirectoryInfo data = null;
            DirectoryInfo managed = null;

            patcherDir = $"{gameDir.FullName}/MLPatcherPlugin";

            Directory.CreateDirectory(patcherDir);

            foreach (DirectoryInfo dir in gameDir.EnumerateDirectories()) {
                if (dir.Name.EndsWith("_Data") == true) {
                    data = dir;
                    break;
                }
            }

            if (data == null) {
                Console.Error.WriteLine($"No '_Data' directory found in '{gameDir.Name}'");
                Environment.Exit(1);
            }

            foreach (DirectoryInfo dir in data.EnumerateDirectories()) {
                if (dir.Name.Equals("Managed") == true) {
                    managed = dir;
                    break;
                }
            }

            if (managed == null) {
                Console.Error.WriteLine($"No 'Managed' directory found in '{data.Name}'");
                Environment.Exit(1);
            }

            foreach (FileInfo f in managed.EnumerateFiles()) {
                if (f.Name.EndsWith(".dll") == true) {
                    PatchDLL(f);
                }
            }
        }

        /**
         * <summary>
         * Early entry point for MelonLoader.
         * </summary>
         */
        public override void OnPreInitialization() {
            PatchAll();
        }
    }
}
