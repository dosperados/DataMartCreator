USE [TADWH]
GO
--
Declare @Proc_text nvarchar(max)
	,@VersionId INT = 1
	,@EnableOptionHashId INT = 0
--
	,@SourceDBName nvarchar(128) = 'TADWH'
	,@SourceDBId INT
	,@SourceSchema nvarchar(128) = 'dbo' --@schemaName
	,@SourceMainTable nvarchar(128)  = 'ServiceInvoiceLine'--'Customer' --'TACtrServicePackageLine' --@SourceTableName
	,@SourceMainTableId INT
--
	,@DestDBName nvarchar(128) = 'TADM'
	,@DestDBId Int
	,@DestSchema nvarchar(128) = 'dbo'
	,@DestMainTable nvarchar(128) = 'dimServiceInvoiceLine'--'dimCustomer' --'dimContractServicePackageLine' --@DMTableName
	,@DestMainTableId INT 
	,@DestColumnFunction Nvarchar(max) = NULL
-- ,@NoColumn nvarchar(128) = 'No'--,@CodeColumn nvarchar(128) = 'Code'
	
	,@TableDefineId int
	,@SourceReferenceTableDefineId INT
	,@SourceTableAOLId int

	,@RefTableName01 nvarchar(128) = null --'TAContractLine'
	,@RefTable01Id int
	,@ReferenceTableDefine01Id int
	
	,@RefTableName02 nvarchar(128) = NULL --'TAContractLine' --OptionList Temp Table
	,@RefColName02 nvarchar(128) = NULL --'TAStatus' --OptionList Temp Column
	,@RefTableName02Id int
	,@ReferenceTableDefine02Id int
	,@RefColumnFunction Nvarchar(max) = NULL


Declare  @SourceColumnName NVARCHAR(128) --= 'CompanyId' 
		,@RefKeyColumnName NVARCHAR(128) --= 'CompanyId'
		,@SourceColumnId int
		,@RefKeyColumnId int

DROP TABLE IF EXISTS #OptionColumnList;
CREATE TABLE #OptionColumnList (
	[ColumnName] NVARCHAR(128) NOT NULL,
	[SortNo] FLOAT NOT NULL,
	[DataType] nvarchar(128) NOT NULL,
	[MaxLength] int NULL,
	[Precision] tinyint NULL,
	[Scale] int NULL,
	[CollationName] nvarchar(255) NULL,
	[DefaultValue] nvarchar(255) NULL,
	[KeySortNo] float NOT NULL,
	[IsNullable] int NOT NULL,
	[IsOptionField] int NOT NULL,
	[FieldType] nvarchar(128) NULL,
	);
INSERT INTO #OptionColumnList ([ColumnName],[SortNo],[DataType], [MaxLength], [Precision], [Scale], [CollationName], [DefaultValue], [KeySortNo], [IsNullable], [IsOptionField],[FieldType])
	VALUES   ('OptionId', 0.40, 'nvarchar', 162, NULL, NULL, 'Latin1_General_CI_AS',NULL, 0,0,1,'OptionId')
			/*,('OptionNo', 0.10, 'INT', NULL, 10, 0, NULL, NULL, 0,0,1,'OptionNo')
			,('OptionDE', 0.20, 'nvarchar', 128, NULL, NULL, 'Latin1_General_CI_AS',NULL, 0,0,1,'OptionDE')
			,('OptionENU', 0.30, 'nvarchar', 128, NULL, NULL, 'Latin1_General_CI_AS',NULL, 0,0,1,'OptionENU')*/
			;


--Set [Setting].[ColumnNameChange]
INSERT INTO [TADM].[Setting].[DefaultColumnNameChange] ([SourceTableName], [SourceColumnName], [DestTableName], [DestColumnName], [Deleted], [ModifiedAt], [ModifiedBy])
Select a.[SourceTableName], a.[SourceColumnName], a.[DestTableName], a.[DestColumnName], 0 as [Deleted], GETDATE() as [ModifiedAt], ORIGINAL_LOGIN() AS [ModifiedBy]
FROM (
SELECT	 'ServiceInvoiceLine' AS [SourceTableName]
		,'TAFSMDocNo' as [SourceColumnName]
		,'dimServiceInvoiceLine' AS [DestTableName]
		,'FSMDocumentNo' as [DestColumnName]
UNION
SELECT	 'ServiceInvoiceLine' AS [SourceTableName]
		,'DocumentNo' as [SourceColumnName]
		,'dimServiceInvoiceLine' AS [DestTableName]
		,'ServiceInvoiceNo' as [DestColumnName]
UNION
SELECT	 'ServiceInvoiceLine' AS [SourceTableName]
		,'No' as [SourceColumnName]
		,'dimServiceInvoiceLine' AS [DestTableName]
		,'ItemNo' as [DestColumnName]
UNION
SELECT	 'ServiceInvoiceLine' AS [SourceTableName]
		,'LineNo' as [SourceColumnName]
		,'dimServiceInvoiceLine' AS [DestTableName]
		,'DocumentLineNo' as [DestColumnName]


) as a
	LEFT JOIN [TADM].[Setting].[DefaultColumnNameChange] as b
		ON a.[DestTableName] = b.[DestTableName]
			AND a.[DestColumnName] = b.[DestColumnName]
			AND a.[SourceTableName] = b.[SourceTableName]
			AND a.[SourceColumnName] = b.[SourceColumnName]
	WHERE b.[DestColumnName] IS NULL

Select @VersionId = Max([VersionId]) FROM [TADM].[Setting].[Version]
PRINT('@VersionId='+CAST(@VersionId as nvarchar(10)));

--DB
--Source - DBName--
IF EXISTS (Select [DBId] FROM [TADM].[Setting].[DB] WHERE [DBName] = @SourceDBName)
	BEGIN
		SELECT @SourceDBId = [DBId] FROM [TADM].[Setting].[DB] WHERE [DBName] = @SourceDBName 
		PRINT('@SourceDBName: '+@SourceDBName + 'SET @SourceDBId='+CAST(@SourceDBId as nvarchar(15)))
	END
IF NOT EXISTS (Select [DBId] FROM [TADM].[Setting].[DB] WHERE [DBName] = @SourceDBName)
	BEGIN
		PRINT('Insert @SourceDBName: '+@SourceDBName)
		INSERT INTO [TADM].[Setting].[DB] ([DBName],[Description],[ModifiedBy])
		VALUES(@SourceDBName,'Source', ORIGINAL_LOGIN());
		SET @SourceDBId = @@IDENTITY;
		PRINT('@SourceDBId='+CAST(@SourceDBId as nvarchar(15)))
	END
-- Dest - DBName--
IF EXISTS (Select [DBId] FROM [TADM].[Setting].[DB] WHERE [DBName] = @DestDBName)
	BEGIN
		SELECT @DestDBId = [DBId] FROM [TADM].[Setting].[DB] WHERE [DBName] = @DestDBName 
		PRINT('@DestDBName: '+@DestDBName+' SET @DestDBId='+CAST(@DestDBId as nvarchar(15)))
	END
IF NOT EXISTS (Select [DBId] FROM [TADM].[Setting].[DB] WHERE [DBName] = @DestDBName)
	BEGIN
		PRINT('Insert @DestDBName: '+@DestDBName)
		INSERT INTO [TADM].[Setting].[DB] ([DBName],[Description],[ModifiedBy])
		VALUES(@DestDBName,'Dest', ORIGINAL_LOGIN());
		SET @DestDBId = @@IDENTITY;
		PRINT('@DestDBId='+CAST(@DestDBId as nvarchar(15)))
	END

DROP TABLE IF EXISTS #AOL;
SELECT aol.[TableNameDWH]
,aol.[FieldNameDWH]
,aol.[TableNameDWH]+'_AOL' AS [RefTableName]
,aol.[FieldNameDWH] AS [RefColumnName]
,DENSE_RANK() OVER ( ORDER BY aol.[TableNo] asc, aol.[FieldNo] ASC) AS [ColumnSortNo]
, aol.[OptionId]
,CAST(HASHBYTES('SHA2_256', aol.[OptionId]) AS VARBINARY(256)) as [OptionHashId]
,aol.[TableNo]
,aol.[FieldNo]
,aol.[OptionNo]
, aol.[OptionDE]
, aol.[OptionENU]
INTO #AOL
FROM [TADWH].[dbo].[ActiveOptionList] as aol
	WHERE aol.[TableNameDWH] = @SourceMainTable 

