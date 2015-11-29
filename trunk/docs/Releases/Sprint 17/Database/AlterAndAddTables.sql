ALTER TABLE [ShopGunIntegrationTest].[dbo].[Products]
ADD ImageUrlSmall NVARCHAR(255)
,ImageUrlMedium NVARCHAR(255)
,ImageUrlLarge NVARCHAR(255)
,Durability NVARCHAR(255)
,SupplierArticleNumber NVARCHAR(255)
,Nutrition NVARCHAR(255);

CREATE TABLE [ShopGunIntegrationTest].[dbo].AllergyInformation
(
Id int IDENTITY PRIMARY KEY,
Remark nvarchar(4000),
ProductId int FOREIGN KEY REFERENCES [ShopGunIntegrationTest].[dbo].[Products](Id),
IngredientId int FOREIGN KEY REFERENCES [ShopGunIntegrationTest].[dbo].[Ingredients](Id)
)

CREATE TABLE [ShopGunIntegrationTest].[dbo].CertificationMarks
(
Id int IDENTITY PRIMARY KEY,
CertificationName NVARCHAR(255) NOT NULL,
Gs1Code NVARCHAR(255),
CertificationMarkImageUrlSmall NVARCHAR(255),
CertificationMarkImageUrlMedium NVARCHAR(255),
CertificationMarkImageUrlLarge NVARCHAR(255),
MentorId int FOREIGN KEY REFERENCES [ShopGunIntegrationTest].[dbo].[Mentors](Id)
)

CREATE TABLE [ShopGunIntegrationTest].[dbo].Product_CertificationMark
(
Id int IDENTITY PRIMARY KEY,
ProductId int FOREIGN KEY REFERENCES [ShopGunIntegrationTest].[dbo].[Products](Id),
CertificationMarkId int FOREIGN KEY REFERENCES [ShopGunIntegrationTest].[dbo].[CertificationMarks](Id)
)

