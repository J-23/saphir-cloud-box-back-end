using Microsoft.AspNetCore.Identity;
using SaphirCloudBox.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace SaphirCloudBox.Models
{
    public class Role: IdentityRole<int>
    {
        public RoleType RoleType { get; set; }

        public Boolean IsActive { get; set; }
    }
}
