using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KartverketWebApp.Migrations
{
    /// <inheritdoc />
    public partial class MakeTildelAnsattIdNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "TildelAnsattId",
                table: "Rapport",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.UpdateData(
                table: "Meldinger",
                keyColumn: "MeldingsId",
                keyValue: 1,
                column: "Tidsstempel",
                value: new DateTime(2024, 11, 25, 10, 52, 43, 785, DateTimeKind.Local).AddTicks(4817));

            migrationBuilder.UpdateData(
                table: "Meldinger",
                keyColumn: "MeldingsId",
                keyValue: 2,
                column: "Tidsstempel",
                value: new DateTime(2024, 11, 25, 10, 52, 43, 785, DateTimeKind.Local).AddTicks(4823));

            migrationBuilder.UpdateData(
                table: "Meldinger",
                keyColumn: "MeldingsId",
                keyValue: 3,
                column: "Tidsstempel",
                value: new DateTime(2024, 11, 25, 10, 52, 43, 785, DateTimeKind.Local).AddTicks(4828));

            migrationBuilder.UpdateData(
                table: "Meldinger",
                keyColumn: "MeldingsId",
                keyValue: 4,
                column: "Tidsstempel",
                value: new DateTime(2024, 11, 25, 10, 52, 43, 785, DateTimeKind.Local).AddTicks(4832));

            migrationBuilder.UpdateData(
                table: "Meldinger",
                keyColumn: "MeldingsId",
                keyValue: 5,
                column: "Tidsstempel",
                value: new DateTime(2024, 11, 25, 10, 52, 43, 785, DateTimeKind.Local).AddTicks(4836));

            migrationBuilder.UpdateData(
                table: "Meldinger",
                keyColumn: "MeldingsId",
                keyValue: 6,
                column: "Tidsstempel",
                value: new DateTime(2024, 11, 25, 10, 52, 43, 785, DateTimeKind.Local).AddTicks(4840));

            migrationBuilder.UpdateData(
                table: "Meldinger",
                keyColumn: "MeldingsId",
                keyValue: 7,
                column: "Tidsstempel",
                value: new DateTime(2024, 11, 24, 10, 52, 43, 785, DateTimeKind.Local).AddTicks(4844));

            migrationBuilder.UpdateData(
                table: "Meldinger",
                keyColumn: "MeldingsId",
                keyValue: 8,
                column: "Tidsstempel",
                value: new DateTime(2024, 11, 25, 10, 52, 43, 785, DateTimeKind.Local).AddTicks(4851));

            migrationBuilder.UpdateData(
                table: "Meldinger",
                keyColumn: "MeldingsId",
                keyValue: 9,
                column: "Tidsstempel",
                value: new DateTime(2024, 11, 25, 10, 52, 43, 785, DateTimeKind.Local).AddTicks(4855));

            migrationBuilder.UpdateData(
                table: "Meldinger",
                keyColumn: "MeldingsId",
                keyValue: 10,
                column: "Tidsstempel",
                value: new DateTime(2024, 11, 25, 10, 52, 43, 785, DateTimeKind.Local).AddTicks(4859));

            migrationBuilder.UpdateData(
                table: "Meldinger",
                keyColumn: "MeldingsId",
                keyValue: 11,
                column: "Tidsstempel",
                value: new DateTime(2024, 11, 25, 10, 52, 43, 785, DateTimeKind.Local).AddTicks(4863));

            migrationBuilder.UpdateData(
                table: "Meldinger",
                keyColumn: "MeldingsId",
                keyValue: 12,
                column: "Tidsstempel",
                value: new DateTime(2024, 11, 25, 10, 52, 43, 785, DateTimeKind.Local).AddTicks(4867));

            migrationBuilder.UpdateData(
                table: "Meldinger",
                keyColumn: "MeldingsId",
                keyValue: 13,
                column: "Tidsstempel",
                value: new DateTime(2024, 11, 25, 10, 52, 43, 785, DateTimeKind.Local).AddTicks(4871));

            migrationBuilder.UpdateData(
                table: "Meldinger",
                keyColumn: "MeldingsId",
                keyValue: 14,
                column: "Tidsstempel",
                value: new DateTime(2024, 11, 25, 10, 52, 43, 785, DateTimeKind.Local).AddTicks(4876));

            migrationBuilder.UpdateData(
                table: "Meldinger",
                keyColumn: "MeldingsId",
                keyValue: 15,
                column: "Tidsstempel",
                value: new DateTime(2024, 11, 25, 10, 52, 43, 785, DateTimeKind.Local).AddTicks(4880));

            migrationBuilder.UpdateData(
                table: "Meldinger",
                keyColumn: "MeldingsId",
                keyValue: 16,
                column: "Tidsstempel",
                value: new DateTime(2024, 11, 25, 10, 52, 43, 785, DateTimeKind.Local).AddTicks(4883));

            migrationBuilder.UpdateData(
                table: "Meldinger",
                keyColumn: "MeldingsId",
                keyValue: 17,
                column: "Tidsstempel",
                value: new DateTime(2024, 11, 25, 10, 52, 43, 785, DateTimeKind.Local).AddTicks(4887));

            migrationBuilder.UpdateData(
                table: "Meldinger",
                keyColumn: "MeldingsId",
                keyValue: 18,
                column: "Tidsstempel",
                value: new DateTime(2024, 11, 25, 10, 52, 43, 785, DateTimeKind.Local).AddTicks(4891));

            migrationBuilder.UpdateData(
                table: "Meldinger",
                keyColumn: "MeldingsId",
                keyValue: 19,
                column: "Tidsstempel",
                value: new DateTime(2024, 11, 25, 10, 52, 43, 785, DateTimeKind.Local).AddTicks(4895));

            migrationBuilder.UpdateData(
                table: "Meldinger",
                keyColumn: "MeldingsId",
                keyValue: 20,
                column: "Tidsstempel",
                value: new DateTime(2024, 11, 25, 10, 52, 43, 785, DateTimeKind.Local).AddTicks(4899));

            migrationBuilder.UpdateData(
                table: "Rapport",
                keyColumn: "RapportId",
                keyValue: 1,
                column: "Opprettet",
                value: new DateTime(2024, 11, 25, 10, 52, 43, 785, DateTimeKind.Local).AddTicks(4453));

            migrationBuilder.UpdateData(
                table: "Rapport",
                keyColumn: "RapportId",
                keyValue: 2,
                column: "Opprettet",
                value: new DateTime(2024, 11, 25, 10, 52, 43, 785, DateTimeKind.Local).AddTicks(4549));

            migrationBuilder.UpdateData(
                table: "Rapport",
                keyColumn: "RapportId",
                keyValue: 4,
                column: "Opprettet",
                value: new DateTime(2024, 11, 25, 10, 52, 43, 785, DateTimeKind.Local).AddTicks(4563));

            migrationBuilder.UpdateData(
                table: "Rapport",
                keyColumn: "RapportId",
                keyValue: 5,
                column: "Opprettet",
                value: new DateTime(2024, 11, 25, 10, 52, 43, 785, DateTimeKind.Local).AddTicks(4569));

            migrationBuilder.UpdateData(
                table: "Rapport",
                keyColumn: "RapportId",
                keyValue: 7,
                column: "Opprettet",
                value: new DateTime(2024, 11, 25, 10, 52, 43, 785, DateTimeKind.Local).AddTicks(4576));

            migrationBuilder.UpdateData(
                table: "Rapport",
                keyColumn: "RapportId",
                keyValue: 8,
                column: "Opprettet",
                value: new DateTime(2024, 11, 25, 10, 52, 43, 785, DateTimeKind.Local).AddTicks(4581));

            migrationBuilder.UpdateData(
                table: "Rapport",
                keyColumn: "RapportId",
                keyValue: 10,
                column: "Opprettet",
                value: new DateTime(2024, 11, 25, 10, 52, 43, 785, DateTimeKind.Local).AddTicks(4587));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "TildelAnsattId",
                table: "Rapport",
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
                value: new DateTime(2024, 11, 25, 1, 15, 11, 855, DateTimeKind.Local).AddTicks(8109));

            migrationBuilder.UpdateData(
                table: "Meldinger",
                keyColumn: "MeldingsId",
                keyValue: 2,
                column: "Tidsstempel",
                value: new DateTime(2024, 11, 25, 1, 15, 11, 855, DateTimeKind.Local).AddTicks(8112));

            migrationBuilder.UpdateData(
                table: "Meldinger",
                keyColumn: "MeldingsId",
                keyValue: 3,
                column: "Tidsstempel",
                value: new DateTime(2024, 11, 25, 1, 15, 11, 855, DateTimeKind.Local).AddTicks(8114));

            migrationBuilder.UpdateData(
                table: "Meldinger",
                keyColumn: "MeldingsId",
                keyValue: 4,
                column: "Tidsstempel",
                value: new DateTime(2024, 11, 25, 1, 15, 11, 855, DateTimeKind.Local).AddTicks(8116));

            migrationBuilder.UpdateData(
                table: "Meldinger",
                keyColumn: "MeldingsId",
                keyValue: 5,
                column: "Tidsstempel",
                value: new DateTime(2024, 11, 25, 1, 15, 11, 855, DateTimeKind.Local).AddTicks(8117));

            migrationBuilder.UpdateData(
                table: "Meldinger",
                keyColumn: "MeldingsId",
                keyValue: 6,
                column: "Tidsstempel",
                value: new DateTime(2024, 11, 25, 1, 15, 11, 855, DateTimeKind.Local).AddTicks(8119));

            migrationBuilder.UpdateData(
                table: "Meldinger",
                keyColumn: "MeldingsId",
                keyValue: 7,
                column: "Tidsstempel",
                value: new DateTime(2024, 11, 24, 1, 15, 11, 855, DateTimeKind.Local).AddTicks(8121));

            migrationBuilder.UpdateData(
                table: "Meldinger",
                keyColumn: "MeldingsId",
                keyValue: 8,
                column: "Tidsstempel",
                value: new DateTime(2024, 11, 25, 1, 15, 11, 855, DateTimeKind.Local).AddTicks(8124));

            migrationBuilder.UpdateData(
                table: "Meldinger",
                keyColumn: "MeldingsId",
                keyValue: 9,
                column: "Tidsstempel",
                value: new DateTime(2024, 11, 25, 1, 15, 11, 855, DateTimeKind.Local).AddTicks(8126));

            migrationBuilder.UpdateData(
                table: "Meldinger",
                keyColumn: "MeldingsId",
                keyValue: 10,
                column: "Tidsstempel",
                value: new DateTime(2024, 11, 25, 1, 15, 11, 855, DateTimeKind.Local).AddTicks(8127));

            migrationBuilder.UpdateData(
                table: "Meldinger",
                keyColumn: "MeldingsId",
                keyValue: 11,
                column: "Tidsstempel",
                value: new DateTime(2024, 11, 25, 1, 15, 11, 855, DateTimeKind.Local).AddTicks(8129));

            migrationBuilder.UpdateData(
                table: "Meldinger",
                keyColumn: "MeldingsId",
                keyValue: 12,
                column: "Tidsstempel",
                value: new DateTime(2024, 11, 25, 1, 15, 11, 855, DateTimeKind.Local).AddTicks(8131));

            migrationBuilder.UpdateData(
                table: "Meldinger",
                keyColumn: "MeldingsId",
                keyValue: 13,
                column: "Tidsstempel",
                value: new DateTime(2024, 11, 25, 1, 15, 11, 855, DateTimeKind.Local).AddTicks(8133));

            migrationBuilder.UpdateData(
                table: "Meldinger",
                keyColumn: "MeldingsId",
                keyValue: 14,
                column: "Tidsstempel",
                value: new DateTime(2024, 11, 25, 1, 15, 11, 855, DateTimeKind.Local).AddTicks(8135));

            migrationBuilder.UpdateData(
                table: "Meldinger",
                keyColumn: "MeldingsId",
                keyValue: 15,
                column: "Tidsstempel",
                value: new DateTime(2024, 11, 25, 1, 15, 11, 855, DateTimeKind.Local).AddTicks(8136));

            migrationBuilder.UpdateData(
                table: "Meldinger",
                keyColumn: "MeldingsId",
                keyValue: 16,
                column: "Tidsstempel",
                value: new DateTime(2024, 11, 25, 1, 15, 11, 855, DateTimeKind.Local).AddTicks(8138));

            migrationBuilder.UpdateData(
                table: "Meldinger",
                keyColumn: "MeldingsId",
                keyValue: 17,
                column: "Tidsstempel",
                value: new DateTime(2024, 11, 25, 1, 15, 11, 855, DateTimeKind.Local).AddTicks(8140));

            migrationBuilder.UpdateData(
                table: "Meldinger",
                keyColumn: "MeldingsId",
                keyValue: 18,
                column: "Tidsstempel",
                value: new DateTime(2024, 11, 25, 1, 15, 11, 855, DateTimeKind.Local).AddTicks(8142));

            migrationBuilder.UpdateData(
                table: "Meldinger",
                keyColumn: "MeldingsId",
                keyValue: 19,
                column: "Tidsstempel",
                value: new DateTime(2024, 11, 25, 1, 15, 11, 855, DateTimeKind.Local).AddTicks(8144));

            migrationBuilder.UpdateData(
                table: "Meldinger",
                keyColumn: "MeldingsId",
                keyValue: 20,
                column: "Tidsstempel",
                value: new DateTime(2024, 11, 25, 1, 15, 11, 855, DateTimeKind.Local).AddTicks(8145));

            migrationBuilder.UpdateData(
                table: "Rapport",
                keyColumn: "RapportId",
                keyValue: 1,
                column: "Opprettet",
                value: new DateTime(2024, 11, 25, 1, 15, 11, 855, DateTimeKind.Local).AddTicks(7948));

            migrationBuilder.UpdateData(
                table: "Rapport",
                keyColumn: "RapportId",
                keyValue: 2,
                column: "Opprettet",
                value: new DateTime(2024, 11, 25, 1, 15, 11, 855, DateTimeKind.Local).AddTicks(7993));

            migrationBuilder.UpdateData(
                table: "Rapport",
                keyColumn: "RapportId",
                keyValue: 4,
                column: "Opprettet",
                value: new DateTime(2024, 11, 25, 1, 15, 11, 855, DateTimeKind.Local).AddTicks(7998));

            migrationBuilder.UpdateData(
                table: "Rapport",
                keyColumn: "RapportId",
                keyValue: 5,
                column: "Opprettet",
                value: new DateTime(2024, 11, 25, 1, 15, 11, 855, DateTimeKind.Local).AddTicks(7999));

            migrationBuilder.UpdateData(
                table: "Rapport",
                keyColumn: "RapportId",
                keyValue: 7,
                column: "Opprettet",
                value: new DateTime(2024, 11, 25, 1, 15, 11, 855, DateTimeKind.Local).AddTicks(8001));

            migrationBuilder.UpdateData(
                table: "Rapport",
                keyColumn: "RapportId",
                keyValue: 8,
                column: "Opprettet",
                value: new DateTime(2024, 11, 25, 1, 15, 11, 855, DateTimeKind.Local).AddTicks(8003));

            migrationBuilder.UpdateData(
                table: "Rapport",
                keyColumn: "RapportId",
                keyValue: 10,
                column: "Opprettet",
                value: new DateTime(2024, 11, 25, 1, 15, 11, 855, DateTimeKind.Local).AddTicks(8005));
        }
    }
}
