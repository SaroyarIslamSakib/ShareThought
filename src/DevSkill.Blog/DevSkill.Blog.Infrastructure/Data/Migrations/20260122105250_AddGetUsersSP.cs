using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DevSkill.Blog.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddGetUsersSP : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var sql = """
                CREATE OR ALTER PROCEDURE [dbo].[GetUsers]
                    @PageIndex INT,
                    @PageSize INT,
                    @OrderBy NVARCHAR(100) = 'RegistrationDate DESC',
                    @Name NVARCHAR(250) = NULL,
                    @RegistrationFrom DATE = NULL,
                    @RegistrationTo DATE = NULL,
                    @Total INT OUTPUT,
                    @TotalDisplay INT OUTPUT
                AS
                BEGIN
                    SET NOCOUNT ON;

                    DECLARE @sql NVARCHAR(MAX);
                    DECLARE @paramList NVARCHAR(MAX);

                    DECLARE @countSql NVARCHAR(MAX);
                    DECLARE @countParamList NVARCHAR(MAX);

                    IF (@OrderBy = 'Name ASC')
                        SET @OrderBy = 'U.FirstName ASC, U.LastName ASC';
                    ELSE IF (@OrderBy = 'Name DESC')
                        SET @OrderBy = 'U.FirstName DESC, U.LastName DESC';
                    ELSE IF (@OrderBy = 'Email ASC')
                        SET @OrderBy = 'U.Email ASC';
                    ELSE IF (@OrderBy = 'Email DESC')
                        SET @OrderBy = 'U.Email DESC';
                    ELSE IF (@OrderBy = 'RegistrationDate ASC')
                        SET @OrderBy = 'U.RegistrationDate ASC';
                    ELSE IF (@OrderBy = 'RegistrationDate DESC')
                        SET @OrderBy = 'U.RegistrationDate DESC';
                    ELSE IF (@OrderBy = 'Role ASC')
                        SET @OrderBy = 'MIN(R.Name) ASC';
                    ELSE IF (@OrderBy = 'Role DESC')
                        SET @OrderBy = 'MIN(R.Name) DESC';
                    ELSE
                        SET @OrderBy = 'U.RegistrationDate DESC';


                    SELECT @Total = COUNT(*) FROM AspNetUsers;

                    SET @countSql = '
                        SELECT @xTotalDisplay = COUNT(DISTINCT U.Id)
                        FROM AspNetUsers U
                        LEFT JOIN AspNetUserRoles UR ON U.Id = UR.UserId
                        LEFT JOIN AspNetRoles R ON UR.RoleId = R.Id
                        WHERE 1 = 1
                    ';

                    IF @Name IS NOT NULL
                        SET @countSql += '
                            AND (U.FirstName + '' '' + U.LastName) LIKE ''%'' + @xName + ''%''
                        ';

                    IF @RegistrationFrom IS NOT NULL
                        SET @countSql += '
                            AND U.RegistrationDate >= @xRegistrationFrom
                        ';

                    IF @RegistrationTo IS NOT NULL
                        SET @countSql += '
                            AND U.RegistrationDate < DATEADD(DAY, 1, @xRegistrationTo)
                        ';

                    SET @countParamList = '
                        @xName NVARCHAR(250),
                        @xRegistrationFrom DATE,
                        @xRegistrationTo DATE,
                        @xTotalDisplay INT OUTPUT
                    ';

                    EXEC sp_executesql
                        @countSql,
                        @countParamList,
                        @xName = @Name,
                        @xRegistrationFrom = @RegistrationFrom,
                        @xRegistrationTo = @RegistrationTo,
                        @xTotalDisplay = @TotalDisplay OUTPUT;

                    SET @sql = '
                        SELECT
                            U.Id,
                            (U.FirstName + '' '' + U.LastName) AS FullName,
                            U.Email,
                            U.PhoneNumber,
                            ISNULL(STRING_AGG(R.Name, '', ''), '''') AS Role,
                            U.RegistrationDate
                        FROM AspNetUsers U
                        LEFT JOIN AspNetUserRoles UR ON U.Id = UR.UserId
                        LEFT JOIN AspNetRoles R ON UR.RoleId = R.Id
                        WHERE 1 = 1
                    ';

                    IF @Name IS NOT NULL
                        SET @sql += '
                            AND (U.FirstName + '' '' + U.LastName) LIKE ''%'' + @xName + ''%''
                        ';

                    IF @RegistrationFrom IS NOT NULL
                        SET @sql += '
                            AND U.RegistrationDate >= @xRegistrationFrom
                        ';

                    IF @RegistrationTo IS NOT NULL
                        SET @sql += '
                            AND U.RegistrationDate < DATEADD(DAY, 1, @xRegistrationTo)
                        ';

                    SET @sql += '
                        GROUP BY
                            U.Id,
                            U.FirstName,
                            U.LastName,
                            U.Email,
                            U.PhoneNumber,
                            U.RegistrationDate
                        ORDER BY ' + @OrderBy + '
                        OFFSET @xPageSize * (@xPageIndex - 1) ROWS
                        FETCH NEXT @xPageSize ROWS ONLY
                    ';

                    SET @paramList = '
                        @xName NVARCHAR(250),
                        @xRegistrationFrom DATE,
                        @xRegistrationTo DATE,
                        @xPageIndex INT,
                        @xPageSize INT
                    ';

                    EXEC sp_executesql
                        @sql,
                        @paramList,
                        @xName = @Name,
                        @xRegistrationFrom = @RegistrationFrom,
                        @xRegistrationTo = @RegistrationTo,
                        @xPageIndex = @PageIndex,
                        @xPageSize = @PageSize;

                    PRINT @countSql;
                    PRINT @sql;
                END
                
                """;

            migrationBuilder.Sql(sql);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            var sql = "DROP PROCEDURE [dbo].[GetUsers]";

            migrationBuilder.Sql(sql);
        }
    }
}
