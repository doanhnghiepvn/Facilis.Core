using Facilis.Core.Abstractions;
using Facilis.ExtendedEntities.Abstractions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json;

namespace Facilis.ExtendedEntities.EntityFrameworkCore
{
    public interface IProfileAttributesBinder
    {
        bool HasPendingChanges();

        void DbContextSavedChanges(object sender, SavedChangesEventArgs e);

        void DbContextSavingChanges(object sender, SavingChangesEventArgs e);
    }

    public class ProfileAttributesBinder<T> : IProfileAttributesBinder
        where T : class, IExtendedAttribute, new()
    {
        private IList<T> newEntities { get; } = new List<T>();
        private IList<T> updateEntities { get; } = new List<T>();

        private IExtendedAttributes<T> attributes { get; }
        private IOperators operators { get; }

        #region Constructor(s)

        public ProfileAttributesBinder(
            IExtendedAttributes<T> extendedAttributes,
            IOperators operators
        )
        {
            this.attributes = extendedAttributes;
            this.operators = operators;
        }

        #endregion Constructor(s)

        public bool HasPendingChanges()
        {
            return (this.newEntities.Count + this.updateEntities.Count) > 0;
        }

        public void DbContextSavingChanges(object sender, SavingChangesEventArgs e)
        {
            this.Clear();

            if (sender is DbContext context)
            {
                foreach (var tracked in context.ChangeTracker
                    .Entries()
                    .Where(tracked =>
                        new[] { EntityState.Modified, EntityState.Added, }
                            .Contains(tracked.State)
                    )
                )
                {
                    if (tracked.Entity is IEntityWithProfile entity)
                    {
                        var profile = entity.Profile;
                        if (profile == null) continue;

                        var scope = $"{entity.GetType().Namespace}.{entity.GetType().Name}";
                        var scopedId = ((IEntityWithId)entity).Id;

                        var attributes = this.attributes.ChangeScope(scope);

                        foreach (var property in profile.GetType().GetProperties())
                        {
                            var key = property.Name;
                            var rawValue = ToRawValue(profile, property);

                            if (attributes.AnyEnabled(scopedId, key))
                            {
                                var attribute = attributes
                                    .WhereEnabledDescendingSort(scopedId, key)
                                    .Cast<T>()
                                    .First();

                                attribute.UpdatedBy = this.operators.GetCurrentOperatorName();
                                attribute.UpdatedAtUtc = DateTime.UtcNow;
                                attribute.Value = rawValue;

                                this.updateEntities.Add(attribute);
                            }
                            else
                            {
                                this.newEntities
                                    .Add(attributes.CreateEntity(scopedId, key, rawValue));
                            }
                        }
                    }
                }
            }
        }

        public void DbContextSavedChanges(object sender, SavedChangesEventArgs e)
        {
            if (this.HasPendingChanges())
            {
                if (this.newEntities.Any()) this.attributes.Entities.Add(this.newEntities);
                if (this.updateEntities.Any()) this.attributes.Entities.Update(this.updateEntities);

                this.attributes.Save();
            }

            this.Clear();
        }

        private void Clear()
        {
            this.newEntities.Clear();
            this.updateEntities.Clear();
        }

        private static string ToRawValue(object profile, PropertyInfo property)
        {
            var value = property.GetValue(profile, null);
            var rawValue = value == null ? null : JsonSerializer.Serialize(value);

            return property.PropertyType == typeof(string) && value != null ?
                rawValue[1..^1] :
                rawValue;
        }
    }

    public class ProfileAttributesBinder
        : ProfileAttributesBinder<ExtendedAttribute>
    {
        #region Constructor(s)

        public ProfileAttributesBinder(
            IExtendedAttributes<ExtendedAttribute> extendedAttributes,
            IOperators operators
        ) : base(extendedAttributes, operators)
        {
        }

        #endregion Constructor(s)
    }
}