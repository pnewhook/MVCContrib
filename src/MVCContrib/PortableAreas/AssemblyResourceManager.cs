using System;
using System.Collections.Generic;

namespace MvcContrib.PortableAreas
{
    /// <summary>
    /// Manages all .NET assemblies that have registered their embedded resources.
    /// </summary>
    public static class AssemblyResourceManager
    {
        private static Dictionary<string, AssemblyResourceStore> assemblyResourceStores = new Dictionary<string, AssemblyResourceStore>();

        public static System.IO.Stream GetResourceStream(string areaName, string resourceName)
        {
            var assemblyResourceStore = assemblyResourceStores[areaName.ToLower()];
            return assemblyResourceStore.GetResourceStream(resourceName);
        }

        public static void RegisterAreaResources(string areaName, Type type)
        {
            assemblyResourceStores.Add(areaName.ToLower(), new AssemblyResourceStore(type));
        }
    }
}
