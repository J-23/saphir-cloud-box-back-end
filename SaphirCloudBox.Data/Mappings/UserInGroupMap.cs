using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SaphirCloudBox.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace SaphirCloudBox.Data.Mappings
{
    public class UserInGroupMap : IEntityTypeConfiguration<UserInGroup>
    {
        public void Configure(EntityTypeBuilder<UserInGroup> builder)
        {
            builder.HasKey(x => new { x.GroupId, x.UserId });

            builder.HasOne(x => x.User).WithMany(x => x.UserInGroups).HasForeignKey(x => x.UserId);
        }
    }
}
