ALTER TABLE [ShopGunIntegrationTest].[dbo].[Mentors]
ADD Description NVARCHAR(MAX),
Url NVARCHAR(255);

CREATE TABLE [ShopGunIntegrationTest].[dbo].CertificationMarkMappings
(
Id int IDENTITY PRIMARY KEY,
CertificationMarkMappingType nvarchar(55) not null,
CertificationMarkId int FOREIGN KEY REFERENCES [ShopGunIntegrationTest].[dbo].[CertificationMarks](Id),
ProviderCertificationId int,
ProviderCertificationName nvarchar(255)
)

CREATE TABLE [ShopGunIntegrationTest].[dbo].Tags
(
Id int IDENTITY PRIMARY KEY,
TagType nvarchar(55) not null,
Name nvarchar(50)
)

ALTER TABLE [ShopGunIntegrationTest].[dbo].[Advices]
ADD TagId int;

ALTER TABLE [ShopGunIntegrationTest].[dbo].[Advices]
ADD FOREIGN KEY (TagId) REFERENCES [ShopGunIntegrationTest].[dbo].[Tags](Id)

ALTER TABLE [ShopGunIntegrationTest].[dbo].[Advices]
ALTER COLUMN KeyWords nvarchar(4000)

ALTER TABLE [ShopGunIntegrationTest].[dbo].[Ingredients]
ADD ParentId int;