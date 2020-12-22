using System;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenMod.Rust.Oxide.Events;
using Oxide.Core.Plugins;

namespace OpenMod.Rust.Oxide.Tests.Events
{
    [TestClass]
    public class OxideEventsListenersTests
    {
        // Name in the Oxide.Core.Plugins.HookMethodAttribute should be the same as the method's attribute attached to.
        // This test was added to prevent copy-paste typos.
        [TestMethod]
        public void HookSubscriber_ShouldHaveSameName_AsHookingMethod()
        {
            var types = OxideEventsActivator.FindListenerTypes();

            foreach (Type type in types)
            {
                var methods = type.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

                foreach (MethodInfo methodInfo in methods)
                {
                    var attributes = methodInfo.GetCustomAttributes<HookMethodAttribute>(true);

                    foreach (HookMethodAttribute attribute in attributes)
                    {
                        Assert.AreEqual(methodInfo.Name, attribute.Name,
                            "Method name and hook name mismatch in " + type.FullName);
                    }
                }
            }
        }
    }
}
