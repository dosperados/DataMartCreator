CREATE TABLE [Setting].[TableDefine]
(
	[TableDefineId] int NOT NULL PRIMARY KEY IDENTITY(1000,1),
	[VersionId] int NOT NULL,
	[SourceMainDBId] int NOT NULL,
	[SourceMainDBName] NVARCHAR(128) NOT NULL,
	[SourceMainSchemaName] NVARCHAR(128) NOT NULL,
	[SourceMainTableId] int NOT NULL,
	[SourceMainTableName] NVARCHAR(128) NOT NULL,
	[DestMainDBId] int NOT NULL,
	[DestMainDBName] NVARCHAR(128) NOT NULL,
	[DestMainSchemaName] NVARCHAR(128) NOT NULL,
	[DestMainTableId] int NOT NULL,
	[DestMainTableName] NVARCHAR(128) NOT NULL,
	[ModifiedAt]               DATETIME        CONSTRAINT [df_Setting_TableDefine_ModifiedAt] DEFAULT (getdate()) NOT NULL,
    [ModifiedBy]               [sysname]       CONSTRAINT [df_Setting_TableDefine_ModifiedBy] DEFAULT ('SYSTEM') NOT NULL,
    [Deleted]                  TINYINT         CONSTRAINT [df_Setting_TableDefine_Deleted] DEFAULT 0 NOT NULL, 
	CONSTRAINT [FK_TableDefine_SourceMainTableId_Setting_Table] FOREIGN KEY ([SourceMainTableId]) REFERENCES [Setting].[Table]([TableId]),
	CONSTRAINT [FK_TableDefine_DestMainTableId_Setting_Table] FOREIGN KEY ([DestMainTableId]) REFERENCES [Setting].[Table]([TableId]),
)
--CONSTRAINT [FK_TableDefine_VersionId_Setting_Version] FOREIGN KEY ([VersionId]) REFERENCES [Setting].[Version]([VersionId]),
    
