-- Tworzenie funkcji fnFinancialReport
CREATE FUNCTION dbo.fnFinancialReport
(
    @UserID INT,          -- Deklaracja parametru @UserID typu INT, kt�ry przyjmuje identyfikator u�ytkownika
    @StartDate DATE,      -- Deklaracja parametru @StartDate typu DATE, kt�ry przyjmuje dat� pocz�tkow� zakresu
    @EndDate DATE         -- Deklaracja parametru @EndDate typu DATE, kt�ry przyjmuje dat� ko�cow� zakresu
)
-- Funkcja zwraca tabel� @FinancialReport z kolumnami UserID, TotalIncomes, TotalSpendings i Balance
RETURNS @FinancialReport TABLE
(
    UserID INT,               -- Kolumna UserID typu INT
    TotalIncomes DECIMAL(18, 2),  -- Kolumna TotalIncomes typu DECIMAL z dok�adno�ci� 18, 2
    TotalSpendings DECIMAL(18, 2), -- Kolumna TotalSpendings typu DECIMAL z dok�adno�ci� 18, 2
    Balance DECIMAL(18, 2)         -- Kolumna Balance typu DECIMAL z dok�adno�ci� 18, 2
)
AS
BEGIN
    -- Wstawianie danych do tabeli @FinancialReport
    INSERT INTO @FinancialReport (UserID, TotalIncomes, TotalSpendings, Balance)
    SELECT
        @UserID AS UserID,     -- Przypisanie warto�ci parametru @UserID do kolumny UserID
        SUM(CASE WHEN Type = 'Incomes' THEN Amount ELSE 0 END) AS TotalIncomes,  -- Obliczanie ��cznych przychod�w (TotalIncomes)
        SUM(CASE WHEN Type = 'Spendings' THEN Amount ELSE 0 END) AS TotalSpendings, -- Obliczanie ��cznych wydatk�w (TotalSpendings)
        SUM(CASE WHEN Type = 'Incomes' THEN Amount ELSE 0 END) - SUM(CASE WHEN Type = 'Spendings' THEN Amount ELSE 0 END) AS Balance -- Obliczanie salda (Balance) jako r�nicy mi�dzy przychodami a wydatkami
    FROM Transactions
    WHERE UserID = @UserID AND Date BETWEEN @StartDate AND @EndDate; -- Filtracja transakcji wed�ug UserID i zakresu dat

    RETURN; -- Zwr�cenie tabeli @FinancialReport
END;
GO

-- Wywo�anie funkcji fnFinancialReport dla u�ytkownika o ID 1 za okres od 2023-01-01 do 2024-12-31
SELECT * FROM dbo.fnFinancialReport(1, '2023-01-01', '2024-12-31');

-- Powt�rzenie wywo�ania funkcji fnFinancialReport dla u�ytkownika o ID 1 za ten sam okres (dla demonstracji)
SELECT * FROM dbo.fnFinancialReport(1, '2023-01-01', '2024-12-31');

-- Wywo�anie funkcji fnFinancialReport dla u�ytkownika o ID 2 za okres od 2023-01-01 do 2024-06-30
SELECT * FROM dbo.fnFinancialReport(2, '2023-01-01', '2024-06-30');

-- Wywo�anie funkcji fnFinancialReport dla u�ytkownika o ID 3 za okres od 2023-07-01 do 2024-12-31
SELECT * FROM dbo.fnFinancialReport(3, '2023-07-01', '2024-12-31');
