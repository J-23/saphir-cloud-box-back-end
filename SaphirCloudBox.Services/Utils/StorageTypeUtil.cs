using SaphirCloudBox.Enums;
using SaphirCloudBox.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SaphirCloudBox.Services.Utils
{
    public static class StorageTypeUtil
    {
        public static string GetStorageType(bool isDirectory, IEnumerable<File> files)
        {
            if (isDirectory)
            {
                return FileStorageType.folder.ToString();
            }
            else
            {
                var file = files.FirstOrDefault(x => x.IsActive);

                if (file == null)
                {
                    throw new ArgumentNullException(nameof(file));
                }

                foreach (var fileType in Constants.fileTypes)
                {
                    if (fileType.Value.Any(x => file.Extension.Equals(x)))
                    {
                        return fileType.Key.ToString();
                    }
                }
            }

            return FileStorageType.insert_drive_file.ToString();
        }
    }
}
