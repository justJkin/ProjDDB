
-- Get Total Spending By User
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

-- Get Highest Transaction By User
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

-- Get Saving Goal Progress
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

-- Get Next Reminder By User
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

-- Get User Group Memberships
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