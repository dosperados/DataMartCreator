CREATE TABLE [Setting].[ForeingColumnDefine] (
    [ForeingColumnDefineId] INT             IDENTITY (7000, 1) NOT NULL,
    [ForeingKeyDefineId]    INT             NOT NULL,
    [ColumnId]              INT             NOT NULL,
    [TableId]               INT             NOT NULL,
    [Default]               BIT             NOT NULL,
    [Description]           NVARCHAR (2500) NULL,
    [ModifiedAt]            DATETIME        NOT NULL,
    [ModifiedBy]            NVARCHAR (128)  NOT NULL,
    [Deleted]               TINYINT         NOT NULL,
    PRIMARY KEY CLUSTERED ([ForeingColumnDefineId] ASC),
    CONSTRAINT [FK_ForeingColumnDefine_ForeingKeyDefineId_Setting_ForeingKeyDefine] FOREIGN KEY ([ForeingKeyDefineId]) REFERENCES [Setting].[ForeingKeyDefine]([ForeingKeyDefineId]), 
    CONSTRAINT [FK_ForeingColumnDefine_ColumnId_Setting_Column] FOREIGN KEY ([ColumnId]) REFERENCES [Setting].[Column]([ColumnId]), 
    CONSTRAINT [FK_ForeingColumnDefine_TableId_Setting_Table] FOREIGN KEY ([TableId]) REFERENCES [Setting].[Table]([TableId]),
);


