using AutoMapper;
using SmartHealthcare.Models.DTOs;
using SmartHealthcare.Models.Entities;

namespace SmartHealthcare.API.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // ── User ──────────────────────────────────────────────
            CreateMap<User, UserDTO>();
            CreateMap<RegisterDTO, User>();

            // ── Patient ───────────────────────────────────────────
            CreateMap<Patient, PatientDTO>()
                .ForMember(dest => dest.FullName,
                    opt => opt.MapFrom(src => src.User != null ? src.User.FullName : string.Empty))
                .ForMember(dest => dest.Email,
                    opt => opt.MapFrom(src => src.User != null ? src.User.Email : string.Empty))
                .ForMember(dest => dest.Gender,
                    opt => opt.MapFrom(src => src.Gender.ToString()));

            CreateMap<CreatePatientDTO, Patient>();

            // ── Doctor ────────────────────────────────────────────
            CreateMap<Doctor, DoctorDTO>()
                .ForMember(dest => dest.FullName,
                    opt => opt.MapFrom(src => src.User != null ? src.User.FullName : string.Empty))
                .ForMember(dest => dest.Email,
                    opt => opt.MapFrom(src => src.User != null ? src.User.Email : string.Empty))
                .ForMember(dest => dest.Specializations,
                    opt => opt.MapFrom(src => src.DoctorSpecializations
                        .Select(ds => ds.Specialization != null ? ds.Specialization.Name : string.Empty)
                        .ToList()));

            CreateMap<CreateDoctorDTO, Doctor>();

            // ── Appointment ───────────────────────────────────────
            CreateMap<Appointment, AppointmentDTO>()
                .ForMember(dest => dest.PatientName,
                    opt => opt.MapFrom(src => src.Patient != null && src.Patient.User != null
                        ? src.Patient.User.FullName : string.Empty))
                .ForMember(dest => dest.DoctorName,
                    opt => opt.MapFrom(src => src.Doctor != null && src.Doctor.User != null
                        ? src.Doctor.User.FullName : string.Empty))
                .ForMember(dest => dest.Status,
                    opt => opt.MapFrom(src => src.Status.ToString()));

            CreateMap<CreateAppointmentDTO, Appointment>();

            // ── Prescription ──────────────────────────────────────
            CreateMap<Prescription, PrescriptionDTO>()
                .ForMember(dest => dest.PatientName,
                    opt => opt.MapFrom(src => src.Appointment != null &&
                        src.Appointment.Patient != null && src.Appointment.Patient.User != null
                        ? src.Appointment.Patient.User.FullName : string.Empty))
                .ForMember(dest => dest.DoctorName,
                    opt => opt.MapFrom(src => src.Appointment != null &&
                        src.Appointment.Doctor != null && src.Appointment.Doctor.User != null
                        ? src.Appointment.Doctor.User.FullName : string.Empty));

            CreateMap<CreatePrescriptionDTO, Prescription>();

            // ── Specialization ────────────────────────────────────
            CreateMap<Specialization, SpecializationDTO>();
            CreateMap<CreateSpecializationDTO, Specialization>();
        }
    }
}