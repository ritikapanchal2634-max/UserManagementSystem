namespace UserManagementSystem.Services
{
    public interface IFileUploadService
    {
        Task<(bool Success, List<string> FilePaths, string ErrorMessage)> UploadFilesAsync(List<IFormFile> files);
        bool DeleteFile(string filePath);
        (bool IsValid, string ErrorMessage) ValidateFile(IFormFile file);
    }
    public class FileUploadService: IFileUploadService
    {

        private readonly IConfiguration _configuration;
        private readonly List<string> _allowedExtensions;
        private readonly long _maxFileSizeInBytes;
        private readonly string _uploadPath;

        public FileUploadService(IConfiguration configuration)
        {
            _configuration = configuration;
            _allowedExtensions = _configuration.GetSection("FileUploadSettings:AllowedExtensions").Get<List<string>>();
            _maxFileSizeInBytes = long.Parse(_configuration["FileUploadSettings:MaxFileSizeInBytes"]);
            _uploadPath = _configuration["FileUploadSettings:UploadPath"];

            // Ensure upload directory exists
            if (!Directory.Exists(_uploadPath))
            {
                Directory.CreateDirectory(_uploadPath);
            }
        }

        public (bool IsValid, string ErrorMessage) ValidateFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return (false, "File is empty");
            }

            // Check file size
            if (file.Length > _maxFileSizeInBytes)
            {
                var maxSizeInMB = _maxFileSizeInBytes / (1024.0 * 1024.0);
                return (false, $"File size exceeds maximum allowed size of {maxSizeInMB:F2} MB");
            }

            // Check file extension
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!_allowedExtensions.Contains(extension))
            {
                return (false, $"File type '{extension}' is not allowed. Allowed types: {string.Join(", ", _allowedExtensions)}");
            }

            // Additional security: Check file content (magic numbers)
            if (!IsValidFileContent(file, extension))
            {
                return (false, "File content does not match its extension");
            }

            return (true, null);
        }

        public async Task<(bool Success, List<string> FilePaths, string ErrorMessage)> UploadFilesAsync(List<IFormFile> files)
        {
            var filePaths = new List<string>();

            if (files == null || !files.Any())
            {
                return (true, filePaths, null);
            }

            foreach (var file in files)
            {
                var (isValid, errorMessage) = ValidateFile(file);
                if (!isValid)
                {
                    // Clean up already uploaded files
                    foreach (var path in filePaths)
                    {
                        DeleteFile(path);
                    }
                    return (false, null, errorMessage);
                }

                try
                {
                    // Generate unique filename
                    var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(file.FileName)}";
                    var filePath = Path.Combine(_uploadPath, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    filePaths.Add(filePath);
                }
                catch (Exception ex)
                {
                    // Clean up already uploaded files
                    foreach (var path in filePaths)
                    {
                        DeleteFile(path);
                    }
                    return (false, null, $"Error uploading file: {ex.Message}");
                }
            }

            return (true, filePaths, null);
        }

        public bool DeleteFile(string filePath)
        {
            try
            {
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        private bool IsValidFileContent(IFormFile file, string extension)
        {
            try
            {
                using var reader = new BinaryReader(file.OpenReadStream());
                var headerBytes = reader.ReadBytes(8);

                // Check magic numbers for common file types
                return extension switch
                {
                    ".pdf" => headerBytes.Length >= 4 &&
                              headerBytes[0] == 0x25 && headerBytes[1] == 0x50 &&
                              headerBytes[2] == 0x44 && headerBytes[3] == 0x46,

                    ".jpg" or ".jpeg" => headerBytes.Length >= 3 &&
                                         headerBytes[0] == 0xFF && headerBytes[1] == 0xD8 &&
                                         headerBytes[2] == 0xFF,

                    ".png" => headerBytes.Length >= 8 &&
                              headerBytes[0] == 0x89 && headerBytes[1] == 0x50 &&
                              headerBytes[2] == 0x4E && headerBytes[3] == 0x47,

                    ".doc" or ".docx" => headerBytes.Length >= 4 &&
                                         headerBytes[0] == 0xD0 && headerBytes[1] == 0xCF &&
                                         headerBytes[2] == 0x11 && headerBytes[3] == 0xE0,

                    _ => true // Allow other types without strict validation
                };
            }
            catch
            {
                return false;
            }
        }

    }
}
