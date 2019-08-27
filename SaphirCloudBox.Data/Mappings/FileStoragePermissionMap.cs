using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SaphirCloudBox.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace SaphirCloudBox.Data.Mappings
{
    public class FileStoragePermissionMap : IEntityTypeConfiguration<FileStoragePermission>
    {
        public void Configure(EntityTypeBuilder<FileStoragePermission> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Type).IsRequired();

            builder.Property(x => x.StartDate).IsRequired();

            builder.HasOne(x => x.Recipient).WithMany().HasForeignKey(x => x.RecipientId);

            builder.HasOne(x => x.Sender).WithMany().HasForeignKey(x => x.SenderId);
        }
    }
}
