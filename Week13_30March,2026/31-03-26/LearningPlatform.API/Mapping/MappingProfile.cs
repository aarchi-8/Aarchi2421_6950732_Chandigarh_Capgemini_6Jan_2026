using LearningPlatform.DTOs;
using AutoMapper;
using LearningPlatform.Models;
 
namespace LearnHub.Mappings;
 
public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // ── User ─────────────────────────────────────────────
        CreateMap<User, UserDto>();
 
        // ── Profile ──────────────────────────────────────────
        CreateMap<Profile, ProfileDto>();
        CreateMap<UpdateProfileDto, Profile>()
            .ForAllMembers(opt => opt.Condition((_, _, srcMember) => srcMember != null));
 
        // ── Course ───────────────────────────────────────────
        CreateMap<Course, CourseDto>()
            .ForMember(d => d.InstructorName,   opt => opt.MapFrom(s => s.Instructor.Username))
            .ForMember(d => d.EnrollmentCount,  opt => opt.MapFrom(s => s.Enrollments.Count))
            .ForMember(d => d.LessonCount,      opt => opt.MapFrom(s => s.Lessons.Count));
 
        CreateMap<CreateCourseDto, Course>();
 
        CreateMap<UpdateCourseDto, Course>()
            .ForAllMembers(opt => opt.Condition((_, _, srcMember) => srcMember != null));
 
        // ── Lesson ───────────────────────────────────────────
        CreateMap<Lesson, LessonDto>();
        CreateMap<CreateLessonDto, Lesson>();
 
        // ── Enrollment ───────────────────────────────────────
        CreateMap<Enrollment, EnrollmentDto>()
            .ForMember(d => d.Username,    opt => opt.MapFrom(s => s.User.Username))
            .ForMember(d => d.CourseTitle, opt => opt.MapFrom(s => s.Course.Title));
    }
}