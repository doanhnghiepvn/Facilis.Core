using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Reflection;

namespace Facilis.Core.EntityFrameworkCore.Helpers
{
    public static class OnModelCreatingHelper
    {
        public static PropertyInfo[] GetDbSetProperties(this DbContext context)
        {
            return context.GetType()
                .GetTypeInfo()
                .DeclaredProperties
                .Where(property =>
                {
                    var typeInfo = property.PropertyType.GetTypeInfo();
                    return typeInfo.IsGenericType &&
                        typeof(DbSet<>).MakeGenericType(typeInfo.GenericTypeArguments) == typeInfo;
                })
                .ToArray();
        }

        public static DbContext UseStringifyEnumColumns(
            this DbContext context,
            ModelBuilder builder
        )
        {
            foreach (var dbSet in context.GetDbSetProperties())
            {
                var typeColumnProperty = dbSet.PropertyType;
                var typeEntity = typeColumnProperty.GenericTypeArguments[0];
                var enumProperties = typeEntity
                    .GetTypeInfo()
                    .DeclaredProperties
                    .Where(property => property.PropertyType.IsEnum)
                    .ToArray();
                if (enumProperties.Length == 0) continue;

                var entity = builder.Entity(typeEntity);

                foreach (var property in enumProperties)
                {
                    entity.Property(property.PropertyType, property.Name)
                        .HasConversion<string>();
                }
            }

            return context;
        }
    }
}