SET NOCOUNT ON -- Don't display system messages such as "rows effected" etc.

IF DB_ID('VisitorStatistics') IS NULL
	BEGIN
	  CREATE DATABASE VisitorStatistics
	  PRINT '>> The database was created successfully!'
	END
ELSE
	BEGIN
		PRINT '>> The database already exists!'
	END

GO

CREATE TABLE VisitorStatistics.dbo.VS_Applications
(
	ID Int Identity(1, 1) PRIMARY KEY,
	Name Varchar(50) UNIQUE NOT NULL
)

CREATE TABLE VisitorStatistics.dbo.VS_Admins
(
	ID Int Identity(1, 1) PRIMARY KEY,
	Firstname Varchar(20),
	Lastname Varchar(20),
	Email Varchar(50) UNIQUE NOT NULL,
	Password Varchar(500) NOT NULL
)

CREATE TABLE VisitorStatistics.dbo.VS_Visitors
(
	ID Int Identity(1, 1) PRIMARY KEY,
	IPAddress Varchar(40) UNIQUE NOT NULL,
	HostName Varchar(100),
	Agent Varchar(100),
	IsIgnored Bit NOT NULL DEFAULT 0,
	DeleteDate Date NOT NULL,
	AppID Int NOT NULL
		REFERENCES VisitorStatistics.dbo.VS_Applications(ID)
		ON DELETE CASCADE
)

CREATE TABLE VisitorStatistics.dbo.VS_AppURLs
(
	ID Int Identity(1, 1) PRIMARY KEY,
	AppID Int NOT NULL
		REFERENCES VS_Applications(ID)
		ON DELETE CASCADE,
	RegisteredURL Varchar(500) NOT NULL
)

CREATE UNIQUE INDEX Unique_AppURL ON VisitorStatistics.dbo.VS_AppURLs(AppID, RegisteredURL);

CREATE TABLE VisitorStatistics.dbo.VS_Visits
(
	ID Int Identity(1, 1) PRIMARY KEY,
	VisitorID Int NOT NULL
		REFERENCES VS_Visitors(ID)
		ON DELETE CASCADE,
	RefererURL Varchar(500) NOT NULL,
	VisitURL Varchar(500) NOT NULL,
	VisitTime DateTime NOT NULL
)

CREATE TABLE VisitorStatistics.dbo.VS_AdminVisits
(
	ID Int Identity(1, 1) PRIMARY KEY,
	AdminID Int NOT NULL
		REFERENCES VS_Admins(ID)
		ON DELETE CASCADE,
	VisitID Int NOT NULL
		REFERENCES VS_Visits(ID)
		ON DELETE CASCADE
)

CREATE UNIQUE INDEX Unique_AdminVisit ON VisitorStatistics.dbo.VS_AdminVisits(AdminID, VisitID);

INSERT INTO VisitorStatistics.dbo.VS_Applications VALUES('Visitor Statistics 1.0')

INSERT INTO VisitorStatistics.dbo.VS_Admins VALUES
(
	'YourFirstName', -- Optional.
	'YourLastName', -- Optional.
	'your@email.com',
	-- Keep the hashed password below until your first login. Default password is set to
	-- "admin", but can be changed under the admin panel once you're logged in:
	'AQAAAAEAACcQAAAAEBehHmgEHZmjXlTBGlKSW9KVuxMIHp1f4r8sC502SFQkGGxiYeef6HFntNMCMdZ76w=='
)