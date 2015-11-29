

INSERT INTO [Shopgun].[dbo].[Products]
	(BrandId, ProductName , GlobalTradeItemNumber,Quantity, 
			QuantityUnit, TableOfContent, Labels,LastUpdated)
			
SELECT	[Shopgun].[dbo].[Brands].[Id],
		[MentorInfo].[dbo].[Products].[Product] COLLATE Finnish_Swedish_CI_AS,
		--cast(cast ([ExtShopGunDemo].[dbo].[Products].[globalTradeItemNumber] as decimal(15,0)) as varchar(20)),
		[MentorInfo].[dbo].[Products].GTIN,
		[MentorInfo].[dbo].[Products].[quantity], 
		[MentorInfo].[dbo].[Products].[Quantity unit],
		[MentorInfo].[dbo].[Products].[Ingredients],
		[MentorInfo].[dbo].[Products].[Labels],
		CASE WHEN([MentorInfo].[dbo].[Products].[CollectedDate] IS NULL) THEN GETDATE() ELSE  [MentorInfo].[dbo].[Products].[CollectedDate] END
FROM	[Shopgun].[dbo].[Brands] INNER JOIN
		[MentorInfo].[dbo].[Products] ON
		[Shopgun].[dbo].[Brands].BrandName = [MentorInfo].[dbo].[Products].Brand COLLATE Finnish_Swedish_CI_AS
		
		
		
		




