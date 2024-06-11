-- Insert Users
INSERT INTO Users (Username, Password, Email, Role) VALUES
('Admin', 'admin123', 'admin@example.com', 'Admin'),
('Anna', 'anna123', 'anna@example.com', 'User'),
('Jan', 'jan123', 'jan@example.com', 'User'),
('Ewa', 'ewa123', 'ewa@example.com', 'User'),
('Piotr', 'piotr123', 'piotr@example.com', 'User'),
('Kasia', 'kasia123', 'kasia@example.com', 'User'),
('Tomek', 'tomek123', 'tomek@example.com', 'User'),
('Magda', 'magda123', 'magda@example.com', 'User'),
('Pawe³', 'pawel123', 'pawel@example.com', 'User'),
('Agnieszka', 'agnieszka123', 'agnieszka@example.com', 'User');

-- Insert UserGroups
INSERT INTO UserGroups (GroupName) VALUES
('Dom'),
('Znajomi'),
('Praca'),
('Mieszkanie');

-- Insert UserGroupMemberships
INSERT INTO UserGroupMemberships (UserID, GroupID) VALUES
(1, 1),
(2, 1),
(3, 2),
(4, 2),
(5, 3),
(6, 3),
(7, 4),
(8, 4),
(9, 1);

-- Insert Transactions
INSERT INTO Transactions (UserID, Amount, Date, Description, Type) VALUES
(1, 150.75, GETDATE(), 'Zakup sprzêtu', 'Spendings'),
(2, 200.00, GETDATE(), 'Op³ata za us³ugi', 'Spendings');

-- Insert SavingGoals
INSERT INTO SavingGoals (UserID, Amount, GoalType, Description) VALUES
(1, 1000.00, 'Short-term', 'Vacation fund'),
(2, 5000.00, 'Long-term', 'Car fund');

-- Insert Reminders
INSERT INTO Reminders (UserID, Frequency, NextReminderDate, Description) VALUES
(1, 'Monthly', DATEADD(month, 1, GETDATE()), 'Rent payment'),
(2, 'Yearly', DATEADD(year, 1, GETDATE()), 'Insurance payment');


-- Insert PeriodicReports
INSERT INTO PeriodicReports (UserID, ReportType, DateFrom, DateTo, ReportData) VALUES
(1, 'Monthly', DATEADD(month, -1, GETDATE()), GETDATE(), 'Sample report data'),
(2, 'Yearly', DATEADD(year, -1, GETDATE()), GETDATE(), 'Sample report data');


