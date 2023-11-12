using CommandParser.Attributes;
using Infrastructure.Data;

namespace Interface.Commands.Commands;

[Verb]
public class AccountCommands
{
    private readonly IUserRepository _repo;

    public AccountCommands(IUserRepository repo)
        => _repo = repo;
    
    [Command("register", "Adds an ign the bots player list.")]
    public void Register(string name)
        => _repo.RegisterUser(name);
    
    [Command("users", "Lists all the registered players.")]
    public void Users()
        => _repo.GetUsers();
}