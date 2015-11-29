CREATE TABLE [ShopGun].[dbo].Tips
(
Id int IDENTITY PRIMARY KEY,
Tip nvarchar(400),
LastUpdated datetime,
Published bit
)