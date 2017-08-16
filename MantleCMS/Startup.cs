﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using MantleCMS.Data;
using MantleCMS.Data.Domain;
using MantleCMS.Identity;
using MantleCMS.Options;
using MantleCMS.Services;
using Mantle.Collections;
using Mantle.Identity.Services;
using Mantle.Infrastructure;
using Mantle.Tasks;
using Mantle.Tenants.Domain;
using Mantle.Web;
using Mantle.Web.Mvc.Assets;
using Mantle.Web.Mvc.EmbeddedResources;
using Mantle.Web.Mvc.Razor;
using Mantle.Web.Tenants;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Cors.Infrastructure;
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
using Mantle.Web.Plugins;
using Mantle.Web.Common.Areas.Admin.Regions;
using Mantle.Data.Entity.EntityFramework;
using System.IO;
using Mantle.Web.Configuration;

namespace MantleCMS
{
    public class Startup
    {
        /// <summary>
        /// The name of the default CORS policy.
        /// </summary>
        internal const string DefaultCorsPolicyName = "DefaultCorsPolicy";

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

        public IConfigurationRoot Configuration { get; }

        public IHostingEnvironment HostingEnvironment { get; private set; }

        public IServiceProvider ServiceProvider { get; private set; }

        public ICollection<EmbeddedFileProvider> EmbeddedFileProviders { get; private set; }

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

            // Add application services.
            services.AddTransient<IEmailSender, AuthMessageSender>();
            services.AddTransient<ISmsSender, AuthMessageSender>();

            services.AddSingleton<IConfiguration>(Configuration);
            services.AddSingleton((p) => p.GetRequiredService<IOptions<SiteOptions>>().Value);

            services
                .AddMemoryCache()
                .AddDistributedMemoryCache();

            services.AddCors(ConfigureCors);

            services.AddRouting((p) =>
            {
                p.AppendTrailingSlash = true;
                p.LowercaseUrls = true;
            });

            services.AddMultitenancy<Tenant, MantleTenantResolver>();

            services.AddMantleLocalization();

            var mvcBuilder = services.AddMvc(ConfigureMvc)
                .AddJsonOptions((p) => services.AddSingleton(ConfigureJsonFormatter(p)));

            EmbeddedFileProviders = new List<EmbeddedFileProvider>
            {
                new EmbeddedFileProvider(typeof(MantleWebConstants).GetTypeInfo().Assembly, "Mantle.Web"),
                new EmbeddedFileProvider(typeof(IRegionSettings).GetTypeInfo().Assembly, "Mantle.Web.Common"),
                //TODO: Add more - and better to detect them automatically somehow
            };

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

            services.Configure<RazorViewEngineOptions>(options =>
            {
                //Add the file provider to the Razor view engine

                options.FileProviders.Add(new EmbeddedViewFileProvider()); // allow embedded views for special cases like dynamic settings in framework
                foreach (var embeddedFileProvider in EmbeddedFileProviders)
                {
                    options.FileProviders.Add(embeddedFileProvider);
                }

                //options.FileProviders.Add(embeddedFileProvider);
                options.ViewLocationExpanders.Add(new TenantViewLocationExpander());
            });

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();

            //===================================================================
            // FRAMEWORK CONFIG
            //===================================================================
            var config = new MantleWebOptions();
            Configuration.Bind(config);
            services.AddSingleton(config);

            // Tell Mantle it is a website (not something like unit test or whatever)
            Mantle.Hosting.HostingEnvironment.IsHosted = true;

            EngineContext.Default = new AutofacEngine(services);
            EngineContext.Initialize(false);

            // Create the IServiceProvider based on the container.
            var provider = EngineContext.Current.ServiceProvider;
            if (provider != null)
            {
                ServiceProvider = provider;
            }

            ServiceProvider = provider;

            var mantleWebOptions = provider.GetRequiredService<MantleWebOptions>();
            PluginManager.Initialize(mvcBuilder.PartManager, HostingEnvironment, mantleWebOptions);

            //if (DataSettingsHelper.IsDatabaseInstalled && MantleConfigurationSection.Instance.ScheduledTasks.Enabled)
            //{
            TaskManager.Instance.Initialize();
            TaskManager.Instance.Start();
            //}

