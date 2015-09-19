CREATE TABLE [dbo].[Table]
(
	[workShiftId] INT NOT NULL PRIMARY KEY IDENTITY, 
    [workDayId] INT NOT NULL, 
    [workAreaId] INT NOT NULL, 
    [startTime] INT NOT NULL, 
    [endTime] INT NOT NULL, 
    [workShiftType] INT NOT NULL, 
    [notificationTimerDelay] INT NOT NULL, 
    [alarmTimerDelay] INT NOT NULL, 
    [shiftState] INT NOT NULL
)
