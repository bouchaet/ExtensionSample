using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Server.Adapters
{
    public class PluginManager
    {
        private readonly IDynamicLoader _loader;
        private const string DllExtension = ".dll";
        private const string ExtensionComponent = "Extension";

        public PluginManager(IDynamicLoader loader)
        {
            _loader = loader;
        }

        private static IEnumerable<string> GetDllFilenames()
        {
            return Directory.GetFiles(AppContext.BaseDirectory)
                .Select(f => new FileInfo(f))
                .Where(info => info.Name.EndsWith(DllExtension)
                            && info.Name.Contains(ExtensionComponent))
                .Select(info => info.FullName);
        }

        public IEnumerable<T> Load<T>() where T : class
        {
            return GetDllFilenames()
                .SelectMany(filename => _loader.GetClass<T>(filename));
        }
    }
}