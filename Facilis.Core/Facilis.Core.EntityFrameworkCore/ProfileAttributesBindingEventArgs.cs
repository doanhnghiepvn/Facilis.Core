using Facilis.Core.Abstractions;
using System.Collections.Generic;

namespace Facilis.Core.EntityFrameworkCore
{
    public class ProfileAttributesBindingEventArgs<T>
        where T : class, IExtendedAttribute, new()
    {
        public IExtendedAttributes<T> Attributes { get; set; }
        public string ScopedId { get; set; }
        public Dictionary<string, string> ValuesGroupedInKeys { get; } = new();
    }
}