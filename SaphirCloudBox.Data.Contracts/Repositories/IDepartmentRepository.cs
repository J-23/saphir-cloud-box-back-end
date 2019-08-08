using Anthill.Common.Data.Contracts;
using SaphirCloudBox.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SaphirCloudBox.Data.Contracts.Repositories
{
    public interface IDepartmentRepository: IRepository
    {
        Task<Department> GetById(int departmentId);

        Task Update(Department department);

        Task Remove(Department department);

        Task<IEnumerable<Department>> GetAll();

        Task<IEnumerable<Department>> GetByClientId(int clientId);
    }
}
