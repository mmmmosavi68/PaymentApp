CREATE OR ALTER PROCEDURE [dbo].[Get_UserCurrentAccess]
    @CurrentUserId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    SELECT DISTINCT
        M.Id AS DISTINCT_MenuID,
        M.Id,
        @CurrentUserId AS UserId,
        M.MenuTitle,
        M.ParentId,
        M.PageCode,
        M.ShowMenu,
        M.CssClass,
        M.Icon,
        M.Link,
        M.Area,
        M.ControllerName,
        M.ActionName,

        CONVERT(BIT, 0) AS CurentUser_Person_Checkecd,
        CONVERT(BIT, 0) AS CurentUser_Group_Checkecd,

        X.Person_Checkecd,
        X.Group_Checkecd,
        X.Group_Checkecd AS [disabled],

        ISNULL(
            (
                SELECT COUNT(1)
                FROM dbo.Menus AS MP
                WHERE MP.ParentId = M.Id
            ), 0
        ) AS IsParent

    FROM dbo.Menus AS M

    LEFT JOIN dbo.UserAccessMenus AS AM
        ON AM.MenuId = M.Id
        AND AM.UserId = @CurrentUserId

    LEFT JOIN dbo.GroupAccessMenus AS AMG
        ON AMG.MenuId = M.Id
        AND AMG.GroupId IN
        (
            SELECT UG.GroupId
            FROM dbo.GroupUsers AS UG
            WHERE UG.UserId = @CurrentUserId
              AND UG.IsActive = 1
              AND UG.IsDeleted = 0
        )

    CROSS APPLY
    (
        SELECT
            CONVERT(BIT,
                CASE
                    WHEN AM.IsDeleted = 0 THEN 1
                    ELSE 0
                END
            ) AS Person_Checkecd,

            CONVERT(BIT,
                CASE
                    WHEN AMG.IsDeleted = 0 THEN 1
                    ELSE 0
                END
            ) AS Group_Checkecd
    ) AS X

    WHERE M.IsDeleted = 0
      AND M.IsActive = 1
      AND
      (
          X.Person_Checkecd = 1
          OR X.Group_Checkecd = 1
      )

    ORDER BY M.PageCode, M.ControllerName;
END