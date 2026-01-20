using System;
using API.Helpers;
using API.Interfaces;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.Extensions.Options;

namespace API.Services;

public class PhotoService : IPhotoService
{
  // ctor: create a constructor to intialize a Cloudinary account based on the parameters provided in appsettings.json
  private readonly Cloudinary _cloudinary; // _ = private field for C# class

  // IOptions<T>: provides access to strongly-typed configuration settings through dependency injection.
  public PhotoService(IOptions<CloudinarySettings> config)
  {
    var account = new Account( // Account class from CloudinaryDotNet
      config.Value.CloudName,
      config.Value.ApiKey,
      config.Value.ApiSecret
    );

    // Cloudinary: main class for interacting with Cloudinary API, initializes a new instance of the Cloudinary class with Cloudinary account.
    _cloudinary = new Cloudinary(account);

  }

  public async Task<DeletionResult> DeletePhotoAsync(string publicId)
  {
    var deleteParams = new DeletionParams(publicId);
    return await _cloudinary.DestroyAsync(deleteParams);
  }

  public async Task<ImageUploadResult> UploadPhotoAsync(IFormFile file)
  {
    var uploadResult = new ImageUploadResult();

    if (file.Length > 0)
    {
      // using: ensures that the stream is properly disposed of after use
      // OpenReadStream(): opens a read-only stream to access the file's content, so that Cloudinary can read and upload it
      await using var stream = file.OpenReadStream();
      var uploadParams = new ImageUploadParams
      {
        File = new FileDescription(file.FileName, stream),
        Transformation = new Transformation().Height(500).Width(500).Crop("fill").Gravity("face"),
        Folder = "da-ang20"
      };
      uploadResult = await _cloudinary.UploadAsync(uploadParams);
    }
    return uploadResult;
  }
}
