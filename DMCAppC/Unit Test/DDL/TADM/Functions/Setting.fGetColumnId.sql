CREATE FUNCTION [Setting].[fGetColumnId]
(
	 @ColumnName nvarchar(128)
	,@TableName nvarchar(128)
	,@DBName nvarchar(128)
	,@VersionId INT
)
RETURNS INT
AS
BEGIN
	DECLARE @ColumnId int
			,@TableId int

	SELECT  @TableId = [Setting].[fGetTableId](@TableName, @DBName, @VersionId)
   

	SELECT @ColumnId = [ColumnId] from [Setting].[Column]
		WHERE [ColumnName] = @ColumnName
			AND [TableId] = @TableId
			AND [VersionId] = @VersionId

	RETURN @ColumnId
END