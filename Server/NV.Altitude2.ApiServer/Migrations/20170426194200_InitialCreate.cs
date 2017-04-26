using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace NV.Altitude2.ApiServer.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateSequence(
                name: "MeasurementNumbers");

            migrationBuilder.CreateTable(
                name: "Measurements",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false, defaultValueSql: "NEXT VALUE FOR MeasurementNumbers"),
                    Altitude = table.Column<decimal>(nullable: false),
                    DeviceId = table.Column<Guid>(nullable: false),
                    HorizontalAccuracy = table.Column<decimal>(nullable: false),
                    Latitude = table.Column<decimal>(nullable: false),
                    Longitude = table.Column<decimal>(nullable: false),
                    Timestamp = table.Column<DateTime>(nullable: false),
                    VerticalAccuracy = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Measurements", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Measurements");

            migrationBuilder.DropSequence(
                name: "MeasurementNumbers");
        }
    }
}
