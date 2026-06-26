namespace EventsManager.Application.Common.Interfaces.Tools
{
    public interface IPasswordHasher
    {
        string Hash(string password);
        bool Verify(string password, string hashedPassword);
    }
}
