﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using Autofac;
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
using Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using NLog;
using NLog.Targets;
using NLog.Targets.Wrappers;
using Pchp.Core;

namespace MantleCMS
{
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

        public IWebHostEnvironment WebHostEnvironment { get; private set; }

        public IServiceProvider ServiceProvider { get; private set; }

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
            services.ConfigureMantleOptions(Configuration);

            services.AddMultitenancy<Tenant, MantleTenantResolver>();

            services.AddMantleLocalization();

            services.AddOData();

            var mvcBuilder = services.AddMvc(options =>
            {
                options.EnableEndpointRouting = false; // For OData (until they support endpoint routing)
            })
            .AddNewtonsoftJson((jsonOptions) => services.AddSingleton(ConfigureJsonSerializerSettings(jsonOptions)))
            .AddRazorRuntimeCompilation();

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

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();

            #region Mantle Framework Config

            services.ConfigureMantleOptions(Configuration);
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
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(
            IApplicationBuilder app,
            IWebHostEnvironment env,
            ILoggerFactory loggerFactory,
            IHostApplicationLifetime appLifetime)
        {
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

            app.UseAuthentication();
            app.UseAuthorization();

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

                //databaseTarget.DBProvider = "Npgsql"; // If using other provider...

                var dataSettings = EngineContext.Current.Resolve<DataSettings>();
                databaseTarget.ConnectionString = dataSettings.ConnectionString;
            }
            catch { }
        }

        public void ConfigureContainer(ContainerBuilder builder)
        {
            // Add extra registrations here, if needed...
            //  But it's better to use IDependencyRegistrar
        }
    }
}