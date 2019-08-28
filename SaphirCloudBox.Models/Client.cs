using System;
using System.Collections.Generic;
using System.Text;

namespace SaphirCloudBox.Models
{
    public class Client
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime? UpdateDate { get; set; }

        public virtual IEnumerable<User> Users { get; set; }

        public virtual ICollection<Department> Departments { get; set; }

        public Boolean IsActive { get; set; }
    }
}