--SourceTable_OptionList --Column
PRINT('INTO #SourceTable_OptionList')
DROP table if exists #SourceTable_OptionList;
SELECT aol.[TableNameDWH]
,aol.[FieldNameDWH]
,aol.[TableNameDWH]+'_AOL' AS [RefTableName] /*+aol.[FieldNameDWH]*/
,aol.[FieldNameDWH] AS [RefColumnName]
,'RefOption_'+aol.[FieldNameDWH] AS [RefTableAliasName]
,AOL.[ColumnSortNo]
,CAST(NULL AS nvarchar(4)) [RefLevelNo]
, aol.[OptionId]
,aol.[OptionHashId]
,aol.[TableNo]
,aol.[FieldNo]
,aol.[OptionNo]
, aol.[OptionDE]
, aol.[OptionENU]
,CAST(NULL as INT) AS ReferenceTableDefineId
INTO #SourceTable_OptionList
FROM #AOL as aol

--SourceTable_OptionList
PRINT('UPDATE #SourceTable_OptionList - SET A.[RefLevelNo] CASE ... = 1')
UPDATE A
	SET A.[RefLevelNo] = CASE WHEN LEN(a.[ColumnSortNo]) = 1 THEN '0'+CAST(a.[ColumnSortNo] AS NVARCHAR(4)) ELSE CAST(a.[ColumnSortNo] AS NVARCHAR(4)) END
FROM #SourceTable_OptionList  as A;


--Table
-- Source - TableName--
IF EXISTS (Select [TableId] FROM [TADM].[Setting].[Table] WHERE [DBId] = @SourceDBId AND [VersionId] = @VersionId  AND [SchemaName] = @SourceSchema AND [TableName] = @SourceMainTable)
	BEGIN
		SELECT @SourceMainTableId = [TableId] FROM [TADM].[Setting].[Table] WHERE [DBId] = @SourceDBId AND [VersionId] = @VersionId  AND [SchemaName] = @SourceSchema AND [TableName] = @SourceMainTable
		PRINT('@SourceMainTable: '+@SourceMainTable +' SET @SourceMainTableId='+CAST(@SourceMainTableId as nvarchar(15)))
	END
IF NOT EXISTS (Select [TableId] FROM [TADM].[Setting].[Table] WHERE [DBId] = @SourceDBId AND [VersionId] = @VersionId  AND [SchemaName] = @SourceSchema AND [TableName] = @SourceMainTable)
	BEGIN
		PRINT('Insert @SourceMainTable: '+@SourceMainTable)
		INSERT INTO [TADM].[Setting].[Table] ([DBId], [VersionId], [TableName], [SchemaName],[Description],[Type], [ModifiedBy])
		VALUES(@SourceDBId, @VersionId, @SourceMainTable, @SourceSchema, 'Source','Source', ORIGINAL_LOGIN());
		SET @SourceMainTableId = @@IDENTITY;
		PRINT('@SourceMainTableId='+CAST(@SourceMainTableId as nvarchar(15)))
	END
-- Dest - TableName--
IF EXISTS (Select [TableId] FROM [TADM].[Setting].[Table] WHERE [DBId] = @DestDBId AND [VersionId] = @VersionId  AND [SchemaName] = @DestSchema AND [TableName] = @DestMainTable)
	BEGIN
		SELECT @DestMainTableId = [TableId] FROM [TADM].[Setting].[Table] WHERE [DBId] = @DestDBId AND [VersionId] = @VersionId  AND [SchemaName] = @DestSchema AND [TableName] = @DestMainTable
		PRINT('@DestMainTable: '+@DestMainTable+' SET @DestMainTableId='+CAST(@DestMainTableId as nvarchar(15)))
	END
IF NOT EXISTS (Select [TableId] FROM [TADM].[Setting].[Table] WHERE [DBId] = @DestDBId AND [VersionId] = @VersionId  AND [SchemaName] = @DestSchema AND [TableName] = @DestMainTable)
	BEGIN
		PRINT('Insert @DestMainTable: '+@DestMainTable)
		INSERT INTO [TADM].[Setting].[Table] ([DBId], [VersionId], [TableName], [SchemaName],[Description],[Type], [ModifiedBy])
		VALUES(@DestDBId, @VersionId, @DestMainTable, @DestSchema, 'Dest','Dest', ORIGINAL_LOGIN());
		SET @DestMainTableId = @@IDENTITY;
		PRINT('@DestMainTableId='+CAST(@DestMainTableId as nvarchar(15)))
	END

--Source AOL
IF EXISTS (Select [TableId] FROM [TADM].[Setting].[Table] WHERE [DBId] = @SourceDBId AND [VersionId] = @VersionId  AND [SchemaName] = @SourceSchema AND [TableName] = @SourceMainTable+'_AOL')
	BEGIN
		SELECT @SourceTableAOLId = [TableId] FROM [TADM].[Setting].[Table] WHERE [DBId] = @SourceDBId AND [VersionId] = @VersionId  AND [SchemaName] = @SourceSchema AND [TableName] = @SourceMainTable+'_AOL'
		PRINT('Source Table AOL : '+@SourceMainTable+'_AOL' +' SET @SourceTableAOLId='+CAST(@SourceTableAOLId as nvarchar(15)))
	END
IF NOT EXISTS (Select [TableId] FROM [TADM].[Setting].[Table] WHERE [DBId] = @SourceDBId AND [VersionId] = @VersionId  AND [SchemaName] = @SourceSchema AND [TableName] = @SourceMainTable+'_AOL')
	BEGIN
		PRINT('Insert Source Table AOL: '+@SourceMainTable+'_AOL')
		INSERT INTO [TADM].[Setting].[Table] ([DBId], [VersionId], [TableName], [SchemaName],[Description],[Type], [ModifiedBy])
		VALUES(@SourceDBId, @VersionId, @SourceMainTable+'_AOL', @SourceSchema, 'Source','OptionList', ORIGINAL_LOGIN());
		SET @SourceTableAOLId = @@IDENTITY;
		PRINT('@SourceTableAOLId='+CAST(@SourceTableAOLId as nvarchar(15)))
	END


--TableDefine
IF EXISTS (Select [TableDefineId] FROM [TADM].[Setting].[TableDefine] WHERE [SourceMainDBId] = @SourceDBId AND [SourceMainTableId] = @SourceMainTableId  AND [DestMainDBId] = @DestDBId AND [DestMainTableId] = @DestMainTableId)
	BEGIN
		SELECT @TableDefineId = [TableDefineId] FROM [TADM].[Setting].[TableDefine] WHERE [SourceMainDBId] = @SourceDBId AND [SourceMainTableId] = @SourceMainTableId  AND [DestMainDBId] = @DestDBId AND [DestMainTableId] = @DestMainTableId
		PRINT('@SourceMainTable: '+@SourceMainTable+'-> @DestMainTable:'+@DestMainTable+' SET @TableDefineId='+CAST(@TableDefineId as nvarchar(15)))
	END
IF NOT EXISTS (Select [TableDefineId] FROM [TADM].[Setting].[TableDefine] WHERE [SourceMainDBId] = @SourceDBId AND [SourceMainTableId] = @SourceMainTableId  AND [DestMainDBId] = @DestDBId AND [DestMainTableId] = @DestMainTableId)
	BEGIN
		PRINT('Insert @TableDefineId: '+@DestSchema+'.'+@DestMainTable)
		INSERT INTO [TADM].[Setting].[TableDefine] ([VersionId], [SourceMainDBId], [SourceMainDBName], [SourceMainSchemaName], [SourceMainTableId], [SourceMainTableName], [DestMainDBId], [DestMainDBName], [DestMainSchemaName], [DestMainTableId], [DestMainTableName], [ModifiedBy])
		VALUES(@VersionId, @SourceDBId, @SourceDBName, @SourceSchema,  @SourceMainTableId, @SourceMainTable, @DestDBId, @DestDBName, @DestSchema, @DestMainTableId, @DestMainTable,  ORIGINAL_LOGIN());
		SET @TableDefineId = @@IDENTITY;
		PRINT('@TableDefineId='+CAST(@TableDefineId as nvarchar(15)))
	END

