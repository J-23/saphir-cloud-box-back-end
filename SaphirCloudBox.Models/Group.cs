using System;
using System.Collections.Generic;
using System.Text;

namespace SaphirCloudBox.Models
{
    public class Group
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int OwnerId { get; set; }

        public virtual User Owner { get; set; }

        public bool IsActive { get; set; }

        public virtual ICollection<UserInGroup> UsersInGroup { get; set; }
    }
}
