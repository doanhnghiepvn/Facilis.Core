using System;

namespace Facilis.Core.Abstractions
{
    public interface IEntityWithUpdateStamps
    {
        string UpdatedBy { get; }
        DateTime UpdatedAtUtc { get; }
    }
}