using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace abfi_weighing_scale_api.Migrations
{
    /// <inheritdoc />
    public partial class WeighingDetailsModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PortClassification",
                columns: table => new
                {
                    PortNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Class = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PortClassification", x => x.PortNumber);
                });

            migrationBuilder.CreateTable(
                name: "ProdClassification",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    prodCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    NumHeads = table.Column<int>(type: "int", nullable: true),
                    TotalIndvWeight_Min = table.Column<decimal>(type: "numeric(18,3)", nullable: true),
                    TotalIndvWeight_Max = table.Column<decimal>(type: "numeric(18,3)", nullable: true),
                    Class = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProdClassification", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WeighingDetails",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SerialData = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Qty = table.Column<decimal>(type: "numeric(18,3)", nullable: true),
                    UoM = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Heads = table.Column<int>(type: "int", nullable: true),
                    ProdCode = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    CreatedDateTime = table.Column<DateTime>(type: "datetime", nullable: true),
                    PortNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Class = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    ProductionId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WeighingDetails", x => x.id);
                    table.ForeignKey(
                        name: "FK_WeighingDetails_Production_ProductionId",
                        column: x => x.ProductionId,
                        principalTable: "Production",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProdClassification_Class_WeightRange",
                table: "ProdClassification",
                columns: new[] { "Class", "TotalIndvWeight_Min", "TotalIndvWeight_Max" });

            migrationBuilder.CreateIndex(
                name: "IX_WeighingDetails_CreatedDateTime",
                table: "WeighingDetails",
                column: "CreatedDateTime");

            migrationBuilder.CreateIndex(
                name: "IX_WeighingDetails_ProductionId",
                table: "WeighingDetails",
                column: "ProductionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PortClassification");

            migrationBuilder.DropTable(
                name: "ProdClassification");

            migrationBuilder.DropTable(
                name: "WeighingDetails");
        }
    }
}
