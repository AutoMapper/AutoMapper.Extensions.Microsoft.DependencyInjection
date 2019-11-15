namespace AutoMapper
{
    using System;
    using System.Linq;
    using System.Reflection;
    using Microsoft.Extensions.DependencyInjection;

    /// <summary>
    /// extension methods for type <see cref="IServiceProvider"/>
    /// </summary>
    internal static class ServiceProviderExtensions
    {
        /// <summary>
        /// Instantiates an instance of type <typeparamref name="TService"/> that has not been registered 
        /// with the <see cref="IServiceCollection"/> the <see cref="IServiceProvider"/> was created from, 
        /// and fulfilling all dependencies from the provided <see cref="IServiceProvider"/>.
        /// </summary>
        /// <typeparam name="TService">The concrete type of the service to instantiate</typeparam>
        /// <param name="serviceProvider">The service provider to use. </param>
        /// <returns>An instance of type <typeparamref name="TService"/></returns>
        public static TService AsSelf<TService>(this IServiceProvider serviceProvider)
        {
            return (TService)AsSelf(serviceProvider, typeof(TService));
        }

        /// <summary>
        /// Instantiates an instance of the provided <paramref name="serviceType"/> that has not been registered 
        /// with the <see cref="IServiceCollection"/> the <see cref="IServiceProvider"/> was created from, 
        /// and fulfilling all dependencies from the provided <see cref="IServiceProvider"/>.
        /// </summary>
        /// <typeparam name="TService">The concrete type of the service to instantiate</typeparam>
        /// <param name="serviceProvider">The service provider to use. </param>
        /// <returns>An instance of type <typeparamref name="TService"/></returns>
        public static object AsSelf(this IServiceProvider serviceProvider, Type serviceType)
        {
            if (serviceType.IsAbstract)
            {
                throw new InvalidOperationException($"Type {serviceType.FullName} is Abstract. Abstract types can not be instantiated.");
            }
            var constructors = serviceType.GetConstructors(BindingFlags.Public | BindingFlags.Instance)
                .Select(o => o.GetParameters())
                .ToArray()
                .OrderByDescending(o => o.Length)
                .ToArray();

            if (!constructors.Any())
            {
                // there are no public instance constructors. We cant create this type
                return null;
            }

            if (constructors.Length == 1 && constructors.First().Length == 0)
            {
                // we have a public default constructor. The requested type has no dependencies
                return Activator.CreateInstance(serviceType);
            }

            object[] arguments = ResolveParameters(serviceProvider, constructors);

            if (arguments == null)
            {
                return null;
            }

            return Activator.CreateInstance(serviceType, arguments);
        }

        private static object[] ResolveParameters(IServiceProvider resolver, ParameterInfo[][] constructors)
        {
            foreach (ParameterInfo[] constructor in constructors)
            {
                bool unresolved = false;
                object[] values = new object[constructor.Length];
                for (int i = 0; i < constructor.Length; i++)
                {
                    var value = resolver.GetService(constructor[i].ParameterType);
                    if (value == null)
                    {
                        unresolved = true;
                        break;
                    }
                    values[i] = value;
                }
                if (!unresolved)
                {
                    // found a constructor we can create.
                    return values;
                }
            }

            return null;
        }
    }
}
