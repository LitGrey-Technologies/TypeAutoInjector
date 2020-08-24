using System;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using TypeAutoInjector.Attributes;
using TypeAutoInjector.Enums;

namespace TypeAutoInjector
{
    public static class TypeAutoInjection
    {
        public static IServiceCollection Inject(this IServiceCollection collection, TypeLifeScope scope,
            AssemblyName assemblyName,
            string typeNameEndsWith, bool hasInterfaces, Action<string> logMessageAction)
        {
            switch (hasInterfaces)
            {
                case true:
                {
                    var assembly = Assembly.Load(assemblyName);
                    var interfaces = assembly.GetTypes().Where(t => t.IsInterface).ToList();
                    foreach (var type in assembly.GetTypes())
                    {
                        if (!type.IsClass || type.IsAbstract || type.IsGenericType ||
                            !type.Name.EndsWith(typeNameEndsWith))
                            continue;

                        if (type.GetCustomAttribute(typeof(NotAutoInjectTypeAttribute)) != null)
                            continue;
                        var typeInterfaceName = $"I{type.Name}";
                        var typeInterface = interfaces.Find(t => t.Name == typeInterfaceName);

                        if (typeInterface?.GetCustomAttribute(typeof(NotAutoInjectTypeAttribute)) != null)
                            continue;

                        if (type.GetInterface(typeInterfaceName) == null)
                            throw new NotImplementedException(
                                $"{typeInterface?.Name} does not implement on {type.Name}");
                        AddTypeWithInterface(logMessageAction, collection, scope, type, typeInterface);
                    }

                    break;
                }

                case false:
                {
                    var assembly = Assembly.Load(assemblyName);
                    foreach (var type in assembly.GetTypes())
                    {
                        if (!type.IsClass
                            || type.IsAbstract
                            || !type.Name.EndsWith(typeNameEndsWith)
                            || type.GetCustomAttribute(typeof(NotAutoInjectTypeAttribute)) != null)
                            continue;
                        AddTypeWithoutInterface(logMessageAction, collection, scope, type);
                    }

                    break;
                }
            }

            return collection;
        }

        private static void AddTypeWithoutInterface(Action<string> logMessageAction, IServiceCollection collection,
            TypeLifeScope scope, Type type)
        {
            logMessageAction.Invoke($"{type.Name} found.");
            switch (scope)
            {
                case TypeLifeScope.Transient:
                    collection.AddTransient(type);
                    logMessageAction.Invoke(
                        $"{type.Name} injected as {scope} successfully.");
                    break;
                case TypeLifeScope.Scoped:
                    collection.AddScoped(type);
                    logMessageAction.Invoke(
                        $"{type.Name} injected as {scope} successfully.");
                    break;
                case TypeLifeScope.Singleton:
                    collection.AddSingleton(type);
                    logMessageAction.Invoke(
                        $"{type.Name} injected as {scope} successfully.");
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(scope), scope, null);
            }
        }

        private static void AddTypeWithInterface(Action<string> logMessageAction, IServiceCollection collection,
            TypeLifeScope scope,
            Type type, Type typeInterface)
        {
            logMessageAction.Invoke($"{typeInterface.Name} interface found for {type.Name}");
            switch (scope)
            {
                case TypeLifeScope.Transient:
                    collection.AddTransient(type, typeInterface);
                    logMessageAction.Invoke(
                        $"{type.Name} with interface {typeInterface.Name} injected as {scope} successfully.");
                    break;
                case TypeLifeScope.Scoped:
                    collection.AddScoped(type, typeInterface);
                    logMessageAction.Invoke(
                        $"{type.Name} with interface {typeInterface.Name} injected as {scope} successfully.");
                    break;
                case TypeLifeScope.Singleton:
                    collection.AddSingleton(type, typeInterface);
                    logMessageAction.Invoke(
                        $"{type.Name} with interface {typeInterface.Name} injected as {scope} successfully.");
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(scope), scope, null);
            }
        }
    }
}