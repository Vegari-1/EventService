using Microsoft.EntityFrameworkCore;
using OpenTracing;
using Jaeger.Reporters;
using Jaeger;
using Jaeger.Senders.Thrift;
using Jaeger.Samplers;
using OpenTracing.Contrib.NetCore.Configuration;
using OpenTracing.Util;
using Prometheus;
using BusService;
using EventService.Messaging;
using EventService.Repository;
using EventService.Repository.Interface;
using EventService.Service.Interface;

var builder = WebApplication.CreateBuilder(args);

// Default Logger
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

// Environment variables
builder.Configuration.AddEnvironmentVariables();

// Nats
builder.Services.AddSingleton<IMessageBusService, MessageBusService>();
builder.Services.Configure<MessageBusSettings>(builder.Configuration.GetSection("Nats"));
builder.Services.AddHostedService<EventMessageBusService>();

// Postgres
// DB_HOST from Docker-Compose or Local if null
var dbHost = Environment.GetEnvironmentVariable("DB_HOST");
if (dbHost == null)
    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseNpgsql(
            builder.Configuration.GetConnectionString("DislinktDbConnection"),
            x => x.MigrationsHistoryTable("__MigrationsHistory", "events")));
else
    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseNpgsql(dbHost, x => x.MigrationsHistoryTable("__MigrationsHistory", "events")));
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

// Repositories
builder.Services.AddScoped<IEventRepository, EventRepository>();

// Services
builder.Services.AddScoped<IEventService, EventService.Service.EventService>();

// Sync services
builder.Services.AddScoped<IEventSyncService, EventService.Service.EventSyncService>();

builder.Services.AddControllers();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "EventService", Version = "v1" });
});

builder.Services.AddOpenTracing();

builder.Services.AddSingleton<ITracer>(sp =>
{
    var serviceName = sp.GetRequiredService<IWebHostEnvironment>().ApplicationName;
    var loggerFactory = sp.GetRequiredService<ILoggerFactory>();
    var reporter = new RemoteReporter.Builder()
                    .WithLoggerFactory(loggerFactory)
                    .WithSender(new UdpSender("host.docker.internal", 6831, 0))
                    .Build();
    var tracer = new Tracer.Builder(serviceName)
        // The constant sampler reports every span.
        .WithSampler(new ConstSampler(true))
        // LoggingReporter prints every reported span to the logging framework.
        .WithLoggerFactory(loggerFactory)
        .WithReporter(reporter)
        .Build();

    GlobalTracer.Register(tracer);

    return tracer;
});

builder.Services.Configure<HttpHandlerDiagnosticOptions>(options =>
        options.OperationNameResolver =
            request => $"{request.Method.Method}: {request?.RequestUri?.AbsoluteUri}");

builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(80);
});

var app = builder.Build();

// Run all migrations if Docker container
if (dbHost != null)
    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;

        var context = services.GetRequiredService<AppDbContext>();
        context.Database.Migrate();
    }

// Configure the HTTP request pipeline.
if (builder.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "ProfileService v1"));
}

app.MapControllers();

// Prometheus metrics
app.UseMetricServer();

app.Run();

namespace EventService
{
    public partial class Program { }
}
