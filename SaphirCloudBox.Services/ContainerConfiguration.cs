using SaphirCloudBox.Services.Contracts.Mappers;
using SaphirCloudBox.Services.Contracts.Services;
using SaphirCloudBox.Services.Mappers;
using SaphirCloudBox.Services.Services;
using System;
using System.Collections.Generic;
using System.Text;
using Unity;
using Unity.Lifetime;

namespace SaphirCloudBox.Services
{
    public class ContainerConfiguration
    {
        public static void RegisterTypes<TLifetime>(IUnityContainer container)
          where TLifetime : ITypeLifetimeManager, new()
        {
            Data.ContainerConfiguration.RegisterTypes<TLifetime>(container);

            container.RegisterType<IUserService, UserService>(new TLifetime());
            container.RegisterType<IClientService, ClientService>(new TLifetime());
            container.RegisterType<IDepartmentService, DepartmentService>(new TLifetime());
            container.RegisterType<IRoleService, RoleService>(new TLifetime());

            container.RegisterType<IUserMapper, UserMapper>(new TLifetime());
            container.RegisterType<IClientMapper, ClientMapper>(new TLifetime());
            container.RegisterType<IDepartmentMapper, DepartmentMapper>(new TLifetime());
            container.RegisterType<IRoleMapper, RoleMapper>(new TLifetime());
        }
    }
}
