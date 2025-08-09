using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataWarehouse.API.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "inventory_mismatches",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PartNumber = table.Column<string>(type: "text", nullable: false),
                    DetectedPositionCode = table.Column<string>(type: "text", nullable: true),
                    MismatchType = table.Column<int>(type: "integer", nullable: false),
                    DetectedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_inventory_mismatches", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "parts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PartNumber = table.Column<string>(type: "text", nullable: false),
                    PartName = table.Column<string>(type: "text", nullable: false),
                    ImageUrl = table.Column<string>(type: "text", nullable: true),
                    Width = table.Column<float>(type: "real", nullable: false),
                    Height = table.Column<float>(type: "real", nullable: false),
                    Depth = table.Column<float>(type: "real", nullable: false),
                    Weight = table.Column<float>(type: "real", nullable: false),
                    Category = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_parts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "scan_sessions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ScannedPositionCode = table.Column<string>(type: "text", nullable: false),
                    ScannedPartNumber = table.Column<string>(type: "text", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    ScannedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_scan_sessions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "suggested_placements",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SuggestedPositionCode = table.Column<string>(type: "text", nullable: false),
                    PartId = table.Column<Guid>(type: "uuid", nullable: false),
                    AlgorithmUsed = table.Column<int>(type: "integer", nullable: false),
                    FitScore = table.Column<float>(type: "real", nullable: false),
                    GeneratedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_suggested_placements", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "warehouse_racks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    RackCode = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_warehouse_racks", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "workers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FullName = table.Column<string>(type: "text", nullable: false),
                    EmployeeCode = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_workers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "rack_positions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PositionCode = table.Column<string>(type: "text", nullable: false),
                    Level = table.Column<int>(type: "integer", nullable: false),
                    RackId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_rack_positions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_rack_positions_warehouse_racks_RackId",
                        column: x => x.RackId,
                        principalTable: "warehouse_racks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "worker_positions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    WorkerId = table.Column<Guid>(type: "uuid", nullable: false),
                    CurrentPositionCode = table.Column<string>(type: "text", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_worker_positions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_worker_positions_workers_WorkerId",
                        column: x => x.WorkerId,
                        principalTable: "workers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "packages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PartId = table.Column<Guid>(type: "uuid", nullable: false),
                    RackPositionId = table.Column<Guid>(type: "uuid", nullable: false),
                    StoredAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_packages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_packages_parts_PartId",
                        column: x => x.PartId,
                        principalTable: "parts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_packages_rack_positions_RackPositionId",
                        column: x => x.RackPositionId,
                        principalTable: "rack_positions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "movement_logs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PackageId = table.Column<Guid>(type: "uuid", nullable: false),
                    FromPositionCode = table.Column<string>(type: "text", nullable: false),
                    ToPositionCode = table.Column<string>(type: "text", nullable: false),
                    MovedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_movement_logs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_movement_logs_packages_PackageId",
                        column: x => x.PackageId,
                        principalTable: "packages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_inventory_mismatches_PartNumber",
                table: "inventory_mismatches",
                column: "PartNumber");

            migrationBuilder.CreateIndex(
                name: "IX_movement_logs_PackageId",
                table: "movement_logs",
                column: "PackageId");

            migrationBuilder.CreateIndex(
                name: "IX_packages_PartId",
                table: "packages",
                column: "PartId");

            migrationBuilder.CreateIndex(
                name: "IX_packages_RackPositionId",
                table: "packages",
                column: "RackPositionId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_packages_RackPositionId_PartId",
                table: "packages",
                columns: new[] { "RackPositionId", "PartId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_parts_PartNumber",
                table: "parts",
                column: "PartNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_rack_positions_PositionCode",
                table: "rack_positions",
                column: "PositionCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_rack_positions_RackId",
                table: "rack_positions",
                column: "RackId");

            migrationBuilder.CreateIndex(
                name: "IX_scan_sessions_ScannedPositionCode_ScannedPartNumber",
                table: "scan_sessions",
                columns: new[] { "ScannedPositionCode", "ScannedPartNumber" });

            migrationBuilder.CreateIndex(
                name: "IX_suggested_placements_PartId",
                table: "suggested_placements",
                column: "PartId");

            migrationBuilder.CreateIndex(
                name: "IX_worker_positions_WorkerId",
                table: "worker_positions",
                column: "WorkerId");

            migrationBuilder.CreateIndex(
                name: "IX_workers_EmployeeCode",
                table: "workers",
                column: "EmployeeCode",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "inventory_mismatches");

            migrationBuilder.DropTable(
                name: "movement_logs");

            migrationBuilder.DropTable(
                name: "scan_sessions");

            migrationBuilder.DropTable(
                name: "suggested_placements");

            migrationBuilder.DropTable(
                name: "worker_positions");

            migrationBuilder.DropTable(
                name: "packages");

            migrationBuilder.DropTable(
                name: "workers");

            migrationBuilder.DropTable(
                name: "parts");

            migrationBuilder.DropTable(
                name: "rack_positions");

            migrationBuilder.DropTable(
                name: "warehouse_racks");
        }
    }
}
