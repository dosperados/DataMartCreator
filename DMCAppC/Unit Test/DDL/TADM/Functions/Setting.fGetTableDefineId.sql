CREATE FUNCTION [Setting].[fGetTableDefineId]
(
	@SourceDBName nvarchar(128),
	@SourceTableName nvarchar(128),
	@DestDBName nvarchar(128),
	@DestTableName nvarchar(128)
)
RETURNS INT
AS 
BEGIN
	DECLARE @TableDefineId int

	SELECT @TableDefineId = [TableDefineId] from [Setting].[TableDefine]
		WHERE [SourceMainDBName] = @SourceDBName
			AND [SourceMainTableName] = @SourceTableName
			AND [DestMainDBName] = @DestDBName
			AND [DestMainTableName] = @DestTableName
	RETURN @TableDefineId
END
