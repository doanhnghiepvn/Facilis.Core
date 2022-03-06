using Facilis.Core.Abstractions;
using Facilis.Core.EntityFrameworkCore;
using Facilis.Core.EntityFrameworkCore.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Facilis.Extensions.EntityFrameworkCore
{
    public static class DbContextHelper
    {
        public static IServiceCollection AddDefaultEntities(
            this IServiceCollection services
        )
        {
            return services
                .AddScoped(typeof(IEntities<>), typeof(Entities<>))
                .AddScoped(typeof(IEntitiesWithId<>), typeof(EntitiesWithId<>))
                .AddScoped(typeof(IScopedEntities<>), typeof(ScopedEntities<>));
        }

        public static IServiceCollection UseProfileAttributesBuilder<TContext, TBinder>(
            this IServiceCollection services
        )
            where TContext : DbContext
            where TBinder : class, IProfileAttributesBinder
        {
            return services
                .AddScoped<IProfileAttributesBinder, TBinder>()
                .UseProfileAttributesBuilder<TContext>();
        }

        public static IServiceCollection UseProfileAttributesBuilder<T>(
            this IServiceCollection services
        ) where T : DbContext
        {
            return services.AddScoped(provider =>
            {
                var binder = provider.GetRequiredService<IProfileAttributesBinder>();
                var context = provider.GetRequiredService<T>();

                context.SavingChanges += binder.DbContextSavingChanges;
                context.SavedChanges += binder.DbContextSavedChanges;

                return (DbContext)context;
            });
        }
    }
}