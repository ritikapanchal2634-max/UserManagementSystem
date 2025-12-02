using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UserManagementSystem.Models.Entities
{
    public class City
    {
        [Key]
        public int Id { get; set; }

        [Required, StringLength(100)]
        public string Name { get; set; }

        [Required]
        public int StateId { get; set; }

        [ForeignKey(nameof(StateId))]
        public virtual State State { get; set; }
    }
}
