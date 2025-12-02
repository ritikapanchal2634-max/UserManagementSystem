using System.ComponentModel.DataAnnotations;

namespace UserManagementSystem.Models
{
    public class State
    {

        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        public virtual ICollection<City> Cities { get; set; }
    }
}
