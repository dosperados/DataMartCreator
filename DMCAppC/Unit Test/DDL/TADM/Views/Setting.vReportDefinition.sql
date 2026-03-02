CREATE VIEW [setting].[vReportDefinition]
	AS 
	SELECT	[ReportId]
		,[Name] AS [ReportName]
		,[TableSchema] AS [ReportTableSchema]
		,[TableName] AS [ReportTableName]
		,[Description] AS [ReportDescription]
		,[Type] AS [ReportType]
		,CASE 
			WHEN [Type] = 10 THEN 'Relational'
			WHEN [Type] = 20 THEN 'Multidimensional'
			ELSE 'Others - NotDefined'
		END as [ReportTypeName]
		,[Script] AS [ReportScript]
		,[ModifiedAt]
		,[Deleted]
  FROM [Setting].[ReportDefinition]