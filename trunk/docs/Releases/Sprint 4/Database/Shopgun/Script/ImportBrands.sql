INSERT INTO [ShopGunIntegrationTest].[dbo].[Brands]([BrandName])
SELECT DISTINCT [brand] 
  FROM [ExtShopGunDemo].[dbo].[Products]
  ORDER BY [brand]  