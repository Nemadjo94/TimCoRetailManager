CREATE TABLE [dbo].[Sale]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [CachierId] NVARCHAR(128) NOT NULL, 
    [SaleDate] DATETIME2 NOT NULL, 
    [SubTotal] MONEY NOT NULL, 
    [Tax] MONEY NOT NULL DEFAULT 0, 
    [Total] MONEY NOT NULL, 
    CONSTRAINT [FK_Sale_ToUserTable] FOREIGN KEY ([CachierId]) REFERENCES [User]([Id])
)
