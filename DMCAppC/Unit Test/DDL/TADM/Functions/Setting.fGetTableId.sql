CREATE FUNCTION [Setting].[fGetTableId]
(
	 @TableName nvarchar(128)
	,@DBName nvarchar(128)
	,@VersionId INT
)
RETURNS INT
AS
BEGIN
	DECLARE  @TableId int
			,@DBId int
	
	SELECT @DBId = [Setting].[fGetDBId](@DBName)
	

    -- Insert statements for procedure here
	SELECT @TableId = [TableId] FROM [Setting].[Table]
		WHERE [TableName] = @TableName
			AND [DBId] = @DBId
			AND [VersionId] = @VersionId

	RETURN @TableId

END


