using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace abfi_weighing_scale_api.Migrations
{
    /// <inheritdoc />
    public partial class modifyProductionTblAddedProductionFarmTbl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Productions_Farms_FarmId",
                table: "Productions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Productions",
                table: "Productions");

            migrationBuilder.DropIndex(
                name: "IX_Production_FarmId",
                table: "Productions");

            migrationBuilder.DropColumn(
                name: "FarmId",
                table: "Productions");

            migrationBuilder.DropColumn(
                name: "ForcastedTrips",
                table: "Productions");

            migrationBuilder.RenameTable(
                name: "Productions",
                newName: "Production");

            migrationBuilder.RenameColumn(
                name: "NoOfHeads",
                table: "Production",
                newName: "TotalHeads");

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                table: "Farms",
                type: "datetime",
                nullable: false,
                defaultValueSql: "GETUTCDATE()",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<string>(
                name: "FarmName",
                table: "Farms",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Farms",
                type: "datetime",
                nullable: false,
                defaultValueSql: "GETUTCDATE()",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Production",
                type: "datetime",
                nullable: true,
                defaultValueSql: "GETUTCDATE()",
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Production",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProductionName",
                table: "Production",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Production",
                table: "Production",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "ProductionFarms",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductionId = table.Column<int>(type: "int", nullable: false),
                    FarmId = table.Column<int>(type: "int", nullable: false),
                    ForcastedTrips = table.Column<int>(type: "int", nullable: true, defaultValue: 0),
                    AllocatedHeads = table.Column<int>(type: "int", nullable: true, defaultValue: 0),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductionFarms", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductionFarms_Farms_FarmId",
                        column: x => x.FarmId,
                        principalTable: "Farms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProductionFarms_Production_ProductionId",
                        column: x => x.ProductionId,
                        principalTable: "Production",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Farms_FarmName",
                table: "Farms",
                column: "FarmName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Farms_IsActive",
                table: "Farms",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_Production_CreatedAt",
                table: "Production",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Production_ProductionName",
                table: "Production",
                column: "ProductionName");

            migrationBuilder.CreateIndex(
                name: "IX_ProductionFarm_FarmId",
                table: "ProductionFarms",
                column: "FarmId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductionFarm_ProductionId",
                table: "ProductionFarms",
                column: "ProductionId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductionFarm_ProductionId_FarmId",
                table: "ProductionFarms",
                columns: new[] { "ProductionId", "FarmId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProductionFarms");

            migrationBuilder.DropIndex(
                name: "IX_Farms_FarmName",
                table: "Farms");

            migrationBuilder.DropIndex(
                name: "IX_Farms_IsActive",
                table: "Farms");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Production",
                table: "Production");

            migrationBuilder.DropIndex(
                name: "IX_Production_CreatedAt",
                table: "Production");

            migrationBuilder.DropIndex(
                name: "IX_Production_ProductionName",
                table: "Production");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Production");

            migrationBuilder.DropColumn(
                name: "ProductionName",
                table: "Production");

            migrationBuilder.RenameTable(
                name: "Production",
                newName: "Productions");

            migrationBuilder.RenameColumn(
                name: "TotalHeads",
                table: "Productions",
                newName: "NoOfHeads");

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                table: "Farms",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValueSql: "GETUTCDATE()");

            migrationBuilder.AlterColumn<string>(
                name: "FarmName",
                table: "Farms",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Farms",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValueSql: "GETUTCDATE()");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Productions",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true,
                oldDefaultValueSql: "GETUTCDATE()");

            migrationBuilder.AddColumn<int>(
                name: "FarmId",
                table: "Productions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ForcastedTrips",
                table: "Productions",
                type: "int",
                nullable: true,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Productions",
                table: "Productions",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Production_FarmId",
                table: "Productions",
                column: "FarmId");

            migrationBuilder.AddForeignKey(
                name: "FK_Productions_Farms_FarmId",
                table: "Productions",
                column: "FarmId",
                principalTable: "Farms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
