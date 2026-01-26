CREATE TABLE [Setting].[ReferenceColumnDefine] (
    [ReferenceColumnDefineId]      INT             IDENTITY (4000, 1) NOT NULL,
    [VersionId]                    INT             NOT NULL,
    [SourceReferenceTableDefineId] INT             NOT NULL,
    [SourceTableName]              NVARCHAR (128)  NOT NULL,
    [SourceColumnId]               INT             NOT NULL,
    [SourceColumnFunction]         NVARCHAR (MAX)  NULL,
    [RefReferenceTableDefineId]    INT             NOT NULL,
    [RefTableName]                 NVARCHAR (128)  NOT NULL,
    [RefColumnId]                  INT             NOT NULL,
    [RefColumnName]                NVARCHAR (128)  NOT NULL,
    [RefColumnFunction]            NVARCHAR (MAX)  NULL,
    [KeySortNo]                    FLOAT (53)      NOT NULL,
    [Description]                  NVARCHAR (2500) NULL,
    [ModifiedAt]                   DATETIME        CONSTRAINT [df_Setting_ReferenceColumnDefine_ModifiedAt] DEFAULT (getdate()) NOT NULL,
    [ModifiedBy]                   [sysname]       CONSTRAINT [df_Setting_ReferenceColumnDefine_ModifiedBy] DEFAULT ('SYSTEM') NOT NULL,
    [Deleted]                      TINYINT         CONSTRAINT [df_Setting_ReferenceColumnDefine_Deleted] DEFAULT ((0)) NOT NULL,
    PRIMARY KEY CLUSTERED ([ReferenceColumnDefineId] ASC),
    CONSTRAINT [FK_ReferenceColumnDefine_SourceReferenceTableDefineId_Setting_ReferenceTableDefine] FOREIGN KEY ([SourceReferenceTableDefineId]) REFERENCES [Setting].[ReferenceTableDefine]([ReferenceTableDefineId]), 
    CONSTRAINT [FK_ReferenceColumnDefine_RefReferenceTableDefineId_Setting_ReferenceTableDefine]    FOREIGN KEY ([RefReferenceTableDefineId]) REFERENCES [Setting].[ReferenceTableDefine]([ReferenceTableDefineId]), 
    CONSTRAINT [FK_ReferenceColumnDefine_SourceColumnId_Setting_Column] FOREIGN KEY ([SourceColumnId]) REFERENCES [Setting].[Column]([ColumnId]),
    CONSTRAINT [FK_ReferenceColumnDefine_RefColumnId_Setting_Column] FOREIGN KEY ([RefColumnId]) REFERENCES [Setting].[Column]([ColumnId]),
);



GO

--EXEC sp_addextendedproperty @name = N'MS_Description',
--    @value = N'SourceMainTableId',
--    @level0type = N'SCHEMA',
--    @level0name = N'Setting',
--    @level1type = N'TABLE',
--    @level1name = N'ReferenceColumnDefine',
--    @level2type = N'COLUMN',
--    @level2name = N'LookupTableId'
--GO

/*
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'SourceMainTableName',
    @level0type = N'SCHEMA',
    @level0name = N'Setting',
    @level1type = N'TABLE',
    @level1name = N'ReferenceColumnDefine',
    @level2type = N'COLUMN',
    @level2name = N'LookupTableName'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'SourceMainKeyColumnId',
    @level0type = N'SCHEMA',
    @level0name = N'Setting',
    @level1type = N'TABLE',
    @level1name = N'ReferenceColumnDefine',
    @level2type = N'COLUMN',
    @level2name = N'LookupColumnId'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'SourceMainKeyColumnName',
    @level0type = N'SCHEMA',
    @level0name = N'Setting',
    @level1type = N'TABLE',
    @level1name = N'ReferenceColumnDefine',
    @level2type = N'COLUMN',
    @level2name = N'LookupColumnName'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'SourceMainKeyColumnFunction',
    @level0type = N'SCHEMA',
    @level0name = N'Setting',
    @level1type = N'TABLE',
    @level1name = N'ReferenceColumnDefine',
    @level2type = N'COLUMN',
    @level2name = N'LookupColumnFunction'
GO
*/