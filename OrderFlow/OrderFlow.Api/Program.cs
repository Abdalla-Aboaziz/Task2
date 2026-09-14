using FluentValidation;
using Hangfire;
using Hangfire.SqlServer;
using Mapster;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using OrderFlow.Application.Contracts;
using OrderFlow.Application.Features.Orders.CreateOrder;
using OrderFlow.Application.Mappings;
using OrderFlow.Infrastructure.BackgroundJobs;
using OrderFlow.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")!;

builder.Services.AddDbContext<OrderDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddScoped<IOrderDbContext>(sp => sp.GetRequiredService<OrderDbContext>());

builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(CreateOrderHandler).Assembly));

builder.Services.AddValidatorsFromAssembly(typeof(CreateOrderValidator).Assembly);

var mapsterConfig = TypeAdapterConfig.GlobalSettings;
mapsterConfig.Scan(typeof(MappingConfig).Assembly);
builder.Services.AddSingleton(mapsterConfig);
builder.Services.AddScoped<IMapper, ServiceMapper>();

builder.Services.AddHybridCache();

builder.Services.AddHangfire(config => config
    .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
    .UseSimpleAssemblyNameTypeSerializer()
    .UseRecommendedSerializerSettings()
    .UseSqlServerStorage(connectionString, new SqlServerStorageOptions
    {
        CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
        SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
        QueuePollInterval = TimeSpan.Zero,
        UseRecommendedIsolationLevel = true,
        DisableGlobalLocks = true
    }));

builder.Services.AddHangfireServer();

builder.Services.AddTransient<OrderProcessingJob>();
builder.Services.AddTransient<MaterializedViewRefreshJob>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<OrderDbContext>();
    db.Database.Migrate();
}

app.UseSwagger();
app.UseSwaggerUI();

app.UseHangfireDashboard("/hangfire");

RecurringJob.AddOrUpdate<OrderProcessingJob>(
    "process-pending-orders",
    job => job.ProcessPendingOrdersAsync(),
    "*/1 * * * *");

RecurringJob.AddOrUpdate<MaterializedViewRefreshJob>(
    "refresh-materialized-view",
    job => job.RefreshAsync(),
    "*/2 * * * *");

app.MapControllers();

app.Run();
