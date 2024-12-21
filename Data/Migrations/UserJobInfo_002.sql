-- Filename: Data\Migrations\UserJobInfo.sql

DROP TABLE IF EXISTS UserJobInfo;

-- IF OBJECT_ID('TutorialAppSchema.UserJobInfo') IS NOT NULL
--     DROP TABLE TutorialAppSchema.UserJobInfo;

CREATE TABLE UserJobInfo
(
    UserId INT UNIQUE
    , JobTitle NVARCHAR(50)
    , Department NVARCHAR(50),
);


INSERT INTO UserJobInfo (UserId, JobTitle, Department) VALUES 
(1, 'Software Engineer', 'IT'),
(2, 'Project Manager', 'Management'),
(3, 'Data Analyst', 'Analytics'),
(4, 'Marketing Specialist', 'Marketing'),
(5, 'HR Coordinator', 'Human Resources');
