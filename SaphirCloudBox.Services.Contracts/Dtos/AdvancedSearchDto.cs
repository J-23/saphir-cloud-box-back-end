using System;
using System.Collections.Generic;
using System.Text;

namespace SaphirCloudBox.Services.Contracts.Dtos
{
    public class AdvancedSearchDto
    {
        public IEnumerable<int> ClientIds { get; set; }

        public IEnumerable<int> DepartmentIds { get; set; }

        public IEnumerable<int> UserIds { get; set; }

        public IEnumerable<int> UserGroupIds { get; set; }

        public IEnumerable<int> FolderIds { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public string SearchString { get; set; }
    }
}
