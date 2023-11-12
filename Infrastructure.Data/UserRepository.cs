using Infrastructure.Data.Entities;
using LiteDB;

namespace Infrastructure.Data;

public interface IUserRepository
{
    void RegisterUser(string username);
    IEnumerable<User> GetUsers();
    User? GetUser(int userId);
}

public class UserRepository : IUserRepository
{
    private readonly LiteDatabase _database = new($"{nameof(UserRepository)}.db");
    private ILiteCollection<User> Users { get; }

    public UserRepository()
    {
        Users = _database.GetCollection<User>(nameof(Users));
        Users.EnsureIndex(x => x.Id);
    }

    public void RegisterUser(string username)
    {
        Users.Insert(new User
        {
            Username = username
        });
    }

    public IEnumerable<User> GetUsers() => Users.FindAll().ToList();
    public User? GetUser(int userId) => Users.FindOne(x => x.Id == userId);
}