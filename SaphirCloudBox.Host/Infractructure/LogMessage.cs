using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SaphirCloudBox.Host.Infractructure
{
    public class LogMessage
    {
        public const string SuccessActionMessage = @"{0} with Name = {1} was {2} successfully by user = {3}";
        public const string SuccessActionByIdMessage = @"{0} with Id = {1} was {2} successfully by user = {3}";
        public const string PermissionMessage = @"Permission for file storage with id = {0} was {1} successfully by user with id = {2}";

        public const string FileEntityName = "File";
        public const string FolderEntityName = "Folder";
        public const string UserEntityName = "User";
        public const string RoleEntityName = "Role";
        public const string DepartmentEntityName = "Department";
        public const string ClientEntityName = "Client";

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

        public static string CreateSuccessByIdMessage(string entityName, int id, string action, int userId)
        {
            var result = String.Format(SuccessActionByIdMessage, entityName, id, action, userId);
            return result;
        }

        public static string CreatePermissionMessage(int fileStorageId, string action, int userId)
        {
            return String.Format(PermissionMessage, fileStorageId, action, userId);
        }
    }
}
