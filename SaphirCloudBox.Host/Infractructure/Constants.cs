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

    public class LogMessage
    {
        public const string NotFoundMessage = @"{0} with Id = {1} not found";
        public const string NotFoundByEmailMeesage = @"{0} with Email = {1} not found";
        public const string NotFoundParentMessage = @"{0} with Id = {1} not found for {2} with Name = {3}";
        public const string SuccessActionMessage = @"{0} with Name = {1} was {2} successfully by user = {3}";
        public const string SuccessActionByIdMessage = @"{0} with Id = {1} was {2} successfully by user = {3}";
        public const string FoundSameObjectMessage = @"{0} with Name = {1} already exists";
        public const string NoAccessMessage = @"{0} with Id = {1} is unavailable for user with id = {2}";
        public const string PermissionMessage = @"Permission for file storage with id = {0} was {1} successfully by user with id = {2}";
        public const string UnavailablePermissionMessage = @"User with id = {0} can't {1} permission for user with email = {2}";
        public const string PermissionExistMessage = @"User with email = {0} has permission on file storage with id = {1}";

        public const string FileEntityName = "File";
        public const string FolderEntityName = "Folder";
        public const string UserEntityName = "User";

        public const string CreateAction = "created";
        public const string UpdateAction = "updated";
        public const string RemoveAction = "removed";

        public const string CreateVerb = "create";
        public const string UpdateVerb = "update";
        public const string RemoveVerb = "remove";

        public static string CreateSuccessByNameMessage(string entityName, string name, string action, int userId)
        {
            var result = String.Format(SuccessActionMessage, entityName, name, action, userId);
            return result;
        }

        public static string CreateNotFoundMessage(string entityName, int id)
        {
            var result = String.Format(NotFoundMessage, entityName, id);
            return result;
        }

        public static string CreateNotFoundByEmailMessage(string entityName, string email)
        {
            return String.Format(NotFoundByEmailMeesage, entityName, email);
        }

        public static string CreateNotFoundParentMessage(string parentEntityName, int parentId, string entityName, string name)
        {
            var result = String.Format(NotFoundParentMessage, parentEntityName, parentId, entityName, name);
            return result;
        }

        public static string CreateFoundSameObjectMessage(string entityName, string name)
        {
            var result = String.Format(FoundSameObjectMessage, entityName, name);
            return result;
        }

        public static string CreateSuccessByIdMessage(string entityName, int id, string action, int userId)
        {
            var result = String.Format(SuccessActionByIdMessage, entityName, id, action, userId);
            return result;
        }

        public static string CreateNoAccessMessage(string entityName, int id, int userId)
        {
            var result = String.Format(NoAccessMessage, entityName, id, userId);
            return result;
        }

        public static string CreatePermissionMessage(int fileStorageId, string action, int userId)
        {
            return String.Format(PermissionMessage, fileStorageId, action, userId);
        }

        public static string CreateUnavailablePermissionMessage(int senderId, string actionVerb, string recipientEmail)
        {
            return string.Format(UnavailablePermissionMessage, senderId, actionVerb, recipientEmail);
        }

        public static string CreatePermissionExistMessage(string email, int fileStorageId)
        {
            return string.Format(PermissionExistMessage, email, fileStorageId);
        }
    }
}
