﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using TorunLive.Persistance;

#nullable disable

namespace TorunLive.Persistance.Migrations
{
    [DbContext(typeof(TorunLiveContext))]
    partial class TorunLiveContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.13")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("TorunLive.Domain.EntitiesV2.Direction", b =>
                {
                    b.Property<string>("LineId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("DirectionId")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("LineId", "DirectionId");

                    b.ToTable("Directions");
                });

            modelBuilder.Entity("TorunLive.Domain.EntitiesV2.Line", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Lines");
                });

            modelBuilder.Entity("TorunLive.Domain.EntitiesV2.LineStop", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("DirectionId")
                        .HasColumnType("int");

                    b.Property<int>("DirectionId1")
                        .HasColumnType("int");

                    b.Property<string>("DirectionLineId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<bool>("IsOnDemand")
                        .HasColumnType("bit");

                    b.Property<string>("LineId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("StopId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("StopOrder")
                        .HasColumnType("int");

                    b.Property<int?>("TimeToNextStop")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("StopId");

                    b.HasIndex("DirectionLineId", "DirectionId1");

                    b.HasIndex("LineId", "DirectionId", "StopId");

                    b.HasIndex(new[] { "Id" }, "IX_LineStop_LineStop");

                    b.ToTable("LineStops");
                });

            modelBuilder.Entity("TorunLive.Domain.EntitiesV2.LineStopTime", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("DayMinute")
                        .HasColumnType("int");

                    b.Property<bool>("IsHolidays")
                        .HasColumnType("bit");

                    b.Property<bool>("IsSaturdaySundays")
                        .HasColumnType("bit");

                    b.Property<bool>("IsWeekday")
                        .HasColumnType("bit");

                    b.Property<bool>("IsWinterHoliday")
                        .HasColumnType("bit");

                    b.Property<int>("LineStopId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("LineStopId");

                    b.ToTable("LineStopTimes");
                });

            modelBuilder.Entity("TorunLive.Domain.EntitiesV2.Stop", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Stops");
                });

            modelBuilder.Entity("TorunLive.Domain.EntitiesV2.Direction", b =>
                {
                    b.HasOne("TorunLive.Domain.EntitiesV2.Line", null)
                        .WithMany("Directions")
                        .HasForeignKey("LineId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("TorunLive.Domain.EntitiesV2.LineStop", b =>
                {
                    b.HasOne("TorunLive.Domain.EntitiesV2.Line", "Line")
                        .WithMany("LineStops")
                        .HasForeignKey("LineId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("TorunLive.Domain.EntitiesV2.Stop", "Stop")
                        .WithMany("LineStops")
                        .HasForeignKey("StopId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("TorunLive.Domain.EntitiesV2.Direction", "Direction")
                        .WithMany()
                        .HasForeignKey("DirectionLineId", "DirectionId1")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Direction");

                    b.Navigation("Line");

                    b.Navigation("Stop");
                });

            modelBuilder.Entity("TorunLive.Domain.EntitiesV2.LineStopTime", b =>
                {
                    b.HasOne("TorunLive.Domain.EntitiesV2.LineStop", "LineStop")
                        .WithMany("LineStopTimes")
                        .HasForeignKey("LineStopId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("LineStop");
                });

            modelBuilder.Entity("TorunLive.Domain.EntitiesV2.Line", b =>
                {
                    b.Navigation("Directions");

                    b.Navigation("LineStops");
                });

            modelBuilder.Entity("TorunLive.Domain.EntitiesV2.LineStop", b =>
                {
                    b.Navigation("LineStopTimes");
                });

            modelBuilder.Entity("TorunLive.Domain.EntitiesV2.Stop", b =>
                {
                    b.Navigation("LineStops");
                });
#pragma warning restore 612, 618
        }
    }
}
