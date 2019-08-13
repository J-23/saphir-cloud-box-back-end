using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SaphirCloudBox.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace SaphirCloudBox.Data.Mappings
{
    public class FileStorageAccessUserMap : IEntityTypeConfiguration<FileStorageAccessUser>
    {
        public void Configure(EntityTypeBuilder<FileStorageAccessUser> builder)
        {
            builder.HasKey(x => new { x.FileStorageId, x.UserId });

            builder.HasOne(x => x.User).WithMany().HasForeignKey(x => x.UserId);
        }
    }
}
