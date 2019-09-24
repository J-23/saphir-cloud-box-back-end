using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SaphirCloudBox.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace SaphirCloudBox.Data.Mappings
{
    public class FileViewingMap : IEntityTypeConfiguration<FileViewing>
    {
        public void Configure(EntityTypeBuilder<FileViewing> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.IsActive).HasDefaultValue(1);

            builder.Property(x => x.ViewDate).IsRequired();

            builder.HasOne(x => x.ViewBy).WithMany().HasForeignKey(x => x.ViewById);

            builder.HasOne(x => x.FileStorage).WithMany(x => x.FileViewings).HasForeignKey(x => x.FileStorageId);
        }
    }
}
