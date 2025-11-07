SELECT name, definition
FROM sys.check_constraints
WHERE parent_object_id = OBJECT_ID('dbo.Exam');

ALTER TABLE dbo.Exam
DROP CONSTRAINT CK__Exam__Format__41EDCAC5;
