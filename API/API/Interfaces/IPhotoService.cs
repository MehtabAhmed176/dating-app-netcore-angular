using CloudinaryDotNet.Actions;

namespace API.Interfaces;

public interface IPhotoService
{
    /*Returning a Task of Type ImageUpload.
     This is similar to return simple promise in JS as there are no types there */
    public Task<ImageUploadResult> AddPhotoAsync(IFormFile file);
    public Task<DeletionResult> DeletePhotoAsync(string publicId);
}