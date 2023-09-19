using RiyazDentalClinicAPI.Utility;

namespace RiyazDentalClinicAPI.Services
{
    public class SaveImageService
    {
        public async Task<string> SaveImage(IFormFile file, string ImageFolderName, IWebHostEnvironment getHostEnvironment)
        {
            var wwwRootPath = getHostEnvironment.WebRootPath;
            var uploadPath = Path.Combine(wwwRootPath, @"Images/"+ImageFolderName);
            var fileName=Guid.NewGuid().ToString();
            var extension=Path.GetExtension(file.FileName);
            using (var filestream = new FileStream(Path.Combine(uploadPath, fileName + extension), FileMode.Create))
            {
                file.CopyTo(filestream);
            }
            return @"Images\"+ImageFolderName+"\\"+fileName+extension;
        }
    }
}
