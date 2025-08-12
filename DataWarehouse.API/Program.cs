using System.Reflection;
using log4net;
using log4net.Config;
using Microsoft.EntityFrameworkCore;
using DataWarehouse.API.Data;
using DataWarehouse.API.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddLog4Net("log4net.config");

builder.ConfigureAppSettings();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.RegisterCoreServices(builder.Configuration);
builder.Services.RegisterRepositories();
builder.Services.RegisterBusinessServices();
builder.Services.RegisterJwtAuthentication(builder.Configuration);
builder.Services.RegisterCors(builder.Configuration);

builder.Services.RegisterGraphQl(builder.Configuration);

var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config"));

var app = builder.Build();

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

app.MapGraphQL("/graphql");

app.MapControllers();
app.MapGet("/", () => "Hello from DataWarehouse!");

app.Run();

public partial class Program { }