--ReferenceTableDefine -0 --SourceMainTable
IF EXISTS (Select [ReferenceTableDefineId] FROM [TADM].[Setting].[ReferenceTableDefine] WHERE [TableDefineId] = @TableDefineId 
	AND [VersionId] = @VersionId AND [RefTableName] = @SourceMainTable AND [RefTableId] = @SourceMainTableId AND [RefTableAliasName] = 'SourceMainTable'
	AND [RefLevelNo] = '0')
	BEGIN
		SELECT @SourceReferenceTableDefineId = [ReferenceTableDefineId] FROM [TADM].[Setting].[ReferenceTableDefine] WHERE [TableDefineId] = @TableDefineId AND [VersionId] = @VersionId AND [RefTableName] = @SourceMainTable AND [RefTableId] = @SourceMainTableId AND [RefTableAliasName] = 'SourceMainTable' AND [RefLevelNo] = '0'
		PRINT('@SourceMainTable: '+@SourceMainTable+' SET @SourceReferenceTableDefineId='+CAST(@SourceReferenceTableDefineId as nvarchar(15)))
	END
IF NOT EXISTS (Select [ReferenceTableDefineId] FROM [TADM].[Setting].[ReferenceTableDefine] WHERE [TableDefineId] = @TableDefineId 
	AND [VersionId] = @VersionId AND [RefTableName] = @SourceMainTable AND [RefTableId] = @SourceMainTableId AND [RefTableAliasName] = 'SourceMainTable'
	AND [RefLevelNo] = '0')
	BEGIN
		INSERT INTO [TADM].[Setting].[ReferenceTableDefine] ([TableDefineId], [VersionId], [RefTableName], [RefTableId], [RefTableAliasName], [CustomeTableSelect], [RefLevelNo], [RefType], [JoinType], [Description], [ModifiedBy])
			VALUES(@TableDefineId, @VersionId, @SourceMainTable, @SourceMainTableId, 'SourceMainTable', NULL, '0', 'Main', 'INNER JOIN', 'SourceMainTable',  ORIGINAL_LOGIN());
		SET @SourceReferenceTableDefineId = @@IDENTITY;
		PRINT('@SourceMainTable: '+@SourceMainTable+' INSERT @SourceReferenceTableDefineId='+CAST(@SourceReferenceTableDefineId as nvarchar(15)));
	END

DROP TABLE IF EXISTS #DefaultTableReference;
SELECT  [TADM].[Setting].[fGetTableDefineId](a.[SourceDBName], a.[SourceTableName], a.[DestDBName], a.[DestTableName]) AS [TableDefineId]
		,a.[SourceDBName]
		,[TADM].[Setting].[fGetDbId]([SourceDBName]) AS [SourceDBId]
		,a.[SourceTableName]
		,[TADM].[Setting].[fGetTableId]([SourceTableName], [SourceDBName], 1) AS [SourceTableId]
		,a.[SourceColumnName]
		,[TADM].[Setting].[fGetColumnId]([SourceColumnName], [SourceTableName], [SourceDBName], 1) AS [SourceColumnId]
		,a.[DestDBName]
		,[TADM].[Setting].[fGetDbId]([DestDBName]) AS [DestDBId]
		,a.[DestTableName]
		,[TADM].[Setting].[fGetTableId]([DestTableName], [DestDBName], 1) AS [DestTableId]
		,a.[DestColumnName]
		,[TADM].[Setting].[fGetColumnId]([DestColumnName], [DestTableName], [DestDBName], 1) AS [DestColumnId]
		,@DestColumnFunction as [@DestColumnFunction]
		,a.[RefDBName]
		,[TADM].[Setting].[fGetDbId]([RefDBName]) as [RefDBId]
		,a.[RefSchemaName]
		,a.[RefTableName]
		,[TADM].[Setting].[fGetTableId]([RefTableName], [RefDBName], 1)  as [RefTableId]
		,'Ref'+a.[RefTableName]+'_'+a.[RefColumnName] as [RefTableAliasName]
		,CAST(DENSE_RANK() OVER(order by [RefTableName],a.[SourceColumnName]) AS NVARCHAR(3)) as [RefLevelNo]
		,a.[RefColumnName]
		,[TADM].[Setting].[fGetColumnId]([RefColumnName],[RefTableName], [RefDBName], 1) as [RefColumnId]
		,a.[RefColumnIdName]
		,@RefColumnFunction as [@RefColumnFunction]
		,[TADM].[Setting].[fGetColumnId]([RefColumnIdName],[RefTableName], [RefDBName], 1) AS [RefColumnIdNameID]
		,a.[AddRefDestColumnIdName]
		,[TADM].[Setting].[fGetColumnId]([AddRefDestColumnIdName],[DestTableName], [DestDBName], 1) AS [AddRefDestColumnIdNameID]
	INTO #DefaultTableReference
FROM [TADM].[Setting].[DefaultTableReference] as a 
	WHERE a.[SourceTableName] = @SourceMainTable AND a.[DestTableName] = @DestMainTable


