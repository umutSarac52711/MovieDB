using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MovieDB.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Awardables",
                columns: table => new
                {
                    Awardable_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Kind = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Awardables", x => x.Awardable_ID);
                });

            migrationBuilder.CreateTable(
                name: "ProductionCompanies",
                columns: table => new
                {
                    Company_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Country = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Founded_Year = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductionCompanies", x => x.Company_ID);
                });

            migrationBuilder.CreateTable(
                name: "Actors",
                columns: table => new
                {
                    Awardable_ID = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Birth_Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Nationality = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Actors", x => x.Awardable_ID);
                    table.ForeignKey(
                        name: "FK_Actors_Awardables_Awardable_ID",
                        column: x => x.Awardable_ID,
                        principalTable: "Awardables",
                        principalColumn: "Awardable_ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Directors",
                columns: table => new
                {
                    Awardable_ID = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Birth_Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Nationality = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Directors", x => x.Awardable_ID);
                    table.ForeignKey(
                        name: "FK_Directors_Awardables_Awardable_ID",
                        column: x => x.Awardable_ID,
                        principalTable: "Awardables",
                        principalColumn: "Awardable_ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Movies",
                columns: table => new
                {
                    Awardable_ID = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Release_Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Language = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Duration = table.Column<int>(type: "int", nullable: true),
                    Budget = table.Column<decimal>(type: "decimal(15,2)", nullable: true),
                    Revenue = table.Column<decimal>(type: "decimal(15,2)", nullable: true),
                    Rating = table.Column<decimal>(type: "decimal(3,1)", nullable: true),
                    PosterUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Movies", x => x.Awardable_ID);
                    table.ForeignKey(
                        name: "FK_Movies_Awardables_Awardable_ID",
                        column: x => x.Awardable_ID,
                        principalTable: "Awardables",
                        principalColumn: "Awardable_ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Awards",
                columns: table => new
                {
                    Award_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Event_Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Category = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Award_Year = table.Column<int>(type: "int", nullable: false),
                    Nominee_Awardable_ID = table.Column<int>(type: "int", nullable: false),
                    Movie_Context_ID = table.Column<int>(type: "int", nullable: true),
                    Nomination_Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Awardable_ID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Awards", x => x.Award_ID);
                    table.ForeignKey(
                        name: "FK_Awards_Awardables_Awardable_ID",
                        column: x => x.Awardable_ID,
                        principalTable: "Awardables",
                        principalColumn: "Awardable_ID");
                    table.ForeignKey(
                        name: "FK_Awards_Awardables_Nominee_Awardable_ID",
                        column: x => x.Nominee_Awardable_ID,
                        principalTable: "Awardables",
                        principalColumn: "Awardable_ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Awards_Movies_Movie_Context_ID",
                        column: x => x.Movie_Context_ID,
                        principalTable: "Movies",
                        principalColumn: "Awardable_ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MovieActors",
                columns: table => new
                {
                    MovieId = table.Column<int>(type: "int", nullable: false),
                    ActorId = table.Column<int>(type: "int", nullable: false),
                    Role = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MovieActors", x => new { x.MovieId, x.ActorId });
                    table.ForeignKey(
                        name: "FK_MovieActors_Actors_ActorId",
                        column: x => x.ActorId,
                        principalTable: "Actors",
                        principalColumn: "Awardable_ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MovieActors_Movies_MovieId",
                        column: x => x.MovieId,
                        principalTable: "Movies",
                        principalColumn: "Awardable_ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MovieCompanies",
                columns: table => new
                {
                    Movie_ID = table.Column<int>(type: "int", nullable: false),
                    Company_ID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MovieCompanies", x => new { x.Movie_ID, x.Company_ID });
                    table.ForeignKey(
                        name: "FK_MovieCompanies_Movies_Movie_ID",
                        column: x => x.Movie_ID,
                        principalTable: "Movies",
                        principalColumn: "Awardable_ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MovieCompanies_ProductionCompanies_Company_ID",
                        column: x => x.Company_ID,
                        principalTable: "ProductionCompanies",
                        principalColumn: "Company_ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MovieDirectors",
                columns: table => new
                {
                    MovieId = table.Column<int>(type: "int", nullable: false),
                    DirectorId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MovieDirectors", x => new { x.MovieId, x.DirectorId });
                    table.ForeignKey(
                        name: "FK_MovieDirectors_Directors_DirectorId",
                        column: x => x.DirectorId,
                        principalTable: "Directors",
                        principalColumn: "Awardable_ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MovieDirectors_Movies_MovieId",
                        column: x => x.MovieId,
                        principalTable: "Movies",
                        principalColumn: "Awardable_ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MovieGenres",
                columns: table => new
                {
                    Movie_ID = table.Column<int>(type: "int", nullable: false),
                    Genre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MovieGenres", x => new { x.Movie_ID, x.Genre });
                    table.ForeignKey(
                        name: "FK_MovieGenres_Movies_Movie_ID",
                        column: x => x.Movie_ID,
                        principalTable: "Movies",
                        principalColumn: "Awardable_ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Reviews",
                columns: table => new
                {
                    Review_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Movie_ID = table.Column<int>(type: "int", nullable: false),
                    Reviewer = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Rating = table.Column<int>(type: "int", nullable: true),
                    Comment_Text = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Date_Posted = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reviews", x => x.Review_ID);
                    table.ForeignKey(
                        name: "FK_Reviews_Movies_Movie_ID",
                        column: x => x.Movie_ID,
                        principalTable: "Movies",
                        principalColumn: "Awardable_ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Awards_Awardable_ID",
                table: "Awards",
                column: "Awardable_ID");

            migrationBuilder.CreateIndex(
                name: "IX_Awards_Movie_Context_ID",
                table: "Awards",
                column: "Movie_Context_ID");

            migrationBuilder.CreateIndex(
                name: "IX_Awards_Nominee_Awardable_ID",
                table: "Awards",
                column: "Nominee_Awardable_ID");

            migrationBuilder.CreateIndex(
                name: "IX_UniqueNomination",
                table: "Awards",
                columns: new[] { "Event_Name", "Category", "Award_Year", "Nominee_Awardable_ID", "Movie_Context_ID" },
                unique: true,
                filter: "[Movie_Context_ID] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_MovieActors_ActorId",
                table: "MovieActors",
                column: "ActorId");

            migrationBuilder.CreateIndex(
                name: "IX_MovieCompanies_Company_ID",
                table: "MovieCompanies",
                column: "Company_ID");

            migrationBuilder.CreateIndex(
                name: "IX_MovieDirectors_DirectorId",
                table: "MovieDirectors",
                column: "DirectorId");

            migrationBuilder.CreateIndex(
                name: "IX_Movies_Title_Release_Date",
                table: "Movies",
                columns: new[] { "Title", "Release_Date" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_Movie_ID",
                table: "Reviews",
                column: "Movie_ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Awards");

            migrationBuilder.DropTable(
                name: "MovieActors");

            migrationBuilder.DropTable(
                name: "MovieCompanies");

            migrationBuilder.DropTable(
                name: "MovieDirectors");

            migrationBuilder.DropTable(
                name: "MovieGenres");

            migrationBuilder.DropTable(
                name: "Reviews");

            migrationBuilder.DropTable(
                name: "Actors");

            migrationBuilder.DropTable(
                name: "ProductionCompanies");

            migrationBuilder.DropTable(
                name: "Directors");

            migrationBuilder.DropTable(
                name: "Movies");

            migrationBuilder.DropTable(
                name: "Awardables");
        }
    }
}
