using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project1.Data
{
    public static class ExtraMigration
    {
        public static void Steps(MigrationBuilder migrationBuilder)
        {
            //Artwork Table Triggers for Concurrency
            migrationBuilder.Sql(
                @"
                    CREATE TRIGGER SetArtworkTimestampOnUpdate
                    AFTER UPDATE ON Artworks
                    BEGIN
                        UPDATE Artworks
                        SET RowVersion = randomblob(8)
                        WHERE rowid = NEW.rowid;
                    END
                ");
            migrationBuilder.Sql(
                @"
                    CREATE TRIGGER SetArtworkTimestampOnInsert
                    AFTER INSERT ON Artworks
                    BEGIN
                        UPDATE Artworks
                        SET RowVersion = randomblob(8)
                        WHERE rowid = NEW.rowid;
                    END
                ");

            //ArtType Table Triggers for Concurrency
            migrationBuilder.Sql(
                @"
                    CREATE TRIGGER SetArtTypeTimestampOnUpdate
                    AFTER UPDATE ON ArtTypes
                    BEGIN
                        UPDATE ArtTypes
                        SET RowVersion = randomblob(8)
                        WHERE rowid = NEW.rowid;
                    END
                ");
            migrationBuilder.Sql(
                @"
                    CREATE TRIGGER SetArtTypeTimestampOnInsert
                    AFTER INSERT ON ArtTypes
                    BEGIN
                        UPDATE ArtTypes
                        SET RowVersion = randomblob(8)
                        WHERE rowid = NEW.rowid;
                    END
                ");
        }
    }
}
