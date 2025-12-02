using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace UserManagementSystem.Models.Entities
{
    [Index(nameof(UserName), IsUnique = true)]
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required, StringLength(100)]
        public string Name { get; set; }

        [Required, StringLength(50)]
        public string UserName { get; set; }

        [Required]
        public string PasswordHash { get; set; }

        [Required]
        public DateTime DateOfBirth { get; set; }

        [Required, StringLength(10)]
        public string Gender { get; set; }

        public string Hobbies { get; set; }

        [StringLength(500)]
        public string Address { get; set; }

        [Required]
        public int StateId { get; set; }

        [Required]
        public int CityId { get; set; }

        [Required, StringLength(10)]
        public string Pincode { get; set; }

        [Required, StringLength(20)]
        public string Role { get; set; }

        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }

        public bool IsActive { get; set; }

        // Navigation
        [ForeignKey(nameof(StateId))]
        public virtual State State { get; set; }

        [ForeignKey(nameof(CityId))]
        public virtual City City { get; set; }

        public virtual ICollection<UserDocument> Documents { get; set; }
    }
}
