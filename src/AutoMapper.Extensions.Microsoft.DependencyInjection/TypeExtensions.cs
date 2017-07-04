namespace AutoMapper
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    internal static class TypeExtensions
    {
        public static bool ImplementsGenericInterface(this Type type, Type interfaceType)
        {
            return type.IsGenericType(interfaceType) || type.GetTypeInfo().ImplementedInterfaces.Any(@interface => @interface.IsGenericType(interfaceType));
        }

        public static bool IsGenericType(this Type type, Type genericType)
            => type.GetTypeInfo().IsGenericType && type.GetGenericTypeDefinition() == genericType;

        public static IEnumerable<TypeInfo> GetTypesImplementingGenericInterface(this IEnumerable<TypeInfo> types, IEnumerable<Type> interfaceTypes)
        {
            return types
                .Where(t => t.IsClass)
                .Where(t => !t.IsAbstract)
                .Where(t => interfaceTypes.Any(it => ImplementsGenericInterface(t.AsType(), it)));
        }
    }
}
