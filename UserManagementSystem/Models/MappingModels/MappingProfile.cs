using AutoMapper;
using UserManagementSystem.Models.Entities;
using UserManagementSystem.Models.ViewModels;

namespace UserManagementSystem.Models.MappingModels
{
    public class MappingProfile: Profile
    {
        public MappingProfile()
        {
            // User -> RegisterViewModel
            CreateMap<User, RegisterViewModel>();

            // RegisterViewModel -> User
            CreateMap<RegisterViewModel, User>()
                .ForMember(dest => dest.Hobbies,
                           opt => opt.MapFrom(src => string.Join(",", src.Hobbies ?? new List<string>())));

            // User -> EditUserViewModel
        /*    CreateMap<User, EditUserViewModel>()
                .ForMember(dest => dest.Hobbies,
                           opt => opt.MapFrom(src => src.Hobbies != null ? src.Hobbies.Split(',').ToList() : new List<string>()));*/

            // EditUserViewModel -> User
            CreateMap<EditUserViewModel, User>()
                .ForMember(dest => dest.Hobbies,
                           opt => opt.MapFrom(src => string.Join(",", src.Hobbies ?? new List<string>())));
        }
    }
}
