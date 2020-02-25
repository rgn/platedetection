﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using Trivadis.PlateDetection;

namespace Trivadis.PlateDetection.Database.Migrations
{
    [DbContext(typeof(ApplicationDatabaseContext))]
    [Migration("20200225100443_TableNames")]
    partial class TableNames
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                .HasAnnotation("ProductVersion", "3.1.1")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            modelBuilder.Entity("Trivadis.PlateDetection.Model.DetectedPlate", b =>
                {
                    b.Property<Guid>("DetectedPlateId")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("detectedplateid")
                        .HasColumnType("uuid");

                    b.Property<string>("Characters")
                        .HasColumnName("characters")
                        .HasColumnType("text");

                    b.Property<Guid>("DetectionResultId")
                        .HasColumnName("detectionresultid")
                        .HasColumnType("uuid");

                    b.Property<bool>("MatchesTemplate")
                        .HasColumnName("matchestemplate")
                        .HasColumnType("boolean");

                    b.Property<float>("OverallConfidence")
                        .HasColumnName("overallconfidence")
                        .HasColumnType("real");

                    b.HasKey("DetectedPlateId")
                        .HasName("pk_plates");

                    b.HasIndex("DetectionResultId")
                        .HasName("ix_plates_detectionresultid");

                    b.ToTable("plates");
                });

            modelBuilder.Entity("Trivadis.PlateDetection.Model.DetectionResult", b =>
                {
                    b.Property<Guid>("DetectionResultId")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("detectionresultid")
                        .HasColumnType("uuid");

                    b.Property<Guid>("JobId")
                        .HasColumnName("jobid")
                        .HasColumnType("uuid");

                    b.Property<float>("ProcessingTimeInMs")
                        .HasColumnName("processingtimeinms")
                        .HasColumnType("real");

                    b.Property<string>("Region")
                        .HasColumnName("region")
                        .HasColumnType("text");

                    b.Property<int>("RegionConfidence")
                        .HasColumnName("regionconfidence")
                        .HasColumnType("integer");

                    b.Property<int>("RequestedTopN")
                        .HasColumnName("requestedtopn")
                        .HasColumnType("integer");

                    b.HasKey("DetectionResultId")
                        .HasName("pk_results");

                    b.HasIndex("JobId")
                        .HasName("ix_results_jobid");

                    b.ToTable("results");
                });

            modelBuilder.Entity("Trivadis.PlateDetection.Model.Job", b =>
                {
                    b.Property<Guid>("JobId")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("jobid")
                        .HasColumnType("uuid");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnName("createdat")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("CreatedBy")
                        .HasColumnName("createdby")
                        .HasColumnType("text");

                    b.Property<string>("FileName")
                        .HasColumnName("filename")
                        .HasColumnType("text");

                    b.Property<byte[]>("ImageData")
                        .HasColumnName("imagedata")
                        .HasColumnType("bytea");

                    b.Property<int>("ImageHeight")
                        .HasColumnName("imageheight")
                        .HasColumnType("integer");

                    b.Property<int>("ImageWidth")
                        .HasColumnName("imagewidth")
                        .HasColumnType("integer");

                    b.Property<DateTime>("LastUpdatedAt")
                        .HasColumnName("lastupdatedat")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("LastUpdatedBy")
                        .HasColumnName("lastupdatedby")
                        .HasColumnType("text");

                    b.Property<string>("Result")
                        .HasColumnName("result")
                        .HasColumnType("text");

                    b.Property<int>("State")
                        .HasColumnName("state")
                        .HasColumnType("integer");

                    b.Property<float>("TotalProcessingTimeInMs")
                        .HasColumnName("totalprocessingtimeinms")
                        .HasColumnType("real");

                    b.HasKey("JobId")
                        .HasName("pk_jobs");

                    b.ToTable("jobs");
                });

            modelBuilder.Entity("Trivadis.PlateDetection.Model.Rectangle", b =>
                {
                    b.Property<Guid>("RectangleId")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("rectangleid")
                        .HasColumnType("uuid");

                    b.Property<int>("Height")
                        .HasColumnName("height")
                        .HasColumnType("integer");

                    b.Property<Guid>("JobId")
                        .HasColumnName("jobid")
                        .HasColumnType("uuid");

                    b.Property<int>("Width")
                        .HasColumnName("width")
                        .HasColumnType("integer");

                    b.Property<int>("X")
                        .HasColumnName("x")
                        .HasColumnType("integer");

                    b.Property<int>("Y")
                        .HasColumnName("y")
                        .HasColumnType("integer");

                    b.HasKey("RectangleId")
                        .HasName("pk_rectangles");

                    b.HasIndex("JobId")
                        .HasName("ix_rectangles_jobid");

                    b.ToTable("rectangles");
                });

            modelBuilder.Entity("Trivadis.PlateDetection.Model.DetectedPlate", b =>
                {
                    b.HasOne("Trivadis.PlateDetection.Model.DetectionResult", "DetectionResult")
                        .WithMany("DetectedPlates")
                        .HasForeignKey("DetectionResultId")
                        .HasConstraintName("fk_plates_results_detectionresultid")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Trivadis.PlateDetection.Model.DetectionResult", b =>
                {
                    b.HasOne("Trivadis.PlateDetection.Model.Job", "DetectionJob")
                        .WithMany("DetectionResults")
                        .HasForeignKey("JobId")
                        .HasConstraintName("fk_results_jobs_jobid")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Trivadis.PlateDetection.Model.Rectangle", b =>
                {
                    b.HasOne("Trivadis.PlateDetection.Model.Job", "DetectionJob")
                        .WithMany("Rectangles")
                        .HasForeignKey("JobId")
                        .HasConstraintName("fk_rectangles_jobs_jobid")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
