ALTER TABLE [ShopGunIntegrationTest].[dbo].[CertificationMarks]
ADD Description NVARCHAR(MAX),
Url NVARCHAR(255);

ALTER TABLE [ShopgunIntegrationTest].[dbo].[Users]
ADD MentorId int;

ALTER TABLE [ShopgunIntegrationTest].[dbo].[Users]
ADD FOREIGN KEY (MentorId) REFERENCES [ShopGunIntegrationTest].[dbo].[Mentors](Id)
