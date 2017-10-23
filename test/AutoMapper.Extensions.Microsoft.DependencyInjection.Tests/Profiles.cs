namespace AutoMapper.Extensions.Microsoft.DependencyInjection.Tests
{
    public class Source
    {
        
    }

    public class Dest
    {
        
    }

    public class Source2
    {
    }

    public class Dest2
    {
        public int ResolvedValue { get; set; }
    }

    public class Profile1 : Profile
    {
        public Profile1()
        {
            CreateMap<Source, Dest>();
        }
    }

    public abstract class AbstractProfile : Profile { }

    internal class Profile2 : Profile
    {
        public Profile2()
        {
            CreateMap<Source2, Dest2>()
                .ForMember(d => d.ResolvedValue, opt => opt.ResolveUsing<DependencyResolver>());
        }
    }

    public class DependencyResolver : IValueResolver<object, object, int>
    {
        private readonly ISomeService _service;

        public DependencyResolver(ISomeService service)
        {
            _service = service;
        }

        public int Resolve(object source, object destination, int destMember, ResolutionContext context)
        {
            return _service.Modify(destMember);
        }
    }

    public interface ISomeService
    {
        int Modify(int value);
    }

    public class FooService : ISomeService
    {
        private readonly int _value;

        public FooService(int value)
        {
            _value = value;
        }

        public int Modify(int value) => value + _value;
    }

    internal class FooMappingAction : IMappingAction<object, object>
    {
        public void Process(object source, object destination) { }
    }

    internal class FooValueResolver: IValueResolver<object, object, object>
    {
        public object Resolve(object source, object destination, object destMember, ResolutionContext context)
        {
            return null;
        }
    }

    internal class FooMemberValueResolver : IMemberValueResolver<object, object, object, object>
    {
        public object Resolve(object source, object destination, object sourceMember, object destMember, ResolutionContext context)
        {
            return null;
        }
    }

    internal class FooTypeConverter : ITypeConverter<object, object>
    {
        public object Convert(object source, object destination, ResolutionContext context)
        {
            return null;
        }
    }
}