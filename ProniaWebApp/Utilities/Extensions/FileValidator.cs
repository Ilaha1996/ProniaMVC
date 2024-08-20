using ProniaWebApp.Models;
using ProniaWebApp.Utilities.Enums;

namespace ProniaWebApp.Utilities.Extensions
{
    public static class FileValidator
    {
        public static bool ValidateType(this IFormFile file, string type)
        {
            if(file.ContentType.Contains(type))
            {
                return true;
            }
            return false;
        }

        public static bool ValidateSize(this IFormFile file, FileSizes filesize, int size) 
        {
            switch (filesize) 
            {
              case FileSizes.KB:
                return file.Length <= size * 1024;
              case FileSizes.MB:
                 return file.Length <= size * 1024 * 1024;
            }
            return false;      
        }

        public static async Task<string> CreateFileAsync(this IFormFile file, params string[] roots)
        {
            string fileName = String.Concat(Guid.NewGuid().ToString(), file.FileName);

            string path = string.Empty;

            for (int i = 0; i < roots.Length; i++)
            {
                path=Path.Combine(path, roots[i]);
            }

            path=Path.Combine(path, fileName);

            using (FileStream fileStream = new FileStream(path, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            return fileName;           
        }

    }
}
