using System;
using System.Reflection;

namespace AutoMapper
{
    internal sealed class ProxyInfo
    {
        public Type ProxiedInterface { get; private set; }

        /// <summary> TDestination IMapper.Map{TDestination}(object source) </summary>
        public MethodInfo Map_TDest_source { get; private set; }

        /// <summary> TDestination IMapper.Map{TSource, TDestination}(TSource source) </summary>
        public MethodInfo Map_TSource_TDest_source { get; private set; }

        /// <summary> TDestination IMapper.Map{TSource, TDestination}(TSource source, TDestination destination) </summary>
        public MethodInfo Map_TSource_TDest_source_dest { get; private set; }

        /// <summary> object IMapper.Map(object source, Type sourceType, Type destinationType) </summary>
        public MethodInfo Map_source_sourcetype_desttype { get; private set; }

        /// <summary> object IMapper.Map(object source, object destination, Type sourceType, Type destinationType) </summary>
        public MethodInfo Map_source_dest_sourcetype_desttype { get; private set; }

        private ProxyInfo() { }

        public static ProxyInfo BuildFrom(Type proxiedInterface)
        {
            if (proxiedInterface == null)
                throw new ArgumentNullException(nameof(proxiedInterface));

            if (!proxiedInterface.IsInterface || !proxiedInterface.IsPublic)
                throw new InvalidOperationException($"Parameter '{proxiedInterface}' should be public interface.");

            foreach (var member in proxiedInterface.GetMembers())
                if (member.MemberType != MemberTypes.Method)
                    throw new InvalidOperationException($"Member '{member.Name}' ({member.MemberType}) found on interface. Only methods are supported.");

            var proxyInfo = new ProxyInfo { ProxiedInterface = proxiedInterface };

            foreach (var method in proxiedInterface.GetMethods())
                if (!proxyInfo.TryProject(method))
                    throw new InvalidOperationException($"In the {proxiedInterface} interface, a method {method.Name} was found that cannot be matched to any of the 'AutoMapper.IMapper' interface methods. Matching is done by the method signature, except for its name — the name can be arbitrary.");

            if (proxyInfo.IsEmpty)
                throw new InvalidOperationException($"Not a single method was found in the interface '{proxiedInterface}' that could be mapped to any method from the interface 'AutoMapper.IMapper'. Matching is done by the method signature, except for its name — the name can be arbitrary.");

            return proxyInfo;
        }

        private bool IsEmpty =>
            Map_TDest_source == null &&
            Map_TSource_TDest_source == null &&
            Map_TSource_TDest_source_dest == null &&
            Map_source_sourcetype_desttype == null &&
            Map_source_dest_sourcetype_desttype == null;

        private bool TryProject(MethodInfo m) =>
                TryProject_Map_TDest_source(m) ||
                TryProject_Map_TSource_TDest_source(m) ||
                TryProject_Map_TSource_TDest_source_dest(m) ||
                TryProject_Map_source_sourcetype_desttype(m) ||
                TryProject_Map_source_dest_sourcetype_desttype(m);

        // TDestination Map<TDestination>(object source)
        private bool TryProject_Map_TDest_source(MethodInfo checkedMethod)
        {
            if (!checkedMethod.IsGenericMethod) return false;

            var args = checkedMethod.GetGenericArguments();
            if (args.Length != 1 || checkedMethod.ReturnType != args[0]) return false;

            var prms = checkedMethod.GetParameters();
            if (prms.Length != 1 || prms[0].ParameterType != typeof(object)) return false;

            if (Map_TDest_source != null)
                throw CreateAmbiguousException(Map_TDest_source, checkedMethod, "TDestination Map<TDestination>(object source)");
            Map_TDest_source = checkedMethod;

            return true;
        }

        // TDestination Map<TSource, TDestination>(TSource source)
        private bool TryProject_Map_TSource_TDest_source(MethodInfo checkedMethod)
        {
            if (!checkedMethod.IsGenericMethod) return false;

            var args = checkedMethod.GetGenericArguments();
            if (args.Length != 2 || checkedMethod.ReturnType != args[1]) return false;

            var prms = checkedMethod.GetParameters();
            if (prms.Length != 1 || prms[0].ParameterType != args[0]) return false;

            if (Map_TSource_TDest_source != null)
                throw CreateAmbiguousException(Map_TSource_TDest_source, checkedMethod, "TDestination Map<TSource, TDestination>(TSource source)");
            Map_TSource_TDest_source = checkedMethod;

            return true;
        }

        // TDestination Map<TSource, TDestination>(TSource source, TDestination destination)
        private bool TryProject_Map_TSource_TDest_source_dest(MethodInfo checkedMethod)
        {
            if (!checkedMethod.IsGenericMethod) return false;

            var args = checkedMethod.GetGenericArguments();
            if (args.Length != 2 || checkedMethod.ReturnType != args[1]) return false;

            var prms = checkedMethod.GetParameters();
            if (prms.Length != 2 || prms[0].ParameterType != args[0] || prms[1].ParameterType != args[1]) return false;

            if (Map_TSource_TDest_source_dest != null)
                throw CreateAmbiguousException(Map_TSource_TDest_source_dest, checkedMethod, "TDestination Map<TSource, TDestination>(TSource source, TDestination destination)");
            Map_TSource_TDest_source_dest = checkedMethod;

            return true;
        }

        // object Map(object source, Type sourceType, Type destinationType)
        private bool TryProject_Map_source_sourcetype_desttype(MethodInfo checkedMethod)
        {
            if (checkedMethod.IsGenericMethod) return false;

            var prms = checkedMethod.GetParameters();
            if (prms.Length != 3) return false;
            if (prms[0].ParameterType != typeof(object) || prms[1].ParameterType != typeof(Type) | prms[2].ParameterType != typeof(Type)) return false;

            if (Map_source_sourcetype_desttype != null)
                throw CreateAmbiguousException(Map_source_sourcetype_desttype, checkedMethod, "object Map(object source, Type sourceType, Type destinationType)");
            Map_source_sourcetype_desttype = checkedMethod;

            return true;
        }

        // object Map(object source, object destination, Type sourceType, Type destinationType)
        private bool TryProject_Map_source_dest_sourcetype_desttype(MethodInfo checkedMethod)
        {
            if (checkedMethod.IsGenericMethod) return false;

            var prms = checkedMethod.GetParameters();
            if (prms.Length != 4) return false;
            if (prms[0].ParameterType != typeof(object) || prms[1].ParameterType != typeof(object) || prms[2].ParameterType != typeof(Type) | prms[3].ParameterType != typeof(Type)) return false;

            if (Map_source_dest_sourcetype_desttype != null)
                throw CreateAmbiguousException(Map_source_dest_sourcetype_desttype, checkedMethod, "object Map(object source, object destination, Type sourceType, Type destinationType)");
            Map_source_dest_sourcetype_desttype = checkedMethod;

            return true;
        }

        private InvalidOperationException CreateAmbiguousException(MethodInfo firstMethod, MethodInfo secondMethod, string methodSignature)
        {
            throw new InvalidOperationException($@"
More than one match for method '{methodSignature}' found in interface '{ProxiedInterface}':
1) {firstMethod}
2) {secondMethod}

The interface '{ProxiedInterface}' must have no more than 1 match for the method signature with the methods of the interface 'AutoMapper.IMapper'.");
        }
    }
}
