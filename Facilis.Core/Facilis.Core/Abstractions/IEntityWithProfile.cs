namespace Facilis.Core.Abstractions
{
    public interface IEntityWithProfile
    {
        object Profile { get; }
        string SerializedProfile { get; }

        void SetProfile(object profile);

        T GetProfile<T>();
    }
}