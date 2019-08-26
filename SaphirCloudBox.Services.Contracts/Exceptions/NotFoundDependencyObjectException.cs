using System;
using System.Collections.Generic;
using System.Text;

namespace SaphirCloudBox.Services.Contracts.Exceptions
{
    public class NotFoundDependencyObjectException: Exception
    {
        public NotFoundDependencyObjectException(string objectName, int id) : base($"{objectName} with Id = {id} not found") { }

        public NotFoundDependencyObjectException(string objectName) : base($"Active {objectName} not found") { }
    }
}
