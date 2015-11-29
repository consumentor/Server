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
WHERE [PA-level] = 'Land' AND [Mentor] = 'Greenpeace'
ORDER BY [Search objekt]

OPEN mentor_advices_cursor

FETCH NEXT FROM mentor_advices_cursor
INTO @search_object, @mentor, @semaphore, @headline,@introduction, @runnning_text,
@last_updated, @informer, @initials

WHILE @@FETCH_STATUS = 0
BEGIN

	SELECT @splitedRetVal = '', @valAlreadyExists = ''
	
	SELECT TOP 1 @splitedRetVal = val FROM dbo.SPLIT(@search_object,1,0)
	--DEGUG : SELECT @splitedRetVal
	SELECT @valAlreadyExists = [Shopgun].[dbo].CountryCodes.Name
	FROM [Shopgun].[dbo].CountryCodes
	WHERE Name = @splitedRetVal
	
	--DEGUG :SELECT @valAlreadyExists
	
	IF @valAlreadyExists = '' OR @valAlreadyExists IS NULL
	BEGIN
		--INSERT INTO [Shopgun].[dbo].CountryCodes(, LastUpdated) 
		--			VALUES(@splitedRetVal, @last_updated)
		SELECT @splitedRetVal
	END 
	
	FETCH NEXT FROM mentor_advices_cursor
	INTO @search_object, @mentor, @semaphore, @headline,@introduction, @runnning_text,
	@last_updated, @informer, @initials
END

CLOSE mentor_advices_cursor

DEALLOCATE mentor_advices_cursor