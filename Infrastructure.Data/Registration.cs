using Domain.Contracts;
using Domain.Services;
using Domain.Workers;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Data;

public static class Registration
{
    public static IServiceCollection AddData(this IServiceCollection sp)
        => sp.AddScoped<IPlayerRepository, PlayerRepository>();
}  