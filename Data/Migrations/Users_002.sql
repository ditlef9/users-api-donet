-- Filename: Data\Migrations\Users.sql


DROP TABLE IF EXISTS Users;

CREATE TABLE Users
(
    UserId INT IDENTITY(1, 1) PRIMARY KEY, 
    FirstName NVARCHAR(50), 
    LastName NVARCHAR(50), 
    Email NVARCHAR(50) UNIQUE, 
    Gender NVARCHAR(50), 
    Active BIT
);

INSERT INTO Users (FirstName, LastName, Email, Gender, Active) VALUES 
('John', 'Doe', 'john.doe@example.com', 'Male', 1),
('Jane', 'Smith', 'jane.smith@example.com', 'Female', 1),
('Emily', 'Johnson', 'emily.johnson@example.com', 'Female', 1),
('Michael', 'Brown', 'michael.brown@example.com', 'Male', 0),
('Emma', 'Davis', 'emma.davis@example.com', 'Female', 1);


-- USE DotNetCourseDatabase;
-- GO

-- SELECT  [UserId]
--         , [FirstName]
--         , [LastName]
--         , [Email]
--         , [Gender]
--         , [Active]
--   FROM  TutorialAppSchema.Users;

-- SELECT  [UserId]
--         , [Salary]
--   FROM  TutorialAppSchema.UserSalary;

-- SELECT  [UserId]
--         , [JobTitle]
--         , [Department]
--   FROM  TutorialAppSchema.UserJobInfo;
