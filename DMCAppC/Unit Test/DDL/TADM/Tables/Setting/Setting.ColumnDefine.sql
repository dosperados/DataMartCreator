CREATE TABLE [Setting].[ColumnDefine] (
    [ColumnDefineId]         INT             IDENTITY (2000, 1) NOT NULL,
    [ReferenceTableDefineId] INT             NOT NULL,
    [TableDefineId]          INT             NOT NULL,
    [VersionId]              INT             NOT NULL,
    [SourceDBId]             INT             NOT NULL,
    [SourceDBName]           NVARCHAR (128)  NOT NULL,
    [SourceTableId]          INT             NOT NULL,
    [SourceTableName]        NVARCHAR (128)  NOT NULL,
    [SourceColumnId]         INT             NOT NULL,
    [SourceColumnName]       NVARCHAR (128)  NOT NULL,
    [CustomeStatment]        NVARCHAR (MAX)  NULL,
    [DestDBId]               INT             NOT NULL,
    [DestDBName]             NVARCHAR (128)  NOT NULL,
    [DestTableId]            INT             NOT NULL,
    [DestTableName]          NVARCHAR (128)  NOT NULL,
    [DestColumnId]           INT             NOT NULL,
    [DestColumnName]         NVARCHAR (128)  NOT NULL,
    [ColumnSortNo]           FLOAT (53)      NOT NULL,
    [KeySortNo]              FLOAT (53)      NOT NULL,
    [Description]            NVARCHAR (2500) NULL,
    [ModifiedAt]             DATETIME        CONSTRAINT [df_Setting_ColumnDefine_ModifiedAt] DEFAULT (getdate()) NOT NULL,
    [ModifiedBy]             [sysname]       CONSTRAINT [df_Setting_ColumnDefine_ModifiedBy] DEFAULT ('SYSTEM') NOT NULL,
    [Deleted]                TINYINT         CONSTRAINT [df_Setting_ColumnDefine_Deleted] DEFAULT ((0)) NOT NULL,
    PRIMARY KEY CLUSTERED ([ColumnDefineId] ASC),
    CONSTRAINT [FK_ColumnDefine_DestColumnId_Setting_Column_ColumnId] FOREIGN KEY ([DestColumnId]) REFERENCES [Setting].[Column] ([ColumnId]),
    CONSTRAINT [FK_ColumnDefine_SourceColumnId_Setting_Column_ColumnId] FOREIGN KEY ([SourceColumnId]) REFERENCES [Setting].[Column] ([ColumnId])
);


GO;

/* disable CONSTRAINT *
ALTER TABLE [Setting].[ColumnDefine] NOCHECK CONSTRAINT [FK_ColumnDefine_Setting_TableDefine];
GO;
*/

/* Enable CONSTRAINT
ALTER TABLE [Setting].[TableColumnDefines] WITH CHECK CHECK CONSTRAINT [FK_TableColumnDefines_Setting_TableHeadDefines]];
GO;
*/
