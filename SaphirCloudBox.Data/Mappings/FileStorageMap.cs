using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SaphirCloudBox.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace SaphirCloudBox.Data.Mappings
{
    public class FileStorageMap : IEntityTypeConfiguration<FileStorage>
    {
        public void Configure(EntityTypeBuilder<FileStorage> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.IsDirectory).IsRequired();

            builder.Property(x => x.Name).HasMaxLength(255).IsRequired();

            builder.HasOne(x => x.ParentFileStorage).WithMany().HasForeignKey(x => x.ParentFileStorageId);

            builder.HasMany(x => x.Permissions).WithOne(x => x.FileStorage).HasForeignKey(x => x.FileStorageId);

            builder.HasOne(x => x.CreateBy).WithMany().HasForeignKey(x => x.CreateById);

            builder.HasOne(x => x.UpdateBy).WithMany().HasForeignKey(x => x.UpdateById);

            builder.HasOne(x => x.Owner).WithMany().HasForeignKey(x => x.OwnerId);

            builder.HasOne(x => x.Client).WithMany().HasForeignKey(x => x.ClientId);
        }
    }
}
