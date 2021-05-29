using AutoMapper;
using Retroactiune.Models;

namespace Retroactiune
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<FeedbackReceiver, FeedbackReceiverDto>().ReverseMap();
        }
    }
}