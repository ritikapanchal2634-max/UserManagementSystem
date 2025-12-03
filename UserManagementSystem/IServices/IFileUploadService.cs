namespace UserManagementSystem.IServices
{
    public interface IFileUploadService
    {
        Task<(bool Success, List<string> FilePaths, string ErrorMessage)> UploadFilesAsync(List<IFormFile> files);
        bool DeleteFile(string filePath);
        (bool IsValid, string ErrorMessage) ValidateFile(IFormFile file);
    }
}
