using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SaphirCloudBox.Host.Infractructure
{
    public enum ResponseMessage
    {
        NOT_FOUND,
        UNATHORIZED,
        SERVER_ERROR,
        SAME_NAME,
        REMOVE_ERROR,
        UPDATE_ERROR,
        ADD_ERROR,
        NOT_FOUNT_DEPENDENCY_OBJECT,
        EXIST_DEPENDENCY_ERROR,
        NOT_FOUND_USER
    }
}