-- RefTable Add #Column
DROP TABLE IF EXISTS #Column;
	SELECT  a.[DestTableId] AS [TableId]
			,[VersionId]
			,a.[AddRefDestColumnIdName] AS [ColumnName]
			,a.[RefDBName]+'.'+[RefTableName]+'.'+[RefColumnIdName]+'|'+a.[DestDBName]+'.'+[DestTableName]+'.'+a.[AddRefDestColumnIdName] AS [Description]
			,c.[ColumnSortNo]+0.1 AS [ColumnSortNo]
			,c.[DataType],c.[MaxLength],c.[Precision],c.[Scale],c.[CollationName],c.[DefaultValue]
			,0 as [KeySortNo],c.[IsNullable],c.[IsOptionField]
			,'RefID' as [FieldType]
			,1 as [IsLookupField]
			,GETDATE() as [ModifiedAt]
			,ORIGINAL_LOGIN() as [ModifiedBy]
			,0 as [Deleted]
	INTO #Column
	FROM #DefaultTableReference as a
		INNER JOIN [TADM].[Setting].[Column] as c
		ON a.[RefColumnIdNameID] = c.[ColumnId]
	
	PRINT('Add new Column to [Setting].[Column]');
	INSERT INTO [TADM].[Setting].[Column] ([TableId],[VersionId],[ColumnName],[Description],[ColumnSortNo],[DataType],[MaxLength],[Precision],[Scale],[CollationName],[DefaultValue],[KeySortNo],[IsNullable],[IsOptionField],[FieldType],[IsLookupField],[ModifiedAt],[ModifiedBy],[Deleted])
	Select a.* FROM #Column as a
		LEFT JOIN [TADM].[Setting].[Column] as b
			ON a.[TableId] = b.[TableId]
				AND a.[ColumnName] = b.[ColumnName]
	WHERE b.[TableId] IS NULL

	DROP TABLE IF EXISTS #ReferenceTableDefine;
	SELECT   DISTINCT [TADM].[Setting].[fGetTableDefineId](a.[SourceDBName], a.[SourceTableName], a.[DestDBName], a.[DestTableName]) as [TableDefineId]
			,1 as [VersionId]
			,[TADM].[Setting].[fGetReferenceTableDefineId]([TADM].[Setting].[fGetTableDefineId](a.[SourceDBName], a.[SourceTableName], a.[DestDBName], a.[DestTableName]), a.[SourceTableId] ,1) as [ParentReferenceTableDefineId]
			,a.[RefTableName]
			,a.[RefTableId]
			,a.[SourceColumnName]
			,'Ref'+a.[RefTableName]+'_'+a.[RefColumnName] as [RefTableAliasName]
			,NULL as [CustomeTableSelect]
			,CAST(DENSE_RANK() OVER(order by [RefTableId],a.[SourceColumnName]) AS NVARCHAR(3)) as [RefLevelNo]
			,'Id' AS [RefType]
			,'LEFT JOIN' as [JoinType]
			,'Ref'+a.[RefTableName]+'_'+a.[RefColumnName]+CAST(DENSE_RANK() OVER(order by [RefTableId],a.[SourceColumnName]) AS NVARCHAR(3)) AS [Description]
			,GETDATE() as [ModifiedAt]
		,ORIGINAL_LOGIN() as [ModifiedBy]
		,0 as [Deleted]
	INTO #ReferenceTableDefine
	FROM #DefaultTableReference as a

	
	PRINT('Add new ReferenceTableDefine RefTable ');
	INSERT INTO [TADM].[Setting].[ReferenceTableDefine] ([TableDefineId],[VersionId],[ParentReferenceTableDefineId],[RefTableName],[RefTableId],[RefTableAliasName],[CustomeTableSelect],[RefLevelNo],[RefType],[JoinType],[Description],[ModifiedAt],[ModifiedBy],[Deleted])
	Select a.[TableDefineId],a.[VersionId],a.[ParentReferenceTableDefineId],a.[RefTableName],a.[RefTableId],a.[RefTableAliasName],a.[CustomeTableSelect],a.[RefLevelNo],a.[RefType],a.[JoinType],a.[Description],a.[ModifiedAt],a.[ModifiedBy],a.[Deleted]
	FROM #ReferenceTableDefine as a
		LEFT JOIN [TADM].[Setting].[ReferenceTableDefine] as b
			ON a.[TableDefineId] = b.[TableDefineId]
				AND a.[ParentReferenceTableDefineId] = b.[ParentReferenceTableDefineId]
				AND a.[RefTableId] = b.[RefTableId]
				AND a.[RefLevelNo] = b.[RefLevelNo]
	WHERE b.[ReferenceTableDefineId] IS NULL AND b.[RefLevelNo] IS NULL


	DROP TABLE IF EXISTS #ReferenceColumnDefine;
	SELECT * 
	INTO #ReferenceColumnDefine
	FROM (
	SELECT 1 as [VersionId]
		,rtd.[ParentReferenceTableDefineId] AS [SourceReferenceTableDefineId] 
		,a.[DestTableName] AS [SourceTableName]
		,[TADM].[Setting].[fGetColumnId]('CompanyId', a.[DestTableName], a.[DestDBName], 1) as [SourceColumnId]
		,NULL AS [SourceColumnFunction]
		,rtd.[ReferenceTableDefineId] as [RefReferenceTableDefineId]
		,rtd.[RefTableAliasName] as [RefTableName]
		,a.[RefTableAliasName]
		,a.[RefLevelNo]
		,[TADM].[Setting].[fGetColumnId]('CompanyId', a.[RefTableName], a.[RefDBName], 1) as [RefColumnId]
		,'CompanyId' as [RefColumnName]
		,NULL AS [RefColumnFunction]
		,1 as [KeySortNo]
		,NULL AS [Description]
		,GETDATE() as [ModifiedAt]
		,ORIGINAL_LOGIN() as [ModifiedBy]
		,0 as [Deleted]
	FROM #DefaultTableReference as a
		INNER JOIN [TADM].[Setting].[ReferenceTableDefine] as rtd
			ON a.[TableDefineId] = rtd.[TableDefineId]
				AND a.[RefTableId] = rtd.[RefTableId]
	UNION ALL
	SELECT 1 as [VersionId]
		,rtd.[ParentReferenceTableDefineId] AS [SourceReferenceTableDefineId] 
		,a.[DestTableName] AS [SourceTableName]
		,a.[DestColumnId] as [SourceColumnId]
		,a.[@DestColumnFunction] AS [SourceColumnFunction]
		,rtd.[ReferenceTableDefineId] as [RefReferenceTableDefineId]
		,rtd.[RefTableAliasName] as [RefTableName]
		,a.[RefTableAliasName]
		,a.[RefLevelNo]
		,a.[RefColumnId] as [RefColumnId] /*[TADM].[Setting].[fGetColumnId](a.[RefColumnName], a.[RefTableName], a.[RefDBName], 1)*/
		,a.[RefColumnName] as [RefColumnName]
		,a.[@RefColumnFunction] AS [RefColumnFunction]
		,2 as [KeySortNo]
		,NULL AS [Description]
		,GETDATE() as [ModifiedAt]
		,ORIGINAL_LOGIN() as [ModifiedBy]
		,0 as [Deleted]
	FROM #DefaultTableReference as a
			INNER JOIN [TADM].[Setting].[ReferenceTableDefine] as rtd
				ON a.[TableDefineId] = rtd.[TableDefineId]
					AND a.[RefTableId] = rtd.[RefTableId]
	) as a
	
--ReferenceTableDefine - OptionTable
INSERT INTO [TADM].[Setting].[ReferenceTableDefine] ([TableDefineId], [ParentReferenceTableDefineId],[VersionId], [RefTableName], [RefTableId], [RefTableAliasName], [CustomeTableSelect], [RefLevelNo], [RefType], [JoinType], [Description], [ModifiedBy])
SELECT DISTINCT @TableDefineId as [TableDefineId]
	,@SourceReferenceTableDefineId as [ParentReferenceTableDefineId]
	,@VersionId
	,SourceOpl.[RefTableName]
	,[TADM].[Setting].[fGetTableId](SourceOpl.[RefTableName], @SourceDBName, @VersionId) as [RefTableId]
	,'RefOption_'+SourceOpl.[RefColumnName] as [RefTableAliasName]
	,NULL AS [CustomeTableSelect]
	,SourceOpl.[RefLevelNo]
	,'OptionList' as [RefType]
	,'LEFT JOIN' AS [JoinType]
	,'RefOption_'+SourceOpl.[RefColumnName]+SourceOpl.[RefLevelNo] as [Description]
	,ORIGINAL_LOGIN() [ModifiedBy]
FROM #SourceTable_OptionList as SourceOpl
	LEFT JOIN [TADM].[Setting].[ReferenceTableDefine] as rtd
		ON SourceOpl.[RefTableName] = rtd.[RefTableName]
			AND rtd.[VersionId] = @VersionId
WHERE rtd.[ReferenceTableDefineId] IS NULL


