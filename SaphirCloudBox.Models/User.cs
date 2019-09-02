using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace SaphirCloudBox.Models
{
    public class User: IdentityUser<int>
    {
        public int ClientId { get; set; }

        public int? DepartmentId { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime? UpdateDate { get; set; }

        public string ResetPasswordCode { get; set; }

        public Boolean IsActive { get; set; }

        public virtual Client Client { get; set; }

        public virtual Department Department { get; set; }
    }
}
