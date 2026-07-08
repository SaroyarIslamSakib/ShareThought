using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DevSkill.Blog.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddGetBlogSP : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var sql = """
                CREATE OR ALTER PROCEDURE GetBlogPosts
                    @PageIndex INT,
                    @PageSize INT,
                    @OrderBy NVARCHAR(50) = 'Title ASC',
                    @Title NVARCHAR(250) = NULL,
                    @PublishFrom DATE = NULL,
                    @PublishTo DATE = NULL,
                    @Total INT OUTPUT,
                    @TotalDisplay INT OUTPUT
                AS
                BEGIN
                    DECLARE @sql NVARCHAR(2000);
                    DECLARE @paramList NVARCHAR(MAX);

                    DECLARE @countSql NVARCHAR(2000);
                    DECLARE @countParamList NVARCHAR(MAX);

                    /* Total data count */
                    SELECT @Total = COUNT(*) FROM BlogPosts;

                    /* Preparing count query */
                    SET @countSql = '
                        SELECT @xTotalDisplay = COUNT(*)
                        FROM BlogPosts
                        WHERE 1 = 1
                    ';

                    IF @Title IS NOT NULL
                        SET @countSql = @countSql + ' AND Title LIKE ''%'' + @xTitle + ''%''';

                    IF @PublishFrom IS NOT NULL
                        SET @countSql = @countSql + ' AND CreatedAt >= @xPublishFrom';

                    IF @PublishTo IS NOT NULL
                        SET @countSql = @countSql + ' AND CreatedAt < DATEADD(DAY, 1, @xPublishTo)';

                    /* Preparing main query */
                    SET @sql = '
                        SELECT *
                        FROM BlogPosts
                        WHERE 1 = 1
                    ';

                    IF @Title IS NOT NULL
                        SET @sql = @sql + ' AND Title LIKE ''%'' + @xTitle + ''%''';

                    IF @PublishFrom IS NOT NULL
                        SET @sql = @sql + ' AND CreatedAt >= @xPublishFrom';

                    IF @PublishTo IS NOT NULL
                        SET @sql = @sql + ' AND CreatedAt < DATEADD(DAY, 1, @xPublishTo)';

                    SET @sql = @sql + '
                        ORDER BY ' + @OrderBy + '
                        OFFSET @xPageSize * (@xPageIndex - 1) ROWS
                        FETCH NEXT @xPageSize ROWS ONLY
                    ';

                    /* Preparing count parameters */
                    SET @countParamList = '
                        @xTitle NVARCHAR(250),
                        @xPublishFrom DATE,
                        @xPublishTo DATE,
                        @xTotalDisplay INT OUTPUT
                    ';

                    /* Executing count sql */
                    EXEC sp_executesql
                        @countSql,
                        @countParamList,
                        @xTitle = @Title,
                        @xPublishFrom = @PublishFrom,
                        @xPublishTo = @PublishTo,
                        @xTotalDisplay = @TotalDisplay OUTPUT;

                    /* Preparing main sql parameters */
                    SET @paramList = '
                        @xTitle NVARCHAR(250),
                        @xPublishFrom DATE,
                        @xPublishTo DATE,
                        @xPageIndex INT,
                        @xPageSize INT
                    ';

                    /* Executing main sql */
                    EXEC sp_executesql
                        @sql,
                        @paramList,
                        @xTitle = @Title,
                        @xPublishFrom = @PublishFrom,
                        @xPublishTo = @PublishTo,
                        @xPageIndex = @PageIndex,
                        @xPageSize = @PageSize;

                    PRINT @countSql;
                    PRINT @sql;
                END
                GO
                
                
                """;

            migrationBuilder.Sql(sql);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            var sql = "DROP PROCEDURE [dbo].[GetBlogPosts]";

            migrationBuilder.Sql(sql);
        }
    }
}
