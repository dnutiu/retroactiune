using AutoMapper;

namespace Retroactiune.Models
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<FeedbackReceiver, FeedbackReceiverDto>().ReverseMap();
        }
    }
}