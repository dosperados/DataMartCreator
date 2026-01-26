



CREATE VIEW [Setting].[vViewDefine]
	AS SELECT ViewDef.[ViewDefineId]
	          ,ViewDef.[ReferenceTableDefineId]
             -- ,[RefReferenceTableDefineId]
              ,RefTabDef.[ParentReferenceTableDefineId]
              ,RefTabDef.[RefTableName]
	          ,RefTabDef.[RefTableAliasName]
	          ,RefTabDef.[RefLevelNo]
	          ,RefTabDef.[RefTableAliasName] + RefTabDef.[RefLevelNo] AS [AliasName]
              ,ParentRefTabDef.[RefTableAliasName] as [ParentTableAliasName]
	          ,RefTabDef.[RefType]
              ,RefTabDef.[JoinType]
	          /*,RefTabDef.[CustomeTableSelect]*/
              ,ViewDef.[TableDefineId]
              ,ViewDef.[VersionId]
              ,SourDB.[DBId] as [SourceDBId] --[SourceDBId]
	          ,SourDB.[DBName] as [SourceDBName]
	          ,TabDef.[SourceMainTableId] as [SourceTableId] --,SourTable.[TableId] 
	          ,SourTable.[SchemaName] as [SourceSchemaName]
	          ,SourTable.[TableName] as [SourceTableName]
	          ,SourTable.[Type] as [SourceTableType]
	          ,SourTable.[MainTableName] as [SourceTableMainTableName]
              ,SourTable.[MainColumnName] as [SourceTableMainColumnName] 
              ,ViewDef.[SourceColumnId]
	          ,SourCol.[ColumnName] as [SourceColumnName]
	          ,ViewDef.[CustomeStatment]
	          ,DestDB.[DBId] as [DestDBId]
	          ,DestDB.[DBName] as [DestDBName]
	          ,DestTable.[SchemaName] as [DestSchemaName]
	          ,DestTable.[TableId] as [DestTableId]
	          ,DestTable.[TableName] as [DestTableName]
	          ,DestTable.[Type] as [DestTableType]
	          ,DestTable.[MainTableName] as [DestTableMainTableName]
              ,DestTable.[MainColumnName] as [DestTableMainColumnName] 
              ,ViewDef.[DestColumnId]
              ,DestCol.[ColumnName] as [DestColumnName]
              ,ViewDef.[ColumnSortNo]
	          ,DestCol.[DataType]
              ,DestCol.[MaxLength]
              ,DestCol.[Precision]
              ,DestCol.[Scale]
              ,DestCol.[CollationName]
              ,DestCol.[DefaultValue]
              ,DestCol.[IsNullable]
              ,DestCol.[IsOptionField]
              ,DestCol.[FieldType]
              ,DestCol.[IsLookupField]
              ,ViewDef.[ModifiedAt]
              ,ViewDef.[ModifiedBy]
              ,ViewDef.[Deleted]
  FROM [Setting].[ViewDefine] as ViewDef
	INNER JOIN [Setting].[TableDefine] as TabDef
		ON ViewDef.[TableDefineId] = TabDef.[TableDefineId]
	INNER JOIN [Setting].[ReferenceTableDefine] as RefTabDef
		ON ViewDef.[ReferenceTableDefineId] = RefTabDef.[ReferenceTableDefineId]
    LEFT JOIN [Setting].[ReferenceTableDefine] as ParentRefTabDef
		ON RefTabDef.[ParentReferenceTableDefineId] = ParentRefTabDef.[ReferenceTableDefineId]
	INNER JOIN [Setting].[Table] as SourTable
		ON TabDef.[SourceMainTableId] = SourTable.[TableId]
	INNER JOIN [Setting].[Table] as DestTable
		ON TabDef.[DestMainTableId] = DestTable.[TableId]
	INNER JOIN [Setting].[Column] as SourCol
		ON ViewDef.[SourceColumnId] = SourCol.[ColumnId]
	INNER JOIN [Setting].[Column] as DestCol
		ON ViewDef.[DestColumnId] = DestCol.[ColumnId]
	INNER JOIN [Setting].[DB] as SourDB
		ON SourTable.[DBId] = SourDB.[DBId]
	INNER JOIN [Setting].[DB] as DestDB
		ON DestTable.[DBId] = DestDB.[DBId]
  WHERE ViewDef.[Deleted] = 0