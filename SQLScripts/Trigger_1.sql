CREATE TRIGGER trgBeforeUserDelete
ON Users
INSTEAD OF DELETE
AS
BEGIN
    -- Deklaracja zmiennych
    DECLARE @UserID INT;
    DECLARE @TransactionCount INT;

    -- Pobranie ID usuwanego u¿ytkownika
    SELECT @UserID = UserID FROM deleted;

    -- Sprawdzenie, czy u¿ytkownik ma jakieœ transakcje
    SELECT @TransactionCount = COUNT(*)
    FROM Transactions
    WHERE UserID = @UserID;

    -- Jeœli u¿ytkownik ma transakcje, anuluj usuniêcie i wypisz komunikat
    IF @TransactionCount > 0
    BEGIN
        PRINT 'Nie mo¿na usun¹æ u¿ytkownika. U¿ytkownik ma przypisane transakcje.';
    END
    ELSE
    BEGIN
        -- Usuñ u¿ytkownika
        DELETE FROM Users
        WHERE UserID = @UserID;

        -- Wypisz komunikat
        PRINT 'U¿ytkownik zosta³ usuniêty. U¿ytkownik nie mia³ ¿adnych transakcji na koncie.';
    END
END;
GO


-- Próbujemy usun¹æ u¿ytkownika o UserID 1 (Admin), który ma transakcje
DELETE FROM Users WHERE UserID = 1;
