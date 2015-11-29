INSERT INTO [ShopGunIntegrationTest].[dbo].[Advices]
           ([IngredientsId]
           , [AdviceType]    
           ,[MentorId]
           ,[Label]
           ,[Advice]
           ,[KeyWords]
           ,[SemaphoreId])
SELECT [ShopGunIntegrationTest].[dbo].[Ingredients].Id
	  ,'IngredientAdvice'
	--,[ShopGunIntegrationTest].[dbo].[Products].ProductName
	--,[ShopGunIntegrationTest].[dbo].[Products].GlobalTradeItemNumber
	--,[ExtShopGunDemo].[dbo].[PurchaseAdvices].[globalTradeItemNumber] AS demoGTIN
    --,[organisation]
      ,[ShopGunIntegrationTest].[dbo].Mentors.Id
      ,[ExtShopGunDemo].[dbo].IngredientAdvices.[adviceLabel]
      ,[ExtShopGunDemo].[dbo].IngredientAdvices.[advice]
      ,[ExtShopGunDemo].[dbo].[IngredientAdvices].ingredient
      ,[ShopGunIntegrationTest].dbo.Semaphore.Id
      --,[semaphoreColorName]

  FROM	[ShopGunIntegrationTest].[dbo].[Ingredients] INNER JOIN
		[ExtShopGunDemo].[dbo].IngredientAdvices ON
		[ShopGunIntegrationTest].[dbo].[Ingredients].IngredientName = [ExtShopGunDemo].[dbo].[IngredientAdvices].ingredient
		INNER JOIN [ShopGunIntegrationTest].[dbo].Mentors ON
		[ExtShopGunDemo].[dbo].[IngredientAdvices].organisation COLLATE Finnish_Swedish_CI_AS = [ShopGunIntegrationTest].[dbo].Mentors.MentorName
		INNER JOIN [ShopGunIntegrationTest].[dbo].Semaphore ON
		[ExtShopGunDemo].[dbo].[IngredientAdvices].semaphoreColorName COLLATE Finnish_Swedish_CI_AS = [ShopGunIntegrationTest].[dbo].Semaphore.ColorName		
ORDER BY [ShopGunIntegrationTest].[dbo].[Ingredients].IngredientName
		