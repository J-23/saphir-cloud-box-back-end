using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SaphirCloudBox.Host.Models
{
    public class SendMessageModel
    {
        [Required]
        [DataType(DataType.EmailAddress)]
        public string UserEmail { get; set; }

        [Required]
        public string UserName { get; set; }

        [Required]
        public string Theme { get; set; }

        [Required]
        public string Message { get; set; }

        public string FileName { get; set; }

        public string FileContent { get; set; }
    }
}
