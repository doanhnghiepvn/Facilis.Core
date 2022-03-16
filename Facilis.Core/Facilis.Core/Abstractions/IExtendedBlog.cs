using Facilis.Core.Enums;
using System;

namespace Facilis.Core.Abstractions
{
    public interface IExtendedBlog :
        IEntityWithId,
        IEntityWithStatus,
        IEntityWithCreateStamps,
        IEntityWithUpdateStamps,
        IEntityWithScope,
        IEntityWithKeyValuePair<byte[]>,
        IEntityWithMime
    {
    }

    public class ExtendedBlog : IExtendedBlog
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public StatusTypes Status { get; set; }

        public string CreatedBy { get; set; }
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

        public string UpdatedBy { get; set; }
        public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;

        public string Scope { get; set; }
        public string ScopedId { get; set; }

        public string Key { get; set; }
        public byte[] Value { get; set; }

        public string MimeType { get; set; }
        public string Name { get; set; }
    }
}