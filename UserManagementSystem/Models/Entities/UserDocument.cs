using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UserManagementSystem.Models.Entities
{
    public class UserDocument
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required, StringLength(255)]
        public string FileName { get; set; }

        [Required, StringLength(500)]
        public string FilePath { get; set; }

        [StringLength(50)]
        public string FileType { get; set; }

        public long FileSize { get; set; }

        public DateTime UploadedDate { get; set; }

        [ForeignKey(nameof(UserId))]
        public virtual User User { get; set; }
    }
}
