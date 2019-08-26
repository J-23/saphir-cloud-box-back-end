using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SaphirCloudBox.Host.Infractructure
{
    public enum ResponseMessage
    {
        NOT_FOUND,
        SERVER_ERROR,
        SAME_OBJECT,
        NOT_FOUND_DEPENDENCY_OBJECT,
        EXIST_DEPENDENCY_OBJECTS,
        NOT_FOUND_USER,
        NO_ACCESS,
        NOT_FOUND_ROLE,
        SAME_USER,
        SAME_ROLE,
        ERROR
    }
}
