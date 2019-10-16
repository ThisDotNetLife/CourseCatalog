    DROP PROCEDURE IF EXISTS dbo.jsonSaveWebcast
	GO

    SET QUOTED_IDENTIFIER ON
	SET ANSI_NULLS ON
	SET NOCOUNT ON
	GO

/*  ========================================================================================
	Name:	  jsonSaveWebcast
	Author:   Mark Orlando
	Date:	  10/14/2019
	Remarks:  Save (INSERT or UPDATE) developer webcast.
	
	EXEC dbo.jsonSaveWebcast @JsonInput='{"ID":0,"PhysicalPath":"PRIMARY Webcasts\\A-P\\C#","Title":"C# Extension Methods","Vendor":"Pluralsight","Author":"Elton Stoneman","ReleaseDate":"10/01/2019","Summary":"Extension methods are a key feature of C# - they boost productivity, increase the readability of your code, and make it easy to implement standards across your projects. In this course, C# Extension Methods, you will learn all you need to know about extension methods in .NET Framework and .NET Core. First, you will learn the basics of extension methods: how to use them, how to write them, and how the tooling in Visual Studio and VS Code helps you to work with them. Then, you will discover the danger areas and the best practices for building your own extension methods. Finally, you will explore how to package and publish your own extension method library. By the end of this course, you will have a complete understanding of how to get value from this powerful feature.","URL":"https://app.pluralsight.com/library/courses/c-sharp-extension-methods/table-of-contents","Tag":"C#"}'
	EXEC dbo.jsonSaveWebcast @JsonInput='{"ID":0,"PhysicalPath":"PRIMARY Webcasts\\R-Z\\SQL-Server","Title":"T-SQL Data Manipulation Playbook","Vendor":"Pluralsight","Author":"Xavier Morera","ReleaseDate":"09/27/2019","Summary":"In this course, T-SQL Data Manipulation Playbook, you’ll learn foundational knowledge required to add records, modify data, and remove records in your SQL Server database. First, you’ll learn how to add data using the INSERT statement. Next, you’ll discover how to modify data using UPDATE, and how to remove data using the DELETE statement. Moving on, you’ll explore how to maintain database integrity with transactions. Finally, you'll examine some more advanced T-SQL statements that are not that common, but can help you with your daily work. When you’re finished with this course, you'll have the skills and knowledge of the T-SQL Data Manipulation Language needed to insert, update, and delete data in Microsoft SQL Server.","URL":"https://app.pluralsight.com/library/courses/t-sql-data-manipulation-playbook","Tag":"T-SQL"}'
	EXEC dbo.jsonSaveWebcast @JsonInput='{"ID":0,"PhysicalPath":"PRIMARY Webcasts\\A-P\\Cosmos-DB","Title":"Learning Azure Cosmos DB","Vendor":"Pluralsight","Author":"Leonard Lobel","ReleaseDate":"09/25/2019","Summary":"Developers today require a thorough knowledge of the NoSQL technologies that lie at the core of global web and mobile applications. In Learning Azure Cosmos DB, you will learn how to utilize Microsoft’s massively scalable, globally distributed, multi-model NoSQL database service. First, you will discover how to provision throughput, partition, and globally distribute your database. Next, you will explore the SQL API and the document data model, build client applications using the .NET SDK, and leverage the server-side programming model with stored procedures, triggers, and user-defined functions. Finally, you will learn how to use the Table API to migrate Azure Table Storage applications and the Gremlin API to build graph databases. When you are finished with this course, you will have a foundational knowledge of Azure Cosmos DB that will help you as you move forward to build your next generation of global applications.","URL":"https://app.pluralsight.com/library/courses/azure-cosmos-db","Tag":"No-SQL"}'
	
	Revision History:
	Date		Author			Description
	========================================================================================
	
    ======================================================================================== */	
	CREATE PROCEDURE dbo.jsonSaveWebcast(@JsonInput VARCHAR(MAX))	
	AS
	BEGIN
		BEGIN TRY

		DECLARE @SPStep		   INT
		DECLARE @SPMessage     NVARCHAR(1000)	
		DECLARE @VendorID      INT
		DECLARE @AuthorID      INT
		DECLARE @TagID         INT

		DECLARE @ID            INT
		DECLARE @PhysicalPath  VARCHAR(200)
		DECLARE @Title         VARCHAR(500)
		DECLARE @Vendor        VARCHAR(25)
		DECLARE @Author        VARCHAR(50)
		DECLARE @ReleaseDate   DATE
		DECLARE @Summary       VARCHAR(MAX)
		DECLARE @URL           VARCHAR(200)
		DECLARE @Tag           VARCHAR(200)	

		BEGIN 
			SET @SPStep = 2
			SELECT @ID=ID, @PhysicalPath=PhysicalPath, @Title=Title, @Vendor=Vendor, @Author=Author, 
			       @ReleaseDate=CONVERT(VARCHAR(10), ReleaseDate, 101), 
				   @Summary=Summary, @URL=URL, @Tag=Tag
			  FROM OPENJSON(@JsonInput) WITH ( ID            INT           '$.ID',
											   PhysicalPath  VARCHAR(200)  '$.PhysicalPath',
											   Title         VARCHAR(500)  '$.Title',
											   Vendor        VARCHAR(25)   '$.Vendor',
											   Author        VARCHAR(50)   '$.Author',
											   ReleaseDate   VARCHAR(10)   '$.ReleaseDate',
											   Summary       VARCHAR(MAX)  '$.Summary',
											   URL           VARCHAR(200)  '$.URL',
											   Tag           VARCHAR(200)  '$.Tag')
		END -- PARSE JSON INTO LOCAL VARIABLES

		BEGIN 
			SET @SPStep = 4
			SELECT @VendorID=ID FROM dbo.Vendor WHERE CompanyName=@Vendor
			SELECT @AuthorID=ID FROM dbo.Author WHERE FullName=@Author
			SELECT @TagID=ID    FROM dbo.Tag    WHERE Descr=@Tag
		END -- GET PRIMARY KEYS FOR KVPs (VENDOR, AUTHOR, TAG)

		BEGIN 
			SET @SPStep = 6
			IF @ID = 0
			    IF EXISTS(SELECT 1 FROM dbo.Webcast WHERE PhysicalPath=@PhysicalPath AND Title=@Title)
					BEGIN					
						SET @SPMessage = 'Webcast cannot be added because an entry with same name already exists in ' + '"\' +@PhysicalPath + '".'
						RAISERROR(@SPMessage, 16, 1)
					END
		END -- THROW EXCEPTION IF USER ATTEMPTING TO INSERT WEBCAST WHERE ONE WITH SAME NAME ALREADY EXISTS IN GIVEN PATH.

		BEGIN TRANSACTION 

		BEGIN 
		IF @VendorID IS NULL
			BEGIN
				SET @SPStep = 8
				INSERT dbo.Vendor(CompanyName) VALUES (@Vendor)
				SET @VendorID = SCOPE_IDENTITY()
			END
		END -- IF VENDOR IS NEW, ADD ENTRY TO dbo.Vendor TABLE AND GET NEW PRIMARY KEY.

		BEGIN 
		IF @AuthorID IS NULL
			BEGIN
				SET @SPStep = 10
				INSERT dbo.Author(FullName) VALUES (@Author)
				SET @AuthorID = SCOPE_IDENTITY()
			END
		END -- IF AUTHOR IS NEW, ADD ENTRY TO dbo.Author TABLE AND GET NEW PRIMARY KEY.

		BEGIN 
		IF @TagID IS NULL
			BEGIN
				SET @SPStep = 12
				INSERT dbo.Tag(Descr) VALUES (@Tag)
				SET @TagID = SCOPE_IDENTITY()
			END
		END -- IF TAG (TOPIC) IS NEW, ADD ENTRY TO dbo.Tag TABLE AND GET NEW PRIMARY KEY.

		IF @ID = 0
			BEGIN 
				SET @SPStep = 14
				INSERT dbo.Webcast(PhysicalPath, Title, VendorID, AuthorID, ReleaseDate, Summary, URL)
					VALUES(@PhysicalPath, @Title, @VendorID, @AuthorID, @ReleaseDate, @Summary, @URL)
				SET @ID = SCOPE_IDENTITY()
				INSERT dbo.WebcastTag(WebcastID, TagID) VALUES (@ID, @TagID)
			END -- ADD NEW WEBCAST
		ELSE
			BEGIN 
			 -- GET KEY-VALUE PAIRS OF ALL TAGS PREVIOUSLY ASSOCIATED WITH EXISTING WEBCAST
				SET @SPStep = 16
			    DROP TABLE IF EXISTS #Tags
				CREATE TABLE #Tags (TagID INT DEFAULT(0), Descr VARCHAR(50) PRIMARY KEY, ActionToTake VARCHAR(1) DEFAULT 'D')
				INSERT #Tags(TagID, Descr)
					SELECT TagID, Descr
					  FROM dbo.Tag T INNER JOIN dbo.WebcastTag WT ON T.ID=WT.TagID AND WT.WebcastID=@ID

			 -- IF TAG STILL ASSOCIATED WITH THIS WEBCAST THEN MARK ACTION TO TAKE AS (K)EEP.
				SET @SPStep = 18
				UPDATE #Tags
				   SET ActionToTake = 'K' -- KEEP.
				 WHERE Descr = @Tag

			 -- IF USER ASSIGNED NEW TAGS TO THIS WEBCAST THEN INCLUDE THEM IN TABLE TO BE ADDED.
				SET @SPStep = 20
				IF NOT EXISTS(SELECT 1 FROM #Tags WHERE DESCR=@Tag)
					 INSERT #Tags (Descr, ActionToTake) VALUES (@TAG, 'A')  -- ADD.

			 -- UPDATE TEMP TABLE WITH NEW IDs FOR ANY TAGS WE JUST CREATED.
				SET @SPStep = 22
				UPDATE T
				   SET T.TagID=TG.ID
				  FROM #Tags T INNER JOIN dbo.Tag TG ON T.Descr=TG.Descr AND T.ActionToTake='A'

             -- REMOVE ENTRIES FROM dbo.WebcastTag THAT ARE NO LONGER NEEDED BECAUSE TAG ISN'T ASSOCIATED WITH THIS WEBCAST.
				SET @SPStep = 24
			    DELETE WT
				  FROM dbo.WebcastTag WT INNER JOIN #Tags T ON WT.TagID=T.TagID AND WT.WebcastID=@ID AND T.ActionToTake='D'
				
             -- ADD ROWS TO dbo.WebcastTag FOR ALL NEW TAGS ASSOCIATED WITH THIS WEBCAST.
				SET @SPStep = 26
				INSERT dbo.WebcastTag(WebcastID, TagID)
					SELECT @ID, TagID
					  FROM #Tags WHERE ActionToTake='A'

				SET @SPStep = 28
				UPDATE dbo.Webcast
				   SET PhysicalPath=@PhysicalPath, 
				       Title=@Title, VendorID=@VendorID, AuthorID=@AuthorID, ReleaseDate=@ReleaseDate, Summary=@Summary, URL=@URL
				 WHERE ID=@ID

              -- DROP ROWS FROM dbo.Vendor TABLE THAT ARE ORPHANED.
				 SET @SPStep = 30
				 DELETE V FROM dbo.Vendor V                      
				   LEFT JOIN dbo.Webcast W ON W.VendorID = V.ID 
				  WHERE W.VendorID IS NULL

              -- DROP ROWS FROM dbo.Author TABLE THAT ARE ORPHANED.
				 SET @SPStep = 32
				 DELETE A FROM dbo.Author A
				   LEFT JOIN dbo.Webcast W ON W.AuthorID = A.ID 
				  WHERE W.AuthorID IS NULL

			  -- DELETE ANY TAGS NO LONGER ASSOCIATED WITH THIS WEBCAST.
				 SET @SPStep = 34
				 DELETE WT
				   FROM dbo.WebcastTag WT
					INNER JOIN #Tags T ON WT.TagID=T.TagID AND T.ActionToTake='D' AND WT.WebcastID=@ID

			  -- DROP ROWS FROM dbo.Tag TABLE THAT ARE ORPHANED.
			     SET @SPStep = 36
				 DELETE T FROM dbo.Tag T                      
				   LEFT JOIN dbo.WebcastTag WT ON WT.TagID = T.ID 
				  WHERE WT.TagID IS NULL

				 DROP TABLE IF EXISTS #Tags
			END -- UPDATE EXISTING WEBCAST

		SET @SPStep = 36
		EXEC dbo.jsonGetWebcast @ID

		COMMIT TRANSACTION

		RETURN 0

		END TRY	

		BEGIN CATCH
			BEGIN
				IF @@TRANCOUNT > 0
				    ROLLBACK TRANSACTION
				IF LEN(@SPMessage) = 0
					SET @SPMessage = 'Exception occurred in jsonSaveWebcast at step ' + CAST (@SPStep as NVARCHAR(10)) + 
					' with error: ' + CAST(ERROR_NUMBER() as NVARCHAR(10)) + ' - ' + ERROR_MESSAGE();
				THROW 51001, @SPMessage, 1;

				RETURN 1
			END
		END CATCH
		RETURN 
	END
GO