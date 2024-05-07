using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddInvitationIdToGuestTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Invitations_Guests_GuestId",
                table: "Invitations");

            migrationBuilder.DropIndex(
                name: "IX_Invitations_GuestId",
                table: "Invitations");

            migrationBuilder.DropColumn(
                name: "GuestId",
                table: "Invitations");

            migrationBuilder.AddColumn<Guid>(
                name: "InvitationId",
                table: "Guests",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Guests_InvitationId",
                table: "Guests",
                column: "InvitationId",
                unique: true,
                filter: "[InvitationId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Guests_Invitations_InvitationId",
                table: "Guests",
                column: "InvitationId",
                principalTable: "Invitations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Guests_Invitations_InvitationId",
                table: "Guests");

            migrationBuilder.DropIndex(
                name: "IX_Guests_InvitationId",
                table: "Guests");

            migrationBuilder.DropColumn(
                name: "InvitationId",
                table: "Guests");

            migrationBuilder.AddColumn<Guid>(
                name: "GuestId",
                table: "Invitations",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Invitations_GuestId",
                table: "Invitations",
                column: "GuestId",
                unique: true,
                filter: "[GuestId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Invitations_Guests_GuestId",
                table: "Invitations",
                column: "GuestId",
                principalTable: "Guests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
