using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace OpenMod.Common.Helpers
{
    public static class ReflectionExtensions
    {
        private static readonly Regex s_VersionRegex = new("Version=(?<version>.+?), ", RegexOptions.Compiled);

        // ReSharper disable once UnusedMember.Global
        public static MethodBase? GetCallingMethod(Type[]? skipTypes = null, MethodBase[]? skipMethods = null, bool applyAsyncMethodPatch = true)
        {
            var skipList = new List<Type>(skipTypes ?? Type.EmptyTypes) { typeof(ReflectionExtensions) };

            var st = new StackTrace();
            var frameTarget = (StackFrame?)null;
            for (var i = 0; i < st.FrameCount; i++)
            {
                var frame = st.GetFrame(i);
                var frameMethod = frame.GetMethod();
                if (frameMethod == null)
                    continue;

                // Hot fix for async Task methods:
                // If current frame method is called "MoveNext" and parent frame is from "AsyncMethodBuilderCore" type
                //   it's an async method wrapper, so we need to skip these two frames to get the original calling async method
                // Tested on .NET Core 2.1; should be tested on full .NET and mono too
                if (applyAsyncMethodPatch && frameMethod is MethodInfo {Name: "MoveNext"})
                {
                    var tmpIndex = i;
                    var frameOriginal = frame;

                    frame = st.GetFrame(++tmpIndex);
                    frameMethod = frame.GetMethod();

                    // Check parent frame - if its from AsyncMethodBuilderCore, its definitely an async Task
                    if (frameMethod is MethodInfo { DeclaringType.Name: "AsyncMethodBuilderCore" or "AsyncTaskMethodBuilder" })
                    {
                        frame = st.GetFrame(++tmpIndex);
                        frameMethod = frame.GetMethod();

                        i = tmpIndex;
                    }
                    else
                    {
                        //Restore original frame
                        frame = frameOriginal;
                        frameMethod = frameOriginal.GetMethod();
                    }
                }

                if (skipList.Any(c => c == frameMethod?.DeclaringType))
                    continue;

                if (skipMethods?.Any(c => c == frameMethod) ?? false)
                    continue;

                frameTarget = frame;
                break;
            }

            return frameTarget?.GetMethod();
        }

        // ReSharper disable once UnusedMember.Global
        public static MethodBase? GetCallingMethod(params Assembly[] skipAssemblies)
        {
            var st = new StackTrace();
            var frameTarget = (StackFrame?)null;
            for (var i = 0; i < st.FrameCount; i++)
            {
                var frame = st.GetFrame(i);
                if (skipAssemblies.Any(c => Equals(c, frame.GetMethod()?.DeclaringType?.Assembly)))
                    continue;

                frameTarget = frame;
            }

            return frameTarget?.GetMethod();
        }

        // ReSharper disable once UnusedMember.Global
        public static IEnumerable<Type> GetTypeHierarchy(this Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            var types = new List<Type> { type };
            while ((type = type.BaseType!) != null)
            {
                types.Add(type);
            }

            return types;
        }

        public static IEnumerable<Type> FindAllTypes(this Assembly assembly, bool includeAbstractAndInterfaces = false)
        {
            if (assembly == null)
            {
                throw new ArgumentNullException(nameof(assembly));
            }

            var types = assembly.GetLoadableTypes();
            return includeAbstractAndInterfaces ? types : types.Where(t => !t.IsAbstract && !t.IsInterface);
        }

        public static IEnumerable<Type> FindTypes<T>(this Assembly assembly, bool includeAbstractAndInterfaces = false)
        {
            var wantedType = typeof(T);
            var types = assembly.FindAllTypes(includeAbstractAndInterfaces);
            return types.Where(t => t.IsAssignableFrom(wantedType) || t.GetInterfaces().Any(x => x == wantedType));
        }

        public static IEnumerable<Type> FindTypes<T>(this IEnumerable<Type> types, bool includeAbstractAndInterfaces = false)
        {
            var wantedType = typeof(T);
            var validTypes = types.Where(type => includeAbstractAndInterfaces || (!type.IsAbstract && !type.IsInterface));
            return validTypes.Where(type => type.IsAssignableFrom(wantedType) || type.GetInterfaces().Any(x => x == wantedType));
        }

        // ReSharper disable once UnusedMember.Global
        public static string GetVersionIndependentName(string assemblyName)
        {
            return GetVersionIndependentName(assemblyName, out _);
        }

        public static string GetVersionIndependentName(string assemblyName, out string extractedVersion)
        {
            if (assemblyName == null)
            {
                throw new ArgumentNullException(nameof(assemblyName));
            }

            var match = s_VersionRegex.Match(assemblyName);
            extractedVersion = match.Groups[1].Value;
            return s_VersionRegex.Replace(assemblyName, string.Empty);
        }

        // ReSharper disable once UnusedMember.Global
        public static string GetDebugName(this MethodBase mb)
        {
            if (mb == null)
            {
                throw new ArgumentNullException(nameof(mb));
            }

            if (mb is MemberInfo { DeclaringType: not null } mi)
            {
                return $"{mi.DeclaringType.Name}.{mi.Name}";
            }

            return $"<anonymous>#{mb.Name}";
        }

        public static Task InvokeWithTaskSupportAsync(this MethodBase method, object? instance, object?[] @params)
        {
            if (method == null)
            {
                throw new ArgumentNullException(nameof(method));
            }

            var isAsync = false;
            if (method is MethodInfo methodInfo)
            {
                var returntype = methodInfo.ReturnType;
                isAsync = typeof(Task).IsAssignableFrom(returntype);
            }

            if (isAsync)
            {
                return (Task)method.Invoke(instance, @params);
            }

            method.Invoke(instance, @params);
            return Task.CompletedTask;
        }

        public static T ToObject<T>(this Dictionary<object, object> dict)
        {
            if (dict == null)
            {
                throw new ArgumentNullException(nameof(dict));
            }

            const BindingFlags bindingFlags =
                BindingFlags.Instance
                | BindingFlags.Public
                | BindingFlags.IgnoreCase;

            var type = typeof(T);
            var obj = Activator.CreateInstance(type);

            foreach (var kv in dict)
            {
                var prop = type.GetProperty(kv.Key.ToString(), bindingFlags);
                if (prop != null)
                {
                    prop.SetValue(obj, kv.Value);
                    continue;
                }

                var field = type.GetField(kv.Key.ToString(), bindingFlags);
                field?.SetValue(obj, kv.Value);
            }

            return (T)obj;
        }

        public static bool HasConversionOperator(this Type from, Type to)
        {
            if (from == null)
            {
                throw new ArgumentNullException(nameof(from));
            }

            if (to == null)
            {
                throw new ArgumentNullException(nameof(to));
            }

            UnaryExpression BodyFunction(Expression body) => Expression.Convert(body, to);
            var inp = Expression.Parameter(from, "inp");
            try
            {
                // If this succeeds then we can cast 'from' type to 'to' type using implicit coercion
                Expression.Lambda(BodyFunction(inp), inp).Compile();
                return true;
            }
            catch (InvalidOperationException)
            {
                return false;
            }
        }

        public static IEnumerable<Type> GetParametersTypes(this ParameterInfo[] parameterInfos)
        {
            return parameterInfos.Select(x => x.ParameterType);
        }
    }
}