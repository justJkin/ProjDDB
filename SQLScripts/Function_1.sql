CREATE FUNCTION dbo.fnAverageMonthlySpendings
(
    @UserID INT
)
RETURNS DECIMAL(18, 2)
AS
BEGIN
    DECLARE @AverageSpendings DECIMAL(18, 2);

    -- Obliczanie �rednich miesi�cznych wydatk�w
    SELECT @AverageSpendings = AVG(MonthlySpendings)
    FROM
    (
        SELECT SUM(Amount) AS MonthlySpendings
        FROM Transactions
        WHERE UserID = @UserID AND Type = 'Spendings'
        GROUP BY YEAR(Date), MONTH(Date)
    ) AS MonthlyData;

    RETURN @AverageSpendings;
END;
GO


SELECT dbo.fnAverageMonthlySpendings(1) AS AverageMonthlySpendings;