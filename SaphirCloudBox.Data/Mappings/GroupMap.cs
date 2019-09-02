using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SaphirCloudBox.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace SaphirCloudBox.Data.Mappings
{
    public class GroupMap : IEntityTypeConfiguration<Group>
    {
        public void Configure(EntityTypeBuilder<Group> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name).HasMaxLength(255).IsRequired();

            builder.HasOne(x => x.Owner).WithMany(x => x.Groups).HasForeignKey(x => x.OwnerId);

            builder.HasMany(x => x.UsersInGroup).WithOne(x => x.Group).HasForeignKey(x => x.GroupId);
        }
    }
}
