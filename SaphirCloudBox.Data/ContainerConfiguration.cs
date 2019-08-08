using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SaphirCloudBox.Data.Contracts;
using SaphirCloudBox.Data.Contracts.Repositories;
using SaphirCloudBox.Data.Repositories;
using SaphirCloudBox.Models;
using System;
using System.Collections.Generic;
using System.Text;
using Unity;
using Unity.Injection;
using Unity.Lifetime;

namespace SaphirCloudBox.Data
{
    public class ContainerConfiguration
    {
        public static void RegisterTypes<TLifetime>(IUnityContainer container)
          where TLifetime : ITypeLifetimeManager, new()
        {
            var configuration = container.Resolve<IConfiguration>();

            container.RegisterType<ISaphirCloudBoxConnectionConfiguration, SaphirCloudBoxConnectionConfiguration>(
               new TLifetime(),
               new InjectionConstructor(configuration, "DefaultConnection"));

            container.RegisterType<SaphirCloudBoxDataContext>();
            container.RegisterType<ISaphirCloudBoxDataContextManager, SaphirCloudBoxDataContextManager>(new TLifetime());


            container.RegisterType<IRoleStore<IdentityRole<int>>, RoleStore<IdentityRole<int>, SaphirCloudBoxDataContext, int>>();
            container.RegisterType<IUserClaimsPrincipalFactory<User>, UserClaimsPrincipalFactory<User, IdentityRole<int>>>();
            container.RegisterType<IHttpContextAccessor, HttpContextAccessor>();
            container.RegisterType<ILookupNormalizer, LookupNormalizer>();
            container.RegisterType<IPasswordHasher<User>, PasswordHasher<User>>();
            container.RegisterType<IUserStore<User>, UserStore<User, IdentityRole<int>, SaphirCloudBoxDataContext, int>>();
            container.RegisterType<UserManager<User>>();
            container.RegisterType<SignInManager<User>>();
            container.RegisterType<RoleManager<IdentityRole<int>>>();

            container.RegisterType<IClientRepository, ClientRepository>(new TLifetime());
            container.RegisterType<IDepartmentRepository, DepartmentRepository>(new TLifetime());
        }

        public class LookupNormalizer : ILookupNormalizer
        {
            public string Normalize(string key)
            {
                return key.Normalize().ToLowerInvariant();
            }
        }
    }
}
