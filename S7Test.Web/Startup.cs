using AspNet.Security.OpenIdConnect.Primitives;
using AspNet.Security.OpenIdConnect.Server;
using Autofac;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SpaServices.Webpack;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using OpenIddict.Core;
using S7Test.Core.Entity.App;
using S7Test.Infrastructure.Dependency;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection;
using System.Threading;

namespace S7Test.Web
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);

            if (env.IsDevelopment())
            {
                // For more details on using the user secret store see https://go.microsoft.com/fwlink/?LinkID=532709
                builder.AddUserSecrets<Startup>();
            }

            builder.AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IContainer AppContainer;
        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddDbContext(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));
                options.UseOpenIddict<string>();
            });
            services.AddIdentity<User, IdentityRole<string>>(options =>
            {
                //Signin settings
                options.SignIn.RequireConfirmedEmail = false;
                options.SignIn.RequireConfirmedPhoneNumber = false;

                //Password settings
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 8;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = true;
                options.Password.RequireLowercase = false;
                options.Password.RequiredUniqueChars = 6;

                //Lockout settings
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(30);
                options.Lockout.MaxFailedAccessAttempts = 10;
                options.Lockout.AllowedForNewUsers = true;

                //User settings
                options.User.RequireUniqueEmail = true;
                options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";

                //Configure Identity to use the same JWT claims as OpenIddict instead
                //of the legacy WS - Federation claims it uses by default(ClaimTypes),
                // which saves you from doing the mapping in your authorization controller.
                options.ClaimsIdentity.UserNameClaimType = OpenIdConnectConstants.Claims.Name;
                options.ClaimsIdentity.UserIdClaimType = OpenIdConnectConstants.Claims.Subject;
                options.ClaimsIdentity.RoleClaimType = OpenIdConnectConstants.Claims.Role;
            })
            .AddEntityFrameworkStores()
            .AddDefaultTokenProviders();

            services.ConfigureApplicationCookie(options =>
            {
                // Cookie settings
                options.Cookie.HttpOnly = true;
                options.Cookie.SameSite = SameSiteMode.Lax;
                options.Cookie.Expiration = TimeSpan.FromMinutes(60);
                options.SlidingExpiration = true;//new cookie will be issued with a new expiration time
                options.ReturnUrlParameter = CookieAuthenticationDefaults.ReturnUrlParameter;
            });

            services.AddOpenIddict<string>(options =>
            {
                // Register the Entity Framework stores.
                options.AddEntityFrameworkCoreStores();

                // Register the ASP.NET Core MVC binder used by OpenIddict.
                // Note: if you don't call this method, you won't be able to
                // bind OpenIdConnectRequest or OpenIdConnectResponse parameters.
                options.AddMvcBinders();

                // Enable the authorization, logout, token and userinfo endpoints.
                options.EnableAuthorizationEndpoint("/connect/authorize")
                       .EnableLogoutEndpoint("/connect/logout")
                       .EnableTokenEndpoint("/connect/token")
                       .EnableUserinfoEndpoint("/api/userinfo");

                options.SetAccessTokenLifetime(TimeSpan.FromMinutes(20));
                options.SetRefreshTokenLifetime(TimeSpan.FromMinutes(20160));

                // Note: the ClientApp only uses the code flow and the password flow, but you
                // can enable the other flows if you need to support implicit or client credentials.
                options.AllowAuthorizationCodeFlow()
                       //.AllowImplicitFlow()
                       .AllowPasswordFlow()
                       //.AllowClientCredentialsFlow()
                       .AllowRefreshTokenFlow();

                options.RegisterScopes(new[] {
                    OpenIdConnectConstants.Scopes.OpenId,
                    OpenIdConnectConstants.Scopes.Email,
                    OpenIdConnectConstants.Scopes.Address,
                    OpenIdConnectConstants.Scopes.Phone,
                    OpenIdConnectConstants.Scopes.Profile,
                    OpenIdConnectConstants.Scopes.OfflineAccess,
                    OpenIddictConstants.Scopes.Roles
                });
                options.RegisterClaims(new[] {
                    OpenIdConnectConstants.Claims.ClientId,
                    OpenIdConnectConstants.Claims.Name,
                    OpenIdConnectConstants.Claims.Subject,
                    OpenIdConnectConstants.Claims.Role,
                    OpenIdConnectConstants.Claims.Gender,
                    OpenIdConnectConstants.Claims.Profile,
                    OpenIdConnectConstants.Claims.Email,
                    OpenIdConnectConstants.Claims.EmailVerified,
                    OpenIdConnectConstants.Claims.PhoneNumber,
                    OpenIdConnectConstants.Claims.PhoneNumberVerified,
                    OpenIdConnectConstants.Claims.Nonce,
                    OpenIdConnectConstants.Claims.Locale,
                    OpenIdConnectConstants.Claims.Zoneinfo,
                });
                options.RequireClientIdentification();
                // When request caching is enabled, authorization and logout requests
                // are stored in the distributed cache by OpenIddict and the user agent
                // is redirected to the same page with a single parameter (request_id).
                // This allows flowing large OpenID Connect requests even when using
                // an external authentication provider like Google, Facebook or Twitter.
                options.EnableRequestCaching();
                options.AddSigningCertificate(
                    assembly: typeof(Startup).GetTypeInfo().Assembly,
                    resource: "S7Test.Web.Certificate.pfx",
                    password: "OpenIddict");
                options.UseJsonWebTokens();
                //options.DisableHttpsRequirement();
            });
            // If using JWT, disable the automatic JWT -> WS-Federation claims mapping used by the JWT middleware:
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
            JwtSecurityTokenHandler.DefaultOutboundClaimTypeMap.Clear();
            services
            .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.Authority = "https://localhost:44329";
                options.Audience = "resource_server"; //WebApi Server is Resource Server
                options.RequireHttpsMetadata = true;

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    NameClaimType = OpenIdConnectConstants.Claims.Subject,
                    RoleClaimType = OpenIdConnectConstants.Claims.Role,
                };
            });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("CustomApiAuthzPolicy", policy =>
                {
                    policy.AuthenticationSchemes.Add(JwtBearerDefaults.AuthenticationScheme);
                    policy.RequireAuthenticatedUser();
                    policy.RequireRole(new[] { "User" });
                });

            });

            services.AddCors();

            services.AddMvc(options =>
            {
                options.Filters.Add(new RequireHttpsAttribute());
                //options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
            })
            .AddControllersAsServices()
            .AddJsonOptions(options =>
            {
                options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                options.SerializerSettings.Converters.Add(new StringEnumConverter());
                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            });

            return services.AddExternalDependencyServiceProvider(out AppContainer);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, IApplicationLifetime appLifetime)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseWebpackDevMiddleware(new WebpackDevMiddlewareOptions
                {
                    HotModuleReplacement = true
                });
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();
            app.UseCors(builder =>
            {
                builder
                .WithOrigins("https://localhost:44329")
                .WithMethods("GET", "POST")
                .WithHeaders("Authorization");
            });
            app.UseAuthentication();
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");

                routes.MapSpaFallbackRoute(
                    name: "spa-fallback",
                    defaults: new { controller = "Home", action = "Index" });
            });
            appLifetime.ApplicationStopped.Register(() => AppContainer?.Dispose());
            app.ApplicationServices.InitializeDbAsync(CancellationToken.None).GetAwaiter().GetResult();
        }
    }
}
