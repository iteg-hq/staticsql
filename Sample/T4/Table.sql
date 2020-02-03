
CREATE TABLE dbo.PersonTable (
    PersonID INT NOT NULL
  , FirstName NVARCHAR(50) NOT NULL
  , LastName NVARCHAR(50) NOT NULL
  , Age INT NULL
  , InsertedOn DATETIME2(7) DEFAULT SYSUTCDATETIME()
  , CONSTRAINT PK_Person PRIMARY KEY ( PersonID, InsertedOn )
  )

GO
