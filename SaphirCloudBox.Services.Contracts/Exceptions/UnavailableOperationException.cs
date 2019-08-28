using System;
using System.Collections.Generic;
using System.Text;

namespace SaphirCloudBox.Services.Contracts.Exceptions
{
    public class UnavailableOperationException: Exception
    {
        public UnavailableOperationException(string operationName, string objectName, int id, int userId) 
            : base($"Unavailable to {operationName} for {objectName} with id = {id} by user with id = {userId}") { }
    }
}
