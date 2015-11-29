INSERT INTO [Shopgun].[dbo].[Semaphore]
           ([ColorName]
           ,[Value])
     VALUES
           ('R',-1),
           ('Y', 0),
           ('G', 1)
           
UPDATE [Shopgun].[dbo].[Semaphore]
	SET ColorName = 'Red'
	WHERE Value = -1
 
UPDATE [Shopgun].[dbo].[Semaphore]
	SET ColorName = 'Yellow'
	WHERE Value = 0
 
UPDATE [Shopgun].[dbo].[Semaphore]
	SET ColorName = 'Green'
	WHERE Value = 1
 


