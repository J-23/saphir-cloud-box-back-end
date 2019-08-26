using System;
using System.Collections.Generic;
using System.Text;

namespace SaphirCloudBox.Services.Contracts.Exceptions
{
    public class AppUnauthorizedAccessException: Exception
    {
        public UnautorizedType UnautorizedType { get; private set; }

        public ObjectType ObjectType { get; private set; }

        public AppUnauthorizedAccessException(UnautorizedType unautorizedType, ObjectType objectType) 
            : base($"Authentication error: {unautorizedType.ToString()}_{objectType.ToString()}")
        {
            UnautorizedType = unautorizedType;
            ObjectType = objectType;
        }
    }

    public enum UnautorizedType
    {
        NOT_FOUND,
        SAME_OBJECT
    }

    public enum ObjectType
    {
        USER,
        ROLE
    }
}
