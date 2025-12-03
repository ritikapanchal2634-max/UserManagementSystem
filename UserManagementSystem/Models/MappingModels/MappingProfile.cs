using AutoMapper;
using UserManagementSystem.Models.Entities;
using UserManagementSystem.Models.ViewModels;

namespace UserManagementSystem.Models.MappingModels
{
    public class MappingProfile: Profile
    {
        public MappingProfile()
        {
            // RegisterViewModel → User
            CreateMap<RegisterViewModel, User>()
                .ForMember(dest => dest.Hobbies,
                    opt => opt.ConvertUsing(new ListToStringConverter(), src => src.Hobbies)).ForMember(dest => dest.Documents, opt => opt.Ignore());

            // User → EditUserViewModel
            CreateMap<User, EditUserViewModel>()
                .ForMember(dest => dest.Hobbies,
                    opt => opt.ConvertUsing(new StringToListConverter(), src => src.Hobbies));

            // EditUserViewModel → User
            CreateMap<EditUserViewModel, User>()
                .ForMember(dest => dest.Hobbies,
                    opt => opt.ConvertUsing(new ListToStringConverter(), src => src.Hobbies));

            CreateMap<UserDocument, UserDocumentViewModel>();
        }
    }

    public class ListToStringConverter : IValueConverter<List<string>, string>
    {
        public string Convert(List<string> source, ResolutionContext context)
        {
            return source == null ? "" : string.Join(",", source);
        }
    }
    public class StringToListConverter : IValueConverter<string, List<string>>
    {
        public List<string> Convert(string source, ResolutionContext context)
        {
            return string.IsNullOrEmpty(source)
                ? new List<string>()
                : source.Split(',').ToList();
        }
    }
}
