


CREATE VIEW [Setting].[vColumnDefine]
	AS SELECT ColDef.[ColumnDefineId]
	          ,ColDef.[ReferenceTableDefineId]
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
              ,ColDef.[TableDefineId]
              ,ColDef.[VersionId]
              ,SourDB.[DBId] as [SourceDBId] --[SourceDBId]
	          ,SourDB.[DBName] as [SourceDBName]
	          ,TabDef.[SourceMainTableId] as [SourceTableId] --,SourTable.[TableId] 
	          ,SourTable.[SchemaName] as [SourceSchemaName]
	          ,SourTable.[TableName] as [SourceTableName]
	          ,SourTable.[Type] as [SourceTableType]
	          ,SourTable.[MainTableName] as [SourceTableMainTableName]
              ,SourTable.[MainColumnName] as [SourceTableMainColumnName] 
              ,ColDef.[SourceColumnId]
	          ,SourCol.[ColumnName] as [SourceColumnName]
	          ,ColDef.[CustomeStatment]
	          ,DestDB.[DBId] as [DestDBId]
	          ,DestDB.[DBName] as [DestDBName]
	          ,DestTable.[SchemaName] as [DestSchemaName]
	          ,DestTable.[TableId] as [DestTableId]
	          ,DestTable.[TableName] as [DestTableName]
	          ,DestTable.[Type] as [DestTableType]
	          ,DestTable.[MainTableName] as [DestTableMainTableName]
              ,DestTable.[MainColumnName] as [DestTableMainColumnName] 
              ,ColDef.[DestColumnId]
              ,DestCol.[ColumnName] as [DestColumnName]
              ,ColDef.[ColumnSortNo]
	          ,DestCol.[DataType]
              ,DestCol.[MaxLength]
              ,DestCol.[Precision]
              ,DestCol.[Scale]
              ,DestCol.[CollationName]
              ,DestCol.[DefaultValue]
              ,ColDef.[KeySortNo] --,DestCol.[KeySortNo]
              ,DestCol.[IsNullable]
              ,DestCol.[IsOptionField]
              ,DestCol.[FieldType]
              ,DestCol.[IsLookupField]
              ,ColDef.[ModifiedAt]
              ,ColDef.[ModifiedBy]
              ,ColDef.[Deleted]
  FROM [Setting].[ColumnDefine] as ColDef
	INNER JOIN [Setting].[TableDefine] as TabDef
		ON ColDef.[TableDefineId] = TabDef.[TableDefineId]
	INNER JOIN [Setting].[ReferenceTableDefine] as RefTabDef
		ON ColDef.[ReferenceTableDefineId] = RefTabDef.[ReferenceTableDefineId]
    LEFT JOIN [Setting].[ReferenceTableDefine] as ParentRefTabDef
		ON RefTabDef.[ParentReferenceTableDefineId] = ParentRefTabDef.[ReferenceTableDefineId]
	INNER JOIN [Setting].[Table] as SourTable
		ON TabDef.[SourceMainTableId] = SourTable.[TableId]
	INNER JOIN [Setting].[Table] as DestTable
		ON TabDef.[DestMainTableId] = DestTable.[TableId]
	INNER JOIN [Setting].[Column] as SourCol
		ON ColDef.[SourceColumnId] = SourCol.[ColumnId]
	INNER JOIN [Setting].[Column] as DestCol
		ON ColDef.[DestColumnId] = DestCol.[ColumnId]
	INNER JOIN [Setting].[DB] as SourDB
		ON SourTable.[DBId] = SourDB.[DBId]
	INNER JOIN [Setting].[DB] as DestDB
		ON DestTable.[DBId] = DestDB.[DBId]
  WHERE ColDef.[Deleted] = 0