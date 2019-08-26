using System;
using System.Collections.Generic;
using System.Text;

namespace SaphirCloudBox.Services.Contracts.Exceptions
{
    public class UnavailableOperationException: Exception
    {
        public UnavailableOperationException(string operationName, int id, int userId) 
            : base($"Unavailable to {operationName} with id = {id} by user with id = {userId}") { }

        public UnavailableOperationException(string operationName, int fileStorageId, string email,  int userId)
            : base($"Unavailable to {operationName} for file storage = {fileStorageId} and user email = {email} by user with id = {userId}") { }
    }
}
