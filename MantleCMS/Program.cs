using System.Globalization;
using Autofac;
using Extenso.AspNetCore.Mvc.ExtensoUI;
using Extenso.AspNetCore.Mvc.ExtensoUI.Providers;
using Mantle.Identity.Services;
using Mantle.Infrastructure.Autofac;
using Mantle.Tenants.Entities;
using Mantle.Web.CommonResources.Infrastructure;
using Mantle.Web.Infrastructure;
using Mantle.Web.Mvc.EmbeddedResources;
using Mantle.Web.Mvc.Razor;
using Mantle.Web.Mvc.Routing;
using Mantle.Web.Tenants;
using MantleCMS.Identity;
using MantleCMS.Services;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation;
using Microsoft.AspNetCore.OData;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using NLog.Web;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .SetBasePath(builder.Environment.ContentRootPath)
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true);

// Configure logging
builder.Logging.ClearProviders();
builder.Logging.AddDebug();
builder.Logging.SetMinimumLevel(LogLevel.Trace);
builder.Host.UseNLog();

//if (builder.Environment.IsDevelopment())
//{
//    builder.Configuration.AddUserSecrets<Program>(optional: true);
//}

// Add services to the container.
builder.Services.AddApplicationInsightsTelemetry(options =>
    options.DeveloperMode = builder.Environment.IsDevelopment());

if (DataSettingsHelper.IsDatabaseInstalled)
{
    var dataSettings = DataSettingsManager.LoadSettings();
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseSqlServer(dataSettings.ConnectionString));
}
else
{
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseInMemoryDatabase("MantleCMS"));
}

if (builder.Environment.IsDevelopment())
{
    builder.Services.AddDatabaseDeveloperPageExceptionFilter();
}

// This must be added BEFORE we call AddIdentity.
builder.Services.AddScoped(typeof(IRoleValidator<ApplicationRole>), typeof(ApplicationRoleValidator));

#region Account / Identity

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/account/login";
    options.LogoutPath = "/account/log-off";
    options.AccessDeniedPath = "/account/access-denied";
});

builder.Services.AddIdentity<ApplicationUser, ApplicationRole>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddUserStore<ApplicationUserStore>()
    .AddRoleStore<ApplicationRoleStore>()
    .AddDefaultTokenProviders();

#endregion Account / Identity

// Add application services.
builder.Services.AddTransient<IEmailSender, AuthMessageSender>();
builder.Services.AddTransient<ISmsSender, AuthMessageSender>();
builder.Services.AddSingleton<IConfiguration>(builder.Configuration);

builder.Services
    .AddMemoryCache()
    .AddDistributedMemoryCache();

// Peachpie needs this
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
});

builder.Services.AddCors(options => options.AddPolicy("AllowAll", p => p
    .AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader()));

builder.Services.AddRouting(routeOptions =>
{
    routeOptions.AppendTrailingSlash = true;
    routeOptions.LowercaseUrls = true;
});

// Framework
builder.Services.AddMultitenancy<Tenant, MantleTenantResolver>();
builder.Services.AddMantleLocalization();

var mvcBuilder = builder.Services.AddControllersWithViews()
    .AddNewtonsoftJson(jsonOptions =>
    {
        if (builder.Environment.IsDevelopment())
        {
            // Make JSON easier to read for debugging at the expense of larger payloads
            jsonOptions.SerializerSettings.Formatting = Formatting.Indented;
        }

        // Omit nulls to reduce payload size
        jsonOptions.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
        jsonOptions.SerializerSettings.DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate;

        // Explicitly define behavior when serializing DateTime values
        jsonOptions.SerializerSettings.DateFormatString = "yyyy'-'MM'-'dd'T'HH':'mm':'ssK"; // Only return DateTimes to a 1 second precision
    })
    .AddRazorRuntimeCompilation()
    .AddOData((options, serviceProvider) =>
    {
        options.Select().Expand().Filter().OrderBy().SetMaxTop(null).Count();

        var registrars = serviceProvider.GetRequiredService<IEnumerable<IODataRegistrar>>();
        foreach (var registrar in registrars)
        {
            registrar.Register(options);
        }
    });

