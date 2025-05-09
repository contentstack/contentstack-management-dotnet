﻿using System;
using System.Reflection;
using System.Diagnostics;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Contentstack.Management.Core.Unit.Tests, PublicKey=0024000004800000940000000602000000240000525341310004000001000100875b827dbdbc1ad6c58e621a0be924edcd15521351576d70f7133cacbe16828f7c20121e7241d02a6ace417ddf516969cac84ee388fbcf150afbdc0ce8838f58504df23d22d066bf8bbe2adc18da247752a5ad6016a84961508bc03f1b90953bea883f88d9a34aac9c05bfc62845c294ab7aa50e6a1c03f2b29164f8371d74ac")]
[assembly: InternalsVisibleTo("Contentstack.Management.Core.Tests, PublicKey=0024000004800000940000000602000000240000525341310004000001000100875b827dbdbc1ad6c58e621a0be924edcd15521351576d70f7133cacbe16828f7c20121e7241d02a6ace417ddf516969cac84ee388fbcf150afbdc0ce8838f58504df23d22d066bf8bbe2adc18da247752a5ad6016a84961508bc03f1b90953bea883f88d9a34aac9c05bfc62845c294ab7aa50e6a1c03f2b29164f8371d74ac")]
namespace Contentstack.Management.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class CsmJsonConverterAttribute : Attribute
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
        public CsmJsonConverterAttribute(string name, bool isAutoloadEnable = true)
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
                    GetType(assembly, result);
                }
                _types[attribute] = result;
            }
            return _types[attribute].ToArray();
        }

        private static void GetType(Assembly assembly, List<Type> types)
        {
            try
            {
                foreach (Type type in assembly.GetTypes())
                {
                    foreach (var attr in type.GetCustomAttributes(typeof(CsmJsonConverterAttribute)))
                    {
                        CsmJsonConverterAttribute ctdAttr = attr as CsmJsonConverterAttribute;
                        Trace.Assert(ctdAttr != null, "cast is null");
                        if (ctdAttr.isAutoloadEnable)
                        {
                            types.Add(type);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
    
}
