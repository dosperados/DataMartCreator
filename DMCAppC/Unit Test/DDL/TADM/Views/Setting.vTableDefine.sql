CREATE VIEW [Setting].[vTableDefine]
	AS 
	SELECT   TD.[TableDefineId]
			,TD.[VersionId]
			,V.[Description] 
			,TD.[SourceMainDBId]
			,SDB.[DBName] as [SourceDBName]
			,TD.[SourceMainTableId]
			,ST.[SchemaName] as [SourceSchemaName]
			,ST.[TableName] as [SourceTableName] 
			,TD.[DestMainDBId]
			,DDB.[DBName] as [DestDBName]
			,TD.[DestMainTableId]
			,DT.[SchemaName] as [DestSchemaName]
			,DT.[TableName] as [DestTableName]
	
	FROM [Setting].[TableDefine] as TD
		INNER JOIN [Setting].[Version] as V
			ON TD.[VersionId] = V.[VersionId]
		INNER JOIN [Setting].[DB] as SDB
			ON TD.[SourceMainDBId] = SDB.[DBId]
		INNER JOIN [Setting].[Table] as ST
			ON TD.[SourceMainTableId] = st.[TableId]
		INNER JOIN [Setting].[DB] as DDB
			ON TD.[DestMainDBId] = DDB.[DBId]
		INNER JOIN [Setting].[Table] as DT
			ON TD.[DestMainTableId] = DT.[TableId]

