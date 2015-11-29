DECLARE @search_object nvarchar(255), @mentor nvarchar(255), @semaphore nvarchar(255), 
@headline nvarchar(255),@introduction nvarchar(255), @runnning_text nvarchar(max),
@last_updated datetime, @informer nvarchar(255), @initials nvarchar(255), @splitedRetVal nvarchar(max), @valAlreadyExists nvarchar(255)

DECLARE mentor_advices_cursor CURSOR LOCAL FOR

SELECT [MentorInfo].[dbo].[Advices].[Search objekt], [MentorInfo].[dbo].[Advices].[Mentor], 
[MentorInfo].[dbo].[Advices].[Colour signal], [MentorInfo].[dbo].[Advices].[Headline],
[MentorInfo].[dbo].[Advices].Introduction, [MentorInfo].[dbo].[Advices].[Running Text],
[MentorInfo].[dbo].[Advices].[Date], [MentorInfo].[dbo].[Advices].[Informer],
[MentorInfo].[dbo].[Advices].[Initials]
FROM [MentorInfo].[dbo].[Advices]
WHERE [PA-level] = 'Begrepp'
ORDER BY Mentor, [Date], [Search objekt]

OPEN mentor_advices_cursor

FETCH NEXT FROM mentor_advices_cursor
INTO @search_object, @mentor, @semaphore, @headline,@introduction, @runnning_text,
@last_updated, @informer, @initials

DECLARE @conceptId int, @mentorId int, @semaphoreId int
WHILE @@FETCH_STATUS = 0
BEGIN

	SELECT @splitedRetVal = '', @valAlreadyExists = ''
	
	SELECT TOP 1 @splitedRetVal = val FROM [master].dbo.SPLIT(@search_object,1,0)
	--DEGUG : SELECT @splitedRetVal
	--SELECT @valAlreadyExists = [Shopgun].[dbo].Ingredients.IngredientName
	--FROM [Shopgun].[dbo].Ingredients
	--WHERE IngredientName = @splitedRetVal
	
	--DEGUG : SELECT @valAlreadyExists
	SELECT @mentorId = [Shopgun].[dbo].Mentors.Id
	FROM [Shopgun].[dbo].Mentors
	WHERE [Shopgun].[dbo].Mentors.MentorName = @mentor
	
	SELECT @conceptId = [Shopgun].[dbo].[Concepts].Id
	FROM [Shopgun].[dbo].[Concepts]
	WHERE [Shopgun].[dbo].[Concepts].ConceptTerm = @splitedRetVal
	
	SELECT @semaphoreId = [Shopgun].[dbo].Semaphore.Id
	FROM [Shopgun].[dbo].Semaphore
	WHERE [Shopgun].[dbo].Semaphore.ColorName = @semaphore
	
	IF NOT (@mentorId IS NULL OR @conceptId IS NULL OR @semaphoreId IS NULL)
	BEGIN
	
		INSERT INTO [Shopgun].[dbo].[Advices]
			   ([ConceptsId]
			   , [AdviceType]    
			   ,[MentorId]
			   ,[Label]
			   ,[Introduction]
			   ,[Advice]
			   ,[KeyWords]
			   ,[SemaphoreId]
			   ,[Published]
			   ,[PublishDate])
			   VALUES(@conceptId,'ConceptAdvice',@mentorId,
						@headline, @introduction, @runnning_text, @search_object, @semaphoreId, 1, @last_updated)
						
	--DEGUG : SELECT @conceptId, 'IngredientAdvice', @mentorId, @headline, @introduction, @runnning_text,
	--DEGUG : @search_object, @semaphoreId, 1, @last_updated
	
	END 
	
	FETCH NEXT FROM mentor_advices_cursor
	INTO @search_object, @mentor, @semaphore, @headline,@introduction, @runnning_text,
	@last_updated, @informer, @initials
END

CLOSE mentor_advices_cursor

DEALLOCATE mentor_advices_cursor