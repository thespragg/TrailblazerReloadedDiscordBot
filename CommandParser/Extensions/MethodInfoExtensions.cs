using System.Reflection;

namespace CommandParser.Extensions;

public static class MethodInfoExtensions
{
    public static bool IsAsync(this MethodInfo methodInfo)
        => methodInfo.ReturnType == typeof(Task)
               || (methodInfo.ReturnType.IsGenericType && methodInfo.ReturnType.GetGenericTypeDefinition() == typeof(System.Threading.Tasks.Task<>));
    
}