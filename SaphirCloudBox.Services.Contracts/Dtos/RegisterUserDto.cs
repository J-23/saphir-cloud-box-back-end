using System;
using System.Collections.Generic;
using System.Text;

namespace SaphirCloudBox.Services.Contracts.Dtos
{
    public class RegisterUserDto
    {
        public string UserName { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }

        public int ClientId { get; set; }
    }
}
