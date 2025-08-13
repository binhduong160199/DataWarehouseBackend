using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataWarehouse.API.Migrations
{
    /// <inheritdoc />
    public partial class AddCompanyExtraFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ContactEmail",
                table: "companies",
                type: "character varying(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "companies",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.AddColumn<string>(
                name: "Industry",
                table: "companies",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "companies",
                type: "boolean",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<string>(
                name: "LogoUrl",
                table: "companies",
                type: "character varying(2048)",
                maxLength: 2048,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PhoneNumber",
                table: "companies",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TaxId",
                table: "companies",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "companies",
                type: "timestamp with time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ContactEmail",
                table: "companies");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "companies");

            migrationBuilder.DropColumn(
                name: "Industry",
                table: "companies");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "companies");

            migrationBuilder.DropColumn(
                name: "LogoUrl",
                table: "companies");

            migrationBuilder.DropColumn(
                name: "PhoneNumber",
                table: "companies");

            migrationBuilder.DropColumn(
                name: "TaxId",
                table: "companies");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "companies");
        }
    }
}
