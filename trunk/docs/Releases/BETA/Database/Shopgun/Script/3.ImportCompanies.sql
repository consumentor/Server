INSERT INTO [Shopgun].[dbo].[Companies]([Name], [Address], [PostCode]
      ,[City],[PhoneNumber],[URLToHomePage],[ContactEmailAddress],[LastUpdated])
SELECT DISTINCT [Owner],[Address],[Postcode],
		[City],[Telephone],[Homepage],[E-mail],[Date]
  FROM [MentorInfo].[dbo].[Owners]
  ORDER BY [Owner], City