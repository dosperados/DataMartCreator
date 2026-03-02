CREATE TABLE [Setting].[DefaultColumnNameChange] (
    [DefaultColumnNameChangeId] INT            IDENTITY (5000, 1) NOT NULL,
    [SourceTableName]    NVARCHAR (128) NOT NULL,
    [SourceColumnName]   NVARCHAR (128) NOT NULL,
    [DestTableName]      NVARCHAR (128) NOT NULL,
    [DestColumnName]     NVARCHAR (128) NOT NULL,
    [ModifiedAt]         DATETIME       NOT NULL,
    [ModifiedBy]         NVARCHAR (128) NOT NULL,
    [Deleted]            TINYINT        NOT NULL,
    PRIMARY KEY CLUSTERED ([DefaultColumnNameChangeId] ASC),
    CONSTRAINT [UC_Setting_DefaultColumnNameChange] UNIQUE NONCLUSTERED ([SourceTableName] ASC, [SourceColumnName] ASC, [DestTableName] ASC, [DestColumnName] ASC)
);


