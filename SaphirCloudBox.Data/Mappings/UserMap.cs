using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SaphirCloudBox.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace SaphirCloudBox.Data.Mappings
{
    public class UserMap : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasOne(x => x.Client).WithMany(x => x.Users).HasForeignKey(x => x.ClientId);

            builder.HasOne(x => x.Department).WithMany(x => x.Users).HasForeignKey(x => x.DepartmentId);
        }
    }
}
