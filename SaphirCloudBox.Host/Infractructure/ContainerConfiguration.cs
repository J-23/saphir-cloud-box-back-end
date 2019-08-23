using SaphirCloudBox.Host.Controllers;
using SaphirCloudBox.Host.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unity;
using Unity.Lifetime;

namespace SaphirCloudBox.Host.Infractructure
{
    public class ContainerConfiguration
    {
        public static void RegisterTypes<TLifetime>(IUnityContainer container)
           where TLifetime : ITypeLifetimeManager, new()
        {
            Services.ContainerConfiguration.RegisterTypes<TLifetime>(container);

            container.RegisterType<IEmailSender, EmailSender>(new TLifetime());

            container.RegisterType<AccountController>();
            container.RegisterType<ClientController>();
            container.RegisterType<DepartmentController>();
            container.RegisterType<RoleController>();
            container.RegisterType<UserController>();
            container.RegisterType<FeedbackController>();
            container.RegisterType<SharedFileController>();
        }
    }
}
