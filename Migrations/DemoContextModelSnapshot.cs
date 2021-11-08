﻿// <auto-generated />
using System;
using Demo.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Demo.Migrations
{
    [DbContext(typeof(DemoContext))]
    partial class DemoContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasDefaultSchema("authors")
                .HasAnnotation("Relational:MaxIdentifierLength", 63)
                .HasAnnotation("ProductVersion", "5.0.11")
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            modelBuilder.Entity("Demo.Model.Author", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("Name")
                        .HasColumnType("text")
                        .HasColumnName("name");

                    b.HasKey("Id")
                        .HasName("pk_authors");

                    b.HasIndex("Name")
                        .IsUnique()
                        .HasDatabaseName("ix_authors_name");

                    b.ToTable("authors");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Name = "Вася"
                        },
                        new
                        {
                            Id = 2,
                            Name = "Петя"
                        });
                });

            modelBuilder.Entity("Demo.Model.Book", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<int>("AuthorId")
                        .HasColumnType("integer")
                        .HasColumnName("author_id");

                    b.Property<string>("Description")
                        .HasColumnType("text")
                        .HasColumnName("description");

                    b.Property<string>("ISBN_10")
                        .HasColumnType("text")
                        .HasColumnName("isbn_10");

                    b.Property<string>("ISBN_13")
                        .HasColumnType("text")
                        .HasColumnName("isbn_13");

                    b.Property<string>("ImageUrl")
                        .HasColumnType("text")
                        .HasColumnName("image_url");

                    b.Property<string>("Title")
                        .HasColumnType("text")
                        .HasColumnName("title");

                    b.HasKey("Id")
                        .HasName("pk_books");

                    b.HasIndex("AuthorId")
                        .HasDatabaseName("ix_books_author_id");

                    b.ToTable("books");

                    b.HasData(
                        new
                        {
                            Id = 3,
                            AuthorId = 1,
                            Description = "Биографическое описание жизни",
                            Title = "Первая книга Васи"
                        },
                        new
                        {
                            Id = 4,
                            AuthorId = 1,
                            Description = "Фантастическая книга о приключениях",
                            Title = "Вторая книга Васи"
                        },
                        new
                        {
                            Id = 5,
                            AuthorId = 1,
                            Description = "Историческая книга",
                            Title = "Третья книга Васи"
                        },
                        new
                        {
                            Id = 6,
                            AuthorId = 2,
                            Description = "Фентази об эльфах",
                            Title = "Первая книга Пети"
                        },
                        new
                        {
                            Id = 7,
                            AuthorId = 2,
                            Description = "Научная литература, докозательство 3-й теорему Фихтенгольца",
                            Title = "Вторая книга Пети"
                        },
                        new
                        {
                            Id = 8,
                            AuthorId = 2,
                            Description = "Научная фантастика и приключение героя в далёком космосе",
                            Title = "Третья книга Пети"
                        });
                });

            modelBuilder.Entity("Demo.Model.BookDetails", b =>
                {
                    b.Property<int>("BookId")
                        .HasColumnType("integer")
                        .HasColumnName("book_id");

                    b.Property<string>("Editor")
                        .HasColumnType("text")
                        .HasColumnName("editor");

                    b.Property<int>("Genre")
                        .HasColumnType("integer")
                        .HasColumnName("genre");

                    b.Property<decimal>("HardcoverCost")
                        .HasColumnType("numeric")
                        .HasColumnName("hardcover_cost");

                    b.Property<string>("Illustrator")
                        .HasColumnType("text")
                        .HasColumnName("illustrator");

                    b.Property<int>("Language")
                        .HasColumnType("integer")
                        .HasColumnName("language");

                    b.Property<int>("PageCount")
                        .HasColumnType("integer")
                        .HasColumnName("page_count");

                    b.Property<decimal>("PeperbackCost")
                        .HasColumnType("numeric")
                        .HasColumnName("peperback_cost");

                    b.Property<double>("Rank")
                        .HasColumnType("double precision")
                        .HasColumnName("rank");

                    b.Property<byte>("ReadingAge")
                        .HasColumnType("smallint")
                        .HasColumnName("reading_age");

                    b.Property<int>("Reviews")
                        .HasColumnType("integer")
                        .HasColumnName("reviews");

                    b.Property<int>("Status")
                        .HasColumnType("integer")
                        .HasColumnName("status");

                    b.Property<DateTime>("Year")
                        .HasColumnType("timestamp without time zone")
                        .HasColumnName("year");

                    b.HasKey("BookId")
                        .HasName("pk_book_details");

                    b.ToTable("book_details");

                    b.HasData(
                        new
                        {
                            BookId = 3,
                            Editor = "Не известен",
                            Genre = 4,
                            HardcoverCost = 100m,
                            Illustrator = "МИКЕЛАНДЖЕЛО",
                            Language = 0,
                            PageCount = 100,
                            PeperbackCost = 0m,
                            Rank = 7.1120000000000001,
                            ReadingAge = (byte)18,
                            Reviews = 0,
                            Status = 0,
                            Year = new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified)
                        },
                        new
                        {
                            BookId = 4,
                            Editor = "Не известен",
                            Genre = 0,
                            HardcoverCost = 110m,
                            Illustrator = "ЙОХАННЕС ВЕРМЕЕР",
                            Language = 0,
                            PageCount = 200,
                            PeperbackCost = 0m,
                            Rank = 4.343,
                            ReadingAge = (byte)12,
                            Reviews = 0,
                            Status = 0,
                            Year = new DateTime(2001, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified)
                        },
                        new
                        {
                            BookId = 5,
                            Editor = "Не известен",
                            Genre = 4,
                            HardcoverCost = 120m,
                            Illustrator = "ПАБЛО ПИКАССО",
                            Language = 0,
                            PageCount = 300,
                            PeperbackCost = 0m,
                            Rank = 9.1999999999999993,
                            ReadingAge = (byte)10,
                            Reviews = 0,
                            Status = 0,
                            Year = new DateTime(2002, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified)
                        },
                        new
                        {
                            BookId = 6,
                            Editor = "Не известен",
                            Genre = 1,
                            HardcoverCost = 130m,
                            Illustrator = "ВИНСЕНТ ВАН ГОГ",
                            Language = 0,
                            PageCount = 400,
                            PeperbackCost = 0m,
                            Rank = 8.3450000000000006,
                            ReadingAge = (byte)5,
                            Reviews = 0,
                            Status = 0,
                            Year = new DateTime(2003, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified)
                        },
                        new
                        {
                            BookId = 7,
                            Editor = "Не известен",
                            Genre = 3,
                            HardcoverCost = 140m,
                            Illustrator = "РЕМБРАНДТ ВАН РЕЙН",
                            Language = 1,
                            PageCount = 500,
                            PeperbackCost = 0m,
                            Rank = 1.0,
                            ReadingAge = (byte)10,
                            Reviews = 0,
                            Status = 0,
                            Year = new DateTime(2004, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified)
                        },
                        new
                        {
                            BookId = 8,
                            Editor = "Не известен",
                            Genre = 0,
                            HardcoverCost = 150m,
                            Illustrator = "ЛЕОНАРДО ДА ВИНЧИ",
                            Language = 0,
                            PageCount = 600,
                            PeperbackCost = 0m,
                            Rank = 5.0,
                            ReadingAge = (byte)18,
                            Reviews = 0,
                            Status = 0,
                            Year = new DateTime(2005, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified)
                        });
                });

            modelBuilder.Entity("Demo.Model.Publisher", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("Name")
                        .HasColumnType("text")
                        .HasColumnName("name");

                    b.HasKey("Id")
                        .HasName("pk_publishers");

                    b.ToTable("publishers");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Name = "Издательство ООО \"Сервер\""
                        },
                        new
                        {
                            Id = 2,
                            Name = "Издательство ООО \"Восток\""
                        });
                });

            modelBuilder.Entity("Demo.Model.PublishersAuthors", b =>
                {
                    b.Property<int>("AuthorId")
                        .HasColumnType("integer")
                        .HasColumnName("author_id");

                    b.Property<int>("PublisherId")
                        .HasColumnType("integer")
                        .HasColumnName("publisher_id");

                    b.HasKey("AuthorId", "PublisherId")
                        .HasName("pk_publishers_authors");

                    b.HasIndex("PublisherId")
                        .HasDatabaseName("ix_publishers_authors_publisher_id");

                    b.ToTable("publishers_authors");

                    b.HasData(
                        new
                        {
                            AuthorId = 1,
                            PublisherId = 1
                        },
                        new
                        {
                            AuthorId = 2,
                            PublisherId = 1
                        },
                        new
                        {
                            AuthorId = 1,
                            PublisherId = 2
                        });
                });

            modelBuilder.Entity("Demo.Model.Book", b =>
                {
                    b.HasOne("Demo.Model.Author", "Author")
                        .WithMany("Books")
                        .HasForeignKey("AuthorId")
                        .HasConstraintName("fk_books_authors_author_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Author");
                });

            modelBuilder.Entity("Demo.Model.BookDetails", b =>
                {
                    b.HasOne("Demo.Model.Book", "Book")
                        .WithOne("Detatils")
                        .HasForeignKey("Demo.Model.BookDetails", "BookId")
                        .HasConstraintName("fk_book_details_books_book_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Book");
                });

            modelBuilder.Entity("Demo.Model.PublishersAuthors", b =>
                {
                    b.HasOne("Demo.Model.Author", "Author")
                        .WithMany("PublishersAuthors")
                        .HasForeignKey("AuthorId")
                        .HasConstraintName("fk_publishers_authors_authors_author_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Demo.Model.Publisher", "Publisher")
                        .WithMany("PublishersAuthors")
                        .HasForeignKey("PublisherId")
                        .HasConstraintName("fk_publishers_authors_publishers_publisher_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Author");

                    b.Navigation("Publisher");
                });

            modelBuilder.Entity("Demo.Model.Author", b =>
                {
                    b.Navigation("Books");

                    b.Navigation("PublishersAuthors");
                });

            modelBuilder.Entity("Demo.Model.Book", b =>
                {
                    b.Navigation("Detatils");
                });

            modelBuilder.Entity("Demo.Model.Publisher", b =>
                {
                    b.Navigation("PublishersAuthors");
                });
#pragma warning restore 612, 618
        }
    }
}
