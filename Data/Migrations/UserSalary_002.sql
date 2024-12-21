-- Filename: Data\Migrations\UserSalary.sql

DROP TABLE IF EXISTS UserSalary;

-- IF OBJECT_ID('TutorialAppSchema.UserSalary') IS NOT NULL
--     DROP TABLE TutorialAppSchema.UserSalary;

CREATE TABLE UserSalary
(
    UserId INT UNIQUE
    , Salary DECIMAL(18, 4)
);

INSERT INTO UserSalary (UserId, Salary) VALUES 
(1, 85000.00),
(2, 95000.00),
(3, 70000.00),
(4, 65000.00),
(5, 60000.00);
