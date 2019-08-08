using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anthill.Common.Services.Contracts
{
    public class ValidationException : Exception
    {
        public ValidationException(string message)
             : base(message)
        {
            Member = String.Empty;
        }

        public ValidationException(string member, string message)
            : base(message)
        {
            Member = member;
        }

        public String Member { get; set; }
    }
}
