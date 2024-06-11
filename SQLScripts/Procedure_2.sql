CREATE PROCEDURE GenerateUserFinancialReport
    @UserID INT,
    @StartDate DATE,
    @EndDate DATE
AS
BEGIN
    -- Deklaracja zmiennych do przechowywania podsumowania
    DECLARE @TotalIncomes DECIMAL(18, 2);
    DECLARE @TotalSpendings DECIMAL(18, 2);
    DECLARE @Balance DECIMAL(18, 2);

    -- Pobieranie sumy przychod�w
    SELECT @TotalIncomes = ISNULL(SUM(Amount), 0)
    FROM Transactions
    WHERE UserID = @UserID AND Type = 'Incomes' AND Date BETWEEN @StartDate AND @EndDate;

    -- Pobieranie sumy wydatk�w
    SELECT @TotalSpendings = ISNULL(SUM(Amount), 0)
    FROM Transactions
    WHERE UserID = @UserID AND Type = 'Spendings' AND Date BETWEEN @StartDate AND @EndDate;

    -- Obliczanie salda
    SET @Balance = @TotalIncomes - @TotalSpendings;

    -- Wy�wietlanie podsumowania finansowego
    PRINT 'Podsumowanie finansowe u�ytkownika o ID ' + CAST(@UserID AS NVARCHAR(50));
    PRINT '��czne przychody: ' + CAST(@TotalIncomes AS NVARCHAR(50));
    PRINT '��czne wydatki: ' + CAST(@TotalSpendings AS NVARCHAR(50));
    PRINT 'Saldo: ' + CAST(@Balance AS NVARCHAR(50));

    -- Wy�wietlanie listy wszystkich transakcji w danym okresie
    SELECT
        TransactionID,
        Amount,
        Date,
        Description,
        Type
    FROM Transactions
    WHERE UserID = @UserID AND Date BETWEEN @StartDate AND @EndDate
    ORDER BY Date;

    RETURN;
END;
GO


EXEC GenerateUserFinancialReport @UserID = 2, @StartDate = '2023-01-01', @EndDate = '2024-12-31';