--region #ColumnDefine



	PRINT('INSERT INTO [Setting].[ReferenceColumnDefine]');
	INSERT INTO [TADM].[Setting].[ReferenceColumnDefine] ([VersionId], [SourceReferenceTableDefineId], [SourceTableName], [SourceColumnId], [SourceColumnFunction], [RefReferenceTableDefineId], [RefTableName], [RefColumnId], [RefColumnName], [RefColumnFunction], [KeySortNo], [Description], [ModifiedAt], [ModifiedBy], [Deleted])
	SELECT a.[VersionId],a.[SourceReferenceTableDefineId],a.[SourceTableName],a.[SourceColumnId],a.[SourceColumnFunction],a.[RefReferenceTableDefineId],a.[RefTableName],a.[RefColumnId],a.[RefColumnName],a.[RefColumnFunction],a.[KeySortNo],a.[Description],a.[ModifiedAt],a.[ModifiedBy],a.[Deleted]
	FROM #ReferenceColumnDefine as a
		LEFT JOIN [TADM].[Setting].[ReferenceColumnDefine] as b
			ON a.[SourceReferenceTableDefineId] = b.[SourceReferenceTableDefineId]
				AND a.[SourceColumnId] = b.[SourceColumnId]
				AND a.[RefReferenceTableDefineId] = b.[RefReferenceTableDefineId]
				AND a.[RefColumnId] = b.[RefColumnId]
		WHERE b.[ReferenceColumnDefineId] IS NULL /*RefReferenceTableDefineId*/

	DROP TABLE IF EXISTS #ColumnDefine;
	SELECT rtd.[ReferenceTableDefineId] AS [ReferenceTableDefineId]
	,rtd.[TableDefineId]
	,rtd.[VersionId]
	,a.[SourceDBId]
	,a.[SourceDBName]
	,a.[SourceTableId]
	,a.[SourceTableName]
	,a.[RefColumnIdNameID] as [SourceColumnId]
	,a.[RefColumnIdName] as [SourceColumnName]
	,CASE
		WHEN RefCol.[DataType] = 'bigint' THEN 'CAST(0 AS BIGINT)' 
		WHEN RefCol.[DataType] = 'int' THEN 'CAST(0 AS INT)' 
		WHEN RefCol.[DataType] = 'nvarchar' THEN 'CAST('' AS '+RefCol.[DataType]+'('+CAST(RefCol.[MaxLength] AS nvarchar(10))+')'
		ELSE 'I don´r know [DataType] of Column [CustomeStatment] You should check it! 0.8'
	 END AS [CustomeStatment]
	,a.[DestDBId]
	,a.[DestDBName]
	,a.[DestTableId]
	,a.[DestTableName]
	,a.[AddRefDestColumnIdNameID] as [DestColumnId]
	,a.[AddRefDestColumnIdName] as [DestColumnName]
	,DestCol.[ColumnSortNo]+0.1 AS [ColumnSortNo]
	,0 as [KeySortNo]
	,rtd.[RefTableAliasName]+rtd.[RefLevelNo] AS [Description]
	,GETDATE() as [ModifiedAt]
			,ORIGINAL_LOGIN() as [ModifiedBy]
			,0 as [Deleted]
	INTO #ColumnDefine
	FROM #DefaultTableReference as a
		INNER JOIN [TADM].[Setting].[Column] as RefCol
			ON a.[RefColumnIdNameID] = RefCol.[ColumnId]
		INNER JOIN [TADM].[Setting].[Column] as DestCol
			ON a.[DestColumnId] = DestCol.[ColumnId]
		INNER JOIN [TADM].[Setting].[ReferenceTableDefine] as rtd
			ON a.[TableDefineId] = rtd.[TableDefineId]
				AND a.[RefTableId] = rtd.[RefTableId]
		INNER JOIN [TADM].[Setting].[ReferenceColumnDefine] as rcd
			ON rtd.[ReferenceTableDefineId] = rcd.[RefReferenceTableDefineId]
				AND rtd.[ParentReferenceTableDefineId] = rcd.[SourceReferenceTableDefineId]
				AND a.[DestColumnId] = rcd.[SourceColumnId];

	INSERT INTO [TADM].[Setting].[ColumnDefine] ([ReferenceTableDefineId], [TableDefineId], [VersionId], [SourceDBId], [SourceDBName], [SourceTableId], [SourceTableName], [SourceColumnId], [SourceColumnName], [CustomeStatment], [DestDBId], [DestDBName], [DestTableId], [DestTableName], [DestColumnId], [DestColumnName], [ColumnSortNo], [KeySortNo], [Description], [ModifiedAt], [ModifiedBy], [Deleted])
	SELECT a.* FROM #ColumnDefine as a
		LEFT JOIN [TADM].[Setting].[ColumnDefine] as b 
			ON a.[ReferenceTableDefineId] = b.[ReferenceTableDefineId]
				AND a.[TableDefineId] = b.[TableDefineId]
				AND a.[SourceTableId] = b.[SourceTableId]
				AND a.[SourceColumnId] = b.[SourceColumnId]
				AND a.[DestTableId] = b.[DestTableId]
				AND a.[DestColumnId] = b.[DestColumnId]
		WHERE b.[ColumnDefineId] IS NULL



--SET A.[ReferenceTableDefineId]
UPDATE A
	SET A.[ReferenceTableDefineId] = rtd.[ReferenceTableDefineId]
	/*SELECT A.[ReferenceTableDefineId] , rtd.[ReferenceTableDefineId] ,a.[RefTableName], rtd.[RefTableName],rtd.[RefTableAliasName] , a.[RefTableAliasName]*/
	FROM #SourceTable_OptionList as A
	LEFT JOIN [TADM].[Setting].[ReferenceTableDefine] as rtd
		ON a.[RefTableName] = rtd.[RefTableName]
			AND rtd.[RefTableAliasName] = a.[RefTableAliasName]
			AND rtd.[VersionId] = @VersionId
WHERE A.[ReferenceTableDefineId] IS NULL		
--end Region #ColumnDefine

