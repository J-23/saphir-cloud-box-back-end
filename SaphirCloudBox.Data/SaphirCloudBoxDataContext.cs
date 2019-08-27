using Anthill.Common.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using SaphirCloudBox.Data.Contracts;
using SaphirCloudBox.Data.Mappings;
using SaphirCloudBox.Enums;
using SaphirCloudBox.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace SaphirCloudBox.Data
{
    public class SaphirCloudBoxDataContext 
        : AbstractDataContext<User, Role, int>
    {
        public SaphirCloudBoxDataContext(ISaphirCloudBoxConnectionConfiguration configuration) 
            : base(configuration)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);

            optionsBuilder.UseLazyLoadingProxies()
               .ConfigureWarnings(warnings => warnings.Ignore(CoreEventId.DetachedLazyLoadingWarning));
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.ApplyConfiguration(new UserMap());
            builder.ApplyConfiguration(new RoleMap());
            builder.ApplyConfiguration(new ClientMap());
            builder.ApplyConfiguration(new DepartmentMap());

            builder.ApplyConfiguration(new LogMap());

            builder.ApplyConfiguration(new FileStorageMap());
            builder.ApplyConfiguration(new FileStoragePermissionMap());

            builder.ApplyConfiguration(new FileMap());
            builder.ApplyConfiguration(new AzureBlobStorageMap());

            builder.ApplyConfiguration(new NotificationMap());

            /*builder.Entity<IdentityRole<int>>().HasData(
                new IdentityRole<int> { Id = 1, Name = "SUPER ADMIN", NormalizedName = "super admin" },
                new IdentityRole<int> { Id = 2, Name = "CLIENT ADMIN", NormalizedName = "client admin" },
                new IdentityRole<int> { Id = 3, Name = "DEPARTMENT HEAD", NormalizedName = "department head" },
                new IdentityRole<int> { Id = 4, Name = "EMPLOYEE", NormalizedName = "employee" });*/

            /*builder.Entity<FileStorage>().HasData(
                new FileStorage { Id = 1, Name = "root", CreateById = 3, IsDirectory = true });*/
        }
    }
}
