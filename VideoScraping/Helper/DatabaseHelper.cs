using Serilog;
using SqlSugar;

namespace VideoScraping.Helper;

public static class DatabaseHelper
{
    public static ISqlSugarClient UserDatabase
    {
        get
        {
            var logger = Log.ForContext<ISqlSugarClient>();
            var cache = new SugarCache();
            var sqlSugar = new SqlSugarClient(new ConnectionConfig()
                {
                    DbType = DbType.Sqlite,
                    ConnectionString = "DataSource=user.db",
                    IsAutoCloseConnection = true,
                    ConfigureExternalServices = new ConfigureExternalServices()
                    {
                        DataInfoCacheService = cache,
                    },
                    MoreSettings = new ConnMoreSettings()
                    {
                        IsAutoRemoveDataCache = true,
                    }
                },
                db => { db.Aop.OnLogExecuting = (sql, pars) => { logger.Information(sql); }; });
            return sqlSugar;
        }
    }

    public static ISqlSugarClient SystemDatabase
    {
        get
        {
            var logger = Log.ForContext<ISqlSugarClient>();
            var sqlSugar = new SqlSugarClient(new ConnectionConfig()
                {
                    DbType = DbType.Sqlite,
                    ConnectionString = "DataSource=system.db",
                    IsAutoCloseConnection = true
                },
                db => { db.Aop.OnLogExecuting = (sql, pars) => { logger.Information(sql); }; });
            return sqlSugar;
        }
    }
}