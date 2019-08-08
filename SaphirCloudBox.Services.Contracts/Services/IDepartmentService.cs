using SaphirCloudBox.Services.Contracts.Dtos;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SaphirCloudBox.Services.Contracts.Services
{
    public interface IDepartmentService
    {
        Task<IEnumerable<DepartmentDto>> GetAll();

        Task Add(AddDepartmentDto departmentDto);

        Task Update(UpdateDepartmentDto departmentDto);

        Task Remove(RemoveDepartmentDto departmentDto);

        Task<IEnumerable<DepartmentDto>> GetByClientId(int clientId);
    }
}
