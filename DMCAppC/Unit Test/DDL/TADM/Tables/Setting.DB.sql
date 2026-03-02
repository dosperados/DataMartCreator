CREATE TABLE [Setting].[DB] (
    [DBId]        INT             IDENTITY (10, 1) NOT NULL,
    [DBName]      NVARCHAR (128)  NOT NULL,
    [Description] NVARCHAR (2500) NULL,
    [Connection]  NVARCHAR (255)  NULL,
    [ModifiedAt]  DATETIME        CONSTRAINT [df_Setting_DB_ModifiedAt] DEFAULT (getdate()) NOT NULL,
    [ModifiedBy]  [sysname]       CONSTRAINT [df_Setting_DB_ModifiedBy] DEFAULT ('SYSTEM') NOT NULL,
    [Deleted]     TINYINT         CONSTRAINT [df_Setting_DB_Deleted] DEFAULT ((0)) NOT NULL,
    PRIMARY KEY CLUSTERED ([DBId] ASC)
);


