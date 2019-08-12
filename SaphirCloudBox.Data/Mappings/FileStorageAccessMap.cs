using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SaphirCloudBox.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace SaphirCloudBox.Data.Mappings
{
    public class FileStorageAccessMap : IEntityTypeConfiguration<FileStorageAccess>
    {
        public void Configure(EntityTypeBuilder<FileStorageAccess> builder)
        {
            builder.HasKey(x => new { x.FileStorageId, x.UserId });

            builder.HasOne(x => x.User).WithMany().HasForeignKey(x => x.UserId);
        }
    }
}
