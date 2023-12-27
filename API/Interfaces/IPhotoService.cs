using CloudinaryDotNet.Actions;

namespace API.Interfaces;

public interface IPhotoService
{
    Task<ImageUploadResult> AddPhoto(IFormFile file);

    Task<DeletionResult> DeletePhoto(string publicId);
}