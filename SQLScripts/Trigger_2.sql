CREATE TRIGGER trgCheckNegativeBalance
ON Transactions
AFTER INSERT
AS
BEGIN
    -- Deklaracja zmiennych
    DECLARE @UserID INT;
    DECLARE @Amount DECIMAL(18, 2);
    DECLARE @Type NVARCHAR(50);
    DECLARE @CurrentBalance DECIMAL(18, 2);
    DECLARE @NewBalance DECIMAL(18, 2);

    -- Przechodzenie przez wiersze wstawione w operacji INSERT
    DECLARE InsertedCursor CURSOR FOR
    SELECT UserID, Amount, Type
    FROM inserted;

    OPEN InsertedCursor;
    FETCH NEXT FROM InsertedCursor INTO @UserID, @Amount, @Type;

    WHILE @@FETCH_STATUS = 0
    BEGIN
        -- Pobieranie aktualnego salda u¿ytkownika
        SELECT @CurrentBalance = ISNULL(SUM(CASE WHEN Type = 'Incomes' THEN Amount ELSE 0 END), 0)
                                  - ISNULL(SUM(CASE WHEN Type = 'Spendings' THEN Amount ELSE 0 END), 0)
        FROM Transactions
        WHERE UserID = @UserID;

        -- Obliczanie nowego salda po dodaniu transakcji
        IF @Type = 'Incomes'
        BEGIN
            SET @NewBalance = @CurrentBalance + @Amount;
        END
        ELSE
        BEGIN
            SET @NewBalance = @CurrentBalance - @Amount;
        END

        -- Sprawdzanie, czy nowe saldo jest ujemne
        IF @NewBalance < 0
        BEGIN
            -- Generowanie przypomnienia o ujemnym saldzie
            INSERT INTO Reminders (UserID, Frequency, NextReminderDate, Description)
            VALUES (@UserID, 'Once', GETDATE(), 'Twoje saldo po transakcji jest ujemne!');
        END

        FETCH NEXT FROM InsertedCursor INTO @UserID, @Amount, @Type;
    END;

    CLOSE InsertedCursor;
    DEALLOCATE InsertedCursor;
END;
GO

