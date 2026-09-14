CREATE OR ALTER PROCEDURE [dbo].[Calculator_StudentsFinalScores_EducationDocument]
    @StudentId INT,
    @UserId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    ;WITH ItemScores AS
    (
        SELECT
            SFSS.StudentId,
            FI.FormItemCategoryId,
            SFSS.FormItemId,
            FI.Title AS ItemTitle,

            CASE
                WHEN SUM(ISNULL(SFSS.Score, 0)) > FI.AllMaxScore
                    THEN FI.AllMaxScore
                ELSE SUM(ISNULL(SFSS.Score, 0))
            END AS FinalItemScore

        FROM StudentFormSubmissions AS SFSS

        INNER JOIN FormItems AS FI
            ON FI.Id = SFSS.FormItemId
            AND FI.IsActive = 1
            AND FI.IsDeleted = 0

        WHERE SFSS.StudentId = @StudentId
          AND SFSS.IsActive = 1
          AND SFSS.IsDeleted = 0

        GROUP BY
            SFSS.StudentId,
            SFSS.FormItemId,
            FI.AllMaxScore,
            FI.Title,
            FI.FormItemCategoryId
    ),

    RankedItems AS
    (
        SELECT
            IS1.StudentId,
            IS1.FormItemCategoryId,
            IS1.FormItemId,
            IS1.ItemTitle,
            IS1.FinalItemScore,

            ROW_NUMBER() OVER
            (
                PARTITION BY
                    IS1.StudentId,
                    IS1.FormItemCategoryId
                ORDER BY IS1.FinalItemScore DESC
            ) AS rn

        FROM ItemScores AS IS1
    ),

    SelectedItems AS
    (
        SELECT
            R.StudentId,
            R.FormItemCategoryId,
            R.FormItemId,
            R.FinalItemScore,
            R.rn,
            FIC.MaxCount,
            FIC.MaxScore

        FROM RankedItems AS R

        INNER JOIN FormItemCategories AS FIC
            ON FIC.Id = R.FormItemCategoryId
            AND FIC.IsActive = 1
            AND FIC.IsDeleted = 0
    ),

    CategorySums AS
    (
        SELECT
            SI.StudentId,
            SI.FormItemCategoryId,

            SUM
            (
                CASE
                    WHEN SI.MaxCount = 0
                        THEN SI.FinalItemScore

                    WHEN SI.rn <= SI.MaxCount
                        THEN SI.FinalItemScore

                    ELSE 0
                END
            ) AS CategorySum,

            MAX(SI.MaxScore) AS CategoryMaxScore

        FROM SelectedItems AS SI

        GROUP BY
            SI.StudentId,
            SI.FormItemCategoryId
    ),

    FinalCategoryScores AS
    (
        SELECT
            CS.StudentId,
            CS.FormItemCategoryId,

            CASE
                WHEN CS.CategorySum > CS.CategoryMaxScore
                    THEN CS.CategoryMaxScore
                ELSE CS.CategorySum
            END AS FinalCategoryScore

        FROM CategorySums AS CS
    )

    SELECT
        @StudentId AS StudentId,
        SUM(FCS.FinalCategoryScore) AS FinalTotalScore

    INTO #TempFinalScore

    FROM FinalCategoryScores AS FCS

    GROUP BY FCS.StudentId;


    IF EXISTS
    (
        SELECT 1
        FROM EducationDocumentScores
        WHERE StudentId = @StudentId
          AND IsActive = 1
          AND IsDeleted = 0
    )
    BEGIN

        UPDATE EDS
        SET FinalTotalScore = TFS.FinalTotalScore

        FROM EducationDocumentScores AS EDS

        INNER JOIN #TempFinalScore AS TFS
            ON EDS.StudentId = TFS.StudentId

        WHERE EDS.IsActive = 1
          AND EDS.IsDeleted = 0;

    END
    ELSE
    BEGIN

        INSERT INTO EducationDocumentScores
        (
            StudentId,
            FinalTotalScore,
            CreatedDate,
            IsActive,
            IsDeleted,
            CreatedBy
        )
        SELECT
            StudentId,
            FinalTotalScore,
            GETDATE(),
            1,
            0,
            @UserId
        FROM #TempFinalScore;

    END;

    SELECT *
    FROM EducationDocumentScores;

    DROP TABLE #TempFinalScore;
END