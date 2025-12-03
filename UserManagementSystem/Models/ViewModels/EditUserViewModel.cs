using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;

namespace UserManagementSystem.Models.ViewModels
{
    public class EditUserViewModel
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Required]
        public DateTime DateOfBirth { get; set; }

        [Required]
        public string Gender { get; set; }

        public List<string> Hobbies { get; set; }

        [StringLength(500)]
        public string Address { get; set; }

        [Required]
        public int StateId { get; set; }

        [Required]
        public int CityId { get; set; }

        [Required]
        [StringLength(10)]
        public string Pincode { get; set; }

        public List<IFormFile> NewDocuments { get; set; }
        public List<UserDocumentViewModel>? ExistingDocuments { get; set; }
    }
}
