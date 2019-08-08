using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SaphirCloudBox.Host.Models
{
    public class ForgotPasswordModel
    {
        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
    }
}
