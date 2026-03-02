CREATE FUNCTION [Setting].[fGetReferenceTableDefineId]
(
	 @TableDefineId INT
	,@RefTableId INT
	,@VersionId INT
)
RETURNS INT
AS
BEGIN
	DECLARE  @ReferenceTableDefineId int

	SELECT @ReferenceTableDefineId = [ReferenceTableDefineId] 
	FROM [Setting].[ReferenceTableDefine]
		WHERE [TableDefineId] = @TableDefineId
			AND [RefTableId] = @RefTableId 
			AND [VersionId] = @VersionId
	RETURN @ReferenceTableDefineId
END


