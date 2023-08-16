using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HT.WaterAlerts.DatabaseMigration.Migrations
{
    public partial class adding_measurementsites_description_column : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "MeasurementSites",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "MeasurementSites");
        }
    }
}
