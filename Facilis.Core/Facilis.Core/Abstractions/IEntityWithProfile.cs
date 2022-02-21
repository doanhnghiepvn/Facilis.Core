namespace Facilis.Core.Abstractions
{
    public interface IEntityWithProfile
    {
        string SerializedProfile { get; }

        void SetProfile(object profile);

        T GetProfile<T>() where T : class, new();
    }
}