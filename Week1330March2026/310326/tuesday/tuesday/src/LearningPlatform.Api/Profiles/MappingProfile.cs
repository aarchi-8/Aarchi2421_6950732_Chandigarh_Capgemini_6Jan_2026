using AutoMapper;
using LearningPlatform.Api.DTOs;
using LearningPlatform.Api.Models;

namespace LearningPlatform.Api.Profiles;

public class MappingProfile : AutoMapper.Profile
{
    public MappingProfile()
    {
        CreateMap<Course, CourseDto>()
            .ForMember(dest => dest.InstructorUsername, opt => opt.MapFrom(src => src.Instructor != null ? src.Instructor.Username : string.Empty));
        CreateMap<Lesson, LessonDto>();
        CreateMap<CreateCourseDto, Course>();
        CreateMap<CreateLessonDto, Lesson>();
        CreateMap<UserRegisterDto, User>(MemberList.Source)
            .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
            .ForMember(dest => dest.Profile, opt => opt.MapFrom(src => src.Profile));
        CreateMap<ProfileDto, Models.Profile>();
    }
}
