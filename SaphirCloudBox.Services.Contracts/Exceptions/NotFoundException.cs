using System;
using System.Collections.Generic;
using System.Text;

namespace SaphirCloudBox.Services.Contracts.Exceptions
{
    public class NotFoundException: Exception
    {
        public NotFoundException(string objectName, int id) 
            : base($"{objectName} with Id = {id} not found") { }

        public NotFoundException(string objectName, int fileStorageId, string recipientName) 
            : base($"{objectName} with file storage id = {fileStorageId} and recipient email = {recipientName} not found") { }
    }
}
