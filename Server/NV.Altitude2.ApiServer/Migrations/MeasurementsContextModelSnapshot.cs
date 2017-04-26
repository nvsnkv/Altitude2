using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using NV.Altitude2.ApiServer.Models;

namespace NV.Altitude2.ApiServer.Migrations
{
    [DbContext(typeof(MeasurementsContext))]
    partial class MeasurementsContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
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

                    b.Property<decimal>("Altitude");

                    b.Property<Guid>("DeviceId");

                    b.Property<decimal>("HorizontalAccuracy");

                    b.Property<decimal>("Latitude");

                    b.Property<decimal>("Longitude");

                    b.Property<DateTime>("Timestamp");

                    b.Property<decimal>("VerticalAccuracy");

                    b.HasKey("Id");

                    b.ToTable("Measurements");
                });
        }
    }
}
