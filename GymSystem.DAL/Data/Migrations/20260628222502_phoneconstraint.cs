using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GymSystem.DAL.Migrations
{
    /// <inheritdoc />
    public partial class phoneconstraint : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "PhoneConstraint1",
                table: "Trainers");

            migrationBuilder.DropCheckConstraint(
                name: "PhoneConstraint",
                table: "Members");

            migrationBuilder.AddCheckConstraint(
                name: "PhoneConstraint1",
                table: "Trainers",
                sql: "Phone Like '010%' or Phone Like '011%' or Phone Like '012%' or Phone Like '015%' ");

            migrationBuilder.AddCheckConstraint(
                name: "PhoneConstraint",
                table: "Members",
                sql: "Phone Like '010%' or Phone Like '011%' or Phone Like '012%' or Phone Like '015%' ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "PhoneConstraint1",
                table: "Trainers");

            migrationBuilder.DropCheckConstraint(
                name: "PhoneConstraint",
                table: "Members");

            migrationBuilder.AddCheckConstraint(
                name: "PhoneConstraint1",
                table: "Trainers",
                sql: "Phone Like '010%' or Phone Like '011%'");

            migrationBuilder.AddCheckConstraint(
                name: "PhoneConstraint",
                table: "Members",
                sql: "Phone Like '010%' or Phone Like '011%'");
        }
    }
}
