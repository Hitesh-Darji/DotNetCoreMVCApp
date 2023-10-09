using log4net.Config;
using log4net;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.IO;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using DotNetCoreMVCApp.Repository;
using DotNetCoreMVCApp.Models.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using DotNetCoreMVCApp.Repository.Implementation;
using DotNetCoreMVCApp.Service.Abstraction;
using DotNetCoreMVCApp.Service.Implementation;
using AutoMapper;
using DotNetCoreMVCApp.Models.Web;

var builder = WebApplication.CreateBuilder(args);

//Add log4net as logging provider
System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config"));
var _logger = LogManager.GetLogger(typeof(Program));

_logger.Info("Logger registered");

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
_logger.Info($"connectionString: {connectionString}");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString), ServiceLifetime.Transient);

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
    options.SignIn.RequireConfirmedEmail = false;
    options.SignIn.RequireConfirmedPhoneNumber = false;
    options.Lockout.AllowedForNewUsers = false;
}).AddRoles<ApplicationRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddControllersWithViews(options => options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true).AddRazorRuntimeCompilation();

//Services - DI
builder.Services.AddTransient<ApplicationSeeder>();
builder.Services.AddTransient<UnitOfWork>();
builder.Services.AddTransient<ICountryService, CountryService>();

//Automapper
var config = new MapperConfiguration(cfg =>
{
    cfg.CreateMap<Country, CountryViewModel>().ReverseMap();
});
IMapper mapper = config.CreateMapper();
builder.Services.AddTransient<IMapper>(c => mapper);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

using (var scope = app.Services.CreateScope())
{
    var seeder = scope.ServiceProvider.GetRequiredService<ApplicationSeeder>();
    seeder?.Seed().Wait();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();
