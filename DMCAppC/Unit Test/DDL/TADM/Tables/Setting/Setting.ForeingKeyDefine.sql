CREATE TABLE [Setting].[ForeingKeyDefine] (
    [ForeingKeyDefineId] INT             IDENTITY (6000, 1) NOT NULL,
    [ColumnDefineId]     INT             NOT NULL,
    [ForeingTableId]     INT             NOT NULL,
    [ForeingColumnId]    INT             NOT NULL,
    [CheckType]          NVARCHAR (50)   NOT NULL,
    [Description]        NVARCHAR (2500) NULL,
    [ModifiedAt]         DATETIME        NOT NULL,
    [ModifiedBy]         NVARCHAR (128)  NOT NULL,
    [Deleted]            TINYINT         NOT NULL,
    PRIMARY KEY CLUSTERED ([ForeingKeyDefineId] ASC),
    CONSTRAINT [FK_ForeingKeyDefine_ColumnDefineId_Setting_ColumnDefine_ColumnDefineId] FOREIGN KEY ([ColumnDefineId]) REFERENCES [Setting].[ColumnDefine]([ColumnDefineId]), 
    CONSTRAINT [FK_ForeingKeyDefine_ForeingTableId_Setting_Table_TableId] FOREIGN KEY ([ForeingTableId]) REFERENCES [Setting].[Table]([TableId]), 
    CONSTRAINT [FK_ForeingKeyDefine_ForeingColumnId_Setting_Column_ColumnId] FOREIGN KEY ([ForeingColumnId]) REFERENCES [Setting].[Column]([ColumnId]),
);


