using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace AutoMapper.Extensions.Microsoft.DependencyInjection
{
    /// <summary>
    /// a scanner that scan an assembly for looking <see cref="Profile"/> type.
    /// <para>
    /// since <see cref="IValueResolver{TSource, TDestination, TDestMember}"/>
    /// <see cref="IMemberValueResolver{TSource, TDestination, TSourceMember, TDestMember}"/>
    /// <see cref="ITypeConverter{TSource, TDestination}"/> 
    /// <see cref="IValueConverter{TSourceMember, TDestinationMember}"/> 
    /// <see cref="IMappingAction{TSource, TDestination}"/> are associate with the <see cref="Profile"/>, we should scan those type either.
    /// </para>
    /// </summary>
    public interface IAutoMapperAssemblyDescription
    {
        IEnumerable<Assembly> Assemblies { get; }
    }
}
