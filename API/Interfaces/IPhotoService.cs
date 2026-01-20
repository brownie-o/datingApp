using System;
using CloudinaryDotNet.Actions;

namespace API.Interfaces;

public interface IPhotoService
{
  // IFormFile: file in .NET for handling file uploads
  Task<ImageUploadResult> UploadPhotoAsync(IFormFile file);
  Task<DeletionResult> DeletePhotoAsync(string publicId);
  
}
