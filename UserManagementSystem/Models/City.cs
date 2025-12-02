using System.ComponentModel.DataAnnotations;

namespace UserManagementSystem.Models
{
    public class City
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Required]
        public int StateId { get; set; }

        public virtual State State { get; set; }
    }
}
