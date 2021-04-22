using System;
using System.Reflection;
using System.Threading.Tasks;
using Nito.AsyncEx;
using Serilog;

namespace OpenMod.Core.Helpers
{
    public static class AsyncHelper
    {
        /// <summary>
        ///     Checks if given method is an async method.
        /// </summary>
        /// <param name="method">A method to check</param>
        public static bool IsAsync(this MethodInfo method)
        {
            return method.ReturnType.IsTaskOrTaskOfT();
        }

        public static bool IsTaskOrTaskOfT(this Type type)
        {
            return type == typeof(Task) || type.GetTypeInfo().IsGenericType && type.GetGenericTypeDefinition() == typeof(Task<>);
        }

        /// <summary>
        ///     Returns void if given type is Task.
        /// Return T, if given type is Task{T}.
        /// Returns given type otherwise.
        /// </summary>
        public static Type UnwrapTask(Type type)
        {
            if (type == typeof(Task))
            {
                return typeof(void);
            }

            if (type.GetTypeInfo().IsGenericType && type.GetGenericTypeDefinition() == typeof(Task<>))
            {
                return type.GenericTypeArguments[0];
            }

            return type;
        }

        /// <summary>
        ///     Runs a async method synchronously.
        /// </summary>
        /// <param name="func">A function that returns a result</param>
        /// <typeparam name="TResult">Result type</typeparam>
        /// <returns>Result of the async operation</returns>
        public static TResult RunSync<TResult>(Func<Task<TResult>> func)
        {
            return AsyncContext.Run(func);
        }

        /// <summary>
        ///     Runs a async method synchronously.
        /// </summary>
        /// <param name="action">An async action</param>
        public static void RunSync(Func<Task> action)
        {
            AsyncContext.Run(action);
        }

        /// <summary>
        ///    Schedules a task on a different thread (fire and forget)
        /// </summary>
        /// <param name="name">The name of the task.</param>
        /// <param name="task">The task to run.</param>
        /// <param name="exceptionHandler">The optional exception handler.</param>
        public static void Schedule(string name, Func<Task> task, Action<Exception>? exceptionHandler = null)
        {
            Task.Run(async () =>
            {
                try
                {
                    await task();
                }
                catch (Exception e)
                {
                    if (exceptionHandler != null)
                    {
                        exceptionHandler(e);
                    }
                    else
                    {
                        Log.Error(e, "Exception occured in task \"{TaskName}\"", name);
                    }
                }
            });
        }
    }
}