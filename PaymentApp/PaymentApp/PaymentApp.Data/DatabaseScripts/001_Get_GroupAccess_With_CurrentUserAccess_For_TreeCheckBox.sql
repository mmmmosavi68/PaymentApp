CREATE OR ALTER PROCEDURE [dbo].[Get_GroupAccess_With_CurrentUserAccess_For_TreeCheckBox]
    @CurrentUserId UNIQUEIDENTIFIER,
    @GroupSelected_GroupId UNIQUEIDENTIFIER
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

        CONVERT(BIT,
            CASE
                WHEN AM.IsDeleted = 0 THEN 1
                WHEN AM.IsDeleted = 1 THEN 0
                ELSE 0
            END
        ) AS CurentUser_Person_Checkecd,

        CONVERT(BIT,
            CASE
                WHEN AMG_User.IsDeleted = 0 THEN 1
                WHEN AMG_User.IsDeleted = 1 THEN 0
                ELSE 0
            END
        ) AS CurentUser_Group_Checkecd,

        ISNULL(
            (
                SELECT COUNT(Id)
                FROM dbo.Menus AS MP
                WHERE MP.ParentId = M.Id
            ), 0
        ) AS IsParent,

        CONVERT(BIT, 0) AS Person_Checkecd,

        CONVERT(BIT,
            CASE
                WHEN AMG_Group.IsDeleted = 0 THEN 1
                WHEN AMG_Group.IsDeleted = 1 THEN 0
                ELSE 0
            END
        ) AS Group_Checkecd,

        CONVERT(BIT,
            CASE
                WHEN AMG_Group.IsDeleted = 0 THEN 1
                WHEN AMG_Group.IsDeleted = 1 THEN 0
                ELSE 0
            END
        ) AS [disabled]

    FROM dbo.Menus AS M

    LEFT OUTER JOIN dbo.UserAccessMenus AS AM
        ON AM.MenuId = M.Id
        AND AM.UserId = @CurrentUserId

    LEFT OUTER JOIN dbo.GroupAccessMenus AS AMG_User
        ON AMG_User.MenuId = M.Id
        AND AMG_User.GroupId IN
        (
            SELECT UG.GroupId
            FROM dbo.GroupUsers AS UG
            WHERE UG.UserId = @CurrentUserId
              AND UG.GroupId = @GroupSelected_GroupId
              AND UG.IsActive = 1
              AND UG.IsDeleted = 0
        )

    LEFT OUTER JOIN dbo.GroupAccessMenus AS AMG_Group
        ON AMG_Group.MenuId = M.Id
        AND AMG_Group.GroupId = @GroupSelected_GroupId

    WHERE M.IsDeleted = 0
      AND M.IsActive = 1

    ORDER BY M.PageCode, M.ControllerName;
END