
CREATE PROCEDURE dbo.AddPerson
    @PersonID INT
  , @FirstName NVARCHAR(50)
  , @LastName NVARCHAR(50)
  , @Age INT
AS

INSERT INTO dbo.PersonTable (
    PersonID
  , FirstName
  , LastName
  , Age
)
VALUES (
    @PersonID
  , @FirstName    
  , @LastName    
  , @Age    
)

GO
