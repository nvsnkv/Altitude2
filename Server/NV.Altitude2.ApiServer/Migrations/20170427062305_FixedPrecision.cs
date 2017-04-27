using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace NV.Altitude2.ApiServer.Migrations
{
    public partial class FixedPrecision : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "VerticalAccuracy",
                table: "Measurements",
                type: "decimal(18, 10)",
                nullable: false,
                oldClrType: typeof(decimal));

            migrationBuilder.AlterColumn<decimal>(
                name: "Longitude",
                table: "Measurements",
                type: "decimal(18, 10)",
                nullable: false,
                oldClrType: typeof(decimal));

            migrationBuilder.AlterColumn<decimal>(
                name: "Latitude",
                table: "Measurements",
                type: "decimal(18, 10)",
                nullable: false,
                oldClrType: typeof(decimal));

            migrationBuilder.AlterColumn<decimal>(
                name: "HorizontalAccuracy",
                table: "Measurements",
                type: "decimal(18, 10)",
                nullable: false,
                oldClrType: typeof(decimal));

            migrationBuilder.AlterColumn<decimal>(
                name: "Altitude",
                table: "Measurements",
                type: "decimal(18, 10)",
                nullable: false,
                oldClrType: typeof(decimal));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "VerticalAccuracy",
                table: "Measurements",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18, 10)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Longitude",
                table: "Measurements",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18, 10)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Latitude",
                table: "Measurements",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18, 10)");

            migrationBuilder.AlterColumn<decimal>(
                name: "HorizontalAccuracy",
                table: "Measurements",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18, 10)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Altitude",
                table: "Measurements",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18, 10)");
        }
    }
}
