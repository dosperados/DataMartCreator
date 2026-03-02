

CREATE VIEW [Setting].[vReferenceColumnDefine]
	AS 
	SELECT RefColDef.[ReferenceColumnDefineId]
          ,RefColDef.[VersionId]
          ,RefColDef.[SourceReferenceTableDefineId]
          ,RefColDef.[SourceTableName]
          ,SourRefTabDef.[SourceMainTableName]
          ,SourRefTabDef.[SourceMainSchemaName]
          ,SourRefTabDef.[SourceMainDBName]
          ,SourRefTabDef.[RefTableAliasName]
          ,SourRefTabDef.[RefLevelNo]
          ,SourRefTabDef.[RefTableAliasName]+SourRefTabDef.[RefLevelNo] AS [RefTableAlias]
          --,SourRefTabDef.[CustomeTableSelect]
          ,SourRefTabDef.[JoinType]
          ,SourRefTabDef.[RefType]
          ,SourRefTabDef.[DestMainTableName]
          ,RefColDef.[SourceColumnId]
          ,SourCol.[ColumnName] as [SourceColumnName]
          ,SourCol.[FieldType] as [SourceFieldType]
          ,SourCol.[IsOptionField] as [SourceIsOptionField]
          ,RefColDef.[SourceColumnFunction]

          ,RefColDef.[RefReferenceTableDefineId]
          ,RefColDef.[RefTableName]
          ,LookupRefTabDef.[SourceMainTableName] as [RefSourceMainTableName]
          ,RefColDef.[RefColumnId]
          ,LookupCol.[ColumnName] as [RefColumnName]
          ,RefColDef.[RefColumnName] as [ReferenceColumnDefine.RefColumnName]
          ,LookupRefTabDef.[RefTableAliasName]+LookupRefTabDef.[RefLevelNo] AS [RefReferenceTableAlias]
          ,RefColDef.[RefColumnFunction]
          ,RefColDef.[KeySortNo]
          ,RefColDef.[Description]
		  ,RefColDef.[Deleted]
			 
	FROM [Setting].[ReferenceColumnDefine] as RefColDef
		INNER JOIN [Setting].[vReferenceTableDefine] as SourRefTabDef
			ON RefColDef.[SourceReferenceTableDefineId] = SourRefTabDef.[ReferenceTableDefineId]
        INNER JOIN [Setting].[vColumn] as SourCol
            ON RefColDef.[SourceColumnId] = SourCol.[ColumnId]

		INNER JOIN [Setting].[vReferenceTableDefine] as LookupRefTabDef
			ON RefColDef.[RefReferenceTableDefineId] = LookupRefTabDef.[ReferenceTableDefineId]
        INNER JOIN [Setting].[vColumn] as LookupCol
            ON RefColDef.[RefColumnId] = LookupCol.[ColumnId]
	WHERE RefColDef.[Deleted] = 0
