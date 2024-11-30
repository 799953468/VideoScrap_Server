using Serilog;
using Serilog.Events;
using SqlSugar;
using VideoScraping.DTO;
using VideoScraping.Entity;
using VideoScraping.Helper;
using VideoScraping.MediaInfo;
using VideoScraping.Sync;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
    .Enrich.FromLogContext()
    .Enrich.WithThreadId()
    .Enrich.WithThreadName()
    .WriteTo.Console(outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{SourceContext}] [{Level:u3}] [{ThreadId}] {Message:lj} {NewLine}{Exception}")
    .CreateBootstrapLogger();
builder.Host.UseSerilog();

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddMemoryCache();
builder.Services.Configure<AppSetting>(builder.Configuration.GetSection("App"));
builder.Services.AddTransient<CacheManager>();
builder.Services.AddSingleton<TheMovieDbServer>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ISqlSugarClient>(s =>
{
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
        db => { db.Aop.OnLogExecuting = (sql, pars) => { Log.Information(sql); }; });
    return sqlSugar;
});
builder.Services.AddHostedService<SyncTaskService>();

var app = builder.Build();
ServiceLocator.Instance = app.Services;

// init database
Log.Information("Init Database");
using (var serviceScope = app.Services.CreateScope())
{

    var db = DatabaseHelper.UserDatabase;
    db.CodeFirst.InitTables<SyncEntity>();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();