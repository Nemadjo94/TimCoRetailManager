CREATE PROCEDURE [dbo].[spSaleSaleReport]
AS
BEGIN
	SET NOCOUNT ON;

	SELECT [sale].[SaleDate], [sale].[SubTotal], [sale].[Tax], [sale].[Total], [users].[FirstName], [users].[LastName], [users].[EmailAddress]
	FROM dbo.Sale sale
	INNER JOIN dbo.[User] users on sale.CachierId = users.Id;
END
