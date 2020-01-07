CREATE PROCEDURE [dbo].[spInventoryGetAll]
AS
BEGIN
	SET NOCOUNT ON;

	SELECT [ProductId], [Quantity], [PurchasePrice], [PurchaseDate] 
	FROM dbo.Inventory;
END
