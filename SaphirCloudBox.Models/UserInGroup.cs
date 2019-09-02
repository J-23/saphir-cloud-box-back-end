using System;
using System.Collections.Generic;
using System.Text;

namespace SaphirCloudBox.Models
{
    public class UserInGroup
    {
        public int GroupId { get; set; }

        public int UserId { get; set; }

        public virtual Group Group { get; set; }

        public virtual User User { get; set; }
    }
}
