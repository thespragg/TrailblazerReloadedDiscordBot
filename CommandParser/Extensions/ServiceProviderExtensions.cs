namespace CommandParser.Extensions;

public static class ServiceProviderExtensions
{
    public static object? CreateInstance(this IServiceProvider serviceProvider, Type type)
    {
        var constructor = type.GetConstructors().First();
        var parameters = constructor.GetParameters()
            .Select(param => serviceProvider.GetService(param.ParameterType))
            .ToArray();
        return Activator.CreateInstance(type, parameters);
    }
}