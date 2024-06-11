CREATE TRIGGER trg_AfterInsertTransaction
ON Transactions
AFTER INSERT
AS
BEGIN
    INSERT INTO TransactionLog (TransactionID, UserID, EntityID, Amount, Date, Description, CategoryID, CategoryName, Action)
    SELECT TransactionID, UserID, EntityID, Amount, Date, Description, CategoryID, CategoryName, 'INSERT'
    FROM inserted;
END;
CREATE TRIGGER trg_AfterUpdateTransaction
ON Transactions
AFTER UPDATE
AS
BEGIN
    INSERT INTO TransactionLog (TransactionID, UserID, EntityID, Amount, Date, Description, CategoryID, CategoryName, Action)
    SELECT TransactionID, UserID, EntityID, Amount, Date, Description, CategoryID, CategoryName, 'UPDATE'
    FROM inserted;
END;
CREATE TRIGGER trg_BeforeDeleteUser
ON Users
INSTEAD OF DELETE
AS
BEGIN
    IF EXISTS (SELECT 1 FROM Transactions WHERE UserID IN (SELECT UserID FROM deleted))
    BEGIN
        RAISERROR('Cannot delete users with active transactions', 16, 1);
        ROLLBACK TRANSACTION;
    END
    ELSE
    BEGIN
        DELETE FROM Users WHERE UserID IN (SELECT UserID FROM deleted);
    END
END;
CREATE TRIGGER trg_AfterInsertTransaction_UpdateSavingGoal
ON Transactions
AFTER INSERT
AS
BEGIN
    UPDATE SavingGoals
    SET Amount = Amount + i.Amount
    FROM SavingGoals sg
    JOIN inserted i ON sg.UserID = i.UserID
    WHERE i.CategoryID = sg.GoalID;
END;
CREATE TRIGGER trg_AfterInsertUpdateDeleteUserGroupMembership
ON UserGroupMemberships
AFTER INSERT, UPDATE, DELETE
AS
BEGIN
    INSERT INTO UserGroupMembershipLog (UserID, GroupID, Action, ChangeDate)
    SELECT 
        COALESCE(i.UserID, d.UserID),
        COALESCE(i.GroupID, d.GroupID),
        CASE
            WHEN EXISTS (SELECT 1 FROM inserted) AND EXISTS (SELECT 1 FROM deleted) THEN 'UPDATE'
            WHEN EXISTS (SELECT 1 FROM inserted) THEN 'INSERT'
            WHEN EXISTS (SELECT 1 FROM deleted) THEN 'DELETE'
        END,
        GETDATE()
    FROM inserted i
    FULL JOIN deleted d ON i.UserID = d.UserID AND i.GroupID = d.GroupID;
END;
CREATE TRIGGER trg_BeforeInsertUser
ON Users
INSTEAD OF INSERT
AS
BEGIN
    IF EXISTS (SELECT 1 FROM Users WHERE Email IN (SELECT Email FROM inserted))
    BEGIN
        RAISERROR('Email address must be unique', 16, 1);
        ROLLBACK TRANSACTION;
    END
    ELSE
    BEGIN
        INSERT INTO Users (Username, Password, Email, Role)
        SELECT Username, Password, Email, Role
        FROM inserted;
    END
END;
CREATE TRIGGER trg_AfterInsertUser_AddToDefaultGroup
ON Users
AFTER INSERT
AS
BEGIN
    INSERT INTO UserGroupMemberships (UserID, GroupID)
    SELECT UserID, 1 -- Assuming 1 is the ID of the default group
    FROM inserted;
END;
CREATE TRIGGER trg_BeforeDeleteTransaction_UpdateLabels
ON Transactions
INSTEAD OF DELETE
AS
BEGIN
    DELETE FROM Labels WHERE EntityID IN (SELECT EntityID FROM deleted);
    DELETE FROM Transactions WHERE TransactionID IN (SELECT TransactionID FROM deleted);
END;

--------------------------------------------------

CREATE FUNCTION fn_GetTotalSpendingByUser (@UserID INT)
RETURNS DECIMAL(18, 2)
AS
BEGIN
    DECLARE @TotalSpending DECIMAL(18, 2);

    SELECT @TotalSpending = SUM(Amount)
    FROM Transactions
    WHERE UserID = @UserID;

    RETURN @TotalSpending;
END;
CREATE FUNCTION fn_GetHighestTransactionByUser (@UserID INT)
RETURNS DECIMAL(18, 2)
AS
BEGIN
    DECLARE @MaxAmount DECIMAL(18, 2);

    SELECT @MaxAmount = MAX(Amount)
    FROM Transactions
    WHERE UserID = @UserID;

    RETURN @MaxAmount;
END;
CREATE FUNCTION fn_GetSavingGoalProgress (@UserID INT)
RETURNS TABLE
AS
RETURN
(
    SELECT sg.GoalID, sg.Description, sg.Amount, 
           ISNULL(SUM(t.Amount), 0) AS SavedAmount
    FROM SavingGoals sg
    LEFT JOIN Transactions t ON sg.UserID = t.UserID AND sg.GoalID = t.CategoryID
    WHERE sg.UserID = @UserID
    GROUP BY sg.GoalID, sg.Description, sg.Amount
);
CREATE FUNCTION fn_GetNextReminderByUser (@UserID INT)
RETURNS DATETIME
AS
BEGIN
    DECLARE @NextReminder DATETIME;

    SELECT @NextReminder = MIN(NextReminderDate)
    FROM Reminders
    WHERE UserID = @UserID;

    RETURN @NextReminder;
END;
CREATE FUNCTION fn_GetUserGroupMemberships (@UserID INT)
RETURNS TABLE
AS
RETURN
(
    SELECT ug.GroupID, ug.GroupName
    FROM UserGroupMemberships ugm
    JOIN UserGroups ug ON ugm.GroupID = ug.GroupID
    WHERE ugm.UserID = @UserID
);



----------------------------------------------

-- Create TransactionLog table
CREATE TABLE TransactionLog (
    LogID INT PRIMARY KEY IDENTITY(1,1),
    TransactionID INT,
    UserID INT,
    EntityID INT,
    Amount DECIMAL(18,2),
    Date DATETIME,
    Description NVARCHAR(500),
    CategoryID INT,
    CategoryName NVARCHAR(100),
    Action NVARCHAR(10),
    LogDate DATETIME DEFAULT GETDATE()
);

-- Create UserGroupMembershipLog table
CREATE TABLE UserGroupMembershipLog (
    LogID INT PRIMARY KEY IDENTITY(1,1),
    UserID INT,
    GroupID INT,
    Action NVARCHAR(10),
    ChangeDate DATETIME DEFAULT GETDATE()
);
