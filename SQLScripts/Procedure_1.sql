CREATE PROCEDURE CalculateUserBalance
    @UserID INT,
    @StartDate DATE,
    @EndDate DATE
AS
BEGIN
    -- Deklaracja zmiennych na sumy przychodów i wydatków
    DECLARE @TotalIncomes DECIMAL(18, 2);
    DECLARE @TotalSpendings DECIMAL(18, 2);
    DECLARE @Balance DECIMAL(18, 2);

    -- Obliczanie sumy przychodów dla danego u¿ytkownika w podanym przedziale czasowym
    SELECT @TotalIncomes = ISNULL(SUM(Amount), 0)
    FROM Transactions
    WHERE UserID = @UserID
      AND Type = 'Incomes'
      AND Date BETWEEN @StartDate AND @EndDate;

    -- Obliczanie sumy wydatków dla danego u¿ytkownika w podanym przedziale czasowym
    SELECT @TotalSpendings = ISNULL(SUM(Amount), 0)
    FROM Transactions
    WHERE UserID = @UserID
      AND Type = 'Spendings'
      AND Date BETWEEN @StartDate AND @EndDate;

    -- Obliczanie salda
    SET @Balance = @TotalIncomes - @TotalSpendings;

    -- Wypisywanie wyniku
    IF @Balance >= 0
    BEGIN
        PRINT 'User has a positive balance of ' + CAST(@Balance AS NVARCHAR(50));
    END
    ELSE
    BEGIN
        PRINT 'User has a negative balance of ' + CAST(@Balance AS NVARCHAR(50));
    END
END;
GO



EXEC CalculateUserBalance 2, '2023-01-01', '2024-12-31';