namespace Facilis.Core.Abstractions
{
    public interface IEntityWithKeyValuePair<T>
    {
        string Key { get; }
        T Value { get; }
    }

    public interface IEntityWithKeyValuePair : IEntityWithKeyValuePair<string>
    {
    }
}