--procedura składowana, która pobiera wartości do tworzenia wykresów, 
--zwracając sumy przychodów i wydatków dla danego użytkownika w określonym przedziale czasowym
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

--procedura składowana, która zlicza wszystkie wydatki danego użytkownika
CREATE PROCEDURE sp_GetTotalSpendingsByUser
    @UserID INT
AS
BEGIN
    SELECT SUM(Amount) AS TotalSpendings
    FROM Transactions
    WHERE UserID = @UserID AND Type = 'Spendings';
END;


