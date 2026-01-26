CREATE TABLE [Setting].[DefaultColumnBlackList]
(
	[DefaultColumnBlackListId] INT NOT NULL PRIMARY KEY IDENTITY(200,1),
	[ColumnName] NVARCHAR(128) NULL,
	[ModifiedAt]  DATETIME        CONSTRAINT [df_Setting_DefaultColumnBlackList_ModifiedAt] DEFAULT (getdate()) NOT NULL,
    [ModifiedBy]  [sysname]       CONSTRAINT [df_Setting_DefaultColumnBlackList_ModifiedBy] DEFAULT ('SYSTEM') NOT NULL,
    [Deleted]     TINYINT         CONSTRAINT [df_Setting_DefaultColumnBlackList_Deleted] DEFAULT ((0)) NOT NULL,
	CONSTRAINT [UQ_Setting_DefaultColumnBlackList_ColumnName] UNIQUE ([ColumnName])
)
