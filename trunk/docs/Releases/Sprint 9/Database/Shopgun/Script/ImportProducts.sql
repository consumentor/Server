INSERT INTO [ShopGunIntegrationTest].[dbo].[Products]
	(BrandId, ProductName , GlobalTradeItemNumber,Quantity, 
			QuantityUnit, LastUpdated)
			
SELECT DISTINCT	[ShopGunIntegrationTest].[dbo].[Brands].[Id], 
		[ExtShopGunDemo].[dbo].[Products].[productName] COLLATE Finnish_Swedish_CI_AS,
		cast(cast ([ExtShopGunDemo].[dbo].[Products].[globalTradeItemNumber] as decimal(15,0)) as varchar(20)),
		[ExtShopGunDemo].[dbo].[Products].[quantity], 
		[ExtShopGunDemo].[dbo].[Products].[quantityUnit],
		GETDATE()
FROM	[ShopGunIntegrationTest].[dbo].[Brands] INNER JOIN
		[ExtShopGunDemo].[dbo].[Products] ON
		[ShopGunIntegrationTest].[dbo].[Brands].BrandName = [ExtShopGunDemo].[dbo].[Products].brand COLLATE Finnish_Swedish_CI_AS



