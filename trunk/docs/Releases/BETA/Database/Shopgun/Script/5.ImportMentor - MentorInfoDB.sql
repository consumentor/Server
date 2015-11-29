INSERT INTO [Shopgun].[dbo].[Mentors]([MentorName])
SELECT DISTINCT [MentorInfo].[dbo].[Advices].Mentor 
  FROM [MentorInfo].[dbo].[Advices]
  ORDER BY [MentorInfo].[dbo].[Advices].Mentor
  
  
/* Insert more new mentors after the first import*/
INSERT INTO [ShopGun].[dbo].[Mentors]([MentorName])

SELECT DISTINCT [MentorInfo].[dbo].[Advices].Mentor 
	FROM [MentorInfo].[dbo].[Advices] 
  
	WHERE NOT [MentorInfo].[dbo].[Advices].Mentor COLLATE Finnish_Swedish_CI_AS  IN (
		SELECT [ShopGun].[dbo].[Mentors].MentorName
			FROM [ShopGun].[dbo].[Mentors])
ORDER BY [MentorInfo].[dbo].[Advices].Mentor  