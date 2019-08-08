using System;
using System.Collections.Generic;
using System.Text;

namespace SaphirCloudBox.Services.Contracts.Exceptions
{
    public class NotFoundDependencyObjectException: Exception
    {
        public NotFoundDependencyObjectException() : base() { }
    }
}
