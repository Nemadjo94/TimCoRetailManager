CREATE PROCEDURE [dbo].[spSaleInsert]
	@CachierId nvarchar(128),
	@SaleDate datetime2,
	@SubTotal money,
	@Tax money,
	@Total money
AS
BEGIN
	SET NOCOUNT ON;

	INSERT INTO [dbo].[Sale](CachierId, SaleDate, SubTotal, Tax, Total)
	VALUES (@CachierId, @SaleDate, @SubTotal, @Tax, @Total);

END
