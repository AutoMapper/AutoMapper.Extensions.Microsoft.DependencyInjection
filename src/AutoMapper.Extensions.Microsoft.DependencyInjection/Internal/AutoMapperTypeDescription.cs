using System;
using System.Collections.Generic;
using System.Text;

namespace AutoMapper.Extensions.Microsoft.DependencyInjection.Internal
{
    public class AutoMapperTypeDescription : IAutoMapperTypeDescription
    {
        public AutoMapperTypeDescription(Type type):this(new[] { type })
        {
            type = type ?? throw new ArgumentNullException(nameof(type));
        }
        public AutoMapperTypeDescription(IEnumerable<Type> types)
        {
            ProfileTypes = types ?? throw new ArgumentNullException(nameof(types));
        }

        public IEnumerable<Type> ProfileTypes { get; }
    }
}
