using Facilis.Core.Abstractions;
using Facilis.Core.Enums;
using System.Collections.Generic;

namespace Facilis.Core.EntityFrameworkCore.Models
{
    internal class BindingProfileAttributes<T>
        where T : class, IExtendedAttribute, new()
    {
        public string EntityId { get; set; }
        public StatusTypes EntityStatus { get; set; }

        public string Scope { get; set; }
        public T[] Attributes { get; set; }

        public Dictionary<string, string> ValuesGroupedInKeys { get; } = new();
    }
}