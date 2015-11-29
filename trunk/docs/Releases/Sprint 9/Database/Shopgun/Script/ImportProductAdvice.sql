INSERT INTO [ShopGunIntegrationTest].[dbo].[Advices]
           ([ProductsId]
           ,[AdviceType]
           ,[MentorId]
           ,[Label]
           ,[Advice]
           ,[KeyWords]
           ,[SemaphoreId])
SELECT [ShopGunIntegrationTest].[dbo].[Products].Id,
		'ProductAdvice'
	--,[ShopGunIntegrationTest].[dbo].[Products].ProductName
	--,[ShopGunIntegrationTest].[dbo].[Products].GlobalTradeItemNumber
	--,[ExtShopGunDemo].[dbo].[PurchaseAdvices].[globalTradeItemNumber] AS demoGTIN
    --,[organisation]
      ,[ShopGunIntegrationTest].[dbo].Mentors.Id
      ,[ExtShopGunDemo].[dbo].PurchaseAdvices.[adviceLabel]
      ,[ExtShopGunDemo].[dbo].PurchaseAdvices.[advice]
      ,[ShopGunIntegrationTest].[dbo].Products.ProductName
      ,[ShopGunIntegrationTest].dbo.Semaphore.Id
      --,[semaphoreColorName]

  FROM	[ShopGunIntegrationTest].[dbo].[Products] INNER JOIN
		[ExtShopGunDemo].[dbo].[PurchaseAdvices] ON
		[ShopGunIntegrationTest].[dbo].[Products].GlobalTradeItemNumber = [ExtShopGunDemo].[dbo].[PurchaseAdvices].[globalTradeItemNumber]
		INNER JOIN [ShopGunIntegrationTest].[dbo].Mentors ON
		[ExtShopGunDemo].[dbo].PurchaseAdvices.organisation COLLATE Finnish_Swedish_CI_AS = [ShopGunIntegrationTest].[dbo].Mentors.MentorName
		INNER JOIN [ShopGunIntegrationTest].[dbo].Semaphore ON
		[ExtShopGunDemo].[dbo].PurchaseAdvices.semaphoreColorName COLLATE Finnish_Swedish_CI_AS = [ShopGunIntegrationTest].[dbo].Semaphore.ColorName		
ORDER BY [ShopGunIntegrationTest].[dbo].[Products].GlobalTradeItemNumber
		