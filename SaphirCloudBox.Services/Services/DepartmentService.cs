using Anthill.Common.Data.Contracts;
using Anthill.Common.Services;
using SaphirCloudBox.Data.Contracts;
using SaphirCloudBox.Data.Contracts.Repositories;
using SaphirCloudBox.Models;
using SaphirCloudBox.Services.Contracts.Dtos;
using SaphirCloudBox.Services.Contracts.Exceptions;
using SaphirCloudBox.Services.Contracts.Mappers;
using SaphirCloudBox.Services.Contracts.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;

namespace SaphirCloudBox.Services.Services
{
    public class DepartmentService : AbstractService, IDepartmentService
    {
        public DepartmentService(IUnityContainer container, ISaphirCloudBoxDataContextManager dataContextManager) : base(container, dataContextManager)
        {
        }

        public async Task Add(AddDepartmentDto departmentDto)
        {
            var clientRepository = DataContextManager.CreateRepository<IClientRepository>();

            var client = await clientRepository.GetById(departmentDto.ClientId);

            if (client == null)
            {
                throw new NotFoundDependencyObjectException("Client", departmentDto.ClientId);
            }

            if (client.Departments.Where(x => x.IsActive).Select(s => s.Name).Contains(departmentDto.Name))
            {
                throw new FoundSameObjectException("Department", departmentDto.Name);
            }

            var department = new Department
            {
                Name = departmentDto.Name,
                CreateDate = DateTime.UtcNow,
                IsActive = true
            };

            client.Departments.Add(department);

            await clientRepository.Update(client);
        }

        public async Task<IEnumerable<DepartmentDto>> GetAll()
        {
            var departmentRepository = DataContextManager.CreateRepository<IDepartmentRepository>();

            var departments = await departmentRepository.GetAll();

            return MapperFactory.CreateMapper<IDepartmentMapper>().MapCollectionToModel(departments);
        }

        public async Task<IEnumerable<DepartmentDto>> GetByClientId(int clientId)
        {
            var departmentRepository = DataContextManager.CreateRepository<IDepartmentRepository>();
            var departments = await departmentRepository.GetByClientId(clientId);

            return MapperFactory.CreateMapper<IDepartmentMapper>().MapCollectionToModel(departments);
        }

        public async Task Remove(RemoveDepartmentDto departmentDto)
        {
            var departmentRepository = DataContextManager.CreateRepository<IDepartmentRepository>();

            var department = await departmentRepository.GetById(departmentDto.Id);

            if (department == null)
            {
                throw new NotFoundException("Department", departmentDto.Id);
            }

            if (department.Users.Where(x => x.IsActive).Count() > 0)
            {
                throw new ExistDependencyException("Department", departmentDto.Id, new List<string> { "Users" });
            }

            department.IsActive = false;

            await departmentRepository.Update(department);
        }

        public async Task Update(UpdateDepartmentDto departmentDto)
        {
            var departmentRepository = DataContextManager.CreateRepository<IDepartmentRepository>();

            var department = await departmentRepository.GetById(departmentDto.Id);

            if (department == null)
            {
                throw new NotFoundException("Department", departmentDto.Id);
            }

            var clientRepository = DataContextManager.CreateRepository<IClientRepository>();

            var client = await clientRepository.GetById(departmentDto.ClientId);

            if (client == null)
            {
                throw new NotFoundDependencyObjectException("Client", departmentDto.ClientId);
            }

            var otherDepartment = client.Departments.FirstOrDefault(x => x.Name.Equals(departmentDto.Name) && x.IsActive);
            if (otherDepartment != null && otherDepartment.Id != departmentDto.Id)
            {
                throw new FoundSameObjectException("Department", departmentDto.Name);
            }

            department.Name = departmentDto.Name;
            department.UpdateDate = DateTime.UtcNow;
            department.Client = client;
            department.UpdateDate = DateTime.UtcNow;

            await departmentRepository.Update(department);
        }
    }
}
