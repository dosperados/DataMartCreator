CREATE FUNCTION [Setting].[fGetDBId]
(
	@DBName NVARCHAR(128)
	--,@VersionId int
)
RETURNS INT
AS
BEGIN
	DECLARE @DBId int
			--,@DBId int
	SELECT @DBId = [DBId] FROM [Setting].[DB]
		WHERE [DBName] = @DBName
			--AND [VersionId] = @VersionId
	RETURN @DBId
END
