-- Tworzenie funkcji fnFinancialReport
CREATE FUNCTION dbo.fnFinancialReport
(
    @UserID INT,          -- Deklaracja parametru @UserID typu INT, który przyjmuje identyfikator u¿ytkownika
    @StartDate DATE,      -- Deklaracja parametru @StartDate typu DATE, który przyjmuje datê pocz¹tkow¹ zakresu
    @EndDate DATE         -- Deklaracja parametru @EndDate typu DATE, który przyjmuje datê koñcow¹ zakresu
)
-- Funkcja zwraca tabelê @FinancialReport z kolumnami UserID, TotalIncomes, TotalSpendings i Balance
RETURNS @FinancialReport TABLE
(
    UserID INT,               -- Kolumna UserID typu INT
    TotalIncomes DECIMAL(18, 2),  -- Kolumna TotalIncomes typu DECIMAL z dok³adnoœci¹ 18, 2
    TotalSpendings DECIMAL(18, 2), -- Kolumna TotalSpendings typu DECIMAL z dok³adnoœci¹ 18, 2
    Balance DECIMAL(18, 2)         -- Kolumna Balance typu DECIMAL z dok³adnoœci¹ 18, 2
)
AS
BEGIN
    -- Wstawianie danych do tabeli @FinancialReport
    INSERT INTO @FinancialReport (UserID, TotalIncomes, TotalSpendings, Balance)
    SELECT
        @UserID AS UserID,     -- Przypisanie wartoœci parametru @UserID do kolumny UserID
        SUM(CASE WHEN Type = 'Incomes' THEN Amount ELSE 0 END) AS TotalIncomes,  -- Obliczanie ³¹cznych przychodów (TotalIncomes)
        SUM(CASE WHEN Type = 'Spendings' THEN Amount ELSE 0 END) AS TotalSpendings, -- Obliczanie ³¹cznych wydatków (TotalSpendings)
        SUM(CASE WHEN Type = 'Incomes' THEN Amount ELSE 0 END) - SUM(CASE WHEN Type = 'Spendings' THEN Amount ELSE 0 END) AS Balance -- Obliczanie salda (Balance) jako ró¿nicy miêdzy przychodami a wydatkami
    FROM Transactions
    WHERE UserID = @UserID AND Date BETWEEN @StartDate AND @EndDate; -- Filtracja transakcji wed³ug UserID i zakresu dat

    RETURN; -- Zwrócenie tabeli @FinancialReport
END;
GO

-- Wywo³anie funkcji fnFinancialReport dla u¿ytkownika o ID 1 za okres od 2023-01-01 do 2024-12-31
SELECT * FROM dbo.fnFinancialReport(1, '2023-01-01', '2024-12-31');

-- Powtórzenie wywo³ania funkcji fnFinancialReport dla u¿ytkownika o ID 1 za ten sam okres (dla demonstracji)
SELECT * FROM dbo.fnFinancialReport(1, '2023-01-01', '2024-12-31');

-- Wywo³anie funkcji fnFinancialReport dla u¿ytkownika o ID 2 za okres od 2023-01-01 do 2024-06-30
SELECT * FROM dbo.fnFinancialReport(2, '2023-01-01', '2024-06-30');

-- Wywo³anie funkcji fnFinancialReport dla u¿ytkownika o ID 3 za okres od 2023-07-01 do 2024-12-31
SELECT * FROM dbo.fnFinancialReport(3, '2023-07-01', '2024-12-31');
