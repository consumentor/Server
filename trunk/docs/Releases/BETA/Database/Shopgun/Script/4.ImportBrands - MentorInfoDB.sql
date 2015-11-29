INSERT INTO [Shopgun].[dbo].[Brands]([BrandName], [LastUpdated])
SELECT DISTINCT [Brand], GETDATE()
  FROM [MentorInfo].[dbo].[Products]
  ORDER BY [Brand]
  


UPDATE    [Shopgun].[dbo].[Brands]
SET   [Shopgun].[dbo].[Brands].CompanyId = company.[Id]

FROM	MentorInfo.dbo.Products AS mentor INNER JOIN
        [Shopgun].[dbo].Brands ON mentor.Brand COLLATE Finnish_Swedish_CI_AS = [Shopgun].[dbo].[Brands].BrandName INNER JOIN
        [Shopgun].[dbo].Companies AS company ON mentor.Owner COLLATE Finnish_Swedish_CI_AS = company.Name
        
        
/*SELECT     company.Id, company.Name, company.Address, company.PostCode, company.City, company.PhoneNumber, company.URLToHomePage, 
                      company.ContactEmailAddress, company.ParentId, company.CountryId, company.LastUpdated
FROM         MentorInfo.dbo.Products AS mentor INNER JOIN
                      [Shopgun].[dbo].Brands ON mentor.Brand = [Shopgun].[dbo].Brands.BrandName INNER JOIN
                      [Shopgun].[dbo].Companies AS company ON mentor.Owner = company.Name */       

