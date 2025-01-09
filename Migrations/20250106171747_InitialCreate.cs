using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlazorApp.Migrations
{
  /// <inheritdoc />
  public partial class InitialCreate : Migration
  {
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.CreateTable(
          name: "Users",
          columns: table => new
          {
            Id = table.Column<Guid>(type: "TEXT", nullable: false),
            Username = table.Column<string>(type: "TEXT", nullable: false)
          },
          constraints: table =>
          {
            table.PrimaryKey("PK_Users", x => x.Id);
          });

      migrationBuilder.CreateTable(
          name: "Passkeys",
          columns: table => new
          {
            Id = table.Column<Guid>(type: "TEXT", nullable: false),
            UserId = table.Column<Guid>(type: "TEXT", nullable: false),
            CredentialId = table.Column<string>(type: "TEXT", nullable: false),
            PublicKey = table.Column<string>(type: "TEXT", nullable: false),
            UserHandle = table.Column<string>(type: "TEXT", nullable: false),
            Counter = table.Column<uint>(type: "INTEGER", nullable: false)
          },
          constraints: table =>
          {
            table.PrimaryKey("PK_Passkeys", x => x.Id);
            table.ForeignKey(
                      name: "FK_Passkeys_Users_UserId",
                      column: x => x.UserId,
                      principalTable: "Users",
                      principalColumn: "Id",
                      onDelete: ReferentialAction.Cascade);
          });

      migrationBuilder.CreateIndex(
          name: "IX_Passkeys_UserId",
          table: "Passkeys",
          column: "UserId");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.DropTable(
          name: "Passkeys");

      migrationBuilder.DropTable(
          name: "Users");
    }
  }
}