            //KoreUISettings.DefaultAdminProvider = new SmartAdminUIProvider();

            // Unfortunately, at the moment, we don't have any better way than this to let libraries
            //  know where to find shared assets that they will need.
            MantleWebAssets.Init(new MantleWebAssets
            {
                BootstrapFileInput = new AssetCollection
                {
                    Scripts = new List<Asset>
                    {
                        new Asset { Path = "/js/bootstrapFileInput/fileinput.js" }
                    },
                    Styles = new List<Asset>
                    {
                        new Asset { Path = "/css/bootstrapFileInput/css/fileinput.css" }
                    }
                }
            });

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

            app.AddNLogWeb();

            env.ConfigureNLog("NLog.config");

            app.UseApplicationInsightsRequestTelemetry();

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

            app.UseApplicationInsightsExceptionTelemetry();

            var requestLocalizationOptions = app.ApplicationServices.GetService<IOptions<RequestLocalizationOptions>>();
            app.UseRequestLocalization(requestLocalizationOptions.Value);

            // embedded files
            app.UseStaticFiles(new StaticFileOptions
            {
                // Override file provider to allow embedded resources
                FileProvider = new CompositeFileProvider(
                    new EmbeddedScriptFileProvider(),
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

            app.UseForwardedHeaders(
                new ForwardedHeadersOptions()
                {
                    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
                });

            app.UseIdentity();

            app.UseMultitenancy<Tenant>();

            // Add external authentication middleware below. To configure them please see http://go.microsoft.com/fwlink/?LinkID=532715

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "areaRoute",
                    template: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

            // If you want to dispose of resources that have been resolved in the
            // application container, register for the "ApplicationStopped" event.
            appLifetime.ApplicationStopped.Register(() => EngineContext.Current.Dispose());

            TryUpdateNLogConnectionString();

            var contextFactory = EngineContext.Current.Resolve<IDbContextFactory>();
            using (var context = contextFactory.GetContext())
            {
                var efHelper = EngineContext.Current.Resolve<IEntityFrameworkHelper>();
                efHelper.EnsureTables(context);
            }
        }

        /// <summary>
        /// Configures the JSON serializer for MVC.
        /// </summary>
        /// <param name="options">The <see cref="MvcJsonOptions"/> to configure.</param>
        /// <returns>
        /// The <see cref="JsonSerializerSettings"/> to use.
        /// </returns>
        private static JsonSerializerSettings ConfigureJsonFormatter(MvcJsonOptions options)
        {
            // Make JSON easier to read for debugging at the expense of larger payloads
            options.SerializerSettings.Formatting = Formatting.Indented;

            // Omit nulls to reduce payload size
            options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;

            options.SerializerSettings.DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate;

            // Explicitly define behavior when serializing DateTime values
            options.SerializerSettings.DateFormatString = "yyyy'-'MM'-'dd'T'HH':'mm':'ssK";   // Only return DateTimes to a 1 second precision

            return options.SerializerSettings;
        }

        /// <summary>
        /// Configures CORS.
        /// </summary>
        /// <param name="corsOptions">The <see cref="CorsOptions"/> to configure.</param>
        private void ConfigureCors(CorsOptions corsOptions)
        {
            var siteOptions = ServiceProvider.GetService<SiteOptions>();

            corsOptions.AddPolicy(
                DefaultCorsPolicyName,
                (builder) =>
                {
                    builder
                        .WithExposedHeaders(siteOptions.Api.Cors.ExposedHeaders)
                        .WithHeaders(siteOptions.Api.Cors.Headers)
                        .WithMethods(siteOptions.Api.Cors.Methods);

                    if (HostingEnvironment.IsDevelopment())
                    {
                        builder.AllowAnyOrigin();
                    }
                    else
                    {
                        builder.WithOrigins(siteOptions.Api.Cors.Origins);
                    }
                });
        }

        /// <summary>
        /// Configures MVC.
        /// </summary>
        /// <param name="options">The <see cref="MvcOptions"/> to configure.</param>
        private void ConfigureMvc(MvcOptions options)
        {
            if (!HostingEnvironment.IsDevelopment())
            {
                //options.Filters.Add(new RequireHttpsAttribute());
            }
        }

        private void TryUpdateNLogConnectionString()
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