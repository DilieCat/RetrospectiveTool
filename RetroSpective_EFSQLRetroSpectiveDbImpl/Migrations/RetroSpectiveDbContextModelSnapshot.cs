﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Retrospective_EFSQLRetrospectiveDbImpl;

namespace RetroSpective_EFSQLRetroSpectiveDbImpl.Migrations
{
    [DbContext(typeof(RetroSpectiveDbContext))]
    partial class RetroSpectiveDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Retrospective_Core.Models.BaseItem", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Content")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Discriminator")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Position")
                        .HasColumnType("int");

                    b.Property<int>("RetroColumnId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("RetroColumnId");

                    b.ToTable("BaseItem");

                    b.HasDiscriminator<string>("Discriminator").HasValue("BaseItem");
                });

            modelBuilder.Entity("Retrospective_Core.Models.RetroColumn", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("RetrospectiveId")
                        .HasColumnType("int");

                    b.Property<string>("Title")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("RetrospectiveId");

                    b.ToTable("RetroColumns");
                });

            modelBuilder.Entity("Retrospective_Core.Models.Retrospective", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("Date");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Title")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Retrospectives");
                });

            modelBuilder.Entity("Retrospective_Core.Models.RetroCard", b =>
                {
                    b.HasBaseType("Retrospective_Core.Models.BaseItem");

                    b.Property<int>("DownVotes")
                        .HasColumnType("int");

                    b.Property<int>("RetroFamilyId")
                        .HasColumnType("int");

                    b.Property<int>("UpVotes")
                        .HasColumnType("int");

                    b.HasIndex("RetroFamilyId");

                    b.HasDiscriminator().HasValue("RetroCard");
                });

            modelBuilder.Entity("Retrospective_Core.Models.RetroFamily", b =>
                {
                    b.HasBaseType("Retrospective_Core.Models.BaseItem");

                    b.HasDiscriminator().HasValue("RetroFamily");
                });

            modelBuilder.Entity("Retrospective_Core.Models.BaseItem", b =>
                {
                    b.HasOne("Retrospective_Core.Models.RetroColumn", "RetroColumn")
                        .WithMany("RetroItems")
                        .HasForeignKey("RetroColumnId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();
                });

            modelBuilder.Entity("Retrospective_Core.Models.RetroColumn", b =>
                {
                    b.HasOne("Retrospective_Core.Models.Retrospective", "Retrospective")
                        .WithMany("RetroColumns")
                        .HasForeignKey("RetrospectiveId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Retrospective_Core.Models.RetroCard", b =>
                {
                    b.HasOne("Retrospective_Core.Models.RetroFamily", "RetroFamily")
                        .WithMany("RetroCards")
                        .HasForeignKey("RetroFamilyId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
