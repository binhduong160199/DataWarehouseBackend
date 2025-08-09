using System.Reflection;
using log4net;
using log4net.Config;
using Microsoft.EntityFrameworkCore;
using DataWarehouse.API.Data;
using DataWarehouse.API.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Load appsettings.json & environment config
builder.ConfigureAppSettings();

// Register services
builder.Services.RegisterCoreServices(builder.Configuration);
builder.Services.RegisterRepositories();
builder.Services.RegisterBusinessServices();
builder.Services.RegisterJwtAuthentication(builder.Configuration);
builder.Services.RegisterCors();

// Log4Net setup (optional)
var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config"));

// Register DbContext (use factory for testing/mocking if needed)
builder.Services.AddDbContextFactory<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// Dev environment middleware
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
app.UseMiddleware<GlobalExceptionMiddleware>();
app.UseCors("AllowFrontendOnly");
app.UseAuthentication();
app.UseMiddleware<RoleMiddleware>();
app.UseAuthorization();

app.MapControllers();
app.MapGet("/", () => "Hello from DataWarehouse!");

app.Run();

public partial class Program { } // Required for EF CLI tooling