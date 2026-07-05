using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace EmployeeInfoSystem.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddRequestTypesSeed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "recipient_groups",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "text", nullable: false),
                    department = table.Column<string>(type: "text", nullable: true),
                    role = table.Column<string>(type: "text", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_recipient_groups", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "requesttypes",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "text", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    code = table.Column<string>(type: "text", nullable: true),
                    is_system = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_requesttypes", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "requests",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    employee_id = table.Column<int>(type: "integer", nullable: false),
                    request_type_id = table.Column<int>(type: "integer", nullable: false),
                    comment = table.Column<string>(type: "text", nullable: true),
                    new_value = table.Column<string>(type: "text", nullable: true),
                    status = table.Column<string>(type: "text", nullable: false),
                    manager_id = table.Column<int>(type: "integer", nullable: true),
                    resolution_comment = table.Column<string>(type: "text", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    resolved_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    EmployeeId1 = table.Column<int>(type: "integer", nullable: true),
                    ManagerId1 = table.Column<int>(type: "integer", nullable: true),
                    RequestTypeId1 = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_requests", x => x.id);
                    table.ForeignKey(
                        name: "FK_requests_requesttypes_RequestTypeId1",
                        column: x => x.RequestTypeId1,
                        principalTable: "requesttypes",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_requests_requesttypes_request_type_id",
                        column: x => x.request_type_id,
                        principalTable: "requesttypes",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_requests_users_EmployeeId1",
                        column: x => x.EmployeeId1,
                        principalTable: "users",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_requests_users_ManagerId1",
                        column: x => x.ManagerId1,
                        principalTable: "users",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_requests_users_employee_id",
                        column: x => x.employee_id,
                        principalTable: "users",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_requests_users_manager_id",
                        column: x => x.manager_id,
                        principalTable: "users",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "notifications",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    recipient_id = table.Column<int>(type: "integer", nullable: false),
                    sender_id = table.Column<int>(type: "integer", nullable: true),
                    title = table.Column<string>(type: "text", nullable: false),
                    body = table.Column<string>(type: "text", nullable: false),
                    is_read = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    request_id = table.Column<int>(type: "integer", nullable: true),
                    RecipientId1 = table.Column<int>(type: "integer", nullable: true),
                    SenderId1 = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_notifications", x => x.id);
                    table.ForeignKey(
                        name: "FK_notifications_requests_request_id",
                        column: x => x.request_id,
                        principalTable: "requests",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_notifications_users_RecipientId1",
                        column: x => x.RecipientId1,
                        principalTable: "users",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_notifications_users_SenderId1",
                        column: x => x.SenderId1,
                        principalTable: "users",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_notifications_users_recipient_id",
                        column: x => x.recipient_id,
                        principalTable: "users",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_notifications_users_sender_id",
                        column: x => x.sender_id,
                        principalTable: "users",
                        principalColumn: "id");
                });

            migrationBuilder.InsertData(
                table: "requesttypes",
                columns: new[] { "id", "code", "is_active", "is_system", "name" },
                values: new object[,]
                {
                    { 1, "CHANGE_CONTACTS", true, true, "Изменение контактных данных" },
                    { 2, "CHANGE_SIZES", true, true, "Изменение размеров спецодежды" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_notifications_recipient_id",
                table: "notifications",
                column: "recipient_id");

            migrationBuilder.CreateIndex(
                name: "IX_notifications_RecipientId1",
                table: "notifications",
                column: "RecipientId1");

            migrationBuilder.CreateIndex(
                name: "IX_notifications_request_id",
                table: "notifications",
                column: "request_id");

            migrationBuilder.CreateIndex(
                name: "IX_notifications_sender_id",
                table: "notifications",
                column: "sender_id");

            migrationBuilder.CreateIndex(
                name: "IX_notifications_SenderId1",
                table: "notifications",
                column: "SenderId1");

            migrationBuilder.CreateIndex(
                name: "IX_requests_employee_id",
                table: "requests",
                column: "employee_id");

            migrationBuilder.CreateIndex(
                name: "IX_requests_EmployeeId1",
                table: "requests",
                column: "EmployeeId1");

            migrationBuilder.CreateIndex(
                name: "IX_requests_manager_id",
                table: "requests",
                column: "manager_id");

            migrationBuilder.CreateIndex(
                name: "IX_requests_ManagerId1",
                table: "requests",
                column: "ManagerId1");

            migrationBuilder.CreateIndex(
                name: "IX_requests_request_type_id",
                table: "requests",
                column: "request_type_id");

            migrationBuilder.CreateIndex(
                name: "IX_requests_RequestTypeId1",
                table: "requests",
                column: "RequestTypeId1");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "notifications");

            migrationBuilder.DropTable(
                name: "recipient_groups");

            migrationBuilder.DropTable(
                name: "requests");

            migrationBuilder.DropTable(
                name: "requesttypes");
        }
    }
}
