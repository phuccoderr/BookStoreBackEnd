using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.Extensions.Options;

namespace ProjectBook.Helpers
{
    public class CloudinaryService
    {
        private readonly IOptions<CloudinarySettings> _cloudinarySettings;

        public CloudinaryService(IOptions<CloudinarySettings> cloudinarySettings)
        {
            _cloudinarySettings = cloudinarySettings;
        }
        public ImageUploadResult SaveImage(IFormFile file)
        {
            Account account = new Account(
                _cloudinarySettings.Value.CloudName,
                _cloudinarySettings.Value.ApiKey,
                _cloudinarySettings.Value.ApiSecret
            );
            Cloudinary cloudinary = new Cloudinary(account);
            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(file.FileName, file.OpenReadStream())
            };
            return cloudinary.Upload(uploadParams);
        }

        public void deleteImage(String imageURL)
        {
            Account account = new Account(
                   _cloudinarySettings.Value.CloudName,
                   _cloudinarySettings.Value.ApiKey,
                   _cloudinarySettings.Value.ApiSecret
                );
            Cloudinary cloudinary = new Cloudinary(account);

            if ( imageURL != null )
            {
                string publicId = getPublicIdFromURL(imageURL);
                var deletionParams = new DeletionParams(publicId);
                var deletionResult = cloudinary.Destroy(deletionParams);
            }
        }

        private string getPublicIdFromURL(string imageURL)
        {
            string[] parts = imageURL.Split("/");
            string publicIdWithDotFile = parts[parts.Length - 1];
            string[] publicIdParts = publicIdWithDotFile.Split(".");
            return publicIdParts[0];
        }
    }
}
