SELECT 
    name AS ConstraintName,
    definition AS ConstraintDefinition
FROM 
    sys.check_constraints 
WHERE 
    parent_object_id = OBJECT_ID('TeachersToExams');