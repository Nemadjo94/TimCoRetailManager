﻿CREATE TABLE [dbo].[SaleDetail]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY(1,1), 
    [SaleId] INT NOT NULL, 
    [ProductId] INT NOT NULL, 
    [Quantity] INT NOT NULL DEFAULT 1,
    [PurchasePrice] MONEY NOT NULL, 
    [Tax] MONEY NOT NULL DEFAULT 0, 
    CONSTRAINT [FK_SaleDetail_ToProductTable] FOREIGN KEY ([ProductId]) REFERENCES [Product]([Id]), 
    CONSTRAINT [FK_SaleDetail_ToSaleTable] FOREIGN KEY ([SaleId]) REFERENCES [Sale]([Id])
    
)
