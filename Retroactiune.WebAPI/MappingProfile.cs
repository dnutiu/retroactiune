using AutoMapper;
using Retroactiune.Core.Entities;
using Retroactiune.Core.Services;
using Retroactiune.DataTransferObjects;

namespace Retroactiune
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<ListTokensFiltersDto, TokenListFilters>();
            CreateMap<ListFeedbacksFiltersDto, FeedbacksListFilters>();
            CreateMap<FeedbackReceiver, FeedbackReceiverInDto>().ReverseMap();
            CreateMap<FeedbackReceiver, FeedbackReceiverOutDto>();
            CreateMap<FeedbackInDto, Feedback>();
        }
    }
}