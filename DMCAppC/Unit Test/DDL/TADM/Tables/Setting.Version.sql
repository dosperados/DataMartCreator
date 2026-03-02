CREATE TABLE [Setting].[Version]
(
	[VersionId] INT NOT NULL PRIMARY KEY IDENTITY(1,1),
	[Description] NVARCHAR(2500) NULL,
	[ModifiedAt]               DATETIME        CONSTRAINT [df_Setting_Version_ModifiedAt] DEFAULT (getdate()) NOT NULL,
    [ModifiedBy]               [sysname]       CONSTRAINT [df_Setting_Version_ModifiedBy] DEFAULT ('SYSTEM') NOT NULL,
    [Deleted]                  TINYINT         CONSTRAINT [df_Setting_Version_Deleted] DEFAULT 0 NOT NULL,
)
