using System;
using System.Reflection;
using System.Diagnostics;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Contentstack.Management.Core.Unit.Tests")]
[assembly: InternalsVisibleTo("Contentstack.Management.Core.Tests")]
namespace Contentstack.Management.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class CSMJsonConverterAttribute : Attribute
    {

        private readonly string name;
        private readonly bool isAutoloadEnable;
        private static ConcurrentDictionary<Type, List<Type>> _types = new ConcurrentDictionary<Type, List<Type>>();

        /// <summary>
        /// Name for the JsonConverter
        /// </summary>
        public string Name
        {
            get
            {
                return this.name;
            }
        }

        /// <summary>
        /// To enable autoload in ContentstackClient. Default is Enable.
        /// </summary>
        public bool IsAutoloadEnable
        {
            get
            {
                return this.isAutoloadEnable;
            }
        }

        /// <summary>
        /// CSMJsonConverterAttribute constructor
        /// </summary>
        /// <param name="name">Name for the JsonConverter</param>
        /// <param name="isAutoloadEnable"> To enable autoload in ContentstackClient. Default is Enable.</param>
        public CSMJsonConverterAttribute(string name, bool isAutoloadEnable = true)
        {
            this.name = name;
            this.isAutoloadEnable = isAutoloadEnable;
        }
        internal static IEnumerable<Type> GetCustomAttribute(Type attribute)
        {
            if (!_types.ContainsKey(attribute))
            {
                List<Type> result = new List<Type>();
                foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
                {
                    try
                    {
                        foreach (Type type in assembly.GetTypes())
                        {
                            var objectType = type.GetCustomAttributes(attribute, true);
                            foreach (var attr in type.GetCustomAttributes(typeof(CSMJsonConverterAttribute)))
                            {
                                CSMJsonConverterAttribute ctdAttr = attr as CSMJsonConverterAttribute;
                                Trace.Assert(ctdAttr != null, "cast is null");
                                if (ctdAttr.isAutoloadEnable)
                                {
                                    result.Add(type);
                                }
                            }
                        }
                    } catch {}
                }
                _types[attribute] = result;
            }
            return _types[attribute].ToArray();
        }
    }
    
}
