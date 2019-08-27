﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SaphirCloudBox.Data;

namespace SaphirCloudBox.Data.Migrations
{
    [DbContext(typeof(SaphirCloudBoxDataContext))]
    partial class SaphirCloudBoxDataContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.6-servicing-10079")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<int>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<int>("RoleId");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<int>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<int>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<int>", b =>
                {
                    b.Property<string>("LoginProvider");

                    b.Property<string>("ProviderKey");

                    b.Property<string>("ProviderDisplayName");

                    b.Property<int>("UserId");

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<int>", b =>
                {
                    b.Property<int>("UserId");

                    b.Property<int>("RoleId");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetUserRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<int>", b =>
                {
                    b.Property<int>("UserId");

                    b.Property<string>("LoginProvider");

                    b.Property<string>("Name");

                    b.Property<string>("Value");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens");
                });

            modelBuilder.Entity("SaphirCloudBox.Models.AzureBlobStorage", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<Guid>("BlobName");

                    b.Property<int>("FileId");

                    b.HasKey("Id");

                    b.HasIndex("FileId")
                        .IsUnique();

                    b.ToTable("AzureBlobStorage");
                });

            modelBuilder.Entity("SaphirCloudBox.Models.Client", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("CreateDate");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(255);

                    b.Property<DateTime?>("UpdateDate");

                    b.HasKey("Id");

                    b.ToTable("Client");
                });

            modelBuilder.Entity("SaphirCloudBox.Models.Department", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("ClientId");

                    b.Property<DateTime>("CreateDate");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(255);

                    b.Property<DateTime?>("UpdateDate");

                    b.HasKey("Id");

                    b.HasIndex("ClientId");

                    b.ToTable("Department");
                });

            modelBuilder.Entity("SaphirCloudBox.Models.File", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("CreateById");

                    b.Property<DateTime>("CreateDate");

                    b.Property<string>("Extension")
                        .IsRequired()
                        .HasMaxLength(100);

                    b.Property<int>("FileStorageId");

                    b.Property<bool>("IsActive");

                    b.Property<int>("Size");

                    b.Property<string>("SizeType")
                        .IsRequired()
                        .HasMaxLength(5);

                    b.Property<int?>("UpdateById");

                    b.Property<DateTime?>("UpdateDate");

                    b.HasKey("Id");

                    b.HasIndex("CreateById");

                    b.HasIndex("FileStorageId");

                    b.HasIndex("UpdateById");

                    b.ToTable("File");
                });

            modelBuilder.Entity("SaphirCloudBox.Models.FileStorage", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int?>("ClientId");

                    b.Property<int>("CreateById");

                    b.Property<DateTime>("CreateDate");

                    b.Property<bool>("IsDirectory");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(255);

                    b.Property<int?>("OwnerId");

                    b.Property<int?>("ParentFileStorageId");

                    b.Property<int?>("UpdateById");

                    b.Property<DateTime?>("UpdateDate");

                    b.HasKey("Id");

                    b.HasIndex("ClientId");

                    b.HasIndex("CreateById");

                    b.HasIndex("OwnerId");

                    b.HasIndex("ParentFileStorageId");

                    b.HasIndex("UpdateById");

                    b.ToTable("FileStorage");
                });

            modelBuilder.Entity("SaphirCloudBox.Models.FileStoragePermission", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime?>("EndDate");

                    b.Property<int>("FileStorageId");

                    b.Property<int>("RecipientId");

                    b.Property<int>("SenderId");

                    b.Property<DateTime>("StartDate");

                    b.Property<int>("Type");

                    b.HasKey("Id");

                    b.HasIndex("FileStorageId");

                    b.HasIndex("RecipientId");

                    b.HasIndex("SenderId");

                    b.ToTable("FileStoragePermission");
                });

            modelBuilder.Entity("SaphirCloudBox.Models.Log", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("LogType");

                    b.Property<string>("Text")
                        .IsRequired()
                        .HasMaxLength(255);

                    b.HasKey("Id");

                    b.ToTable("Log");
                });

            modelBuilder.Entity("SaphirCloudBox.Models.Notification", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("CreateDate");

                    b.Property<int?>("FileStorageId");

                    b.Property<string>("Message")
                        .IsRequired()
                        .HasMaxLength(511);

                    b.Property<string>("Subject")
                        .IsRequired()
                        .HasMaxLength(127);

                    b.Property<int>("Type");

                    b.Property<int?>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("FileStorageId");

                    b.HasIndex("UserId");

                    b.ToTable("Notification");
                });

            modelBuilder.Entity("SaphirCloudBox.Models.Role", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<string>("Name")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedName")
                        .HasMaxLength(256);

                    b.Property<int>("RoleType");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasName("RoleNameIndex")
                        .HasFilter("[NormalizedName] IS NOT NULL");

                    b.ToTable("AspNetRoles");
                });

            modelBuilder.Entity("SaphirCloudBox.Models.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("AccessFailedCount");

                    b.Property<int>("ClientId");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<DateTime>("CreateDate");

                    b.Property<int?>("DepartmentId");

                    b.Property<string>("Email")
                        .HasMaxLength(256);

                    b.Property<bool>("EmailConfirmed");

                    b.Property<bool>("LockoutEnabled");

                    b.Property<DateTimeOffset?>("LockoutEnd");

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256);

                    b.Property<string>("PasswordHash");

                    b.Property<string>("PhoneNumber");

                    b.Property<bool>("PhoneNumberConfirmed");

                    b.Property<string>("ResetPasswordCode");

                    b.Property<string>("SecurityStamp");

                    b.Property<bool>("TwoFactorEnabled");

                    b.Property<DateTime?>("UpdateDate");

                    b.Property<string>("UserName")
                        .HasMaxLength(256);

                    b.HasKey("Id");

                    b.HasIndex("ClientId");

                    b.HasIndex("DepartmentId");

                    b.HasIndex("NormalizedEmail")
                        .HasName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasName("UserNameIndex")
                        .HasFilter("[NormalizedUserName] IS NOT NULL");

                    b.ToTable("AspNetUsers");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<int>", b =>
                {
                    b.HasOne("SaphirCloudBox.Models.Role")
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<int>", b =>
                {
                    b.HasOne("SaphirCloudBox.Models.User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<int>", b =>
                {
                    b.HasOne("SaphirCloudBox.Models.User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<int>", b =>
                {
                    b.HasOne("SaphirCloudBox.Models.Role")
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("SaphirCloudBox.Models.User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<int>", b =>
                {
                    b.HasOne("SaphirCloudBox.Models.User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("SaphirCloudBox.Models.AzureBlobStorage", b =>
                {
                    b.HasOne("SaphirCloudBox.Models.File", "File")
                        .WithOne("AzureBlobStorage")
                        .HasForeignKey("SaphirCloudBox.Models.AzureBlobStorage", "FileId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("SaphirCloudBox.Models.Department", b =>
                {
                    b.HasOne("SaphirCloudBox.Models.Client", "Client")
                        .WithMany("Departments")
                        .HasForeignKey("ClientId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("SaphirCloudBox.Models.File", b =>
                {
                    b.HasOne("SaphirCloudBox.Models.User", "CreateBy")
                        .WithMany()
                        .HasForeignKey("CreateById")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("SaphirCloudBox.Models.FileStorage", "FileStorage")
                        .WithMany("Files")
                        .HasForeignKey("FileStorageId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("SaphirCloudBox.Models.User", "UpdateBy")
                        .WithMany()
                        .HasForeignKey("UpdateById");
                });

            modelBuilder.Entity("SaphirCloudBox.Models.FileStorage", b =>
                {
                    b.HasOne("SaphirCloudBox.Models.Client", "Client")
                        .WithMany()
                        .HasForeignKey("ClientId");

                    b.HasOne("SaphirCloudBox.Models.User", "CreateBy")
                        .WithMany()
                        .HasForeignKey("CreateById")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("SaphirCloudBox.Models.User", "Owner")
                        .WithMany()
                        .HasForeignKey("OwnerId");

                    b.HasOne("SaphirCloudBox.Models.FileStorage", "ParentFileStorage")
                        .WithMany()
                        .HasForeignKey("ParentFileStorageId");

                    b.HasOne("SaphirCloudBox.Models.User", "UpdateBy")
                        .WithMany()
                        .HasForeignKey("UpdateById");
                });

            modelBuilder.Entity("SaphirCloudBox.Models.FileStoragePermission", b =>
                {
                    b.HasOne("SaphirCloudBox.Models.FileStorage", "FileStorage")
                        .WithMany("Permissions")
                        .HasForeignKey("FileStorageId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("SaphirCloudBox.Models.User", "Recipient")
                        .WithMany()
                        .HasForeignKey("RecipientId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("SaphirCloudBox.Models.User", "Sender")
                        .WithMany()
                        .HasForeignKey("SenderId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("SaphirCloudBox.Models.Notification", b =>
                {
                    b.HasOne("SaphirCloudBox.Models.FileStorage", "FileStorage")
                        .WithMany()
                        .HasForeignKey("FileStorageId");

                    b.HasOne("SaphirCloudBox.Models.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("SaphirCloudBox.Models.User", b =>
                {
                    b.HasOne("SaphirCloudBox.Models.Client", "Client")
                        .WithMany("Users")
                        .HasForeignKey("ClientId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("SaphirCloudBox.Models.Department", "Department")
                        .WithMany("Users")
                        .HasForeignKey("DepartmentId");
                });
#pragma warning restore 612, 618
        }
    }
}
