using Infrastructure.Data.Entities;
using LiteDB;

namespace Infrastructure.Data;

public interface IUserRepository
{
    void RegisterUser(string username, ulong userId);
    IEnumerable<User> GetUsers();
    User? GetUserByIgn(string ign);
    void DeleteUser(int id);
}

public class UserRepository : IUserRepository
{
    private ILiteCollection<User> Users { get; }

    public UserRepository()
    {
        var dir = $"{Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)}/.osrs";
        Directory.CreateDirectory(dir);
        var database = new LiteDatabase($"{dir}/{nameof(UserRepository)}.db");
        Users = database.GetCollection<User>(nameof(Users));
        Users.EnsureIndex(x => x.Id);
    }

    public void RegisterUser(string username, ulong userId)
    {
        Users.Insert(new User
        {
            Username = username,
            DiscordId = userId
        });
    }

    public void DeleteUser(int id)
        => Users.Delete(id);

    public IEnumerable<User> GetUsers() => Users.FindAll().ToList();
    public User? GetUserByIgn(string ign) => Users.FindOne(x => x.Username.Equals(ign, StringComparison.InvariantCultureIgnoreCase));
}