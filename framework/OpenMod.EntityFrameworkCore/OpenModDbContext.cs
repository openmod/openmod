using Microsoft.EntityFrameworkCore;
using System;

namespace OpenMod.EntityFrameworkCore
{
    public abstract class OpenModDbContext<TSelf> : DbContext where TSelf : OpenModDbContext<TSelf>
    {
        private readonly IServiceProvider m_ServiceProvider;

        protected OpenModDbContext(IServiceProvider serviceProvider)
        {
            m_ServiceProvider = serviceProvider;
        }

        protected OpenModDbContext(DbContextOptions<TSelf> options, IServiceProvider serviceProvider) :
            base(options)
        {
            m_ServiceProvider = serviceProvider;
        }
    }
}