using System;
using System.Collections.Generic;
using System.Text;

namespace SaphirCloudBox.Services.Contracts.Exceptions
{
    public class ExistDependencyException: Exception
    {
        public ExistDependencyException(string objectName, int objectId, IEnumerable<string> dependencyObjectNames) 
            : base($"{objectName} has {String.Join(",", dependencyObjectNames)}") { }
    }
}
