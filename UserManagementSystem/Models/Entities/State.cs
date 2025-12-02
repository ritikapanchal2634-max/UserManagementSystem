using System.ComponentModel.DataAnnotations;

namespace UserManagementSystem.Models.Entities
{
    public class State
    {
        [Key]
        public int Id { get; set; }

        [Required, StringLength(100)]
        public string Name { get; set; }

        public virtual ICollection<City> Cities { get; set; }
    }
}
