using Autofac;
using Extenso.AspNetCore.Mvc.ExtensoUI;
using Extenso.AspNetCore.Mvc.ExtensoUI.Providers;
using Mantle.Identity.Services;
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
using System.Globalization;

namespace MantleCMS;

public class Startup
{
    #region Constructor

    public Startup(IWebHostEnvironment env)
    {
        WebHostEnvironment = env;

        var builder = new ConfigurationBuilder()
            .SetBasePath(env.ContentRootPath)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);

        if (env.IsDevelopment())
        {
            // For more details on using the user secret store see https://go.microsoft.com/fwlink/?LinkID=532709
            builder.AddUserSecrets<Startup>(optional: true);

            // This will push telemetry data through Application Insights pipeline faster, allowing you to view results immediately.
            builder.AddApplicationInsightsSettings(developerMode: true);
        }

        builder.AddEnvironmentVariables();
        Configuration = builder.Build();
    }

    #endregion Constructor

    #region Properties

    public IConfigurationRoot Configuration { get; }

    public IWebHostEnvironment WebHostEnvironment { get; private set; }

    #endregion Properties

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddApplicationInsightsTelemetry(Configuration);

        if (DataSettingsHelper.IsDatabaseInstalled)
        {
            var dataSettings = DataSettingsManager.LoadSettings();
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(dataSettings.ConnectionString));
        }
        else
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseInMemoryDatabase("MantleCMS"));
        }

        services.AddDatabaseDeveloperPageExceptionFilter();

        // This must be added BEFORE we call AddIdentity().
        // For further info, see: https://github.com/aspnet/Identity/issues/1112
        services.AddScoped(typeof(IRoleValidator<ApplicationRole>), typeof(ApplicationRoleValidator));

        #region Account / Identity

        services.ConfigureApplicationCookie(options =>
        {
            options.LoginPath = "/account/login";
            options.LogoutPath = "/account/log-off";
            options.AccessDeniedPath = "/account/access-denied";
        });

        services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
        {
            options.SignIn.RequireConfirmedAccount = true;
        })
        .AddEntityFrameworkStores<ApplicationDbContext>()
        .AddUserStore<ApplicationUserStore>()
        .AddRoleStore<ApplicationRoleStore>()
        //.AddRoleValidator<ApplicationRoleValidator>()
        .AddDefaultTokenProviders();

        #endregion Account / Identity

        // Add application services.
        services.AddTransient<IEmailSender, AuthMessageSender>();
        services.AddTransient<ISmsSender, AuthMessageSender>();

        services.AddSingleton<IConfiguration>(Configuration);

        services
            .AddMemoryCache()
            .AddDistributedMemoryCache();

        // Peachpie needs this
        services.AddSession(options =>
        {
            options.IdleTimeout = TimeSpan.FromMinutes(30);
            options.Cookie.HttpOnly = true;
        });

        services.AddCors(options => options.AddPolicy("AllowAll", p => p
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader()));

        services.AddRouting((routeOptions) =>
        {
            routeOptions.AppendTrailingSlash = true;
            routeOptions.LowercaseUrls = true;
        });

        // Framework
        services.AddMultitenancy<Tenant, MantleTenantResolver>();

        services.AddMantleLocalization();

        var mvcBuilder = services.AddControllersWithViews()
            .AddNewtonsoftJson(jsonOptions => services.AddSingleton(ConfigureJsonSerializerSettings(jsonOptions)))
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

        services.AddRazorPages();

        services.AddResponsiveFileManager(options =>
        {
            options.MaxSizeUpload = 32;
        });

        #region RequestLocalizationOptions

        services.Configure<RequestLocalizationOptions>(
            options =>
            {
                IList<CultureInfo> supportedCultures = null;

                //TODO: How can we read languages table in DB and get list of culture codes from that for here? Dependencies maybe not registered yet...
                //  So not sure if can resolve ILanguageService here. Need to try.

                if (supportedCultures.IsNullOrEmpty())
                {
                    supportedCultures = new List<CultureInfo>
                    {
                        new CultureInfo("en-US"),
                        // Can add more if needed later...
                    };
                }

                options.DefaultRequestCulture = new RequestCulture(culture: "en-US", uiCulture: "en-US");
                options.SupportedCultures = supportedCultures;
                options.SupportedUICultures = supportedCultures;
            });

        #endregion RequestLocalizationOptions

        #region RazorViewEngineOptions

        services.Configure<MvcRazorRuntimeCompilationOptions>(options =>
        {
            //Add the file provider to the Razor view engine

            options.FileProviders.Add(new EmbeddedViewFileProvider()); // allow embedded views for special cases like dynamic settings in framework

            var embeddedFileProviders = EngineContext.Current.ResolveAll<IEmbeddedFileProviderRegistrar>()
                .SelectMany(x => x.EmbeddedFileProviders);

            foreach (var embeddedFileProvider in embeddedFileProviders)
            {
                options.FileProviders.Add(embeddedFileProvider);
            }
        });

        services.Configure<RazorViewEngineOptions>(options =>
        {
            options.ViewLocationExpanders.Add(new TenantViewLocationExpander());
        });

        #endregion RazorViewEngineOptions

        services.AddHttpContextAccessor();
        services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();

        services.ConfigureMantleOptions(Configuration);
        services.ConfigureMantleCommonResourceOptions(Configuration);
        services.AddMantleWebOptimizer(Configuration);
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IHostApplicationLifetime appLifetime)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
            app.UseMigrationsEndPoint();
            //app.UseBrowserLink();
        }
        else
        {
            app.UseExceptionHandler("/Home/Error");
        }

        app.UseCors("AllowAll");

        var requestLocalizationOptions = app.ApplicationServices.GetService<IOptions<RequestLocalizationOptions>>();
        app.UseRequestLocalization(requestLocalizationOptions.Value);

        app.UseSession();

        #region Static Files

        app.UseDefaultFiles(); // For PeachPie

        // embedded files
        app.UseStaticFiles(new StaticFileOptions
        {
            // Override file provider to allow embedded resources
            FileProvider = new CompositeFileProvider(
                new EmbeddedScriptFileProvider(),
                new EmbeddedContentFileProvider(),
                WebHostEnvironment.WebRootFileProvider),

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
            OnPrepareResponse = ctx =>
            {
                ctx.Context.Response.Headers.Append(HeaderNames.CacheControl, "public,max-age=604800");
                //if (!string.IsNullOrEmpty(nopConfig.StaticFilesCacheControl))
                //    ctx.Context.Response.Headers.Append(HeaderNames.CacheControl, mantleConfig.StaticFilesCacheControl);
            }
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

        app.UseForwardedHeaders(
            new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });

        // Use odata route debug, /$odata
        app.UseODataRouteDebug();

        // If you want to use /$openapi, enable the middleware.
        //app.UseODataOpenApi();

        // Add OData /$query middleware
        app.UseODataQueryRequest();

        // Add the OData Batch middleware to support OData $Batch
        //app.UseODataBatching();

        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseMultitenancy<Tenant>();

        app.UseWebOptimizer();

        app.UseExtensoUI<Bootstrap5UIProvider>();

        // Add external authentication middleware below. To configure them please see http://go.microsoft.com/fwlink/?LinkID=532715

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllerRoute(
                name: "areaRoute",
                pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

            endpoints.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            endpoints.MapRazorPages();

            var routePublisher = EngineContext.Current.Resolve<IRoutePublisher>();
            routePublisher.RegisterEndpoints(endpoints);
        });

        // If you want to dispose of resources that have been resolved in the
        // application container, register for the "ApplicationStopped" event.
        //appLifetime.ApplicationStopped.Register(() => EngineContext.Current.Dispose());

        ConfigureNLog();
    }

    public void ConfigureContainer(ContainerBuilder builder)
    {
        // Add extra registrations here, if needed...
        //  But it's better to use IDependencyRegistrar
    }

    private static void ConfigureNLog()
    {
        try
        {
            //TODO
            //var target = LogManager.Configuration.FindTargetByName("database");

            //DatabaseTarget databaseTarget = null;
            //var wrapperTarget = target as WrapperTargetBase;

            //// Unwrap the target if necessary.
            //if (wrapperTarget == null)
            //{
            //    databaseTarget = target as DatabaseTarget;
            //}
            //else
            //{
            //    databaseTarget = wrapperTarget.WrappedTarget as DatabaseTarget;
            //}

            ////databaseTarget.DBProvider = "Npgsql"; // If using other provider...

            //var dataSettings = EngineContext.Current.Resolve<DataSettings>();
            //databaseTarget.ConnectionString = dataSettings.ConnectionString;
        }
        catch { }
    }

    private JsonSerializerSettings ConfigureJsonSerializerSettings(MvcNewtonsoftJsonOptions options)
    {
        if (WebHostEnvironment.IsDevelopment())
        {
            // Make JSON easier to read for debugging at the expense of larger payloads
            options.SerializerSettings.Formatting = Formatting.Indented;
        }

        // Omit nulls to reduce payload size
        options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;

        options.SerializerSettings.DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate;

        // Explicitly define behavior when serializing DateTime values
        options.SerializerSettings.DateFormatString = "yyyy'-'MM'-'dd'T'HH':'mm':'ssK";   // Only return DateTimes to a 1 second precision

        return options.SerializerSettings;
    }
}