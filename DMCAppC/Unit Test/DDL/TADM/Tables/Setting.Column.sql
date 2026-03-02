CREATE TABLE [Setting].[Column]
(
	[ColumnId] INT NOT NULL PRIMARY KEY IDENTITY(200,1),
    [TableId] INT NOT NULL,
	[VersionId] INT NOT NULL,
	[ColumnName] nvarchar(128) NOT NULL,
	[Description] NVARCHAR(2500) NULL,
	[ColumnSortNo] float NOT NULL,
	[DataType] nvarchar(128) NOT NULL,
	[MaxLength] int NULL,
	[Precision] tinyint NULL,
	[Scale] int NULL,
	[CollationName] NVARCHAR(255) NULL,
	[DefaultValue] NVARCHAR(255) NULL,
    [KeySortNo] float NOT NULL,
	[IsNullable] int NOT NULL,
	[IsOptionField] INT NOT NULL DEFAULT 0,
	[FieldType] NVARCHAR(128) NULL,
	[IsLookupField] tinyint NOT NULL DEFAULT 0,
	[ModifiedAt]               DATETIME        CONSTRAINT [df_Setting_Column_ModifiedAt] DEFAULT (getdate()) NOT NULL,
    [ModifiedBy]               [sysname]       CONSTRAINT [df_Setting_Column_ModifiedBy] DEFAULT ('SYSTEM') NOT NULL,
    [Deleted]                  TINYINT         CONSTRAINT [df_Setting_Column_Deleted] DEFAULT 0 NOT NULL,
	CONSTRAINT [FK_Column_Setting_Table] FOREIGN KEY ([TableId]) REFERENCES [Setting].[Table]([TableId]), 
    --CONSTRAINT [FK_Column_Setting_Version] FOREIGN KEY ([VersionId]) REFERENCES [Setting].[Version]([VersionId]),
)

GO

EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'DELETE',
    @level0type = N'SCHEMA',
    @level0name = N'Setting',
    @level1type = N'TABLE',
    @level1name = N'Column',
    @level2type = N'COLUMN',
    @level2name = N'IsOptionField'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'DELETE',
    @level0type = N'SCHEMA',
    @level0name = N'Setting',
    @level1type = N'TABLE',
    @level1name = N'Column',
    @level2type = N'COLUMN',
    @level2name = N'IsLookupField'