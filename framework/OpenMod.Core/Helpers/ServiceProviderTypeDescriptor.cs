using System;
using System.ComponentModel;
using Microsoft.Extensions.DependencyInjection;

namespace OpenMod.Core.Helpers
{
    public class ServiceProviderTypeDescriptor : ITypeDescriptorContext
    {
        //todo unimplemented stuff
        public ServiceProviderTypeDescriptor(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
        }

        public IServiceProvider ServiceProvider { get; }

        public object GetService(Type serviceType) => ServiceProvider.GetRequiredService(serviceType);

        public bool OnComponentChanging() => throw new NotImplementedException();

        public void OnComponentChanged()
        {
            throw new NotImplementedException();
        }

        public IContainer Container { get; set; }
        public object Instance { get; set; }
        public PropertyDescriptor PropertyDescriptor { get; }
    }
}