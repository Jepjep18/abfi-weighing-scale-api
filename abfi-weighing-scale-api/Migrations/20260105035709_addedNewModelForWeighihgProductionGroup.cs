using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace abfi_weighing_scale_api.Migrations
{
    /// <inheritdoc />
    public partial class addedNewModelForWeighihgProductionGroup : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "WeighingProductionGroups",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductionId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WeighingProductionGroups", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WeighingProductionGroups_Production_ProductionId",
                        column: x => x.ProductionId,
                        principalTable: "Production",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WeighingProductionGroups_CreatedAt",
                table: "WeighingProductionGroups",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_WeighingProductionGroups_ProductionId",
                table: "WeighingProductionGroups",
                column: "ProductionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WeighingProductionGroups");
        }
    }
}
