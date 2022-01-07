using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Helpers;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.Extensions.Options;

namespace API.Repositories.ImplementedRepositories
{
    public class PhotoService : IPhotoService
    {

        public readonly Cloudinary _cloudnary;
        public PhotoService(IOptions<CloudnarySettings> config) 
        {
            var acc= new Account(
                config.Value.CloudName,
                config.Value.ApiKey,
                config.Value.ApiSecret
            );

            _cloudnary = new Cloudinary(acc);
        }
        public async Task<ImageUploadResult> AddPhotoAsync(IFormFile file)
        {
            var uploadResult= new ImageUploadResult();
            if(file.Length>0)
            {
                using var stream= file.OpenReadStream();
                var uploadParams= new ImageUploadParams(){
                     File= new FileDescription(file.FileName, stream),
                      Transformation= new Transformation().Height(500).Width(500).Crop("fill")
                      .Gravity("face")
                };

                uploadResult= await _cloudnary.UploadAsync(uploadParams);
            }
                return uploadResult;
        }

        public async Task<DeletionResult> DeletePhotoAsync(string publicId)
        {
            var deleteParams=new DeletionParams(publicId);
            var result= await _cloudnary.DestroyAsync(deleteParams);

            return result;
        }
    }
}