using Microsoft.Extensions.DependencyInjection;
using Spectre.Console.Cli;

namespace NicaStack.Cli.Bootstrap;

internal sealed class TypeRegistrar(IServiceCollection builder) : ITypeRegistrar
{
    public ITypeResolver Build()
    {
        return new TypeResolver(builder.BuildServiceProvider());
    }

    public void Register(Type service, Type implementation)
    {
        builder.AddSingleton(service, implementation);
    }

    public void RegisterInstance(Type service, object implementation)
    {
        builder.AddSingleton(service, implementation);
    }

    public void RegisterLazy(Type service, Func<object> factory)
    {
        if (factory is null)
        {
            throw new ArgumentNullException(nameof(factory));
        }

        builder.AddSingleton(service, _ => factory());
    }
}

internal sealed class TypeResolver(ServiceProvider provider) : ITypeResolver, IDisposable
{
    public object? Resolve(Type? type)
    {
        return type is null ? null : provider.GetService(type);
    }

    public void Dispose()
    {
        provider.Dispose();
    }
}