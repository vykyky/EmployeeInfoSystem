using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EmployeeInfoSystem.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateRequestStatusCheck : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Снимаем старое ограничение: new / in_progress / done
            migrationBuilder.Sql(@"
                ALTER TABLE requests DROP CONSTRAINT IF EXISTS requests_status_check;
            ");

            // Переносим старые статусы на новую схему
            migrationBuilder.Sql(@"
                UPDATE requests
                SET status = 'assigned'
                WHERE status = 'new' AND manager_id IS NOT NULL;

                UPDATE requests
                SET status = 'accepted'
                WHERE status = 'new' AND manager_id IS NULL;
            ");

            migrationBuilder.Sql(@"
                ALTER TABLE requests
                ALTER COLUMN status SET DEFAULT 'accepted';
            ");

            migrationBuilder.Sql(@"
                ALTER TABLE requests
                ADD CONSTRAINT requests_status_check
                CHECK (status IN ('accepted', 'assigned', 'in_progress', 'done'));
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                ALTER TABLE requests DROP CONSTRAINT IF EXISTS requests_status_check;
            ");

            migrationBuilder.Sql(@"
                UPDATE requests
                SET status = 'new'
                WHERE status IN ('accepted', 'assigned');
            ");

            migrationBuilder.Sql(@"
                ALTER TABLE requests
                ALTER COLUMN status SET DEFAULT 'new';
            ");

            migrationBuilder.Sql(@"
                ALTER TABLE requests
                ADD CONSTRAINT requests_status_check
                CHECK (status IN ('new', 'in_progress', 'done'));
            ");
        }
    }
}
