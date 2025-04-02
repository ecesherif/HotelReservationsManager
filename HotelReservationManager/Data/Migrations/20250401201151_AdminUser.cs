using HotelReservationManager.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HotelReservationManager.Data.Migrations
{
    /// <inheritdoc />
    public partial class AdminUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var hasher = new PasswordHasher<User>();

            string adminUserId = Guid.NewGuid().ToString();
            string adminRoleId = Guid.NewGuid().ToString();

            // Insert the admin role
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "Name", "NormalizedName" },
                values: new object[] { adminRoleId, "Admin", "ADMIN" }
            );

            // Insert the admin user
            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "UserName", "NormalizedUserName", "Email", "NormalizedEmail",
            "PasswordHash", "SecurityStamp", "ConcurrencyStamp",
            "PhoneNumber", "PhoneNumberConfirmed", "FirstName",
            "SecondName", "LastName", "EGN", "HireDate", "FireDate",
            "EmailConfirmed", "LockoutEnd", "AccessFailedCount", "TwoFactorEnabled", "LockoutEnabled" },
                values: new object[] {
            adminUserId,
            "admin",
            "ADMIN",
            "admin@admin.com",
            "ADMIN@ADMIN.COM",
            hasher.HashPassword(new User(), "Admin123"),
            Guid.NewGuid().ToString(),
            Guid.NewGuid().ToString(),
            "1234567890",
            false,
            "AdminFirstName",
            "AdminSecondName",
            "AdminLastName",
            "1234567890",
            DateTime.UtcNow,
            null,
            true,
            null,
            0,
            false,
            false
                }
            );

            // Insert admin user-role assignment
            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "UserId", "RoleId" },
                values: new object[] { adminUserId, adminRoleId }
            );

            // Logging (Check in output window or logs)
            migrationBuilder.Sql($"PRINT 'Admin user with ID {adminUserId} inserted successfully'");
        }


        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            string adminUserId = "D1F8A4A3-0CFA-4B2E-90D3-8A7A36EDE3E7";
            string adminRoleId = "E3B4EFAA-96F4-4C42-8BE2-92F1B5D562B1";

            // Remove admin role assignment, but DO NOT delete the role itself
            migrationBuilder.Sql(@$"
                DELETE FROM AspNetUserRoles WHERE UserId = '{adminUserId}' AND RoleId = '{adminRoleId}'
            ");
        }
    }
}
