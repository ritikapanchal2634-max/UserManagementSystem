namespace UserManagementSystem.Models.ViewModels
{
    public class UserDocumentViewModel
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public string FileType { get; set; }
        public long FileSize { get; set; }
        public DateTime UploadedDate { get; set; }
    }
}
