CREATE TABLE [Setting].[Table]
(
	[TableId] INT NOT NULL PRIMARY KEY IDENTITY(100,1),
	[DBId] INT NOT NULL, /* ссылка на базу данных */
	[VersionId] INT NOT NULL,  /* версионирование записей, скорее всего поле будет удалено, так как историю изменений будет реальзована на встроенных функциях MS SQL */ 
	[TableName] NVARCHAR(128) NOT NULL, /* Название таблицы */
	[SchemaName] NVARCHAR(128) NOT NULL, /* схема для этой таблицы */
	[Type] NVARCHAR(128) NULL,  /* Тип таблицы, на текущий момент их три Dest - Таблица назначения, , Source - Таблица источник, OptionList - виртуальная таблица для специальных связе по типу OptionList */
	[MainTableName] NVARCHAR(128) NULL, /* имя таблицы без дополнительной информации */
	[MainColumnName] NVARCHAR(128) NULL, /* имя столбца - нужно для спец связи по типу OptionList */
	[Description] NVARCHAR(2500) NULL, /* дополнительное, не обязательное поле, основное предназначение для заметок */
	[ModifiedAt]               DATETIME        CONSTRAINT [df_Setting_Table_ModifiedAt] DEFAULT (getdate()) NOT NULL, /* Когда внесены изменения */
    [ModifiedBy]               [sysname]       CONSTRAINT [df_Setting_Table_ModifiedBy] DEFAULT ('SYSTEM') NOT NULL, /* Кем внесены изменения (пользователь) */
    [Deleted]                  TINYINT         CONSTRAINT [df_Setting_Table_Deleted] DEFAULT 0 NOT NULL, /* устанавливается в 1, если запись удалена, Soft Delete, сама запись не удаляется */
    CONSTRAINT [FK_Table_Setting_DB] FOREIGN KEY ([DBId]) REFERENCES [Setting].[DB]([DBId]), 
    --CONSTRAINT [FK_Table_Setting_Version] FOREIGN KEY ([VersionId]) REFERENCES [Setting].[Version]([VersionId]), 
    CONSTRAINT [CK_Table_UNIQUE] UNIQUE ([DBId], [VersionId], [TableName],[SchemaName]),
)
