using AutoMapper;

namespace TestWebApp
{
    internal sealed class Person
    {
        public int ID { get; set; }
        public string FirstName { get; set; }
    }

    internal sealed class PersonDTO
    {
        public string LastName { get; set; }
    }

    internal sealed class PersonMappingProfile : Profile
    {
        public PersonMappingProfile()
        {
            // Maps intentionally mismatched to ensure config is invalid
            CreateMap<Person, PersonDTO>();
            CreateMap<PersonDTO, Person>();
        }
    }
}
