CREATE FUNCTION [Setting].[fGetParrentReferenceTableDefineId]
(
	@ReferenceTableDefineId int
	,@VersionId INT
)
RETURNS INT
AS
BEGIN
	DECLARE  @ParrentReferenceTableDefineId int
	
    -- Insert statements for procedure here
	SELECT @ParrentReferenceTableDefineId = [ParentReferenceTableDefineId] 
		FROM [Setting].[ReferenceTableDefine]
	WHERE [ReferenceTableDefineId] = @ReferenceTableDefineId
		AND [VersionId] = @VersionId

	RETURN @ParrentReferenceTableDefineId
END
