CREATE TABLE [Setting].[ReferenceTableDefine]
(
	[ReferenceTableDefineId] INT NOT NULL PRIMARY KEY IDENTITY(3000,1),
	[TableDefineId] INT NOT NULL,
	[VersionId] int NOT NULL,
	[ParentReferenceTableDefineId] INT NULL,
	[RefTableName] NVARCHAR(128) NOT NULL,
	[RefTableId] INT NULL,
	[RefTableAliasName] NVARCHAR(128) NOT NULL,
	[CustomeTableSelect] NVARCHAR(MAX) NULL,
	[RefLevelNo] NVARCHAR(3) NOT NULL,
	[RefType] NVARCHAR(128) NULL,
	[JoinType] NVARCHAR(128) NULL,
	[Description] NVARCHAR(2500) NULL,
	[ModifiedAt]               DATETIME        CONSTRAINT [df_Setting_ReferenceTableDefine_ModifiedAt] DEFAULT (getdate()) NOT NULL,
    [ModifiedBy]               [sysname]       CONSTRAINT [df_Setting_ReferenceTableDefine_ModifiedBy] DEFAULT ('SYSTEM') NOT NULL,
    [Deleted]                  TINYINT         CONSTRAINT [df_Setting_ReferenceTableDefine_Deleted] DEFAULT 0 NOT NULL, 
	CONSTRAINT [CHK_Setting_ReferenceTableDefine_RefLevelNo] CHECK (NOT [RefLevelNo] like '%[^0-9]%'), 
    CONSTRAINT [FK_ReferenceTableDefine_ReferenceTableDefineId] FOREIGN KEY ([ParentReferenceTableDefineId]) REFERENCES [Setting].[ReferenceTableDefine]([ReferenceTableDefineId]), 
    CONSTRAINT [FK_ReferenceTableDefine_TableDefineId] FOREIGN KEY (TableDefineId) REFERENCES [Setting].[TableDefine]([TableDefineId])	
);
GO
