using SaphirCloudBox.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace SaphirCloudBox.Services
{
    public class Constants
    {
        public static Dictionary<FileStorageType, List<string>> fileTypes = new Dictionary<FileStorageType, List<string>>
        {
            { FileStorageType.folder, new List<string>() },
            { FileStorageType.image, new List<string> { ".jpeg", ".jpg", ".png", ".tiff", ".gif", ".bmp", ".bat", ".csg" } },
            { FileStorageType.insert_drive_file, new List<string> { ".doc", ".docx", ".odt" , ".xls", ".xlsx", ".ppt", ".pptx", ".txt" } },
            { FileStorageType.library_music, new List<string> { ".mp3", ".wav"} },
            { FileStorageType.picture_as_pdf, new List<string> { ".pdf" } },
            { FileStorageType.videocam, new List<string> { ".mp4", ".wmv", ".avi", ".webm", ".mov" } },
        };

        public class Role
        {
            public const string SUPER_ADMIN_ROLE_NAME = "SUPER ADMIN";

            public const string CLIENT_ADMIN_ROLE_NAME = "CLIENT ADMIN";

            public const string DEPARTMENT_HEAD_ROLE_NAME = "DEPARTMENT HEAD";

            public const string EMPLOYEE_ROLE_NAME = "EMPLOYEE";
        }
    }
}
