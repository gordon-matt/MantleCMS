using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using Extenso.Collections;
using Mantle.Identity.Services;
using Mantle.Infrastructure;
using Mantle.Tenants.Domain;
using Mantle.Web.Infrastructure;
using Mantle.Web.Messaging;
using Mantle.Web.Mvc.Assets;
using Mantle.Web.Mvc.EmbeddedResources;
using Mantle.Web.Mvc.Razor;
using Mantle.Web.Mvc.Routing;
using Mantle.Web.Tenants;
using MantleCMS.Data;
using MantleCMS.Data.Domain;
using MantleCMS.Identity;
using MantleCMS.Options;
using MantleCMS.Services;
using Microsoft.AspNet.OData.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using NLog;
using NLog.Extensions.Logging;
using NLog.Targets;
using NLog.Targets.Wrappers;
using NLog.Web;
using Pchp.Core;
using Peachpie.AspNetCore.Web;

namespace MantleCMS
{
    public class Startup
    {
        #region Constructor

        public Startup(IHostingEnvironment env)
        {
            HostingEnvironment = env;

            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);

            if (env.IsDevelopment())
            {
                // For more details on using the user secret store see https://go.microsoft.com/fwlink/?LinkID=532709
                builder.AddUserSecrets<Startup>();

                // This will push telemetry data through Application Insights pipeline faster, allowing you to view results immediately.
                builder.AddApplicationInsightsSettings(developerMode: true);
            }

            builder.AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        #endregion Constructor

        #region Properties

        public IConfigurationRoot Configuration { get; }

        public IHostingEnvironment HostingEnvironment { get; private set; }

        public IServiceProvider ServiceProvider { get; private set; }

        #endregion Properties

        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddApplicationInsightsTelemetry(Configuration);

            var connectionString = Configuration.GetConnectionString("DefaultConnection");

