-- Triggers

-- After Insert Transaction
CREATE TRIGGER trg_AfterInsertTransaction
ON Transactions
AFTER INSERT
AS
BEGIN
    INSERT INTO TransactionLog (TransactionID, UserID, Amount, Date, Description, Type, Action)
    SELECT TransactionID, UserID, Amount, Date, Description, Type, 'INSERT'
    FROM inserted;
END;

-- After Update Transaction
CREATE TRIGGER trg_AfterUpdateTransaction
ON Transactions
AFTER UPDATE
AS
BEGIN
    INSERT INTO TransactionLog (TransactionID, UserID, Amount, Date, Description, Type, Action)
    SELECT TransactionID, UserID, Amount, Date, Description, Type, 'UPDATE'
    FROM inserted;
END;

-- Before Delete User
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

-- After Insert Transaction Update Saving Goal
CREATE TRIGGER trg_AfterInsertTransaction_UpdateSavingGoal
ON Transactions
AFTER INSERT
AS
BEGIN
    UPDATE SavingGoals
    SET Amount = Amount + i.Amount
    FROM SavingGoals sg
    JOIN inserted i ON sg.UserID = i.UserID
    WHERE i.Type = 'Incomes' AND sg.Description = i.Description; -- Matching description with the goal
END;

-- After Insert Update Delete UserGroupMembership
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

-- Before Insert User
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

-- After Insert User Add to Default Group
CREATE TRIGGER trg_AfterInsertUser_AddToDefaultGroup
ON Users
AFTER INSERT
AS
BEGIN
    INSERT INTO UserGroupMemberships (UserID, GroupID)
    SELECT UserID, 1 -- Assuming 1 is the ID of the default group
    FROM inserted;
END;

-- Before Delete Transaction Update Labels
CREATE TRIGGER trg_BeforeDeleteTransaction_UpdateLabels
ON Transactions
INSTEAD OF DELETE
AS
BEGIN
    DELETE FROM Transactions WHERE TransactionID IN (SELECT TransactionID FROM deleted);
END;

-- Create Log Tables

-- TransactionLog table
CREATE TABLE TransactionLog (
    LogID INT PRIMARY KEY IDENTITY(1,1),
    TransactionID INT,
    UserID INT,
    Amount DECIMAL(18,2),
    Date DATETIME,
    Description NVARCHAR(500),
    Type NVARCHAR(50),
    Action NVARCHAR(10),
    LogDate DATETIME DEFAULT GETDATE()
);

-- UserGroupMembershipLog table
CREATE TABLE UserGroupMembershipLog (
    LogID INT PRIMARY KEY IDENTITY(1,1),
    UserID INT,
    GroupID INT,
    Action NVARCHAR(10),
    ChangeDate DATETIME DEFAULT GETDATE()
);
