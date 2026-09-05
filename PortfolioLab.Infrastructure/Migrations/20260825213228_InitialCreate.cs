using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PortfolioLab.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Trades",
                columns: table => new
                {
                    TradeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    InstrumentId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Side = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(18,8)", precision: 18, scale: 8, nullable: false),
                    UnitPrice = table.Column<decimal>(type: "decimal(18,8)", precision: 18, scale: 8, nullable: false),
                    ExecutedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Trades", x => x.TradeId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Trades");
        }
    }
}
