using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SecureAccountDetails.Mapping;
using SecureAccountDetails.Models;

namespace SecureAccountDetails.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {


            CreateMap<Account, AccountDetailsDTO>()
                .ForMember(dest => dest.MaskedAccountNumber,
                    opt => opt.MapFrom(src =>
                        "XXXX-XXXX-" + src.AccountNumber.Substring(src.AccountNumber.Length - 4)
                    ));
        }
    }
}
