using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using NV.Altitude2.ApiServer.Models;

namespace NV.Altitude2.ApiServer.Migrations
{
    [DbContext(typeof(MeasurementsContext))]
    [Migration("20170427062305_FixedPrecision")]
    partial class FixedPrecision
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.1.1")
                .HasAnnotation("Relational:Sequence:.MeasurementNumbers", "'MeasurementNumbers', '', '1', '1', '', '', 'Int64', 'False'")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("NV.Altitude2.ApiServer.Models.DbMeasurement", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValueSql("NEXT VALUE FOR MeasurementNumbers");

                    b.Property<decimal>("Altitude")
                        .HasColumnType("decimal(18, 10)");

                    b.Property<Guid>("DeviceId");

                    b.Property<decimal>("HorizontalAccuracy")
                        .HasColumnType("decimal(18, 10)");

                    b.Property<decimal>("Latitude")
                        .HasColumnType("decimal(18, 10)");

                    b.Property<decimal>("Longitude")
                        .HasColumnType("decimal(18, 10)");

                    b.Property<DateTime>("Timestamp");

                    b.Property<decimal>("VerticalAccuracy")
                        .HasColumnType("decimal(18, 10)");

                    b.HasKey("Id");

                    b.ToTable("Measurements");
                });
        }
    }
}
