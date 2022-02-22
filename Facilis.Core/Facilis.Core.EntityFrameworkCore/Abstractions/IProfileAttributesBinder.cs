using Facilis.Core.Abstractions;
using Facilis.Core.Attributes;
using Facilis.Core.EntityFrameworkCore.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json;

namespace Facilis.Core.EntityFrameworkCore.Abstractions
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
        private List<BindingProfileAttributes<T>> bindingModels { get; } = new();

        private IScopedEntities<T> attributes { get; }
        private IOperators operators { get; }

        #region Constructor(s)

        public ProfileAttributesBinder(
            IScopedEntities<T> attributes,
            IOperators operators
        )
        {
            this.attributes = attributes;
            this.operators = operators;
        }

        #endregion Constructor(s)

        public virtual bool HasPendingChanges()
        {
            return this.bindingModels.Count > 0;
        }

        public virtual void DbContextSavingChanges(object sender, SavingChangesEventArgs e)
        {
            var trackStates = new[]
            {
                EntityState.Added,
                EntityState.Modified,
            };
            if (sender is DbContext context)
            {
                this.Track(context.ChangeTracker
                    .Entries()
                    .Where(entry => trackStates.Contains(entry.State))
                );
            }
        }

        public virtual void DbContextSavedChanges(object sender, SavedChangesEventArgs e)
        {
            if (this.HasPendingChanges())
            {
                var newAttributes = new List<T>();
                var updatingAttributes = new List<T>();

                foreach (var model in this.bindingModels)
                {
                    newAttributes.AddRange(model.GetNewAttributes());
                    updatingAttributes.AddRange(model.GetUpdatingAttributes());
                }

                if (newAttributes.Any())
                {
                    this.attributes.AddNoSave(newAttributes);
                }

                if (updatingAttributes.Any())
                {
                    this.attributes.UpdateNoSave(updatingAttributes);
                }

                this.Clear();
                this.attributes.Save();
            }
        }

        protected virtual void Track(IEnumerable<EntityEntry> entries)
        {
            foreach (var tracked in entries)
            {
                if (tracked.Entity is IEntityWithProfile entity)
                {
                    var profile = entity.Profile;
                    if (profile == null) continue;

                    var scope = $"{entity.GetType().Namespace}.{entity.GetType().Name}";
                    var scopedId = ((IEntityWithId)entity).Id;

                    var profileAttributes = new BindingProfileAttributes<T>()
                    {
                        EntityId = scopedId,
                        EntityStatus = ((IEntityWithStatus)entity).Status,

                        Scope = scope,
                        Attributes = this.attributes
                            .ChangeScope(scope)
                            .QueryEnabledByScopedId(scopedId)
                            .ToArray(),

                        Operators = this.operators,
                    };

                    foreach (var property in profile.GetType().GetProperties())
                    {
                        var key = property.Name;
                        var rawValue = ToRawValue(profile, property);

                        if (property.GetCustomAttribute(typeof(ImmutableAttribute)) != null)
                        {
                            profileAttributes.ImmutableKeys.Add(key);
                        }
                        profileAttributes.ValuesGroupedInKeys.Add(key, rawValue);
                    }

                    this.bindingModels.Add(profileAttributes);
                }
            }
        }

        private void Clear()
        {
            this.bindingModels.Clear();
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
            IScopedEntities<ExtendedAttribute> attributes,
            IOperators operators
        ) : base(attributes, operators)
        {
        }

        #endregion Constructor(s)
    }
}