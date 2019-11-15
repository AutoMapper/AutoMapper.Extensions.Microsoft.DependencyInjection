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
        public int ConvertedValue { get; set; }
    }

    public class Dest2
    {
        public int ResolvedValue { get; set; }
        public int ConvertedValue { get; set; }
    }

    public class Source3
    {
        public int Value { get; set; }
    }

    [AutoMap(typeof(Source3))]
    public class Dest3
    {
        public int Value { get; set; }
    }


    public class Source4
    {
        public string Value { get; set; }
    }

    public class Dest4
    {
        public string Value { get; set; }
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
                .ForMember(d => d.ResolvedValue, opt => opt.MapFrom<DependencyResolver>())
                .ForMember(d => d.ConvertedValue, opt => opt.ConvertUsing<DependencyValueConverter, int>());
        }
    }

    public class ProfileWithDependency : Profile
    {
        public ProfileWithDependency(ISomeService2 service)
        {
            CreateMap<Source4, Dest4>();
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

    public interface ISomeService2
    {
        string Modify(string value);
    }

    public class MutableService : ISomeService
    {
        public int Value { get; set; }

        public int Modify(int value) => value + Value;
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

    public class TrimStringService : ISomeService2
    {
        public string Modify(string value)
        {
            return value == null
                ? string.Empty
                : value.Trim();
        }
    }

    internal class FooMappingAction : IMappingAction<object, object>
    {
        public void Process(object source, object destination, ResolutionContext context) { }
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

    internal class FooValueConverter : IValueConverter<int, int>
    {
        public int Convert(int sourceMember, ResolutionContext context)
            => sourceMember + 1;
    }

    internal class DependencyValueConverter : IValueConverter<int, int>
    {
        private readonly ISomeService _service;

        public DependencyValueConverter(ISomeService service) => _service = service;

        public int Convert(int sourceMember, ResolutionContext context)
            => _service.Modify(sourceMember);
    }
}