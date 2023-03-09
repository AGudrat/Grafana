using Prometheus;
using Serilog;
using Serilog.Formatting.Compact;
using Serilog.Sinks.Grafana.Loki;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var logger = Log.Logger = new LoggerConfiguration()
                                .WriteTo.Console()
                                .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Information)
                                .WriteTo.Console(new RenderedCompactJsonFormatter())
                                .WriteTo.Seq("http://localhost:5341")
                                .WriteTo.GrafanaLoki("http://localhost:3100")
                                .Enrich.FromLogContext()
                                .MinimumLevel.Information()
                                .CreateLogger();

builder.Logging.ClearProviders();
builder.Host.UseSerilog(logger);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseSerilogRequestLogging();
app.UseAuthorization();

app.MapControllers();

app.UseMetricServer();
app.UseHttpMetrics();

app.Run();
