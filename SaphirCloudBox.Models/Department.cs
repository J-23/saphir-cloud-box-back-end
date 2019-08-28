using System;
using System.Collections.Generic;
using System.Text;

namespace SaphirCloudBox.Models
{
    public class Department
    {
        public int Id { get; set; }

        public int ClientId { get; set; }

        public string Name { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime? UpdateDate { get; set; }

        public virtual Client Client { get; set; }

        public virtual IEnumerable<User> Users { get; set; }

        public Boolean IsActive { get; set; }
    }
}
