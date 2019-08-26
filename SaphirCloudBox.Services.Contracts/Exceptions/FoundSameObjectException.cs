using System;
using System.Collections.Generic;
using System.Text;

namespace SaphirCloudBox.Services.Contracts.Exceptions
{
    public class FoundSameObjectException: Exception
    {
        public FoundSameObjectException(string objectName, string name): base($"{objectName} with name = {name} already exists") { }

        public FoundSameObjectException(string objectName, int fileStorageId, string recipientName)
            : base($"{objectName} with file storage id = {fileStorageId} and recipient email = {recipientName} already exists") { }
    }
}
