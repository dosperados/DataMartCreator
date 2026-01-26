CREATE TABLE [Setting].[ViewDefine] (
    [ViewDefineId]           INT             IDENTITY (9000, 1) NOT NULL,
    [ReferenceTableDefineId] INT             NOT NULL,
    [TableDefineId]          INT             NOT NULL,
    [VersionId]              INT             NOT NULL,
    [SourceDBId]             INT             NOT NULL,
    [SourceTableId]          INT             NOT NULL,
    [SourceColumnId]         INT             NOT NULL,
    [CustomeStatment]        NVARCHAR (MAX)  NULL,
    [DestDBId]               INT             NOT NULL,
    [DestTableId]            INT             NOT NULL,
    [DestColumnId]           INT             NOT NULL,
    [ColumnSortNo]           FLOAT (53)      NOT NULL,
    [Description]            NVARCHAR (2500) NULL,
    [ViewType]               NVARCHAR (20)   NOT NULL,
    [ModifiedAt]             DATETIME        CONSTRAINT [df_Setting_ViewDefine_ModifiedAt] DEFAULT (getdate()) NOT NULL,
    [ModifiedBy]             [sysname]       CONSTRAINT [df_Setting_ViewDefine_ModifiedBy] DEFAULT ('SYSTEM') NOT NULL,
    [Deleted]                TINYINT         CONSTRAINT [df_Setting_ViewDefine_Deleted] DEFAULT ((0)) NOT NULL,
    PRIMARY KEY CLUSTERED ([ViewDefineId] ASC),
    CONSTRAINT [FK_ViewDefine_DestColumnId_Setting_Column_ColumnId] FOREIGN KEY ([DestColumnId]) REFERENCES [Setting].[Column] ([ColumnId]),
    CONSTRAINT [FK_ViewDefine_SourceColumnId_Setting_Column_ColumnId] FOREIGN KEY ([SourceColumnId]) REFERENCES [Setting].[Column] ([ColumnId])
);

