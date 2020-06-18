using Rocket.Core.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Rocket.Core.Utils
{
    public static class RocketHelper
    {
        public static bool IsUri(string uri) {
            if (String.IsNullOrEmpty(uri)) return false;
            Uri uriOut = null;
            if (Uri.TryCreate(uri, UriKind.Absolute, out uriOut) && (uriOut.Scheme == Uri.UriSchemeHttp || uriOut.Scheme == Uri.UriSchemeHttps))
            {
                return true;
            }
            return false;
        }

        public static List<Type> GetTypes(List<Assembly> assemblies)
        {
            List<Type> allTypes = new List<Type>();
            foreach (Assembly assembly in assemblies)
            {
                Type[] types;
                try
                {
                    types = assembly.GetTypes();
                }
                catch (ReflectionTypeLoadException e)
                {
                    types = e.Types;
                }
                allTypes.AddRange(types);
            }
            return allTypes;
        }

        public static List<Type> GetTypesFromParentClass(Assembly assembly, Type parentClass)
        {
            List<Type> allTypes = new List<Type>();
            Type[] types;
            try
            {
                types = assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException e)
            {
                types = e.Types;
            }
            foreach (Type type in types.Where(t => t != null))
            {
                if (type.IsSubclassOf(parentClass))
                {
                    allTypes.Add(type);
                }
            }
            return allTypes;
        }

        public static List<Type> GetTypesFromParentClass(List<Assembly> assemblies, Type parentClass)
        {
            List<Type> allTypes = new List<Type>();
            foreach (Assembly assembly in assemblies)
            {
                allTypes.AddRange(GetTypesFromParentClass(assembly, parentClass));
            }
            return allTypes;
        }

        public static List<Type> GetTypesFromInterface(List<Assembly> assemblies, string interfaceName)
        {
            List<Type> allTypes = new List<Type>();
            foreach (Assembly assembly in assemblies)
            {
                allTypes.AddRange(GetTypesFromInterface(assembly, interfaceName));
            }
            return allTypes;
        }

        public static List<Type> GetTypesFromInterface(Assembly assembly, string interfaceName)
        {
            List<Type> allTypes = new List<Type>();
            Type[] types;
            try
            {
                types = assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException e)
            {
                types = e.Types;
            }
            foreach (Type type in types.Where(t => t != null))
            {
                if (type.GetInterface(interfaceName) != null)
                {
                    allTypes.Add(type);
                }
            }
            return allTypes;
        }
    }
}
