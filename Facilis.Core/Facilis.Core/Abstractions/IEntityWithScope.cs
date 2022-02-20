namespace Facilis.Core.Abstractions
{
    public interface IEntityWithScope
    {
        string Scope { get; }
        string ScopedId { get; }
    }
}