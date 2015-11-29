INSERT INTO [Shopgun].[dbo].CountryCodes(Name, ISOCode)
SELECT DISTINCT Land, ISOCode 
  FROM [MentorInfo].[dbo].LandkodISO
  ORDER BY [Land], [ISOCode]