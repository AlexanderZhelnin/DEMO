using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Demo.Migrations;

public partial class start : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.EnsureSchema(
            name: "authors");

        migrationBuilder.CreateTable(
            name: "authors",
            schema: "authors",
            columns: table => new
            {
                id = table.Column<int>(type: "integer", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                name = table.Column<string>(type: "text", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_authors", x => x.id);
            });

        migrationBuilder.CreateTable(
            name: "publishers",
            schema: "authors",
            columns: table => new
            {
                id = table.Column<int>(type: "integer", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                name = table.Column<string>(type: "text", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_publishers", x => x.id);
            });

        migrationBuilder.CreateTable(
            name: "books",
            schema: "authors",
            columns: table => new
            {
                id = table.Column<int>(type: "integer", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                title = table.Column<string>(type: "text", nullable: true),
                description = table.Column<string>(type: "text", nullable: true),
                image_url = table.Column<string>(type: "text", nullable: true),
                author_id = table.Column<int>(type: "integer", nullable: false),
                isbn_10 = table.Column<string>(type: "text", nullable: true),
                isbn_13 = table.Column<string>(type: "text", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_books", x => x.id);
                table.ForeignKey(
                    name: "fk_books_authors_author_id",
                    column: x => x.author_id,
                    principalSchema: "authors",
                    principalTable: "authors",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "publishers_authors",
            schema: "authors",
            columns: table => new
            {
                author_id = table.Column<int>(type: "integer", nullable: false),
                publisher_id = table.Column<int>(type: "integer", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_publishers_authors", x => new { x.author_id, x.publisher_id });
                table.ForeignKey(
                    name: "fk_publishers_authors_authors_author_id",
                    column: x => x.author_id,
                    principalSchema: "authors",
                    principalTable: "authors",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "fk_publishers_authors_publishers_publisher_id",
                    column: x => x.publisher_id,
                    principalSchema: "authors",
                    principalTable: "publishers",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "book_details",
            schema: "authors",
            columns: table => new
            {
                book_id = table.Column<int>(type: "integer", nullable: false),
                rank = table.Column<double>(type: "double precision", nullable: false),
                status = table.Column<int>(type: "integer", nullable: false),
                peperback_cost = table.Column<decimal>(type: "numeric", nullable: false),
                hardcover_cost = table.Column<decimal>(type: "numeric", nullable: false),
                year = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                editor = table.Column<string>(type: "text", nullable: true),
                illustrator = table.Column<string>(type: "text", nullable: true),
                page_count = table.Column<int>(type: "integer", nullable: false),
                language = table.Column<int>(type: "integer", nullable: false),
                reading_age = table.Column<byte>(type: "smallint", nullable: false),
                genre = table.Column<int>(type: "integer", nullable: false),
                reviews = table.Column<int>(type: "integer", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_book_details", x => x.book_id);
                table.ForeignKey(
                    name: "fk_book_details_books_book_id",
                    column: x => x.book_id,
                    principalSchema: "authors",
                    principalTable: "books",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.InsertData(
            schema: "authors",
            table: "authors",
            columns: new[] { "id", "name" },
            values: new object[,]
            {
                { 1, "Вася" },
                { 2, "Петя" }
            });

        migrationBuilder.InsertData(
            schema: "authors",
            table: "publishers",
            columns: new[] { "id", "name" },
            values: new object[,]
            {
                { 1, "Издательство ООО \"Сервер\"" },
                { 2, "Издательство ООО \"Восток\"" }
            });

        migrationBuilder.InsertData(
            schema: "authors",
            table: "books",
            columns: new[] { "id", "author_id", "description", "isbn_10", "isbn_13", "image_url", "title" },
            values: new object[,]
            {
                { 3, 1, "Биографическое описание жизни", null, null, null, "Первая книга Васи" },
                { 4, 1, "Фантастическая книга о приключениях", null, null, null, "Вторая книга Васи" },
                { 5, 1, "Историческая книга", null, null, null, "Третья книга Васи" },
                { 6, 2, "Фентази об эльфах", null, null, null, "Первая книга Пети" },
                { 7, 2, "Научная литература, докозательство 3-й теорему Фихтенгольца", null, null, null, "Вторая книга Пети" },
                { 8, 2, "Научная фантастика и приключение героя в далёком космосе", null, null, null, "Третья книга Пети" }
            });

        migrationBuilder.InsertData(
            schema: "authors",
            table: "publishers_authors",
            columns: new[] { "author_id", "publisher_id" },
            values: new object[,]
            {
                { 1, 1 },
                { 2, 1 },
                { 1, 2 }
            });

        migrationBuilder.InsertData(
            schema: "authors",
            table: "book_details",
            columns: new[] { "book_id", "editor", "genre", "hardcover_cost", "illustrator", "language", "page_count", "peperback_cost", "rank", "reading_age", "reviews", "status", "year" },
            values: new object[,]
            {
                { 3, "Не известен", 4, 100m, "МИКЕЛАНДЖЕЛО", 0, 100, 0m, 7.1120000000000001, (byte)18, 0, 0, new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                { 4, "Не известен", 0, 110m, "ЙОХАННЕС ВЕРМЕЕР", 0, 200, 0m, 4.343, (byte)12, 0, 0, new DateTime(2001, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                { 5, "Не известен", 4, 120m, "ПАБЛО ПИКАССО", 0, 300, 0m, 9.1999999999999993, (byte)10, 0, 0, new DateTime(2002, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                { 6, "Не известен", 1, 130m, "ВИНСЕНТ ВАН ГОГ", 0, 400, 0m, 8.3450000000000006, (byte)5, 0, 0, new DateTime(2003, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                { 7, "Не известен", 3, 140m, "РЕМБРАНДТ ВАН РЕЙН", 1, 500, 0m, 1.0, (byte)10, 0, 0, new DateTime(2004, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                { 8, "Не известен", 0, 150m, "ЛЕОНАРДО ДА ВИНЧИ", 0, 600, 0m, 5.0, (byte)18, 0, 0, new DateTime(2005, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) }
            });

        migrationBuilder.CreateIndex(
            name: "ix_authors_name",
            schema: "authors",
            table: "authors",
            column: "name",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "ix_books_author_id",
            schema: "authors",
            table: "books",
            column: "author_id");

        migrationBuilder.CreateIndex(
            name: "ix_publishers_authors_publisher_id",
            schema: "authors",
            table: "publishers_authors",
            column: "publisher_id");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "book_details",
            schema: "authors");

        migrationBuilder.DropTable(
            name: "publishers_authors",
            schema: "authors");

        migrationBuilder.DropTable(
            name: "books",
            schema: "authors");

        migrationBuilder.DropTable(
            name: "publishers",
            schema: "authors");

        migrationBuilder.DropTable(
            name: "authors",
            schema: "authors");
    }
}
