using System;
using System.Collections.Generic;
using System.Text;

namespace SaphirCloudBox.Services.Contracts.Exceptions
{
    public class UserManagerException: Exception
    {
        public UserManagerException(string message): base(message) { }

        public UserManagerException(string operationName, string email)
            : base($"Error to {operationName} user with email = {email} using UserManager") { }
    }
}
