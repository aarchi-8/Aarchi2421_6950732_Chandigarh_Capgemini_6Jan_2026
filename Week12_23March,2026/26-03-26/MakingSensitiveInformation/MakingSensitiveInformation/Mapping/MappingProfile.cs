namespace MakingSensitiveInformation.Mapping
{
    using AutoMapper;
    using MakingSensitiveInformation.DTOs;
    using MakingSensitiveInformation.Models;

    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Account, AccountDTO>()
                .ForMember(dest => dest.AccountNumber,
                    opt => opt.MapFrom(src => MaskAccount(src.AccountNumber)));
        }

        // ✅ Custom masking method
        private string MaskAccount(string accNumber)
        {
            if (string.IsNullOrEmpty(accNumber))
                return accNumber;

            // If length <= 4 → return as it is
            if (accNumber.Length <= 4)
                return accNumber;

            // If length <= 6 → mask all except last 2
            if (accNumber.Length <= 6)
                return new string('X', accNumber.Length - 2) + accNumber.Substring(accNumber.Length - 2);

            // Normal case → mask first 6 digits
            return "XXXXXX" + accNumber.Substring(accNumber.Length - 4);
        }
    }
}
