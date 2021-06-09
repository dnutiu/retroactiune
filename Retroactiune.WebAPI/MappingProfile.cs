using AutoMapper;
using Retroactiune.DataTransferObjects;
using Retroactiune.Models;

namespace Retroactiune
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<FeedbackReceiver, FeedbackReceiverInDto>().ReverseMap();
            CreateMap<FeedbackReceiver, FeedbackReceiverOutDto>();
        }
    }
}