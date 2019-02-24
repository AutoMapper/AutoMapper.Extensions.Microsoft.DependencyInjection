using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace AutoMapper
{
    internal static class ProxyGenerator
    {
        private static AssemblyBuilder _asmBuilder;
        private static ModuleBuilder _moduleBuilder;

        static ProxyGenerator()
        {
            var asmName = new AssemblyName("DynamicAssembly_Automapper_Proxies");
            _asmBuilder = AssemblyBuilder.DefineDynamicAssembly(asmName, AssemblyBuilderAccess.RunAndCollect);
            _moduleBuilder = _asmBuilder.DefineDynamicModule(asmName.Name);
        }

        public static Type Generate(ProxyInfo proxyInfo)
        {
            // Just return if we've already added ProxiedInterface
            var alreadyGeneratedType = _moduleBuilder.Assembly.DefinedTypes.FirstOrDefault(t => t.GetInterfaces().Contains(proxyInfo.ProxiedInterface));
            if (alreadyGeneratedType != null)
                return alreadyGeneratedType;

            var typeBuilder = _moduleBuilder.DefineType($"{proxyInfo.ProxiedInterface.FullName}.DynamicImpl", TypeAttributes.NotPublic | TypeAttributes.Sealed, typeof(object), new[] { proxyInfo.ProxiedInterface });

            var fieldBuilder = typeBuilder.DefineField("_mapper", typeof(IMapper), FieldAttributes.Private);

            return typeBuilder
                .Generate_Ctor(fieldBuilder)
                .Generate_Map_TDest_source(proxyInfo, fieldBuilder)
                .Generate_Map_TSource_TDest_source(proxyInfo, fieldBuilder)
                .Generate_Map_TSource_TDest_source_dest(proxyInfo, fieldBuilder)
                .Generate_Map_source_sourcetype_desttype(proxyInfo, fieldBuilder)
                .Generate_Map_source_dest_sourcetype_desttype(proxyInfo, fieldBuilder)
                .CreateTypeInfo();
        }

        private static TypeBuilder Generate_Ctor(this TypeBuilder typeBuilder, FieldBuilder fieldBuilder)
        {
            var ctorBuilder = typeBuilder.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, new[] { typeof(IMapper) });
            var ctorIL = ctorBuilder.GetILGenerator();
            ctorIL.Emit(OpCodes.Ldarg_0);
            ctorIL.Emit(OpCodes.Ldarg_1);
            ctorIL.Emit(OpCodes.Stfld, fieldBuilder);
            ctorIL.Emit(OpCodes.Ret);

            return typeBuilder;
        }

        // TDestination Map<TDestination>(object source)
        private static TypeBuilder Generate_Map_TDest_source(this TypeBuilder typeBuilder, ProxyInfo proxyInfo, FieldBuilder fieldBuilder)
        {
            if (proxyInfo.Map_TDest_source != null)
            {
                var methodBuilder = typeBuilder.DefineMethod(proxyInfo.Map_TDest_source.Name, MethodAttributes.Public | MethodAttributes.Final | MethodAttributes.HideBySig | MethodAttributes.Virtual | MethodAttributes.NewSlot);
                var genericTypeParameterBuilder = methodBuilder.DefineGenericParameters(proxyInfo.Map_TDest_source.GetGenericArguments()[0].Name)[0];
                methodBuilder.SetParameters(typeof(object));
                methodBuilder.SetReturnType(genericTypeParameterBuilder);

                var ilGenerator = methodBuilder.GetILGenerator();
                ilGenerator.Emit(OpCodes.Ldarg_0); // this
                ilGenerator.Emit(OpCodes.Ldfld, fieldBuilder);
                ilGenerator.Emit(OpCodes.Ldarg_1); // source
                ilGenerator.Emit(OpCodes.Callvirt, typeof(IMapper).GetMethod(nameof(IMapper.Map), new[] { typeof(object) }));
                ilGenerator.Emit(OpCodes.Ret);
            }

            return typeBuilder;
        }

        // TDestination Map<TSource, TDestination>(TSource source)
        private static TypeBuilder Generate_Map_TSource_TDest_source(this TypeBuilder typeBuilder, ProxyInfo proxyInfo, FieldBuilder fieldBuilder)
        {
            if (proxyInfo.Map_TSource_TDest_source != null)
            {
                var methodBuilder = typeBuilder.DefineMethod(proxyInfo.Map_TSource_TDest_source.Name, MethodAttributes.Public | MethodAttributes.Final | MethodAttributes.HideBySig | MethodAttributes.Virtual | MethodAttributes.NewSlot);
                var genericTypeParameterBuilders = methodBuilder.DefineGenericParameters(proxyInfo.Map_TSource_TDest_source.GetGenericArguments()[0].Name, proxyInfo.Map_TSource_TDest_source.GetGenericArguments()[1].Name);
                methodBuilder.SetParameters(genericTypeParameterBuilders[0]);
                methodBuilder.SetReturnType(genericTypeParameterBuilders[1]);

                var ilGenerator = methodBuilder.GetILGenerator();
                ilGenerator.Emit(OpCodes.Ldarg_0); // this
                ilGenerator.Emit(OpCodes.Ldfld, fieldBuilder);
                ilGenerator.Emit(OpCodes.Ldarg_1); // source
                ilGenerator.Emit(OpCodes.Callvirt, typeof(IMapper).GetMethods().Where(m => m.Name == nameof(IMapper.Map) && m.IsGenericMethod && m.GetGenericArguments().Length == 2 && m.GetParameters().Length == 1).Single());
                ilGenerator.Emit(OpCodes.Ret);
            }

            return typeBuilder;
        }

        // TDestination Map<TSource, TDestination>(TSource source, TDestination destination)
        private static TypeBuilder Generate_Map_TSource_TDest_source_dest(this TypeBuilder typeBuilder, ProxyInfo proxyInfo, FieldBuilder fieldBuilder)
        {
            if (proxyInfo.Map_TSource_TDest_source_dest != null)
            {
                var methodBuilder = typeBuilder.DefineMethod(proxyInfo.Map_TSource_TDest_source_dest.Name, MethodAttributes.Public | MethodAttributes.Final | MethodAttributes.HideBySig | MethodAttributes.Virtual | MethodAttributes.NewSlot);
                var genericTypeParameterBuilders = methodBuilder.DefineGenericParameters(proxyInfo.Map_TSource_TDest_source_dest.GetGenericArguments()[0].Name, proxyInfo.Map_TSource_TDest_source_dest.GetGenericArguments()[1].Name);
                var y = proxyInfo.Map_TSource_TDest_source_dest.GetGenericArguments();
                methodBuilder.SetParameters(genericTypeParameterBuilders[0], genericTypeParameterBuilders[1]);
                methodBuilder.SetReturnType(genericTypeParameterBuilders[1]);

                var ilGenerator = methodBuilder.GetILGenerator();
                ilGenerator.Emit(OpCodes.Ldarg_0); // this
                ilGenerator.Emit(OpCodes.Ldfld, fieldBuilder);
                ilGenerator.Emit(OpCodes.Ldarg_1); // source
                ilGenerator.Emit(OpCodes.Ldarg_2); // destination
                ilGenerator.Emit(OpCodes.Callvirt, typeof(IMapper).GetMethods().Where(m => m.Name == nameof(IMapper.Map) && m.IsGenericMethod && m.GetGenericArguments().Length == 2 && m.GetParameters().Length == 2 && m.GetParameters()[1].ParameterType.Name == "TDestination").Single());
                ilGenerator.Emit(OpCodes.Ret);
            }

            return typeBuilder;
        }

        // object Map(object source, Type sourceType, Type destinationType)
        private static TypeBuilder Generate_Map_source_sourcetype_desttype(this TypeBuilder typeBuilder, ProxyInfo proxyInfo, FieldBuilder fieldBuilder)
        {
            if (proxyInfo.Map_source_sourcetype_desttype != null)
            {
                var methodBuilder = typeBuilder.DefineMethod(proxyInfo.Map_source_sourcetype_desttype.Name, MethodAttributes.Public | MethodAttributes.Final | MethodAttributes.HideBySig | MethodAttributes.Virtual | MethodAttributes.NewSlot);
                methodBuilder.SetParameters(typeof(object), typeof(Type), typeof(Type));
                methodBuilder.SetReturnType(typeof(object));

                var ilGenerator = methodBuilder.GetILGenerator();
                ilGenerator.Emit(OpCodes.Ldarg_0); // this
                ilGenerator.Emit(OpCodes.Ldfld, fieldBuilder);
                ilGenerator.Emit(OpCodes.Ldarg_1); // source
                ilGenerator.Emit(OpCodes.Ldarg_2); // sourceType
                ilGenerator.Emit(OpCodes.Ldarg_3); // destinationType
                ilGenerator.Emit(OpCodes.Callvirt, typeof(IMapper).GetMethod(nameof(IMapper.Map), new[] { typeof(object), typeof(Type), typeof(Type) }));
                ilGenerator.Emit(OpCodes.Ret);
            }

            return typeBuilder;
        }

        // object Map(object source, object destination, Type sourceType, Type destinationType)
        private static TypeBuilder Generate_Map_source_dest_sourcetype_desttype(this TypeBuilder typeBuilder, ProxyInfo proxyInfo, FieldBuilder fieldBuilder)
        {
            if (proxyInfo.Map_source_dest_sourcetype_desttype != null)
            {
                var methodBuilder = typeBuilder.DefineMethod(proxyInfo.Map_source_dest_sourcetype_desttype.Name, MethodAttributes.Public | MethodAttributes.Final | MethodAttributes.HideBySig | MethodAttributes.Virtual | MethodAttributes.NewSlot);
                methodBuilder.SetParameters(typeof(object), typeof(object), typeof(Type), typeof(Type));
                methodBuilder.SetReturnType(typeof(object));

                var ilGenerator = methodBuilder.GetILGenerator();
                ilGenerator.Emit(OpCodes.Ldarg_0); // this
                ilGenerator.Emit(OpCodes.Ldfld, fieldBuilder);
                ilGenerator.Emit(OpCodes.Ldarg_1); // source
                ilGenerator.Emit(OpCodes.Ldarg_2); // destination
                ilGenerator.Emit(OpCodes.Ldarg_3); // sourceType
                ilGenerator.Emit(OpCodes.Ldarg_S, 4); // destinationType
                ilGenerator.Emit(OpCodes.Callvirt, typeof(IMapper).GetMethod(nameof(IMapper.Map), new[] { typeof(object), typeof(object), typeof(Type), typeof(Type) }));
                ilGenerator.Emit(OpCodes.Ret);
            }

            return typeBuilder;
        }
    }
}
