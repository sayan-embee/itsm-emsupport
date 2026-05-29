SELECT 
    referencing_schema_name,
    referencing_entity_name,
    referencing_class_desc,
    is_caller_dependent
FROM sys.dm_sql_referencing_entities ('dbo.FreshService_T_Tickets', 'OBJECT')
UNION
SELECT 
    referencing_schema_name,
    referencing_entity_name,
    referencing_class_desc,
    is_caller_dependent
FROM sys.dm_sql_referencing_entities ('dbo.FreshService_T_Ticket_CustomFields', 'OBJECT')