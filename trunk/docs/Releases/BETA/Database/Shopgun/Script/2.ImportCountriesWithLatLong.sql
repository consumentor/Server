INSERT INTO [Shopgun].[dbo].[Countries]([CountryCodeId], Capital, Latitude, Longitude, LastUpdated)
SELECT  [Shopgun].[dbo].[CountryCodes].[Id], [MentorInfo].[dbo].[LatLong].[Capital], 
		[MentorInfo].[dbo].[LatLong].[Latitude], [MentorInfo].[dbo].[LatLong].[Longitude], GETDATE()
  FROM [Shopgun].[dbo].[CountryCodes] INNER JOIN
  [MentorInfo].[dbo].[LatLong] ON
  [Shopgun].[dbo].CountryCodes.Name COLLATE Finnish_Swedish_CI_AS = [MentorInfo].[dbo].[LatLong].[Country]
  ORDER BY [Shopgun].[dbo].CountryCodes.[Name], [MentorInfo].[dbo].[LatLong].[Capital]
      
  
-- Step II --
INSERT INTO [Shopgun].[dbo].[Countries]([CountryCodeId], LastUpdated)			
--SELECT Name, ISOCode, [Shopgun].[dbo].[CountryCodes].[Id], GETDATE()
SELECT [Shopgun].[dbo].[CountryCodes].[Id], GETDATE()
FROM [Shopgun].[dbo].[CountryCodes]

WHERE NOT [Shopgun].[dbo].[CountryCodes].Id IN(
		SELECT [Shopgun].[dbo].[Countries].CountryCodeId
			FROM [Shopgun].[dbo].[Countries])
ORDER BY [Name], [ISOCode]

  