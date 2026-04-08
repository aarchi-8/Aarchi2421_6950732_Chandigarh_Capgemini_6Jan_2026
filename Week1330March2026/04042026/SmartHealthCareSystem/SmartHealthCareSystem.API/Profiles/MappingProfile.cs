using AutoMapper;
using SmartHealthCareSystem.Shared.DTOs;
using SmartHealthCareSystem.Shared.Models;

namespace SmartHealthCareSystem.API.Profiles
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // User mappings
            CreateMap<User, UserDto>();
            CreateMap<CreateUserDto, User>();
            CreateMap<UpdateUserDto, User>();

            // Department mappings
            CreateMap<Department, DepartmentDto>();
            CreateMap<CreateDepartmentDto, Department>();
            CreateMap<UpdateDepartmentDto, Department>();

            // Doctor mappings
            CreateMap<Doctor, DoctorDto>()
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.User != null ? src.User.FullName : string.Empty))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.User != null ? src.User.Email : string.Empty))
                .ForMember(dest => dest.DepartmentName, opt => opt.MapFrom(src => src.Department != null ? src.Department.DepartmentName : string.Empty));
            CreateMap<CreateDoctorDto, Doctor>();
            CreateMap<UpdateDoctorDto, Doctor>();

            // Appointment mappings
            CreateMap<Appointment, AppointmentDto>()
                .ForMember(dest => dest.PatientName, opt => opt.MapFrom(src => src.Patient != null ? src.Patient.FullName : string.Empty))
                .ForMember(dest => dest.DoctorName, opt => opt.MapFrom(src => src.Doctor != null && src.Doctor.User != null ? src.Doctor.User.FullName : string.Empty))
                .ForMember(dest => dest.Diagnosis, opt => opt.MapFrom(src => src.Prescription != null ? src.Prescription.Diagnosis : string.Empty));
            CreateMap<CreateAppointmentDto, Appointment>();
            CreateMap<UpdateAppointmentDto, Appointment>();

            // Prescription mappings
            CreateMap<Prescription, PrescriptionDto>();
            CreateMap<CreatePrescriptionDto, Prescription>();
            CreateMap<UpdatePrescriptionDto, Prescription>();

            // Bill mappings
            CreateMap<Bill, BillDto>()
                .ForMember(dest => dest.PatientName, opt => opt.MapFrom(src => src.Appointment != null && src.Appointment.Patient != null ? src.Appointment.Patient.FullName : string.Empty))
                .ForMember(dest => dest.DoctorName, opt => opt.MapFrom(src => src.Appointment != null && src.Appointment.Doctor != null && src.Appointment.Doctor.User != null ? src.Appointment.Doctor.User.FullName : string.Empty))
                .ForMember(dest => dest.Diagnosis, opt => opt.MapFrom(src => src.Appointment != null && src.Appointment.Prescription != null ? src.Appointment.Prescription.Diagnosis : string.Empty))
                .ForMember(dest => dest.AppointmentDate, opt => opt.MapFrom(src => src.Appointment != null ? src.Appointment.AppointmentDate : DateTime.MinValue))
                .ForMember(dest => dest.AppointmentStatus, opt => opt.MapFrom(src => src.Appointment != null ? src.Appointment.Status : string.Empty))
                .ForMember(dest => dest.DepartmentName, opt => opt.MapFrom(src => src.Appointment != null && src.Appointment.Doctor != null && src.Appointment.Doctor.Department != null ? src.Appointment.Doctor.Department.DepartmentName : string.Empty))
                .ForMember(dest => dest.TotalAmount, opt => opt.MapFrom(src => src.TotalAmount));
            CreateMap<CreateBillDto, Bill>();
            CreateMap<UpdateBillDto, Bill>();
        }
    }
}
