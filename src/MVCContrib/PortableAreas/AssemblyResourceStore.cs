using System;
using System.Collections.Generic;
using System.IO;

namespace MvcContrib.PortableAreas
{
    /// <summary>
    /// Stores all the embedded resources for a single assembly.
    /// </summary>
    public class AssemblyResourceStore
    {
        private readonly Dictionary<string, string> resources;
        private readonly Type typeToLocateAssembly;

        public AssemblyResourceStore(Type type)
        {
            this.typeToLocateAssembly = type;

            var resourceNames = type.Assembly.GetManifestResourceNames();
            resources = new Dictionary<string, string>(resourceNames.Length);
            foreach (var name in resourceNames)
            {
                resources.Add(name.ToLower(), name);
            }
        }

        public Stream GetResourceStream(string resourceName)
        {
            string actualResourceName = null;
            if (resources.TryGetValue(resourceName.ToLower(), out actualResourceName))
            {
                return this.typeToLocateAssembly.Assembly.GetManifestResourceStream(actualResourceName);
            }
            else
            {
                return null;
            }
        }
    }
}