var assemblies = AppDomain.CurrentDomain.GetAssemblies()
    .Where(a => a.ExportedTypes.Any(t => t.GetInterfaces().Contains(typeof(IEmbeddedFileProviderRegistrar))));

// View Components won't work unless we do this.
foreach (var assembly in assemblies)
{
    mvcBuilder.AddApplicationPart(assembly);
}

builder.Services.AddRazorPages();
builder.Services.AddResponsiveFileManager(options => options.MaxSizeUpload = 32);

#region RequestLocalizationOptions

builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    IList<CultureInfo> supportedCultures = null;

    if (supportedCultures.IsNullOrEmpty())
    {
        supportedCultures =
        [
            new CultureInfo("en-US"),
        ];
    }

    options.DefaultRequestCulture = new RequestCulture(culture: "en-US", uiCulture: "en-US");
    options.SupportedCultures = supportedCultures;
    options.SupportedUICultures = supportedCultures;
});

#endregion RequestLocalizationOptions

#region RazorViewEngineOptions

builder.Services.Configure<MvcRazorRuntimeCompilationOptions>(options =>
{
    options.FileProviders.Add(new EmbeddedViewFileProvider());

    var embeddedFileProviders = DependoResolver.Instance.ResolveAll<IEmbeddedFileProviderRegistrar>()
        .SelectMany(x => x.EmbeddedFileProviders);

    foreach (var embeddedFileProvider in embeddedFileProviders)
    {
        options.FileProviders.Add(embeddedFileProvider);
    }
});

builder.Services.Configure<RazorViewEngineOptions>(options => options.ViewLocationExpanders.Add(new TenantViewLocationExpander()));

#endregion RazorViewEngineOptions

builder.Services.AddHttpContextAccessor();
builder.Services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
builder.Services.ConfigureMantleOptions(builder.Configuration);
builder.Services.ConfigureMantleCommonResourceOptions(builder.Configuration);
builder.Services.AddMantleWebOptimizer(builder.Configuration);

// Configure Autofac
builder.Host.UseServiceProviderFactory(new MantleDependoAutofacServiceProviderFactory());

var app = builder.Build();

//// Configure NLog with database connection after DI is ready
//NLogStartup.ConfigureNLog(app.Services);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
}

app.UseCors("AllowAll");

var requestLocalizationOptions = app.Services.GetService<IOptions<RequestLocalizationOptions>>();
app.UseRequestLocalization(requestLocalizationOptions.Value);

app.UseSession();

#region Static Files

app.UseDefaultFiles(); // For PeachPie

// embedded files
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new CompositeFileProvider(
        new EmbeddedScriptFileProvider(),
        new EmbeddedContentFileProvider(),
        app.Environment.WebRootFileProvider),
    OnPrepareResponse = (context) =>
    {
        var headers = context.Context.Response.GetTypedHeaders();
        headers.CacheControl = new CacheControlHeaderValue
        {
            MaxAge = TimeSpan.FromDays(7)
        };
    }
});

// plugins
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "Plugins")),
    RequestPath = new PathString("/Plugins"),
    OnPrepareResponse = ctx => ctx.Context.Response.Headers.Append(HeaderNames.CacheControl, "public,max-age=604800")
});

//// Add support for node_modules but only during development
//if (env.IsDevelopment())
//{
//    app.UseStaticFiles(new StaticFileOptions
//    {
//        FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), @"node_modules")),
//        RequestPath = new PathString("/vendor")
//    });
//}

#endregion Static Files

// PeachPie / Responsive File Manager
app.UseResponsiveFileManager();

app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});

// Use odata route debug, /$odata
app.UseODataRouteDebug();

// Add OData /$query middleware
app.UseODataQueryRequest();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseMultitenancy<Tenant>();
app.UseWebOptimizer();
app.UseExtensoUI<Bootstrap5UIProvider>();

app.MapControllerRoute(
    name: "areaRoute",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

var routePublisher = app.Services.GetRequiredService<IRoutePublisher>();
routePublisher.RegisterEndpoints(app);

app.Run();