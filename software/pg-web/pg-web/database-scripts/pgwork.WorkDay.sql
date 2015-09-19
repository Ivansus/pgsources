CREATE TABLE [dbo].WorkDay
(
	[workDayId] INT NOT NULL PRIMARY KEY IDENTITY, 
    [startTime] INT NOT NULL, 
    [endTime] INT NOT NULL, 
    [workDayState] INT NOT NULL
)
