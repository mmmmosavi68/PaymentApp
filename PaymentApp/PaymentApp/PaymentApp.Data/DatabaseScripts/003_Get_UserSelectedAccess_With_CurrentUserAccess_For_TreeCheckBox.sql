CREATE OR ALTER PROCEDURE [dbo].[Get_UserSelectedAccess_With_CurrentUserAccess_For_TreeCheckBox]
    @CurrentUserId UNIQUEIDENTIFIER,
    @UserSelected_UserId UNIQUEIDENTIFIER
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
                WHEN AM_Current.IsDeleted = 0 THEN 1
                WHEN AM_Current.IsDeleted = 1 THEN 0
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

        CONVERT(BIT,
            CASE
                WHEN AM_SelectedUser.IsDeleted = 0 THEN 1
                WHEN AM_SelectedUser.IsDeleted = 1 THEN 0
                ELSE 0
            END
        ) AS Person_Checkecd,

        CONVERT(BIT,
            CASE
                WHEN AMG_UserSelected_Group.IsDeleted = 0 THEN 1
                WHEN AMG_UserSelected_Group.IsDeleted = 1 THEN 0
                ELSE 0
            END
        ) AS Group_Checkecd,

        CONVERT(BIT,
            CASE
                WHEN AMG_UserSelected_Group.IsDeleted = 0 THEN 1
                WHEN AMG_UserSelected_Group.IsDeleted = 1 THEN 0
                ELSE 0
            END
        ) AS [disabled]

    FROM dbo.Menus AS M

    LEFT OUTER JOIN dbo.UserAccessMenus AS AM_Current
        ON AM_Current.MenuId = M.Id
        AND AM_Current.UserId = @CurrentUserId

    LEFT OUTER JOIN dbo.GroupAccessMenus AS AMG_User
        ON AMG_User.MenuId = M.Id
        AND AMG_User.GroupId IN
        (
            SELECT UG.GroupId
            FROM dbo.GroupUsers AS UG
            WHERE UG.UserId = @CurrentUserId
              AND UG.IsActive = 1
              AND UG.IsDeleted = 0
        )

    LEFT OUTER JOIN dbo.UserAccessMenus AS AM_SelectedUser
        ON AM_SelectedUser.MenuId = M.Id
        AND AM_SelectedUser.UserId = @UserSelected_UserId

    LEFT OUTER JOIN dbo.GroupAccessMenus AS AMG_UserSelected_Group
        ON AMG_UserSelected_Group.MenuId = M.Id
        AND AMG_UserSelected_Group.GroupId IN
        (
            SELECT UG.GroupId
            FROM dbo.GroupUsers AS UG
            WHERE UG.UserId = @UserSelected_UserId
              AND UG.IsActive = 1
              AND UG.IsDeleted = 0
        )

    WHERE M.IsDeleted = 0
      AND M.IsActive = 1

    ORDER BY M.PageCode, M.ControllerName;
END