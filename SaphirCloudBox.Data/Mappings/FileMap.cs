using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SaphirCloudBox.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace SaphirCloudBox.Data.Mappings
{
    public class FileMap : IEntityTypeConfiguration<File>
    {
        public void Configure(EntityTypeBuilder<File> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Extension).HasMaxLength(100).IsRequired();

            builder.Property(x => x.Size).IsRequired();

            builder.Property(x => x.SizeType).HasMaxLength(2).IsRequired();

            builder.HasOne(x => x.FileStorage).WithMany(x => x.Files).HasForeignKey(x => x.FileStorageId);

            builder.HasOne(x => x.AzureBlobStorage).WithOne(x => x.File).HasForeignKey<AzureBlobStorage>(x => x.FileId);

            builder.HasOne(x => x.CreateBy).WithMany().HasForeignKey(x => x.CreateById);

            builder.HasOne(x => x.UpdateBy).WithMany().HasForeignKey(x => x.UpdateById);
        }
    }
}
