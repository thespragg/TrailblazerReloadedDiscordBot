using Domain.Contracts;
using Infrastructure.Hiscores.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Hiscores;

public static class Registration
{
    public static IServiceCollection AddHiscoreProvider(this IServiceCollection sp)
        => sp.AddHttpClient<IStatService, HiscoresStatService>().Services;
}