--SELECT * FROM [TADM].[Setting].[ReferenceTableDefine]  WHERE [RefTableName] = 'ServiceInvoiceLine_AOL' AND [RefTableAliasName] = 
drop table if exists #ALL_ColumnDefine;
select 'ColumnDefine' as [Table]
		 ,st.[name] as [TableName]
		 ,[TADM].[Setting].[fGetTableId](st.[name], @SourceDBName, @VersionId) as [fTableId] --SourceTableId OLD [TableId]
		 ,@SourceReferenceTableDefineId AS [ReferenceTableDefineId]
		 ,@TableDefineId as [TableDefineId]
		 ,@VersionId as [VersionId]
		 ,@SourceDBId as [SourceDBId]
		 ,@SourceSchema as [SourceSchema]
		 ,@SourceDBName as [SourceDBName]
		 ,@SourceMainTableId as [SourceMainTableId] --SourceMainTableId old [SourceTableId]
		 ,@SourceMainTable as [SourceTableName] --st.name
		 ,NULL as [SourceColumnId]
		 ----
		 ,@DestDBId as [DestDBId]
		 ,@DestDBName as [DestDBName]
		 ,@DestSchema as [DestSchema]
		 ,@DestMainTableId as [DestTableId]
		 ,@DestMainTable [DestTableName]
		 ,NULL as [DestColumnId]
		 ,CAST(NULL AS NVARCHAR(max)) AS [CustomeStatment]
		,CAST(isc.[ORDINAL_POSITION] as float) as [ColumnSortNo]
		,sc.[column_id] 
		,sch.[name] as [DestSchemaName]
        ,sc.[name] as [SourceColumnName]
		,CAST(CASE 
			WHEN kcu.[COLUMN_NAME] is NOT NULL AND LEFT(@DestMainTable,3) = 'dim' THEN RIGHT(@DestMainTable,LEN(@DestMainTable)-3)+'Id'  
			WHEN kcu.[COLUMN_NAME] is NOT NULL AND LEFT(@DestMainTable,4) = 'fact' THEN RIGHT(@DestMainTable,LEN(@DestMainTable)-4)+'Id' 
			WHEN LEFT(sc.[name], 2) COLLATE Latin1_General_CS_AS = 'TA' THEN RIGHT(sc.[name],LEN(sc.[name])-2) 
			ELSE sc.[name] 
		 END AS NVARCHAR(128)) AS [DestColumnName]
		,CAST(isc.[DATA_TYPE] as NVARCHAR(128)) as [DataType]
		,Cast(isc.[CHARACTER_MAXIMUM_LENGTH] as INT) as [MaxLength]
		,isc.[NUMERIC_PRECISION] as [Precision]
		,isc.[NUMERIC_SCALE] as [Scale]
		,case isc.[IS_NULLABLE] when 'NO' then 0 else 1 end as [IsNullable]
		,sc.[collation_name] as [CollationName]
		,isc.[Column_Default] as [DefaultValue]
		,isnull(kcu.[ORDINAL_POSITION],0) as [KeySortNo]
		,kcu.[COLUMN_NAME] as [SourceKeyColumn]
		,CAST(CASE 
			WHEN kcu.[COLUMN_NAME] is NOT NULL AND LEFT(@DestMainTable,3) = 'dim' THEN RIGHT(@DestMainTable,LEN(@DestMainTable)-3)+'Id' 
			WHEN kcu.[COLUMN_NAME] is NOT NULL AND LEFT(@DestMainTable,4) = 'fact' THEN RIGHT(@DestMainTable,LEN(@DestMainTable)-4)+'Id' 
			ELSE NULL END AS NVARCHAR(128)) as [DestKeyColumn]
		,CASE WHEN OpL.[FieldNameDWH] IS NULL THEN 0 ELSE 1 END AS [IsOptionField]
		,CAST(CASE WHEN OpL.[FieldNameDWH] IS NOT NULL THEN 'OptionNo' ELSE NULL END AS NVARCHAR(128)) AS [FieldType]
		,CASE WHEN OpL.[FieldNameDWH] IS NULL THEN 0 ELSE 1 END AS [AddOptionValue]
		--,CAST(NULL AS INT) as [RefTableOptionNoColumnId]
		,CAST(NULL AS NVARCHAR(128)) AS [RefTableName] /* +sc.name */ /*CASE WHEN OpL.[RefTableName] IS NOT NULL THEN OpL.[RefTableName] ELSE sc.[name] END*/
		,OpL.[RefColumnName]
		,OpL.[RefLevelNo]
		,1 as [AddToDest]
		,ORIGINAL_LOGIN() as  [ModifiedBy]
		,CAST(NULL as NVARCHAR(256)) AS [Description]
	INTO #ALL_ColumnDefine
    from sys.tables st
		inner join sys.columns sc on st.object_id = sc.object_id
		INNER JOIN sys.schemas as sch ON st.[schema_id] = sch.schema_id
		INNER JOIN INFORMATION_SCHEMA.COLUMNS isc
			ON st.[name] = isc.[TABLE_NAME]
				AND sch.name = isc.[TABLE_SCHEMA]
				AND sc.name = isc.[COLUMN_NAME]
		left join INFORMATION_SCHEMA.KEY_COLUMN_USAGE kcu
			on kcu.TABLE_CATALOG = isc.TABLE_CATALOG
				and kcu.TABLE_SCHEMA = isc.TABLE_SCHEMA
				and kcu.TABLE_NAME = isc.TABLE_NAME
				and kcu.COLUMN_NAME = isc.COLUMN_NAME
		LEFT JOIN (Select distinct [TableNameDWH],[FieldNameDWH],[RefTableName],[RefTableAliasName],[RefColumnName],[RefLevelNo],[ReferenceTableDefineId] from #SourceTable_OptionList) as OpL
				ON sc.[name] = OpL.[FieldNameDWH]
					/*AND +'_AOL' = OpL.[RefTableName]*/
    where st.name = @SourceMainTable
			AND sch.name = @SourceSchema
			AND isc.TABLE_CATALOG = @SourceDBName
	ORDER BY isc.[ORDINAL_POSITION];
	
	
/* Update [DestColumnName] from [DefaultColumnNameChange]*/
UPDATE a
	SET a.[DestColumnName] = cln.[DestColumnName]
from #ALL_ColumnDefine as a
	INNER JOIN [TADM].[Setting].[DefaultColumnNameChange] as cln
				ON a.[SourceTableName] = cln.[SourceTableName]
					AND a.[SourceColumnName] = cln.[SourceColumnName]
					AND a.[DestTableName] = cln.[DestTableName]
					AND a.[DestColumnName] != cln.[DestColumnName]

					

DROP TABLE IF EXISTS #DoubleColumn;
select [DestColumnName],COUNT([DestColumnName]) as cnt 
into #DoubleColumn
from #ALL_ColumnDefine
GROUP by [DestColumnName];



UPDATE a
	SET a.[AddToDest] = CASE	WHEN LEFT(a.[SourceColumnName], 2) COLLATE Latin1_General_CS_AS = 'TA' AND [IsOptionField] = 1 THEN 1 ELSE 0 END
	--Select a.[SourceColumnName],a.[DestColumnName],a.[IsOptionField],a.[AddToDest],CASE	WHEN LEFT(a.[SourceColumnName], 2) COLLATE Latin1_General_CS_AS = 'TA' AND [IsOptionField] = 1 THEN 1 ELSE 0 END as [New_addToDest]
	from #ALL_ColumnDefine as a
	Inner JOIN #DoubleColumn as dc
		ON a.DestColumnName = dc.DestColumnName
	Where dc.cnt>1;
UPDATE a
	SET a.[AddToDest] = 0
	from #ALL_ColumnDefine as a
	where [DataType] IN ( 'image', 'uniqueidentifier');
UPDATE a
	SET a.[AddToDest] = 0
	from #ALL_ColumnDefine as a
	where [SourceColumnName] LIKE '%GUID';
UPDATE a
	SET a.[AddToDest] = 0
	from #ALL_ColumnDefine as a
	where [SourceColumnName] IN ( SELECT [ColumnName] FROM [TADM].[Setting].[DefaultColumnBlackList]);

--SELECT [Table],[ReferenceTableDefineId],[TableName],[fTableId],[TableDefineId],[VersionId],[SourceDBId],[SourceSchema],[SourceDBName],[SourceMainTableId],[SourceTableName],[SourceColumnId],[DestDBId],[DestDBName],[DestSchema],[DestTableId],[DestTableName],[DestColumnId],[ColumnSortNo],[column_id],[DestSchemaName],[SourceColumnName],[DestColumnName],[DataType],[MaxLength],[Precision],[Scale],[IsNullable],[CollationName],[DefaultValue],[KeySortNo],[SourceKeyColumn],[DestKeyColumn],[IsOptionField],[FieldType],[AddOptionValue],[RefTableName],[RefColumnName],[RefLevelNo],[AddToDest],[ModifiedBy],[Description] FROM #ALL_ColumnDefine
/*Hier add OptionId Column*/
INSERT INTO #ALL_ColumnDefine ([Table],[ReferenceTableDefineId],[TableName],[fTableId],[TableDefineId],[VersionId],[SourceDBId],[SourceSchema],[SourceDBName],[SourceMainTableId],[SourceTableName],[SourceColumnId],[DestDBId],[DestDBName],[DestSchema],[DestTableId],[DestTableName],[DestColumnId],[ColumnSortNo],[column_id],[DestSchemaName],[SourceColumnName],[DestColumnName],[DataType],[MaxLength],[Precision],[Scale],[IsNullable],[CollationName],[DefaultValue],[KeySortNo],[SourceKeyColumn],[DestKeyColumn],[IsOptionField],[FieldType],[AddOptionValue],[RefTableName],[RefColumnName],[RefLevelNo],[AddToDest],[ModifiedBy],[Description])
Select a.[Table]
		/*,[TADM].[Setting].[fGetReferenceTableDefineId](a.[TableDefineId],[TADM].[Setting].[fGetTableId](a.[SourceTableName]+'_'+a.[RefColumnName], a.[SourceDBName], a.[VersionId]), a.[VersionId]) as [ReferenceTableDefineId] */
		,aol.[ReferenceTableDefineId] as [ReferenceTableDefineId]
		,a.[SourceTableName] as [TableName] /* --a.[TableName] */
		,[TADM].[Setting].[fGetTableId](aol.[RefTableName], a.[SourceDBName], a.[VersionId]) as [fTableId] 
		,a.[TableDefineId]
		,a.[VersionId]
		,a.[SourceDBId]
		,a.[SourceSchema]
		,a.[SourceDBName]
		,a.[SourceMainTableId]
		,a.[SourceTableName]+'_AOL' AS [SourceTableName]
		,[TADM].[Setting].[fGetColumnId]('OptionId', aol.[RefTableName], a.[SourceDBName], a.[VersionId]) as [SourceColumnId] /*a.[SourceColumnId]*/
		,a.[DestDBId]
		,a.[DestDBName]
		,a.[DestSchema]
		,a.[DestTableId]
		,a.[DestTableName]
		,[TADM].[Setting].[fGetColumnId](a.[DestColumnName]+'OptionId', a.[DestTableName], a.[DestDBName], a.[VersionId]) as [DestColumnId]
		,a.[ColumnSortNo]+0.1 as [ColumnSortNo]
		,a.[column_id]
		,a.[DestSchemaName]
		,'OptionId' as [SourceColumnName]
		,a.[DestColumnName]+'OptionId' AS [DestColumnName]	
		,'nvarchar'as [DataType]
		,50 as [MaxLength]
		,null as [Precision]
		,null as[Scale]
		,a.[IsNullable]
		,'Latin1_General_CI_AS' [CollationName]
		,a.[DefaultValue]
		,a.[KeySortNo]
		,'OptionId' as [SourceKeyColumn]
		,a.[DestColumnName]+'OptionId' as [DestKeyColumn]
		,a.[IsOptionField]
		,'OptionId' as [FieldType]
		,0 as [AddOptionValue]
		,aol.[RefTableName]  as [RefTableName]
		,a.[RefColumnName]
		,a.[RefLevelNo]
		,a.[AddToDest]
		,a.[ModifiedBy]
		,a.[DestColumnName] as [Description]
from #ALL_ColumnDefine as a
	LEFT JOIN #ALL_ColumnDefine as b
		ON a.[TableDefineId] = b.[TableDefineId]
				AND a.[SourceMainTableId] = b.[SourceMainTableId]
				/*--AND a.[SourceColumnName] = b.[SourceColumnName]*/
				AND a.[DestColumnName]+'OptionId' = b.[DestColumnName]
	LEFT JOIN (SELECT DISTINCT [FieldNameDWH], [RefTableName], [RefTableAliasName],[RefColumnName],[ReferenceTableDefineId]  FROM #SourceTable_OptionList) as aol
		ON a.[SourceColumnName] = aol.[RefColumnName]
	where a.[IsOptionField] = 1 AND a.[FieldType] = 'OptionNo' AND a.[AddToDest] = 1
		AND b.[DestColumnName] is NULL

UPDATE a
	SET a.[ReferenceTableDefineId] = [TADM].[Setting].[fGetParrentReferenceTableDefineId](a.[ReferenceTableDefineId], a.[VersionId])
--Select a.[ReferenceTableDefineId] as [a.ReferenceTableDefineId], [TADM].[Setting].[fGetParrentReferenceTableDefineId](a.[ReferenceTableDefineId], a.[VersionId]) as [ReferenceTableDefineId] /* @ReferenceTableDefineId int, @VersionId */
FROM #ALL_ColumnDefine as a
Where a.[FieldType] = 'OptionNo'

--[Column] Source
PRINT('Insert Source: [Setting].[Column]')
INSERT INTO [TADM].[Setting].[Column] ([TableId], [VersionId], [ColumnName], [Description], [ColumnSortNo], [DataType], [MaxLength], [Precision], [Scale],[CollationName],[DefaultValue],[KeySortNo], [IsNullable], [IsOptionField],[FieldType], [ModifiedBy])
SELECT /*'Column-Source' as [Table],*/
	a.[SourceMainTableId] as [TableId]
	,a.[VersionId]
	,a.[SourceColumnName] as [ColumnName]
	,a.[SourceDBName]+'.'+a.[SourceSchema]+'.'+a.[SourceTableName]+':'+a.[SourceColumnName]+'|'+a.[DestDBName]+'.'+a.[DestSchema]+'.'+a.[DestTableName]+':'+a.[DestColumnName] as [Description]
	,a.[ColumnSortNo]
	,a.[DataType]
	,a.[MaxLength]
	,a.[Precision]
	,a.[Scale]
	,a.[CollationName]
	,a.[DefaultValue]
	,a.[KeySortNo]
	,a.[IsNullable]
	,a.[IsOptionField]
	,a.[FieldType]
	,a.[ModifiedBy]
FROM #ALL_ColumnDefine as a
	LEFT JOIN [TADM].[Setting].[Column] as C
		ON a.[SourceMainTableId] = c.[TableId]
			AND a.[SourceColumnName] =  c.[ColumnName]
WHERE a.[AddToDest] = 1
		AND (a.[FieldType] IS NULL OR a.[FieldType] = 'OptionNo')
		AND c.[ColumnId] IS NULL
ORDER BY a.[ColumnSortNo];

--[Column] RefTable (OptionNo, OptionDE, OptionENU, OptionId)
PRINT('Insert RefTables: [Setting].[Column]')
INSERT INTO [TADM].[Setting].[Column] ([TableId], [VersionId], [ColumnName], [Description], [ColumnSortNo], [DataType], [MaxLength], [Precision], [Scale],[CollationName],[DefaultValue],[KeySortNo], [IsNullable], [IsOptionField],[FieldType], [ModifiedBy])
Select [TADM].[Setting].[fGetTableId](a.[SourceTableName]+'_AOL', a.[SourceDBName], a.[VersionId] ) as [TableId]
		,a.[VersionId]
		,b.[ColumnName]	
		,a.[SourceColumnName] as [Description]
		,a.[ColumnSortNo]+b.[SortNo] as [ColumnSortNo]
		,b.[DataType]
		,b.[MaxLength]
		,b.[Precision]
		,b.[Scale]
		,b.[CollationName]
		,b.[DefaultValue]
		,b.[KeySortNo]
		,b.[IsNullable]
		,b.[IsOptionField]
		,b.[FieldType]
		,a.[ModifiedBy]
from #ALL_ColumnDefine as a 
	CROSS JOIN #OptionColumnList as b
	LEFT JOIN [TADM].[Setting].[Column] as C
			ON [TADM].[Setting].[fGetTableId](a.[RefTableName], a.[SourceDBName], a.[VersionId] ) = c.[TableId]
				AND b.[ColumnName] =  c.[ColumnName]
where a.[FieldType] = 'OptionNo'
	AND a.[AddToDest] = 1
	AND c.[ColumnId] IS NULL



/*Add Main [ReferenceColumnDefine] */
	INSERT INTO #ReferenceColumnDefine ([VersionId], [SourceReferenceTableDefineId], [SourceTableName], [SourceColumnId], [SourceColumnFunction], [RefReferenceTableDefineId], [RefTableName], [RefTableAliasName], [RefLevelNo], [RefColumnId], [RefColumnName], [RefColumnFunction], [KeySortNo], [Description], [ModifiedAt], [ModifiedBy], [Deleted])
	SELECT 1 as [VersionId]
		,a.[ReferenceTableDefineId] AS [SourceReferenceTableDefineId] 
		,a.[TableName] AS [SourceTableName]
		,a.[SourceColumnId] as [SourceColumnId]
		,NULL AS [SourceColumnFunction]
		,a.[ReferenceTableDefineId] as [RefReferenceTableDefineId]
		,a.[DestTableName] as [RefTableName]
		,a.[TableName] AS [RefTableAliasName]
		,0 AS [RefLevelNo]
		,a.[DestColumnId] as [RefColumnId]
		,a.[DestColumnName] as [RefColumnName]
		,NULL AS [RefColumnFunction]
		,1 as [KeySortNo]
		,NULL AS [Description]
		,GETDATE() as [ModifiedAt]
		,ORIGINAL_LOGIN() as [ModifiedBy]
		,0 as [Deleted]
	FROM #ALL_ColumnDefine as a
		LEFT JOIN #ReferenceColumnDefine as b
			ON a.[ReferenceTableDefineId] = b.[SourceReferenceTableDefineId]
				AND a.[ReferenceTableDefineId] = b.[RefReferenceTableDefineId]
				AND b.[SourceReferenceTableDefineId] = b.[RefReferenceTableDefineId]
	WHERE a.[KeySortNo] = 1 AND b.[SourceReferenceTableDefineId] IS NULL
	UNION ALL
	SELECT 1 as [VersionId]
		,a.[ReferenceTableDefineId] AS [SourceReferenceTableDefineId] 
		,a.[TableName] AS [SourceTableName]
		,[TADM].[Setting].[fGetColumnId]('CompanyId', a.[SourceTableName], a.[SourceDBName], a.[VersionId]) as [SourceColumnId]
		,NULL AS [SourceColumnFunction]
		,a.[ReferenceTableDefineId] as [RefReferenceTableDefineId]
		,a.[DestTableName] as [RefTableName]
		,a.[TableName] AS [RefTableAliasName]
		,0 AS [RefLevelNo]
		,[TADM].[Setting].[fGetColumnId]('CompanyId', a.[DestTableName], a.[DestDBName], a.[VersionId]) as [RefColumnId]
		,a.[DestColumnName] as [RefColumnName]
		,'@CompanyId' AS [RefColumnFunction]
		,2 as [KeySortNo]
		,NULL AS [Description]
		,GETDATE() as [ModifiedAt]
		,ORIGINAL_LOGIN() as [ModifiedBy]
		,0 as [Deleted]
	FROM #ALL_ColumnDefine as a
		LEFT JOIN #ReferenceColumnDefine as b
			ON a.[ReferenceTableDefineId] = b.[SourceReferenceTableDefineId]
				AND a.[ReferenceTableDefineId] = b.[RefReferenceTableDefineId]
				AND b.[SourceReferenceTableDefineId] = b.[RefReferenceTableDefineId]
				AND b.[KeySortNo] = 2
	WHERE a.[KeySortNo] = 1 AND b.[SourceReferenceTableDefineId] IS NULL

	PRINT('INSERT INTO [Setting].[ReferenceColumnDefine]');
	INSERT INTO [TADM].[Setting].[ReferenceColumnDefine] ([VersionId], [SourceReferenceTableDefineId], [SourceTableName], [SourceColumnId], [SourceColumnFunction], [RefReferenceTableDefineId], [RefTableName], [RefColumnId], [RefColumnName], [RefColumnFunction], [KeySortNo], [Description], [ModifiedAt], [ModifiedBy], [Deleted])
	SELECT a.[VersionId],a.[SourceReferenceTableDefineId],a.[SourceTableName],a.[SourceColumnId],a.[SourceColumnFunction],a.[RefReferenceTableDefineId],a.[RefTableName],a.[RefColumnId],a.[RefColumnName],a.[RefColumnFunction],a.[KeySortNo],a.[Description],a.[ModifiedAt],a.[ModifiedBy],a.[Deleted]
	FROM #ReferenceColumnDefine as a
		LEFT JOIN [TADM].[Setting].[ReferenceColumnDefine] as b
			ON a.[SourceReferenceTableDefineId] = b.[SourceReferenceTableDefineId]
				AND a.[SourceColumnId] = b.[SourceColumnId]
				AND a.[RefReferenceTableDefineId] = b.[RefReferenceTableDefineId]
				AND a.[RefColumnId] = b.[RefColumnId]
		WHERE b.[ReferenceColumnDefineId] IS NULL /*RefReferenceTableDefineId*/

--Dest DataType Update
UPDATE c
	SET c.[DataType] = 'DATE'
FROM #ALL_ColumnDefine as c
  where [DestColumnName] like '%Date'
		AND c.[DataType] = 'datetime'
UPDATE c
	SET c.[Precision] = 18
		,c.[Scale] = 2
FROM #ALL_ColumnDefine as c
  where c.[DataType] = 'decimal'
		AND [Precision] = 38

/* Update new KeyColumns from #newPKeyTable*/
UPDATE a
	SET a.[KeySortNo] = b.[KeySortNo]
		,a.[SourceKeyColumn] = b.[SourceKeyColumn]
		,a.[DestKeyColumn] = b.[DestKeyColumn] 
--SELECT a.[SourceColumnName],a.[DestColumnName],b.[KeySortNo],b.[SourceKeyColumn],b.[DestKeyColumn] 
FROM #ALL_ColumnDefine as a
INNER JOIN #newPKeyTable as b
	ON a.[SourceColumnName] = b.[SourceKeyColumn]

--DEST ColumnName Update
--Column Dest
PRINT('Insert Dest: [Setting].[Column]')
INSERT INTO [TADM].[Setting].[Column] ([TableId], [VersionId], [ColumnName], [Description], [ColumnSortNo], [DataType], [MaxLength], [Precision], [Scale],[CollationName],[DefaultValue],[KeySortNo], [IsNullable], [IsOptionField],[FieldType], [ModifiedBy])
SELECT /*'Column-Dest' as [Table],*/
		 a.[DestTableId] as [fTableId]
		,a.[VersionId]
		,a.[DestColumnName] as [ColumnName]
		,a.[SourceDBName]+'.'+a.[SourceSchema]+'.'+a.[SourceTableName]+':'+a.[SourceColumnName]+'|'+a.[DestDBName]+'.'+a.[DestSchema]+'.'+a.[DestTableName]+':'+a.[DestColumnName] as [Description]
		,a.[ColumnSortNo]
		,a.[DataType]
		,a.[MaxLength]
		,a.[Precision]
		,a.[Scale]
		,a.[CollationName]
		,a.[DefaultValue]
		,a.[KeySortNo]
		,a.[IsNullable]
		,a.[IsOptionField]
		,a.[FieldType]
		,a.[ModifiedBy]
FROM #ALL_ColumnDefine as a
	LEFT JOIN [TADM].[Setting].[Column] as C
		ON a.[DestTableId] = c.[TableId]
			AND a.[DestColumnName] =  c.[ColumnName]
WHERE a.[AddToDest] = 1
		--AND (a.[FieldType] IS NULL OR a.[FieldType] = 'OptionNo')
		AND c.[ColumnId] IS NULL
ORDER BY a.[ColumnSortNo];

--Get SourceColumnId AND DestColumnId for #ALL_ColumnDefine
PRINT('UPDATE [SourceColumnId],[DestColumnId] in #ALL_ColumnDefine')
UPDATE a 
	SET a.[SourceColumnId] = SourceC.[ColumnId]
		,a.[DestColumnId]  = DestC.[ColumnId]
--Select a.[SourceColumnId], SourceC.[ColumnId] as [SourceColumnId], DestC.[ColumnId] as [DestColumnId],* 
from #ALL_ColumnDefine as a
	INNER JOIN [TADM].[Setting].[Column] as SourceC
		ON a.SourceColumnName = SourceC.[ColumnName]
			AND a.[SourceMainTableId] = SourceC.[TableId]
	INNER JOIN [TADM].[Setting].[Column] as DestC
		ON a.DestColumnName = DestC.[ColumnName]
			AND a.[DestTableId] = DestC.[TableId]
WHERE ISNULL(a.[SourceColumnId], 0) != ISNULL(SourceC.[ColumnId], 0)
	OR ISNULL(a.[DestColumnId], 0)  != ISNULL(DestC.[ColumnId], 0)

/* Set [ReferenceTableDefineId] for [FieldType] = 'OptionNo'*/
UPDATE a
	SET a.[ReferenceTableDefineId] = [TADM].[Setting].[fGetReferenceTableDefineId](a.[TableDefineId], a.[SourceMainTableId], 1) 
FROM #ALL_ColumnDefine as a WHERE a.[AddToDest] = 1 AND [FieldType] = 'OptionNo'




--ColumnDefine
PRINT('Insert Dest: [Setting].[ColumnDefine]')
INSERT INTO [TADM].[Setting].[ColumnDefine] ( [ReferenceTableDefineId], [TableDefineId], [VersionId], [SourceDBId], [SourceDBName], [SourceTableId], [SourceTableName], [SourceColumnId], [SourceColumnName], [CustomeStatment], [DestDBId], [DestDBName], [DestTableId], [DestTableName], [DestColumnId], [DestColumnName], [ColumnSortNo], [KeySortNo], [Description], [ModifiedAt], [ModifiedBy], [Deleted])
SELECT	 a.[ReferenceTableDefineId]
		,a.[TableDefineId]
		,a.[VersionId]
		,a.[SourceDBId]
		,a.[SourceDBName]
		,a.[SourceMainTableId]
		,a.[SourceTableName]
		,a.[SourceColumnId]
		,a.[SourceColumnName]
		,a.[CustomeStatment]
		,a.[DestDBId]
		,a.[DestDBName]
		,a.[DestTableId]
		,a.[DestTableName]
		,a.[DestColumnId]
		,a.[DestColumnName]
		,a.[ColumnSortNo]
		,a.[KeySortNo]
		,CASE WHEN a.[SourceTableName]+'_AOL' = a.[RefTableName] AND a.[IsOptionField] = 0 THEN NULL ELSE a.[RefTableName] END AS [Description]
		 ,GETDATE() as [ModifiedAt]
		,a.[ModifiedBy]
		,0 as [Deleted]
FROM #ALL_ColumnDefine as a
	LEFT JOIN [TADM].[Setting].[ColumnDefine] as b
		ON a.[TableDefineId]= b.[TableDefineId]
			AND a.[VersionId] = b.[VersionId]
			AND a.[SourceMainTableId] = b.[SourceTableId]
			AND a.[SourceColumnId] = b.[SourceColumnId]
			AND a.[DestTableId] = b.[DestTableId]
			AND a.[DestColumnId] = b.[DestColumnId]
WHERE a.[AddToDest] = 1
		/*AND a.[IsOptionField] = 1*/
		AND b.[ColumnDefineId] IS NULL
ORDER BY a.[ColumnSortNo];

/*SELECT * FROM #ALL_ColumnDefine as a WHERE a.[AddToDest] = 1 AND [SourceColumnId] IS NULL*/
SELECT * FROM [TADM].[Setting].[ColumnDefine] as a WHERE a.[TableDefineId] = @TableDefineId ORDER BY [ColumnSortNo]
