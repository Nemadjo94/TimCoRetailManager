CREATE TABLE [dbo].[Product]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY(1,1), 
    [ProductName] NVARCHAR(100) NOT NULL, 
    [Description] NVARCHAR(MAX) NOT NULL, 
    [RetailPrice] MONEY NOT NULL,
    [QuantityInStock] INT NOT NULL DEFAULT 1,
    [CreateDate] DATETIME2 NOT NULL DEFAULT getutcdate(), 
    [LastModified] DATETIME2 NOT NULL DEFAULT getutcdate(), 
    [IsTaxable] BIT NOT NULL DEFAULT 1 
    
)
