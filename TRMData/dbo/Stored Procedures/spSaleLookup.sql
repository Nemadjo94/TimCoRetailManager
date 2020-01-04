CREATE PROCEDURE [dbo].[spSaleLookup]
	@CachierId nvarchar(128),
	@SaleDate datetime2
AS
BEGIN
	SET NOCOUNT ON;

	SELECT Id
	FROM [dbo].[Sale]
	WHERE CachierId = @CachierId and SaleDate = @SaleDate;
END
