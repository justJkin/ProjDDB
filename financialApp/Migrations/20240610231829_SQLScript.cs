using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace financialApp.Migrations
{
    /// <inheritdoc />
    public partial class SQLScript : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Functions
            migrationBuilder.Sql(@"
        CREATE FUNCTION fn_GetTotalSpendingByUser (@UserID INT)
        RETURNS DECIMAL(18, 2)
        AS
        BEGIN
            DECLARE @TotalSpending DECIMAL(18, 2);
            SELECT @TotalSpending = SUM(Amount)
            FROM Transactions
            WHERE UserID = @UserID AND Type = 'Spendings';
            RETURN @TotalSpending;
        END;
        ");

            migrationBuilder.Sql(@"
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
        ");

            migrationBuilder.Sql(@"
        CREATE FUNCTION fn_GetSavingGoalProgress (@UserID INT)
        RETURNS TABLE
        AS
        RETURN
        (
            SELECT sg.GoalID, sg.Description, sg.Amount, 
                   ISNULL(SUM(t.Amount), 0) AS SavedAmount
            FROM SavingGoals sg
            LEFT JOIN Transactions t ON sg.UserID = t.UserID AND sg.Description = t.Description AND t.Type = 'Incomes'
            WHERE sg.UserID = @UserID
            GROUP BY sg.GoalID, sg.Description, sg.Amount
        );
        ");

            migrationBuilder.Sql(@"
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
        ");

            migrationBuilder.Sql(@"
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
        ");

            // Procedures
            migrationBuilder.Sql(@"
        CREATE PROCEDURE sp_GetTransactionSumsForGraph
            @UserID INT,
            @StartDate DATETIME,
            @EndDate DATETIME
        AS
        BEGIN
            SELECT
                SUM(CASE WHEN Type = 'Incomes' THEN Amount ELSE 0 END) AS TotalIncomes,
                SUM(CASE WHEN Type = 'Spendings' THEN Amount ELSE 0 END) AS TotalSpendings
            FROM Transactions
            WHERE UserID = @UserID
              AND Date BETWEEN @StartDate AND @EndDate;
        END;
        ");

            migrationBuilder.Sql(@"
        CREATE PROCEDURE sp_GetTotalSpendingsByUser
            @UserID INT
        AS
        BEGIN
            SELECT SUM(Amount) AS TotalSpendings
            FROM Transactions
            WHERE UserID = @UserID AND Type = 'Spendings';
        END;
        ");

            // Triggers
            migrationBuilder.Sql(@"
        CREATE TRIGGER trg_AfterInsertTransaction
        ON Transactions
        AFTER INSERT
        AS
        BEGIN
            INSERT INTO TransactionLog (TransactionID, UserID, Amount, Date, Description, Type, Action)
            SELECT TransactionID, UserID, Amount, Date, Description, Type, 'INSERT'
            FROM inserted;
        END;
        ");

            migrationBuilder.Sql(@"
        CREATE TRIGGER trg_AfterUpdateTransaction
        ON Transactions
        AFTER UPDATE
        AS
        BEGIN
            INSERT INTO TransactionLog (TransactionID, UserID, Amount, Date, Description, Type, Action)
            SELECT TransactionID, UserID, Amount, Date, Description, Type, 'UPDATE'
            FROM inserted;
        END;
        ");

            migrationBuilder.Sql(@"
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
        ");

            migrationBuilder.Sql(@"
        CREATE TRIGGER trg_AfterInsertTransaction_UpdateSavingGoal
        ON Transactions
        AFTER INSERT
        AS
        BEGIN
            UPDATE SavingGoals
            SET Amount = Amount + i.Amount
            FROM SavingGoals sg
            JOIN inserted i ON sg.UserID = i.UserID
            WHERE i.Type = 'Incomes' AND sg.Description = i.Description;
        END;
        ");

            migrationBuilder.Sql(@"
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
        ");

            migrationBuilder.Sql(@"
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
        ");

            migrationBuilder.Sql(@"
        CREATE TRIGGER trg_AfterInsertUser_AddToDefaultGroup
        ON Users
        AFTER INSERT
        AS
        BEGIN
            INSERT INTO UserGroupMemberships (UserID, GroupID)
            SELECT UserID, 1 -- Assuming 1 is the ID of the default group
            FROM inserted;
        END;
        ");

            migrationBuilder.Sql(@"
        CREATE TRIGGER trg_BeforeDeleteTransaction_UpdateLabels
        ON Transactions
        INSTEAD OF DELETE
        AS
        BEGIN
            DELETE FROM Transactions WHERE TransactionID IN (SELECT TransactionID FROM deleted);
        END;
        ");

            // Create Log Tables
            migrationBuilder.Sql(@"
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
        ");

            migrationBuilder.Sql(@"
        CREATE TABLE UserGroupMembershipLog (
            LogID INT PRIMARY KEY IDENTITY(1,1),
            UserID INT,
            GroupID INT,
            Action NVARCHAR(10),
            ChangeDate DATETIME DEFAULT GETDATE()
        );
        ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Drop the created functions, procedures, triggers, and tables
            migrationBuilder.Sql(@"DROP FUNCTION IF EXISTS fn_GetTotalSpendingByUser");
            migrationBuilder.Sql(@"DROP FUNCTION IF EXISTS fn_GetHighestTransactionByUser");
            migrationBuilder.Sql(@"DROP FUNCTION IF EXISTS fn_GetSavingGoalProgress");
            migrationBuilder.Sql(@"DROP FUNCTION IF EXISTS fn_GetNextReminderByUser");
            migrationBuilder.Sql(@"DROP FUNCTION IF EXISTS fn_GetUserGroupMemberships");

            migrationBuilder.Sql(@"DROP PROCEDURE IF EXISTS sp_GetTransactionSumsForGraph");
            migrationBuilder.Sql(@"DROP PROCEDURE IF EXISTS sp_GetTotalSpendingsByUser");

            migrationBuilder.Sql(@"DROP TRIGGER IF EXISTS trg_AfterInsertTransaction");
            migrationBuilder.Sql(@"DROP TRIGGER IF EXISTS trg_AfterUpdateTransaction");
            migrationBuilder.Sql(@"DROP TRIGGER IF EXISTS trg_BeforeDeleteUser");
            migrationBuilder.Sql(@"DROP TRIGGER IF EXISTS trg_AfterInsertTransaction_UpdateSavingGoal");
            migrationBuilder.Sql(@"DROP TRIGGER IF EXISTS trg_AfterInsertUpdateDeleteUserGroupMembership");
            migrationBuilder.Sql(@"DROP TRIGGER IF EXISTS trg_BeforeInsertUser");
            migrationBuilder.Sql(@"DROP TRIGGER IF EXISTS trg_AfterInsertUser_AddToDefaultGroup");
            migrationBuilder.Sql(@"DROP TRIGGER IF EXISTS trg_BeforeDeleteTransaction_UpdateLabels");

            migrationBuilder.Sql(@"DROP TABLE IF EXISTS TransactionLog");
            migrationBuilder.Sql(@"DROP TABLE IF EXISTS UserGroupMembershipLog");
        }
    }
}

