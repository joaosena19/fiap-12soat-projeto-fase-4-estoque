namespace Application.Contracts.Services
{
    public interface IPasswordHasher
    {
        string Hash(string password);
    }
}