using System;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using OpenIddict.Core;
using OpenIddict.Models;
using System.Threading;
using S7Test.Infrastructure.Data;

namespace S7Test.Infrastructure.Dependency
{
    public static class DependencyServiceExtensions
    {
        public static IServiceCollection AddDbContext(this IServiceCollection services, Action<DbContextOptionsBuilder> optionsAction)
        {
            return services.AddDbContext<AppDbContext>(optionsAction);
        }
        public static IdentityBuilder AddEntityFrameworkStores(this IdentityBuilder builder)
        {
            return builder.AddEntityFrameworkStores<AppDbContext>();
        }
        public static OpenIddictBuilder AddEntityFrameworkCoreStores(this OpenIddictBuilder builder)
        {
            return builder.AddEntityFrameworkCoreStores<AppDbContext>();
        }
        public static IServiceProvider AddExternalDependencyServiceProvider(this IServiceCollection services, out IContainer container)
        {
            var containerBuilder = new ContainerBuilder();
            containerBuilder.RegisterModule<DefaultRegistrationModule>();
            containerBuilder.Populate(services);
            container = containerBuilder.Build();
            return new AutofacServiceProvider(container);
        }

        public async static Task InitializeDbAsync(this IServiceProvider services, CancellationToken cancellationToken)
        {
            // Create a new service scope to ensure the database context is correctly disposed when this methods returns.
            using (var scope = services.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                await context.Database.EnsureCreatedAsync();

                var manager = scope.ServiceProvider.GetRequiredService<OpenIddictApplicationManager<OpenIddictApplication<string>>>();
                   
                if (await manager.FindByClientIdAsync("angular_client_s6BhdRkqt3", cancellationToken) == null)
                {
                    var descriptor = new OpenIddictApplicationDescriptor
                    {
                        ClientId = "angular_client_s6BhdRkqt3",
                        DisplayName = "public_client",
                        RedirectUris = { new Uri("https://localhost:44329/players") },
                        PostLogoutRedirectUris = { new Uri("https://localhost:44329/login") }
                    };

                    await manager.CreateAsync(descriptor, cancellationToken);
                }

                await DbInitializer.InitializeAsync(context);
            }
        }


    }
}
