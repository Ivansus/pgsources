CREATE TABLE [dbo].[WorkArea] (
    [workAreaId]   INT          IDENTITY (1, 1) NOT NULL,
    [workDayId]    INT          NOT NULL,
    [workAreaName] VARCHAR (50) NOT NULL,
    PRIMARY KEY CLUSTERED ([workAreaId] ASC), 
    CONSTRAINT FK_WorkArea_WorkDay FOREIGN KEY (workDayId) REFERENCES WorkDay(workDayId), 
);

