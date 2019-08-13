using Anthill.Common.AzureBlob;
using Microsoft.Extensions.Configuration;
using SaphirCloudBox.Services.Contracts;
using SaphirCloudBox.Services.Contracts.Mappers;
using SaphirCloudBox.Services.Contracts.Services;
using SaphirCloudBox.Services.Mappers;
using SaphirCloudBox.Services.Services;
using System;
using System.Collections.Generic;
using System.Text;
using Unity;
using Unity.Injection;
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
            container.RegisterType<ILogService, LogService>(new TLifetime());
            container.RegisterType<IFileStorageService, FileStorageService>(new TLifetime());

            container.RegisterType<IUserMapper, UserMapper>(new TLifetime());
            container.RegisterType<IClientMapper, ClientMapper>(new TLifetime());
            container.RegisterType<IDepartmentMapper, DepartmentMapper>(new TLifetime());
            container.RegisterType<IRoleMapper, RoleMapper>(new TLifetime());
            container.RegisterType<IFileStorageMapper, FileStorageMapper>(new TLifetime());

            var configuration = container.Resolve<IConfiguration>();
            var configurationSection = configuration.GetSection("BlobSettings");
            container.RegisterInstance(configurationSection.Get<BlobSettings>());

            var blobSettings = container.Resolve<BlobSettings>();
            container.RegisterType<AzureBlobClient>(new TLifetime(), new InjectionConstructor(blobSettings.ConnectionString));
        }
    }
}
