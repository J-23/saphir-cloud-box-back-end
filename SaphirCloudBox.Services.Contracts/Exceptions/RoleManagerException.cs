using System;
using System.Collections.Generic;
using System.Text;

namespace SaphirCloudBox.Services.Contracts.Exceptions
{
    public class RoleManagerException: Exception
    {
        public RoleManagerException(string operationName, string name)
            : base($"Error to {operationName} user with name = {name} using RoleManager") { }
    }
}
