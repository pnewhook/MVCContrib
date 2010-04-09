using System;
using System.Collections.Generic;
using System.IO;

namespace MvcContrib.UI.InputBuilder.ViewEngine
{
    /// <summary>
    /// Stores all the embedded resources for a single assembly/area.
    /// </summary>
    public class AssemblyResourceStore
    {
        private readonly Dictionary<string, string> resources;
        private readonly Type typeToLocateAssembly;
        private readonly string namespaceName;

        public string VirtualPath { get; private set; }

        public AssemblyResourceStore(Type typeToLocateAssembly, string virtualPath, string namespaceName)
        {
            this.typeToLocateAssembly = typeToLocateAssembly;
            // should we disallow an empty virtual path?
            this.VirtualPath = virtualPath.ToLower();
            this.namespaceName = namespaceName.ToLower();

            var resourceNames = this.typeToLocateAssembly.Assembly.GetManifestResourceNames();
            resources = new Dictionary<string, string>(resourceNames.Length);
            foreach (var name in resourceNames)
            {
                resources.Add(name.ToLower(), name);
            }
        }

        public Stream GetResourceStream(string resourceName)
        {
            var fullResourceName = GetFullyQualifiedTypeFromPath(resourceName);
            string actualResourceName = null;
            if (resources.TryGetValue(fullResourceName, out actualResourceName))
            {
                return this.typeToLocateAssembly.Assembly.GetManifestResourceStream(actualResourceName);
            }
            else
            {
                return null;
            }
        }

        public string GetFullyQualifiedTypeFromPath(string path)
        {
            string resourceName = path.ToLower().Replace("~", this.namespaceName);
            // we can make this more succinct if we don't have to check for emtpy virtual path (by preventing in constuctor)
            if (!string.IsNullOrEmpty(VirtualPath))
                resourceName = resourceName.Replace(VirtualPath, "");
            return resourceName.Replace("/", ".");
        }

        public bool IsPathResourceStream(string path)
        {
            var fullResourceName = GetFullyQualifiedTypeFromPath(path);
            return resources.ContainsKey(fullResourceName);
        }
    }
}