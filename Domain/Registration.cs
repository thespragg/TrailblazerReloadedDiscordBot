using Domain.Contracts;
using Domain.Services;
using Domain.Workers;
using Microsoft.Extensions.DependencyInjection;

namespace Domain;

public static class Registration
{
    public static IServiceCollection AddDomain(this IServiceCollection sp)
        => sp
            .AddSingleton<IStatRetrievalWorker, StatRetrievalWorker>()
            .AddScoped<IPlayerService, PlayerService>();
}