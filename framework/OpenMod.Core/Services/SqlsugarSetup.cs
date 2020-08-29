using Microsoft.Extensions.DependencyInjection;
using OpenMod.Core.Helpers;
using SqlSugar;
using System;

namespace OpenMod.Core.Services
{
    public static class SqlsugarSetup
    {
        public static void AddSqlsugarSetup(this IServiceCollection services)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));
            /*  
             *  这里采取单例模式
             *  有事务操作采用Scope
            */
            services.AddSingleton<ISqlSugarClient>(o =>
            {
                string DbType = Appsettings.app(new string[] { "SqlSugar", "DbType" });
                string ConnectionString = Appsettings.app(new string[] { "SqlSugar", "ConnectionString" });

                Enum.TryParse(DbType,out DbType dbType);

                ConnectionConfig connectionConfig = new ConnectionConfig();
                connectionConfig.ConnectionString = ConnectionString;
                connectionConfig.DbType = dbType;//设置数据库类型
                connectionConfig.IsAutoCloseConnection = true;//自动释放数据务，如果存在事务，在事务结束后释放
                connectionConfig.InitKeyType = InitKeyType.Attribute; //从实体特性中读取主键自增列信息

                connectionConfig.ConfigureExternalServices = new ConfigureExternalServices()
                {
                    /*
                     * Redis 注入
                     * DataInfoCacheService = new RedisCache()
                    */
                };
                return new SqlSugarClient(connectionConfig);
            });

        }
    }
}