            // Add framework services.
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));

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

            services.AddIdentity<ApplicationUser, ApplicationRole>()
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

            services.AddMultitenancy<Tenant, MantleTenantResolver>();

            services.AddMantleLocalization();

            services.AddOData();

            var mvcBuilder = services.AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1)
                .AddJsonOptions((mvcJsonOptions) => services.AddSingleton(ConfigureJsonSerializerSettings(mvcJsonOptions)));

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

            services.Configure<RazorViewEngineOptions>(options =>
            {
                //Add the file provider to the Razor view engine

                options.FileProviders.Add(new EmbeddedViewFileProvider()); // allow embedded views for special cases like dynamic settings in framework

                var embeddedFileProviders = EngineContext.Current.ResolveAll<IEmbeddedFileProviderRegistrar>()
                    .SelectMany(x => x.EmbeddedFileProviders);

                foreach (var embeddedFileProvider in embeddedFileProviders)
                {
                    options.FileProviders.Add(embeddedFileProvider);
                }

                //options.FileProviders.Add(embeddedFileProvider);
                options.ViewLocationExpanders.Add(new TenantViewLocationExpander());
            });

            #endregion RazorViewEngineOptions

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();

            #region Mantle Framework Config

            ServiceProvider = services.ConfigureMantleServices(mvcBuilder.PartManager, Configuration);
            //MantleUISettings.DefaultAdminProvider = new SmartAdminUIProvider();

            // TODO: Use NPM for this: https://www.npmjs.com/search?q=grapesjs
            //  NPM PACKAGES: grapesjs, grapesjs-aviary and grapesjs-mjml
            MantleMessagingAssets.Init(new MantleMessagingAssets
            {
                GrapesJs = new AssetCollection
                {
                    Scripts = new List<Asset> { new Asset { Path = "/js/grapes.min.js" } },
                    Styles = new List<Asset> { new Asset { Path = "/css/grapes.min.css" } }
                },
                GrapesJsAviary = new AssetCollection
                {
                    Scripts = new List<Asset>
                    {
                        new Asset { Path = "http://feather.aviary.com/imaging/v3/editor.js" },
                        new Asset { Path = "/js/grapesjs-aviary.min.js" }
                    },
                },
                GrapesJsMjml = new AssetCollection
                {
                    Scripts = new List<Asset> { new Asset { Path = "/js/grapesjs-mjml.min.js" } },
                }
            });

            #endregion Mantle Framework Config

            return ServiceProvider;
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(
            IApplicationBuilder app,
            IHostingEnvironment env,
            ILoggerFactory loggerFactory,
            IApplicationLifetime appLifetime)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();
            loggerFactory.AddNLog();

            //app.AddNLogWeb();

            env.ConfigureNLog("NLog.config");

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
                //app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseCors("AllowAll");

            var requestLocalizationOptions = app.ApplicationServices.GetService<IOptions<RequestLocalizationOptions>>();
            app.UseRequestLocalization(requestLocalizationOptions.Value);

            #region PeachPie / Responsive File Manager

            // ================================================================================
            // Peachpie
            app.UseSession();

            var rfmOptions = new ResponsiveFileManagerOptions();
            Configuration.GetSection("ResponsiveFileManagerOptions").Bind(rfmOptions);

            string root = Path.Combine(new FileInfo(Assembly.GetEntryAssembly().Location).DirectoryName, "wwwroot");

            app.UsePhp(new PhpRequestOptions(scriptAssemblyName: "ResponsiveFileManager")
            {
                BeforeRequest = (Context ctx) =>
                {
                    // Since the config.php file is compiled, we cannot modify it once deployed... everything is hard coded there.
                    //  TODO: Place these values in appsettings.json and pass them in here to override the ones from config.php

                    ctx.Globals["appsettings"] = (PhpValue)new PhpArray()
                    {
                        { "upload_dir", rfmOptions.UploadDirectory },
                        { "current_path", rfmOptions.CurrentPath },
                        { "thumbs_base_path", rfmOptions.ThumbsBasePath }
                    };
                }
            });

            app.UseDefaultFiles();

            #endregion PeachPie / Responsive File Manager

            #region Static Files

            // embedded files
            app.UseStaticFiles(new StaticFileOptions
            {
                // Override file provider to allow embedded resources
                FileProvider = new CompositeFileProvider(
                    new EmbeddedScriptFileProvider(),
                    new EmbeddedContentFileProvider(),
                    HostingEnvironment.WebRootFileProvider),

                OnPrepareResponse = (context) =>
                {
                    var headers = context.Context.Response.GetTypedHeaders();
                    headers.CacheControl = new CacheControlHeaderValue()
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

            // Responsive File Manager
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(root)
            });

            #endregion Static Files

            app.UseForwardedHeaders(
                new ForwardedHeadersOptions
                {
                    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
                });

            //app.UseIdentity();
            app.UseAuthentication();

            app.UseMultitenancy<Tenant>();

            // Add external authentication middleware below. To configure them please see http://go.microsoft.com/fwlink/?LinkID=532715

            app.UseMvc(routes =>
            {
                // Enable all OData functions
                routes.Select().Expand().Filter().OrderBy().MaxTop(null).Count();

                var registrars = EngineContext.Current.ResolveAll<IODataRegistrar>();
                foreach (var registrar in registrars)
                {
                    registrar.Register(routes, app.ApplicationServices);
                }

                var routePublisher = EngineContext.Current.Resolve<IRoutePublisher>();
                routePublisher.RegisterRoutes(routes);

                //routes.MapRoute(
                //    name: "areaRoute",
                //    template: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

                //routes.MapRoute(
                //    name: "default",
                //    template: "{controller=Home}/{action=Index}/{id?}");
            });

            // If you want to dispose of resources that have been resolved in the
            // application container, register for the "ApplicationStopped" event.
            //appLifetime.ApplicationStopped.Register(() => EngineContext.Current.Dispose());

            ConfigureNLog();
        }

        private JsonSerializerSettings ConfigureJsonSerializerSettings(MvcJsonOptions options)
        {
            if (HostingEnvironment.IsDevelopment())
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

        private void ConfigureNLog()
        {
            try
            {
                var target = LogManager.Configuration.FindTargetByName("database");

                DatabaseTarget databaseTarget = null;
                var wrapperTarget = target as WrapperTargetBase;

                // Unwrap the target if necessary.
                if (wrapperTarget == null)
                {
                    databaseTarget = target as DatabaseTarget;
                }
                else
                {
                    databaseTarget = wrapperTarget.WrappedTarget as DatabaseTarget;
                }

                databaseTarget.DBProvider = "Npgsql"; // TODO: Not sure if this is right
                databaseTarget.ConnectionString = Configuration.GetConnectionString("DefaultConnection");
            }
            catch { }
        }
    }
}