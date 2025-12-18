using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace abfi_weighing_scale_api.Migrations
{
    /// <inheritdoc />
    public partial class addedWeighingDetalsMoelsAndConfiguration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "prodCode",
                table: "ProdClassification",
                newName: "ProdCode");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "ProdClassification",
                newName: "ID");

            migrationBuilder.AlterColumn<string>(
                name: "ProdCode",
                table: "ProdClassification",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AddColumn<decimal>(
                name: "CratesWeight_Max",
                table: "ProdClassification",
                type: "numeric(18,3)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "CratesWeight_Min",
                table: "ProdClassification",
                type: "numeric(18,3)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "IndvWeight_Max",
                table: "ProdClassification",
                type: "numeric(18,3)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "IndvWeight_Min",
                table: "ProdClassification",
                type: "numeric(18,3)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UoMAll",
                table: "ProdClassification",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CratesWeight_Max",
                table: "ProdClassification");

            migrationBuilder.DropColumn(
                name: "CratesWeight_Min",
                table: "ProdClassification");

            migrationBuilder.DropColumn(
                name: "IndvWeight_Max",
                table: "ProdClassification");

            migrationBuilder.DropColumn(
                name: "IndvWeight_Min",
                table: "ProdClassification");

            migrationBuilder.DropColumn(
                name: "UoMAll",
                table: "ProdClassification");

            migrationBuilder.RenameColumn(
                name: "ProdCode",
                table: "ProdClassification",
                newName: "prodCode");

            migrationBuilder.RenameColumn(
                name: "ID",
                table: "ProdClassification",
                newName: "Id");

            migrationBuilder.AlterColumn<string>(
                name: "prodCode",
                table: "ProdClassification",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(150)",
                oldMaxLength: 150);
        }
    }
}
