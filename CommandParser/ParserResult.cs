using CommandParser.Contracts;
using CommandParser.Extensions;
using CommandParser.Models;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;

namespace CommandParser
{
    [PublicAPI]
    public class ParserResult<TContext> : IParserResult<TContext> where TContext : class
    {
        private readonly IVerb _verb;
        private readonly ICommand _commandValue;
        private readonly IEnumerable<string> _args;
        private readonly IServiceProvider? _sp;
        public Action<string>? OnError { get; set; }

        internal ParserResult(IEnumerable<string> args, ICommand command, IVerb verb, IServiceProvider? sp)
        {
            _commandValue = command;
            _sp = sp;
            _verb = verb;
            _args = args.Where(x =>
                !x.Equals(verb?.VerbName, StringComparison.InvariantCultureIgnoreCase)
                && !x.Equals(command.CommandText, StringComparison.InvariantCultureIgnoreCase)).ToArray();
        }

        public async Task<bool> InvokeAsync()
            => await InvokeAsync(null);

        public async Task<bool> InvokeAsync(TContext? ctx)
        {
            if (typeof(TContext) == typeof(EmptyContext)) ctx = new EmptyContext() as TContext;
            try
            {
                ValidateArgumentCount();

                var parameterValues = GetParameterValues(ctx);

                var instance = CreateInstance();

                await InvokeMethodAsync(instance, parameterValues);

                return true;
            }
            catch (Exception ex)
            {
                OnError?.Invoke(ex.Message);
                return false;
            }
        }

        private void ValidateArgumentCount()
        {
            var type = typeof(TContext);
            var mParams = _commandValue.Method.GetParameters().Where(x => x.ParameterType != type)
                .ToArray();
            if (_args.Count() != mParams.Length)
            {
                throw new ArgumentException("Number of strings does not match the number of method parameters.");
            }
        }

        private object?[] GetParameterValues(TContext? ctx)
        {
            var methodParams = _commandValue.Method.GetParameters();
            var parameterValues = new List<object?>();

            if (methodParams.Any(x => x.ParameterType == typeof(TContext)))
            {
                parameterValues.Add(ctx);
                methodParams = methodParams.Where(x => x.ParameterType != typeof(TContext)).ToArray();
            }

            parameterValues.AddRange(_args.Select((stringValue, index) =>
            {
                var parameterType = methodParams[index].ParameterType;
                try
                {
                    return Convert.ChangeType(stringValue, parameterType);
                }
                catch (Exception ex)
                {
                    throw new ArgumentException($"Failed to convert string '{stringValue}' to type {parameterType}.",
                        ex);
                }
            }));

            return parameterValues.ToArray();
        }

        private object? CreateInstance()
            => _sp?.CreateScope()
                .ServiceProvider.CreateInstance(_verb.Type) ?? Activator.CreateInstance(_verb.Type);

        private async Task InvokeMethodAsync(object instance, object[] parameterValues)
        {
            if (_commandValue.Method.IsAsync())
            {
                var t = (Task?)_commandValue.Method.Invoke(instance, parameterValues);
                if (t is not null) await t;
            }
            else
            {
                _commandValue.Method.Invoke(instance, parameterValues);
            }
        }
    }

    [PublicAPI]
    public class ParserResult : ParserResult<EmptyContext>
    {
        internal ParserResult(IEnumerable<string> args, ICommand command, IVerb verb, IServiceProvider? sp)
            : base(args, command, verb, sp)
        {
        }
    }
}