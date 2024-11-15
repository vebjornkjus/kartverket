using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KartverketWebApp.Migrations
{
    /// <inheritdoc />
    public partial class MakeSteddataIdNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "SteddataId",
                table: "Kart",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.UpdateData(
                table: "Meldinger",
                keyColumn: "MeldingsId",
                keyValue: 1,
                column: "Tidsstempel",
                value: new DateTime(2024, 11, 14, 19, 14, 38, 783, DateTimeKind.Local).AddTicks(3304));

            migrationBuilder.UpdateData(
                table: "Meldinger",
                keyColumn: "MeldingsId",
                keyValue: 2,
                column: "Tidsstempel",
                value: new DateTime(2024, 11, 14, 19, 14, 38, 783, DateTimeKind.Local).AddTicks(3310));

            migrationBuilder.UpdateData(
                table: "Meldinger",
                keyColumn: "MeldingsId",
                keyValue: 3,
                column: "Tidsstempel",
                value: new DateTime(2024, 11, 14, 19, 14, 38, 783, DateTimeKind.Local).AddTicks(3313));

            migrationBuilder.UpdateData(
                table: "Meldinger",
                keyColumn: "MeldingsId",
                keyValue: 4,
                column: "Tidsstempel",
                value: new DateTime(2024, 11, 14, 19, 14, 38, 783, DateTimeKind.Local).AddTicks(3315));

            migrationBuilder.UpdateData(
                table: "Meldinger",
                keyColumn: "MeldingsId",
                keyValue: 5,
                column: "Tidsstempel",
                value: new DateTime(2024, 11, 14, 19, 14, 38, 783, DateTimeKind.Local).AddTicks(3317));

            migrationBuilder.UpdateData(
                table: "Meldinger",
                keyColumn: "MeldingsId",
                keyValue: 6,
                column: "Tidsstempel",
                value: new DateTime(2024, 11, 14, 19, 14, 38, 783, DateTimeKind.Local).AddTicks(3320));

            migrationBuilder.UpdateData(
                table: "Meldinger",
                keyColumn: "MeldingsId",
                keyValue: 7,
                column: "Tidsstempel",
                value: new DateTime(2024, 11, 14, 19, 14, 38, 783, DateTimeKind.Local).AddTicks(3322));

            migrationBuilder.UpdateData(
                table: "Meldinger",
                keyColumn: "MeldingsId",
                keyValue: 8,
                column: "Tidsstempel",
                value: new DateTime(2024, 11, 14, 19, 14, 38, 783, DateTimeKind.Local).AddTicks(3324));

            migrationBuilder.UpdateData(
                table: "Meldinger",
                keyColumn: "MeldingsId",
                keyValue: 9,
                column: "Tidsstempel",
                value: new DateTime(2024, 11, 14, 19, 14, 38, 783, DateTimeKind.Local).AddTicks(3326));

            migrationBuilder.UpdateData(
                table: "Meldinger",
                keyColumn: "MeldingsId",
                keyValue: 10,
                column: "Tidsstempel",
                value: new DateTime(2024, 11, 14, 19, 14, 38, 783, DateTimeKind.Local).AddTicks(3329));

            migrationBuilder.UpdateData(
                table: "Meldinger",
                keyColumn: "MeldingsId",
                keyValue: 11,
                column: "Tidsstempel",
                value: new DateTime(2024, 11, 14, 19, 14, 38, 783, DateTimeKind.Local).AddTicks(3330));

            migrationBuilder.UpdateData(
                table: "Meldinger",
                keyColumn: "MeldingsId",
                keyValue: 12,
                column: "Tidsstempel",
                value: new DateTime(2024, 11, 14, 19, 14, 38, 783, DateTimeKind.Local).AddTicks(3332));

            migrationBuilder.UpdateData(
                table: "Meldinger",
                keyColumn: "MeldingsId",
                keyValue: 13,
                column: "Tidsstempel",
                value: new DateTime(2024, 11, 14, 19, 14, 38, 783, DateTimeKind.Local).AddTicks(3334));

            migrationBuilder.UpdateData(
                table: "Meldinger",
                keyColumn: "MeldingsId",
                keyValue: 14,
                column: "Tidsstempel",
                value: new DateTime(2024, 11, 14, 19, 14, 38, 783, DateTimeKind.Local).AddTicks(3336));

            migrationBuilder.UpdateData(
                table: "Meldinger",
                keyColumn: "MeldingsId",
                keyValue: 15,
                column: "Tidsstempel",
                value: new DateTime(2024, 11, 14, 19, 14, 38, 783, DateTimeKind.Local).AddTicks(3338));

            migrationBuilder.UpdateData(
                table: "Meldinger",
                keyColumn: "MeldingsId",
                keyValue: 16,
                column: "Tidsstempel",
                value: new DateTime(2024, 11, 14, 19, 14, 38, 783, DateTimeKind.Local).AddTicks(3340));

            migrationBuilder.UpdateData(
                table: "Meldinger",
                keyColumn: "MeldingsId",
                keyValue: 17,
                column: "Tidsstempel",
                value: new DateTime(2024, 11, 14, 19, 14, 38, 783, DateTimeKind.Local).AddTicks(3342));

            migrationBuilder.UpdateData(
                table: "Meldinger",
                keyColumn: "MeldingsId",
                keyValue: 18,
                column: "Tidsstempel",
                value: new DateTime(2024, 11, 14, 19, 14, 38, 783, DateTimeKind.Local).AddTicks(3344));

            migrationBuilder.UpdateData(
                table: "Meldinger",
                keyColumn: "MeldingsId",
                keyValue: 19,
                column: "Tidsstempel",
                value: new DateTime(2024, 11, 14, 19, 14, 38, 783, DateTimeKind.Local).AddTicks(3346));

            migrationBuilder.UpdateData(
                table: "Meldinger",
                keyColumn: "MeldingsId",
                keyValue: 20,
                column: "Tidsstempel",
                value: new DateTime(2024, 11, 14, 19, 14, 38, 783, DateTimeKind.Local).AddTicks(3347));

            migrationBuilder.UpdateData(
                table: "Rapport",
                keyColumn: "RapportId",
                keyValue: 1,
                column: "Opprettet",
                value: new DateTime(2024, 11, 14, 19, 14, 38, 783, DateTimeKind.Local).AddTicks(3055));

            migrationBuilder.UpdateData(
                table: "Rapport",
                keyColumn: "RapportId",
                keyValue: 2,
                column: "Opprettet",
                value: new DateTime(2024, 11, 14, 19, 14, 38, 783, DateTimeKind.Local).AddTicks(3162));

            migrationBuilder.UpdateData(
                table: "Rapport",
                keyColumn: "RapportId",
                keyValue: 3,
                column: "Opprettet",
                value: new DateTime(2024, 11, 14, 19, 14, 38, 783, DateTimeKind.Local).AddTicks(3165));

            migrationBuilder.UpdateData(
                table: "Rapport",
                keyColumn: "RapportId",
                keyValue: 4,
                column: "Opprettet",
                value: new DateTime(2024, 11, 14, 19, 14, 38, 783, DateTimeKind.Local).AddTicks(3168));

            migrationBuilder.UpdateData(
                table: "Rapport",
                keyColumn: "RapportId",
                keyValue: 5,
                column: "Opprettet",
                value: new DateTime(2024, 11, 14, 19, 14, 38, 783, DateTimeKind.Local).AddTicks(3170));

            migrationBuilder.UpdateData(
                table: "Rapport",
                keyColumn: "RapportId",
                keyValue: 6,
                column: "Opprettet",
                value: new DateTime(2024, 11, 14, 19, 14, 38, 783, DateTimeKind.Local).AddTicks(3173));

            migrationBuilder.UpdateData(
                table: "Rapport",
                keyColumn: "RapportId",
                keyValue: 7,
                column: "Opprettet",
                value: new DateTime(2024, 11, 14, 19, 14, 38, 783, DateTimeKind.Local).AddTicks(3175));

            migrationBuilder.UpdateData(
                table: "Rapport",
                keyColumn: "RapportId",
                keyValue: 8,
                column: "Opprettet",
                value: new DateTime(2024, 11, 14, 19, 14, 38, 783, DateTimeKind.Local).AddTicks(3177));

            migrationBuilder.UpdateData(
                table: "Rapport",
                keyColumn: "RapportId",
                keyValue: 9,
                column: "Opprettet",
                value: new DateTime(2024, 11, 14, 19, 14, 38, 783, DateTimeKind.Local).AddTicks(3180));

            migrationBuilder.UpdateData(
                table: "Rapport",
                keyColumn: "RapportId",
                keyValue: 10,
                column: "Opprettet",
                value: new DateTime(2024, 11, 14, 19, 14, 38, 783, DateTimeKind.Local).AddTicks(3182));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "SteddataId",
                table: "Kart",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "Meldinger",
                keyColumn: "MeldingsId",
                keyValue: 1,
                column: "Tidsstempel",
                value: new DateTime(2024, 11, 14, 11, 34, 46, 218, DateTimeKind.Local).AddTicks(6939));

            migrationBuilder.UpdateData(
                table: "Meldinger",
                keyColumn: "MeldingsId",
                keyValue: 2,
                column: "Tidsstempel",
                value: new DateTime(2024, 11, 14, 11, 34, 46, 218, DateTimeKind.Local).AddTicks(6941));

            migrationBuilder.UpdateData(
                table: "Meldinger",
                keyColumn: "MeldingsId",
                keyValue: 3,
                column: "Tidsstempel",
                value: new DateTime(2024, 11, 14, 11, 34, 46, 218, DateTimeKind.Local).AddTicks(6944));

            migrationBuilder.UpdateData(
                table: "Meldinger",
                keyColumn: "MeldingsId",
                keyValue: 4,
                column: "Tidsstempel",
                value: new DateTime(2024, 11, 14, 11, 34, 46, 218, DateTimeKind.Local).AddTicks(6947));

            migrationBuilder.UpdateData(
                table: "Meldinger",
                keyColumn: "MeldingsId",
                keyValue: 5,
                column: "Tidsstempel",
                value: new DateTime(2024, 11, 14, 11, 34, 46, 218, DateTimeKind.Local).AddTicks(6948));

            migrationBuilder.UpdateData(
                table: "Meldinger",
                keyColumn: "MeldingsId",
                keyValue: 6,
                column: "Tidsstempel",
                value: new DateTime(2024, 11, 14, 11, 34, 46, 218, DateTimeKind.Local).AddTicks(6950));

            migrationBuilder.UpdateData(
                table: "Meldinger",
                keyColumn: "MeldingsId",
                keyValue: 7,
                column: "Tidsstempel",
                value: new DateTime(2024, 11, 14, 11, 34, 46, 218, DateTimeKind.Local).AddTicks(6952));

            migrationBuilder.UpdateData(
                table: "Meldinger",
                keyColumn: "MeldingsId",
                keyValue: 8,
                column: "Tidsstempel",
                value: new DateTime(2024, 11, 14, 11, 34, 46, 218, DateTimeKind.Local).AddTicks(6955));

            migrationBuilder.UpdateData(
                table: "Meldinger",
                keyColumn: "MeldingsId",
                keyValue: 9,
                column: "Tidsstempel",
                value: new DateTime(2024, 11, 14, 11, 34, 46, 218, DateTimeKind.Local).AddTicks(6957));

            migrationBuilder.UpdateData(
                table: "Meldinger",
                keyColumn: "MeldingsId",
                keyValue: 10,
                column: "Tidsstempel",
                value: new DateTime(2024, 11, 14, 11, 34, 46, 218, DateTimeKind.Local).AddTicks(6959));

            migrationBuilder.UpdateData(
                table: "Meldinger",
                keyColumn: "MeldingsId",
                keyValue: 11,
                column: "Tidsstempel",
                value: new DateTime(2024, 11, 14, 11, 34, 46, 218, DateTimeKind.Local).AddTicks(6960));

            migrationBuilder.UpdateData(
                table: "Meldinger",
                keyColumn: "MeldingsId",
                keyValue: 12,
                column: "Tidsstempel",
                value: new DateTime(2024, 11, 14, 11, 34, 46, 218, DateTimeKind.Local).AddTicks(6962));

            migrationBuilder.UpdateData(
                table: "Meldinger",
                keyColumn: "MeldingsId",
                keyValue: 13,
                column: "Tidsstempel",
                value: new DateTime(2024, 11, 14, 11, 34, 46, 218, DateTimeKind.Local).AddTicks(6963));

            migrationBuilder.UpdateData(
                table: "Meldinger",
                keyColumn: "MeldingsId",
                keyValue: 14,
                column: "Tidsstempel",
                value: new DateTime(2024, 11, 14, 11, 34, 46, 218, DateTimeKind.Local).AddTicks(6966));

            migrationBuilder.UpdateData(
                table: "Meldinger",
                keyColumn: "MeldingsId",
                keyValue: 15,
                column: "Tidsstempel",
                value: new DateTime(2024, 11, 14, 11, 34, 46, 218, DateTimeKind.Local).AddTicks(6968));

            migrationBuilder.UpdateData(
                table: "Meldinger",
                keyColumn: "MeldingsId",
                keyValue: 16,
                column: "Tidsstempel",
                value: new DateTime(2024, 11, 14, 11, 34, 46, 218, DateTimeKind.Local).AddTicks(6971));

            migrationBuilder.UpdateData(
                table: "Meldinger",
                keyColumn: "MeldingsId",
                keyValue: 17,
                column: "Tidsstempel",
                value: new DateTime(2024, 11, 14, 11, 34, 46, 218, DateTimeKind.Local).AddTicks(6973));

            migrationBuilder.UpdateData(
                table: "Meldinger",
                keyColumn: "MeldingsId",
                keyValue: 18,
                column: "Tidsstempel",
                value: new DateTime(2024, 11, 14, 11, 34, 46, 218, DateTimeKind.Local).AddTicks(6975));

            migrationBuilder.UpdateData(
                table: "Meldinger",
                keyColumn: "MeldingsId",
                keyValue: 19,
                column: "Tidsstempel",
                value: new DateTime(2024, 11, 14, 11, 34, 46, 218, DateTimeKind.Local).AddTicks(6976));

            migrationBuilder.UpdateData(
                table: "Meldinger",
                keyColumn: "MeldingsId",
                keyValue: 20,
                column: "Tidsstempel",
                value: new DateTime(2024, 11, 14, 11, 34, 46, 218, DateTimeKind.Local).AddTicks(6977));

            migrationBuilder.UpdateData(
                table: "Rapport",
                keyColumn: "RapportId",
                keyValue: 1,
                column: "Opprettet",
                value: new DateTime(2024, 11, 14, 11, 34, 46, 218, DateTimeKind.Local).AddTicks(6775));

            migrationBuilder.UpdateData(
                table: "Rapport",
                keyColumn: "RapportId",
                keyValue: 2,
                column: "Opprettet",
                value: new DateTime(2024, 11, 14, 11, 34, 46, 218, DateTimeKind.Local).AddTicks(6825));

            migrationBuilder.UpdateData(
                table: "Rapport",
                keyColumn: "RapportId",
                keyValue: 3,
                column: "Opprettet",
                value: new DateTime(2024, 11, 14, 11, 34, 46, 218, DateTimeKind.Local).AddTicks(6827));

            migrationBuilder.UpdateData(
                table: "Rapport",
                keyColumn: "RapportId",
                keyValue: 4,
                column: "Opprettet",
                value: new DateTime(2024, 11, 14, 11, 34, 46, 218, DateTimeKind.Local).AddTicks(6829));

            migrationBuilder.UpdateData(
                table: "Rapport",
                keyColumn: "RapportId",
                keyValue: 5,
                column: "Opprettet",
                value: new DateTime(2024, 11, 14, 11, 34, 46, 218, DateTimeKind.Local).AddTicks(6830));

            migrationBuilder.UpdateData(
                table: "Rapport",
                keyColumn: "RapportId",
                keyValue: 6,
                column: "Opprettet",
                value: new DateTime(2024, 11, 14, 11, 34, 46, 218, DateTimeKind.Local).AddTicks(6831));

            migrationBuilder.UpdateData(
                table: "Rapport",
                keyColumn: "RapportId",
                keyValue: 7,
                column: "Opprettet",
                value: new DateTime(2024, 11, 14, 11, 34, 46, 218, DateTimeKind.Local).AddTicks(6832));

            migrationBuilder.UpdateData(
                table: "Rapport",
                keyColumn: "RapportId",
                keyValue: 8,
                column: "Opprettet",
                value: new DateTime(2024, 11, 14, 11, 34, 46, 218, DateTimeKind.Local).AddTicks(6833));

            migrationBuilder.UpdateData(
                table: "Rapport",
                keyColumn: "RapportId",
                keyValue: 9,
                column: "Opprettet",
                value: new DateTime(2024, 11, 14, 11, 34, 46, 218, DateTimeKind.Local).AddTicks(6834));

            migrationBuilder.UpdateData(
                table: "Rapport",
                keyColumn: "RapportId",
                keyValue: 10,
                column: "Opprettet",
                value: new DateTime(2024, 11, 14, 11, 34, 46, 218, DateTimeKind.Local).AddTicks(6836));
        }
    }
}
