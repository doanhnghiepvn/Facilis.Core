using Facilis.Core.Abstractions;
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
        protected delegate void TrackedEventHandler(object sender, ProfileAttributesBindingEventArgs<T> e);

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
            if (sender is DbContext context)
            {
                this.TrackAdded(context.ChangeTracker);
                this.TrackModified(context.ChangeTracker);
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

        protected virtual void TrackAdded(ChangeTracker tracker)
        {
            this.Track(
                tracker.Entries(),
                (sender, args) =>
                {
                    var scopedId = args.ScopedId;

                    foreach (var (key, value) in args.ValuesGroupedInKeys)
                    {
                        this.newEntities.Add(args
                            .Attributes
                            .CreateEntity(scopedId, key, value));
                    }
                },
                EntityState.Added
            );
        }

        protected virtual void TrackModified(ChangeTracker tracker)
        {
            this.Track(
                tracker.Entries(),
                (sender, args) =>
                {
                    var scopedId = args.ScopedId;

                    foreach (var (key, value) in args.ValuesGroupedInKeys)
                    {
                        if (args.Attributes.AnyEnabled(args.ScopedId, key))
                        {
                            var attribute = args
                                .Attributes
                                .WhereEnabledDescendingSort(args.ScopedId, key)
                                .Cast<T>()
                                .First();

                            attribute.UpdatedBy = this.operators.GetCurrentOperatorName();
                            attribute.UpdatedAtUtc = DateTime.UtcNow;
                            attribute.Value = args.ValuesGroupedInKeys[key];

                            this.updateEntities.Add(attribute);
                        }
                    }
                },
                EntityState.Modified
            );
        }

        protected virtual void Track(
            IEnumerable<EntityEntry> entries,
            TrackedEventHandler eventHandler,
            params EntityState[] states
        )
        {
            foreach (var tracked in entries
                .Where(entry => states.Contains(entry.State))
            )
            {
                if (tracked.Entity is IEntityWithProfile entity)
                {
                    var profile = entity.Profile;
                    if (profile == null) continue;

                    var scope = $"{entity.GetType().Namespace}.{entity.GetType().Name}";
                    var scopedId = ((IEntityWithId)entity).Id;

                    var arguments = new ProfileAttributesBindingEventArgs<T>()
                    {
                        Attributes = this.attributes.ChangeScope(scope),
                        ScopedId = scopedId,
                    };

                    foreach (var property in profile.GetType().GetProperties())
                    {
                        var key = property.Name;
                        var rawValue = ToRawValue(profile, property);

                        arguments.ValuesGroupedInKeys.Add(key, rawValue);
                    }

                    eventHandler(this, arguments);
                }
            }
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