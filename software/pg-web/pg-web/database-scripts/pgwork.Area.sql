CREATE TABLE [dbo].[Table]
(
	[areaId] INT NOT NULL PRIMARY KEY IDENTITY, 
    [areaName] VARCHAR(MAX) NOT NULL,
    [outline] NTEXT NOT NULL
)
