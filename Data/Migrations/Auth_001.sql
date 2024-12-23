-- Filename: Data\Migrations\Auth_nnn.sql


DROP TABLE IF EXISTS Auth;

CREATE TABLE Auth(
	Email NVARCHAR(200) PRIMARY KEY,
	PasswordHash VARBINARY(MAX),
	PasswordSalt VARBINARY(MAX)
)
