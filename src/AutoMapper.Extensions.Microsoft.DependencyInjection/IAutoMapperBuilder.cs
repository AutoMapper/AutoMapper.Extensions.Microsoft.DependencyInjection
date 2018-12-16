using Microsoft.Extensions.DependencyInjection;
using System.Text;

namespace AutoMapper.Extensions.Microsoft.DependencyInjection
{
    public interface IAutoMapperBuilder
    {
        IServiceCollection ServiceDescriptor { get; }

    }
}
