INSERT INTO [ShopGunIntegrationTest].[dbo].Ingredients
	(IngredientName, LastUpdated)
			
SELECT DISTINCT	[ExtShopGunDemo].[dbo].[IngredientAdvices].[ingredient], GETDATE()
FROM [ExtShopGunDemo].[dbo].[IngredientAdvices]