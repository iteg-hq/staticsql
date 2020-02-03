
CREATE VIEW dbo.Person
AS
SELECT
    FirstName    
  , LastName    
  , Age    
FROM dbo.PersonTable AS outer_table
WHERE NOT EXISTS (
    SELECT 1
    FROM dbo.PersonTable AS inner_table
    WHERE inner_table.PersonID = outer_table.PersonID
      AND inner_table.InsertedOn > outer_table.InsertedOn
  )

