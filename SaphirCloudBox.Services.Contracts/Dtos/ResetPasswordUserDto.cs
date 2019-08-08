using System;
using System.Collections.Generic;
using System.Text;

namespace SaphirCloudBox.Services.Contracts.Dtos
{
    public class ResetPasswordUserDto
    {
        public string Email { get; set; }

        public string Password { get; set; }

        public string Code { get; set; }
    }
}
