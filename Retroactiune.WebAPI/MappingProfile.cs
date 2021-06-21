using AutoMapper;
using Retroactiune.Core.Entities;
using Retroactiune.DataTransferObjects;

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