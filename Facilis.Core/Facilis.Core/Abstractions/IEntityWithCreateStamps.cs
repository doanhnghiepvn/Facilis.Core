using System;

namespace Facilis.Core.Abstractions
{
    public interface IEntityWithCreateStamps
    {
        string CreatedBy { get; }
        DateTime CreatedAtUtc { get; }
    }
}