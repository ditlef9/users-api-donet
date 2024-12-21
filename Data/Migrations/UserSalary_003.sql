-- Filename: Data\Migrations\UserSalary.sql

DROP TABLE IF EXISTS UserSalary;

CREATE TABLE UserSalary
(
    UserId INT UNIQUE, 
    Salary DECIMAL(18, 4), 
    AvgSalary DECIMAL(18, 4)
);

INSERT INTO UserSalary (UserId, Salary, AvgSalary) VALUES 
(1, 85000.00, 75000.00),
(2, 95000.00, 80000.00),
(3, 70000.00, 65000.00),
(4, 65000.00, 60000.00),
(5, 60000.00, 62000.00);

