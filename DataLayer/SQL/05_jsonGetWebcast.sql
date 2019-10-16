    DROP PROCEDURE IF EXISTS dbo.jsonGetWebcast
	GO

    SET QUOTED_IDENTIFIER ON
	SET ANSI_NULLS ON
	SET NOCOUNT ON
	GO

/*  ========================================================================================
	Name:	  SaveWebcast
	Author:   Mark Orlando
	Date:	  10/14/2019
	Remarks:  Return webcast(s) that meet selection criteria.
	
	EXEC dbo.jsonGetWebcast @ID=1471

	Revision History:
	Date		Author			Description
	========================================================================================
	
    ======================================================================================== */	
	CREATE PROCEDURE dbo.jsonGetWebcast 
	    @ID            INT
	AS
	BEGIN
		BEGIN TRY
	
		SET NOCOUNT ON

		DECLARE @SPStep		        INT
		DECLARE @SPMessage          NVARCHAR(1000)	

		SELECT (SELECT W.ID, W.PhysicalPath, W.Title, V.CompanyName, A.FullName, W.ReleaseDate, W.Summary, W.URL, T.Descr AS Tag
				  FROM Webcast W
						INNER JOIN dbo.Vendor V      ON V.ID=W.VendorID
						INNER JOIN dbo.Author A      ON A.ID=W.AuthorID
						INNER JOIN dbo.WebcastTag WT ON W.ID=WT.WebcastID
						INNER JOIN dbo.Tag T         ON T.ID=WT.TagID
				 WHERE W.ID=@ID FOR JSON PATH) AS JsonOutput

		RETURN 0

		END TRY	

		BEGIN CATCH
			BEGIN
				SET @SPMessage = 'Exception occurred in jsonGetWebcast at step ' + CAST (@SPStep as NVARCHAR(10)) + 
				' with error: ' + CAST(ERROR_NUMBER() as NVARCHAR(10)) + ' - ' + ERROR_MESSAGE();
				THROW 51001, @SPMessage, 1;

				RETURN 1
			END
		END CATCH
		RETURN 
	END
GO