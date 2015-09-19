CREATE TABLE [dbo].[Table]
(
	[workEmployerId] INT NOT NULL PRIMARY KEY IDENTITY, 
    [workDayId] INT NOT NULL, 
    [employerId] INT NOT NULL, 
    [deviceId] INT NOT NULL, 
    [workState] INT NOT NULL
)
