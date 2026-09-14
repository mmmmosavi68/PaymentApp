using Microsoft.EntityFrameworkCore.Migrations;
using System.IO;
using System.Reflection;
using System;
using System.Linq;

namespace PaymentApp.Data.Migrations
{
    public partial class AddInitialazeViewAndStoreProc : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var assembly = Assembly.GetExecutingAssembly();

            var sqlFiles = assembly
                .GetManifestResourceNames()
                .Where(file => file.EndsWith(".sql", StringComparison.OrdinalIgnoreCase))
                .OrderBy(file => file);

            foreach (var sqlFile in sqlFiles)
            {
                using var stream = assembly.GetManifestResourceStream(sqlFile);

                if (stream == null)
                {
                    throw new InvalidOperationException(
                        $"Embedded resource '{sqlFile}' not found.");
                }

                using var reader = new StreamReader(stream);

                var sqlScript = reader.ReadToEnd();

                if (!string.IsNullOrWhiteSpace(sqlScript))
                {
                    migrationBuilder.Sql(sqlScript);
                }
            }
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                DROP PROCEDURE IF EXISTS [dbo].[Get_GroupAccess_With_CurrentUserAccess_For_TreeCheckBox];
                DROP PROCEDURE IF EXISTS [dbo].[Get_UserCurrentAccess];
                DROP PROCEDURE IF EXISTS [dbo].[Get_UserSelectedAccess_With_CurrentUserAccess_For_TreeCheckBox];
                DROP PROCEDURE IF EXISTS [dbo].[Calculator_StudentsFinalScores_EducationDocument];
            ");
        }
    }
}