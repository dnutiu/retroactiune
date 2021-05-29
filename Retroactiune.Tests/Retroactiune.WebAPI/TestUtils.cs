using AutoMapper;

namespace Retroactiune.Tests.Retroactiune.WebAPI
{
    public static class TestUtils
    {
        public static IMapper GetMapper()
        {
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<MappingProfile>();
            });
            var mapper = configuration.CreateMapper();
            return mapper;
        }
    }
}