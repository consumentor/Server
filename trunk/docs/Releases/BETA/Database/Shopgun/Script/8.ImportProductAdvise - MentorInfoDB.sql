INSERT INTO [Shopgun].[dbo].[Advices]
           ([ProductsId]
           ,[AdviceType]
           ,[MentorId]
           ,[Label]
           ,[Introduction]
           ,[Advice]
           ,[KeyWords]
           ,[SemaphoreId],
           [Published],
           [PublishDate])
SELECT [Shopgun].[dbo].[Products].Id,
		'ProductAdvice'
	--,[Shopgun].[dbo].[Products].ProductName
	--,[Shopgun].[dbo].[Products].GlobalTradeItemNumber
    --,[MentorInfo].[dbo].Advices.[PA-level]
      ,[Shopgun].[dbo].Mentors.Id
      ,[MentorInfo].[dbo].Advices.Headline
	  ,[MentorInfo].[dbo].Advices.Introduction
      ,[MentorInfo].[dbo].Advices.[Running text]
      ,  
      CASE WHEN(([Shopgun].[dbo].[Products].TableOfContent IS NULL)) THEN [Shopgun].[dbo].Products.ProductName ELSE([Shopgun].[dbo].Products.ProductName + ', ' + [Shopgun].[dbo].[Products].TableOfContent  ) END
      ,[Shopgun].dbo.Semaphore.Id,
      1,
      [MentorInfo].[dbo].Advices.[Date]
      --,[semaphoreColorName]

  FROM	[Shopgun].[dbo].[Products] INNER JOIN
		[MentorInfo].[dbo].[Advices] ON
		[Shopgun].[dbo].[Products].GlobalTradeItemNumber COLLATE Finnish_Swedish_CI_AS = [MentorInfo].[dbo].[Advices].[Search objekt]
		INNER JOIN [Shopgun].[dbo].Mentors ON
		[MentorInfo].[dbo].Advices.Mentor COLLATE Finnish_Swedish_CI_AS = [Shopgun].[dbo].Mentors.MentorName
		INNER JOIN [Shopgun].[dbo].Semaphore ON
		[MentorInfo].[dbo].Advices.[Colour signal] COLLATE Finnish_Swedish_CI_AS = [Shopgun].[dbo].Semaphore.ColorName		
ORDER BY [Shopgun].[dbo].[Products].GlobalTradeItemNumber

