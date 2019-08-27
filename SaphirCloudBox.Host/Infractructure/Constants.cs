using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SaphirCloudBox.Host.Infractructure
{
    public class Constants
    {
        public const string ISSUER = "SaphirCloudBox";
        public const string AUDIENCE = "http://saphir-cloud-box-ui.azurewebsites.net"; // потребитель токена
        const string KEY = "r#ryLh?wwJ2;7,7m6[MU&`R";   // ключ для шифрации
        public const int LIFETIME = 1440;

        public static SymmetricSecurityKey GetSymmetricSecurityKey()
        {
            return new SymmetricSecurityKey(Encoding.UTF8.GetBytes(KEY));
        }

        public class NotificationMessages
        {
            public const string ForgotPasswordSubject = @"Das Kennwort zurücksetzen";

            public const string ForgotPasswordMessage = @"Guten Tag Herr {0},
Sie haben sich auf unserer Webseite {1} registriert. Um Ihre Registrierung abzuschließen, klicken Sie zur Bestätigung auf folgenden Link: {2}";

            public const string ResetPasswordUrl = @"/auth/reset-password?code={0}";

            public const string TechnicalSupportMessage = @"Message from user {0} ({1}):
Theme: {2}
Message:
{3}";
            public const string TechnicalSupportSubject = @"Message from user {0}";

            public const string NotificationSubject = @"Your permissions is changed";

            public const string AddPermissionNotificationMessage = @"Guten Tag Herr {0},
User {1} added your permission to {2} to {3} {4}. To see it, go to the following link: {5}";

            public const string UpdatePermissionNotificationMessage = @"Guten Tag Herr {0},
User {1} has changed your permission to {2} {3}. Now you can {4}. To see this, follow this link: {5}";

            public const string RemovePermissionNotificationMessage = @"Guten Tag Herr {0},
User {1} deleted your permission to {2} {3}";


            public const string Folder = "folder";
            public const string File = "file";
        }

        

        public class Extension
        {
            public static Dictionary<string, string> TYPES = new Dictionary<string, string>
            {
                {".txt", "text/plain"},
                {".pdf", "application/pdf"},
                {".doc", "application/vnd.ms-word"},
                {".docx", "application/vnd.ms-word"},
                {".xls", "application/vnd.ms-excel"},
                {".xlsx", "application/vnd.openxmlformatsofficedocument.spreadsheetml.sheet"},
                {".png", "image/png"},
                {".jpg", "image/jpeg"},
                {".jpeg", "image/jpeg"},
                {".gif", "image/gif"},
                {".csv", "text/csv"}
            };
        }
    }
}
