using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Entities;
using Server.Adapters;

namespace Server.Details
{
    internal class DynamicLoader : IDynamicLoader
    {
        public IEnumerable<T> GetClass<T>(string filename) where T: class
        {
            if(!typeof(T).IsInterface)
                throw new ArgumentException("T is not an interface.");

            Assembly lib;
            using (var fs = File.Open(filename, FileMode.Open))
            {
                using (var ms = new MemoryStream())
                {
                    var buffer = new byte[1024];
                    int read;
                    while ((read = fs.Read(buffer, 0, 1024)) > 0)
                        ms.Write(buffer, 0, read);
                    lib = Assembly.Load(ms.ToArray());
                }
            }

            foreach (var type in lib.GetExportedTypes())
            {
                if (type.GetInterface(typeof(T).Name) != null)
                    yield return (T)Activator.CreateInstance(type);
            }
        }
    }
}
