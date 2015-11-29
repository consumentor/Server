INSERT INTO [ShopGunIntegrationTest].[dbo].[Mentors]([MentorName])
SELECT DISTINCT [organisation] 
  FROM [ExtShopGunDemo].[dbo].[PurchaseAdvices]
  ORDER BY [organisation]
  
  
  
/* FOR INGREDIENT ADVICES */  
INSERT INTO [ShopGunIntegrationTest].[dbo].[Mentors]([MentorName])

SELECT DISTINCT [organisation] 
	FROM [ExtShopGunDemo].[dbo].[IngredientAdvices]
  
	WHERE NOT [ExtShopGunDemo].[dbo].[IngredientAdvices].organisation IN (
		SELECT [ShopGunIntegrationTest].[dbo].[Mentors].MentorName
			FROM [ShopGunIntegrationTest].[dbo].[Mentors])
ORDER BY [organisation]      