using System.ComponentModel.DataAnnotations;

namespace UserManagementSystem.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Required]
        [StringLength(50)]
        public string UserName { get; set; }

        [Required]
        public string PasswordHash { get; set; }

        [Required]
        public DateTime DateOfBirth { get; set; }

        [Required]
        [StringLength(10)]
        public string Gender { get; set; }

        public string Hobbies { get; set; } // Stored as comma-separated values

        [StringLength(500)]
        public string Address { get; set; }

        [Required]
        public int StateId { get; set; }

        [Required]
        public int CityId { get; set; }

        [Required]
        [StringLength(10)]
        public string Pincode { get; set; }

        [Required]
        [StringLength(20)]
        public string Role { get; set; } // Admin, User

        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool IsActive { get; set; }

        // Navigation properties
        public virtual State State { get; set; }
        public virtual City City { get; set; }
        public virtual ICollection<UserDocument> Documents { get; set; }
    }
}
