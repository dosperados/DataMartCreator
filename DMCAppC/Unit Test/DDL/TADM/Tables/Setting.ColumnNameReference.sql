CREATE TABLE [Setting].[ColumnNameReference]
(
	[ColumnNameReferenceId] INT IDENTITY (5000, 1) NOT NULL,
	[SourceDBName] NVARCHAR (128) NOT NULL,
	[SourceSchemaName] NVARCHAR (128) NOT NULL,
	[SourceTableName]  NVARCHAR (128) NOT NULL,
	[SourceColumnName]   NVARCHAR (128) NOT NULL,
	[DestDBName] NVARCHAR (128) NOT NULL,
	[DestSchemaName] NVARCHAR (128) NOT NULL,
	[DestTableName] NVARCHAR (128) NOT NULL,
	[DestColumnName] NVARCHAR (128) NOT NULL,
	[RefDBName] NVARCHAR (128) NOT NULL,
	[RefTableName] NVARCHAR (128) NOT NULL,
	[RefSchemaName] NVARCHAR (128) NOT NULL,
	[RefColumnName] NVARCHAR (128) NOT NULL,
	[RefColumnIdName] NVARCHAR (128) NOT NULL,
	[AddRefDestColumnIdName] NVARCHAR (128) NOT NULL, /*Pod kakim imenem budet dobalen Id v Dest Table*/
	[ModifiedAt]         DATETIME       NOT NULL,
    [ModifiedBy]         NVARCHAR (128) NOT NULL,
    [Deleted]            TINYINT        NOT NULL,
	PRIMARY KEY CLUSTERED ([ColumnNameReferenceId] ASC),
	CONSTRAINT [UC_Setting_ColumnNameReference] UNIQUE NONCLUSTERED ([SourceDBName] ASC, [SourceColumnName] ASC,[SourceSchemaName] ASC,[DestDBName] ASC,[DestSchemaName] ASC, [DestColumnName] ASC, [RefSchemaName] asc, [RefTableName] asc, [RefColumnName] ASC)
)
