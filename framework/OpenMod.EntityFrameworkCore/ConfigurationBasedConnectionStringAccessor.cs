using Microsoft.Extensions.Configuration;
using OpenMod.API;

namespace OpenMod.EntityFrameworkCore
{
    [OpenModInternal]
    public class ConfigurationBasedConnectionStringAccessor : IConnectionStringAccessor
    {
        private readonly IConfiguration m_Configuration;

        public ConfigurationBasedConnectionStringAccessor(IConfiguration configuration)
        {
            m_Configuration = configuration.GetSection("database");
        }

        public string? GetConnectionString(string name)
        {
            return m_Configuration.GetConnectionString(name);
        }
    }
}