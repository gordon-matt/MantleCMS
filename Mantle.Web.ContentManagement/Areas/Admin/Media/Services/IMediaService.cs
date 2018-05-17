using System.Collections.Generic;
using System.IO;
using System.Linq;
using Extenso.IO;
using Mantle.Web.ContentManagement.Areas.Admin.Media.Models;

namespace Mantle.Web.ContentManagement.Areas.Admin.Media.Services
{
    public interface IMediaService
    {
        /// <summary>
        /// Retrieves the media files within a given relative path.
        /// </summary>
        /// <param name="relativePath">The path where to retrieve the media files from. null means root.</param>
        /// <returns>The media files in the given path.</returns>
        IEnumerable<MediaFile> GetFiles(string directoryPath);
    }

    public class MediaService : IMediaService
    {
        public IEnumerable<MediaFile> GetFiles(string directoryPath)
        {
            if (directoryPath.StartsWith("Media"))
            {
                directoryPath = directoryPath.Substring(5, directoryPath.Length);
            }
            else if (directoryPath.StartsWith("/Media"))
            {
                directoryPath = directoryPath.Substring(6, directoryPath.Length);
            }

            directoryPath = directoryPath.TrimStart(new[] { '/', '\\' });
            //relativePath = relativePath.Replace("/", "\\");
            var files = new DirectoryInfo(directoryPath).GetFiles();
            return files.Select(file => BuildMediaFile(directoryPath, file)).ToList();
        }

        /// <summary>
        /// Returns the public URL for a media file.
        /// </summary>
        /// <param name="mediaPath">The relative path of the media folder containing the media.</param>
        /// <param name="fileName">The media file name.</param>
        /// <returns>The public URL for the media.</returns>
        public string GetPublicUrl(string directoryPath, string fileName)
        {
            return string.IsNullOrEmpty(directoryPath) ? fileName : Path.Combine("/Media/Uploads", directoryPath, fileName);
        }

        private MediaFile BuildMediaFile(string directoryPath, FileInfo file)
        {
            string path = GetPublicUrl(directoryPath, file.Name);

            return new MediaFile
            {
                Name = file.Name,
                Path = path,
                ThumbPath = path.Replace("/Media/Uploads/", "/Media/Uploads/.tmb/"),
                DirectoryPath = directoryPath,
                Type = file.Extension,
                Size = file.GetFileSizeInKiloBytes(),
                LastUpdated = file.LastWriteTimeUtc
            };
        }
    }
}