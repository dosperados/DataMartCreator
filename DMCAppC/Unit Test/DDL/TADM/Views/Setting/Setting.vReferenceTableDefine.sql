


CREATE VIEW [Setting].[vReferenceTableDefine]
	AS SELECT 
	           RefTabDef.[ReferenceTableDefineId]
              ,RefTabDef.[TableDefineId]
              ,RefTabDef.[VersionId]
              ,TabDef.[SourceMainDBId]
              ,SourDB.[DBName] as [SourceMainDBName]
              ,TabDef.[SourceMainTableId]
              ,SourTab.[SchemaName] as [SourceMainSchemaName]
              ,SourTab.[TableName] AS [SourceMainTableName]
              ,TabDef.[DestMainDBId]
              ,DestDB.[DBName] as [DestMainDBName]
              ,[DestMainTableId]
              ,DestTab.[SchemaName] AS [DestMainSchemaName]
              ,DestTab.[TableName] AS [DestMainTableName]
              ,RefTabDef.[ParentReferenceTableDefineId]
              ,RefTabDef.[RefTableId]
			  ,RefDb.[DBName] as [RefDBName]
			  ,RefTab.[SchemaName] as [RefSchemaName]
              ,RefTab.[TableName] as [RefTableName]
              ,RefTabDef.[RefTableAliasName]
              ,RefTabDef.[CustomeTableSelect]
              ,RefTabDef.[RefLevelNo]
              ,RefTabDef.[RefType]
              ,RefTabDef.[JoinType]
              ,RefTabDef.[Description]
			  ,RefTabDef.[Deleted]

  FROM [Setting].[ReferenceTableDefine] as RefTabDef
        INNER JOIN [Setting].[TableDefine] as TabDef
            ON RefTabDef.[TableDefineId] = TabDef.[TableDefineId]
        INNER JOIN [Setting].[DB] as SourDB
            ON TabDef.[SourceMainDBId] = SourDB.[DBId]
        INNER JOIN [Setting].[DB] as DestDB
            ON TabDef.[DestMainDBId] = DestDB.[DBId]
        INNER JOIN [Setting].[Table] as SourTab
            ON TabDef.[SourceMainTableId] = SourTab.[TableId]
        INNER JOIN [Setting].[Table] as DestTab
            ON TabDef.[DestMainTableId] = DestTab.[TableId]
        INNER JOIN [Setting].[Table] as RefTab
            ON RefTabDef.[RefTableId] = RefTab.[TableId]
		INNER JOIN [Setting].[DB] as RefDb
            ON RefTab.[DBId] = RefDb.[DBId]
        /*
		INNER JOIN [Setting].[vColumnDefine] as ColDef
			ON RefTabDef.[ReferenceTableDefineId] = ColDef.[ReferenceTableDefineId]
				AND RefTabDef.[TableDefineId] = ColDef.[TableDefineId]
                */
	WHERE RefTabDef.[Deleted] = 0
