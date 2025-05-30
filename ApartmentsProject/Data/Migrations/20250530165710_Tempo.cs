using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ApartmentsProject.Data.Migrations
{
    /// <inheritdoc />
    public partial class Tempo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Caption",
                table: "ApartmentImages");

            migrationBuilder.DropColumn(
                name: "DisplayOrder",
                table: "ApartmentImages");

            migrationBuilder.DropColumn(
                name: "ImageType",
                table: "ApartmentImages");

            migrationBuilder.AlterColumn<int>(
                name: "Price",
                table: "Apartments",
                type: "int",
                precision: 10,
                scale: 2,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(10,2)",
                oldPrecision: 10,
                oldScale: 2);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "Price",
                table: "Apartments",
                type: "decimal(10,2)",
                precision: 10,
                scale: 2,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldPrecision: 10,
                oldScale: 2);

            migrationBuilder.AddColumn<string>(
                name: "Caption",
                table: "ApartmentImages",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DisplayOrder",
                table: "ApartmentImages",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "ImageType",
                table: "ApartmentImages",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
