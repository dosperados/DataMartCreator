CREATE TABLE [Setting].[IndexDefine] (
    [IndexDefineId]  INT             IDENTITY (8000, 1) NOT NULL,
    [ColumnDefineId] INT             NOT NULL,
    [IndexType]      NVARCHAR (50)   NOT NULL,
    [Description]    NVARCHAR (2500) NULL,
    [ModifiedAt]     DATETIME        NOT NULL,
    [ModifiedBy]     NVARCHAR (128)  NOT NULL,
    [Deleted]        TINYINT         NOT NULL,
    PRIMARY KEY CLUSTERED ([IndexDefineId] ASC),
    CONSTRAINT [FK_IndexDefine_TableDefineId_Setting_TableDefine] FOREIGN KEY ([ColumnDefineId]) REFERENCES [Setting].[ColumnDefine]([ColumnDefineId]), 
);


