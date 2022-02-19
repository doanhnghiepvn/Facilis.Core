namespace Facilis.Core.Abstractions
{
    public interface IPassword
    {
        string HashingMethod { get; }
        string HashedPassword { get; }
        string PasswordSalt { get; }
        int PasswordIterated { get; }
    }
}