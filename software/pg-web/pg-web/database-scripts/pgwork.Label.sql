CREATE TABLE [dbo].[Label]
(
	[labelId] INT NOT NULL PRIMARY KEY, 
    [labelData] INT NOT NULL, 
    [areaId] INT NOT NULL, 
    CONSTRAINT [FK_Label_Area2] FOREIGN KEY ([areaId]) REFERENCES [Area]([areaId])
)
