using System.Reflection;
using log4net;
using log4net.Config;
using Microsoft.EntityFrameworkCore;
using DataWarehouse.API.Data;
using DataWarehouse.API.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddLog4Net("log4net.config");

// Load appsettings.json & environment config
builder.ConfigureAppSettings();

// --- DB: register DbContext (so repos can inject ApplicationDbContext directly)
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    var cs = builder.Configuration.GetConnectionString("DefaultConnection")
             ?? throw new InvalidOperationException("Missing connection string: DefaultConnection");
    options.UseNpgsql(cs);
});

// App services
builder.Services.RegisterCoreServices(builder.Configuration);
builder.Services.RegisterRepositories();
builder.Services.RegisterBusinessServices();
builder.Services.RegisterJwtAuthentication(builder.Configuration);
builder.Services.RegisterCors();

// Log4Net setup (optional legacy init; safe to keep)
var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config"));

var app = builder.Build();

// Dev env middleware
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseCors("AllowFrontendOnly");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapGet("/", () => "Hello from DataWarehouse!");

app.Run();

public partial class Program { }