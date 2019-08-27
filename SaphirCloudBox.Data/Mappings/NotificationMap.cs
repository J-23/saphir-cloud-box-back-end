using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SaphirCloudBox.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace SaphirCloudBox.Data.Mappings
{
    public class NotificationMap : IEntityTypeConfiguration<Notification>
    {
        public void Configure(EntityTypeBuilder<Notification> builder)
        {
            builder.Property(x => x.Subject).HasMaxLength(127).IsRequired();

            builder.Property(x => x.Message).HasMaxLength(511).IsRequired();

            builder.Property(x => x.CreateDate).IsRequired();

            builder.Property(x => x.Type).IsRequired();

            builder.HasOne(x => x.User).WithMany().HasForeignKey(x => x.UserId);

            builder.HasOne(x => x.FileStorage).WithMany().HasForeignKey(x => x.FileStorageId);
        }
    }
}
