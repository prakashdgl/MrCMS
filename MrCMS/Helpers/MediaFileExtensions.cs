using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.DependencyInjection;
using MrCMS.Entities.Documents.Media;
using MrCMS.Models;
using MrCMS.Services;
using MrCMS.Settings;
using MrCMS.Website;

namespace MrCMS.Helpers
{
    public static class MediaFileExtensions
    {
        public static readonly HashSet<string> JpegExtensions = new HashSet<string> { ".jpg", ".jpeg" };
        public static readonly List<string> ImageExtensions = new List<string> { ".jpg", ".jpeg", ".gif", ".png", ".bmp" };

        public static IFileSystem GetFileSystem(string fileUrl, IEnumerable<IFileSystem> possibleFileSystems)
        {
            return possibleFileSystems.FirstOrDefault(system => system.Exists(fileUrl));
        }

        public static bool IsImage(this MediaFile file)
        {
            return file != null && IsImageExtension(file.FileExtension);
        }

        public static bool IsImageExtension(string fileExtension)
        {
            return ImageExtensions.Any(s => s.Equals(fileExtension, StringComparison.InvariantCultureIgnoreCase));
        }

        public static bool IsJpeg(this MediaFile file)
        {
            return file != null &&
                   JpegExtensions.Any(s => s.Equals(file.FileExtension, StringComparison.InvariantCultureIgnoreCase));
        }

        public static IEnumerable<ImageSize> GetSizes(this MediaFile file, MediaSettings mediaSettings)
        {
            if (!IsImage(file))
                yield break;
            yield return new ImageSize("Original", file.Size);
            foreach (
                ImageSize imageSize in
                mediaSettings
                        .ImageSizes.Where(size => ImageProcessor.RequiresResize(file.Size, size.Size)))
            {
                imageSize.ActualSize = ImageProcessor.CalculateDimensions(file.Size, imageSize.Size);
                yield return imageSize;
            }
            // TODO: resized images
        }

        public static IEnumerable<ImageSize> GetImageSizes(this IHtmlHelper helper,  MediaFile file)
        {
            return GetSizes(file, helper.ViewContext.HttpContext.RequestServices.GetRequiredService<MediaSettings>());
        }
    }
}