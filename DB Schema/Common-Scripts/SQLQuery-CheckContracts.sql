SELECT * FROM CP_T_ContractMaster
--WHERE ContractNo = '222-042024-MSD-001'
WHERE DepartmentId = 27000432686
ORDER BY TenantId, CategoryId, SubCategoryId, StartDate;

--ALTER TABLE CP_T_ContractMaster ADD ExtendSupport BIT NULL;
--UPDATE CP_T_ContractMaster SET ExtendSupport = 1 WHERE DepartmentId = 27000189813

SELECT * FROM [dbo].[CP_T_ContractMasterFiles]



SELECT [Id]
		,[Code] AS 'CategoryCode'
		,[Name] AS 'CategoryName'
		,[Active]
	FROM [dbo].[CP_M_Category] C


	SELECT S.[Id]
		,S.[CategoryId]
		,CONCAT(C.Code,S.[Code]) AS 'SubCategoryCode'
		,S.[Name] AS 'SubCategoryName'
		,S.[Active]
	FROM [dbo].[CP_M_SubCategory] S
	INNER JOIN [dbo].[CP_M_Category] C ON C.Id = S.CategoryId


SELECT ContractNo, COUNT(*)
FROM [dbo].[CP_T_ContractMaster]
GROUP BY ContractNo
HAVING COUNT(*) > 1;


SELECT ContractNo, TenantId, DepartmentId, CategoryId, SubCategoryId, COUNT(*) AS DuplicateCount
FROM [dbo].[CP_T_ContractMaster]
GROUP BY ContractNo, TenantId, DepartmentId, CategoryId, SubCategoryId
HAVING COUNT(*) > 1;

-- Ensures no duplicate contracts exist with the same Tenant, Department, Category, SubCategory, and exact Start and End Dates.
SELECT TenantId, DepartmentId, CategoryId, SubCategoryId, StartDate, EndDate, COUNT(*)
FROM CP_T_ContractMaster
GROUP BY TenantId, DepartmentId, CategoryId, SubCategoryId, StartDate, EndDate
HAVING COUNT(*) > 1;

-- Identifies contracts with overlapping date ranges that should not exist.
SELECT c1.Id, c1.ContractNo, c2.Id AS OverlappingId, c2.ContractNo AS OverlappingContract
FROM CP_T_ContractMaster c1
JOIN CP_T_ContractMaster c2
ON c1.TenantId = c2.TenantId
AND c1.DepartmentId = c2.DepartmentId
AND c1.CategoryId = c2.CategoryId
AND c1.SubCategoryId = c2.SubCategoryId
AND c1.Id <> c2.Id
AND (
    (c1.StartDate >= c2.StartDate AND c1.StartDate < c2.EndDate) 
    OR (c1.EndDate > c2.StartDate AND c1.EndDate <= c2.EndDate)
    OR (c1.StartDate <= c2.StartDate AND c1.EndDate >= c2.EndDate)
);

-- Verify Contract Number Sequence Consistency
SELECT TenantId, CategoryId, SubCategoryId, YEAR(StartDate) AS ContractYear, 
       ContractNo, ROW_NUMBER() OVER (PARTITION BY TenantId, CategoryId, SubCategoryId, YEAR(StartDate) ORDER BY StartDate) AS ExpectedSequence
FROM CP_T_ContractMaster
WHERE YEAR(StartDate) = 2024
ORDER BY TenantId, CategoryId, SubCategoryId, StartDate;

DECLARE @IsFinancialYear BIT = 0;
SELECT TenantId, CategoryId, SubCategoryId, 
       StartDate, ContractNo, 
       CASE 
           WHEN @IsFinancialYear = 1 AND MONTH(StartDate) < 4 THEN YEAR(StartDate) - 1
           ELSE YEAR(StartDate)
       END AS FinancialYearReference
FROM CP_T_ContractMaster
ORDER BY TenantId, CategoryId, SubCategoryId, StartDate;


WITH ContractSequence AS (
    SELECT 
        ContractNo,
        TenantId,
        CategoryId,
        SubCategoryId,
        RIGHT(ContractNo, 3) AS SequenceNumber,
        COUNT(*) OVER (PARTITION BY ContractNo) AS DuplicateCount
    FROM CP_T_ContractMaster
)
SELECT ContractNo, TenantId, CategoryId, SubCategoryId, SequenceNumber, DuplicateCount
FROM ContractSequence
WHERE DuplicateCount > 1
ORDER BY ContractNo;


--DELETE FROM [dbo].[CP_T_ContractMaster] WHERE ID IN (45,46) 
--DELETE FROM [dbo].[CP_T_ContractMasterFiles] WHERE ContractId IN (45) 

SELECT * FROM 
[dbo].[CP_T_ContractMaster]
--where DepartmentId = 27000164414
--WHERE CustomerName like 'Medikabazaar'

SELECT * FROM 
[dbo].[FreshService_M_Department]
where id = 27000111932
--WHERE name like '%Bombay Dyeing%'

SELECT * FROM 
[dbo].[FreshService_M_Department_CustomFields]
where department_id = 27000111932

SELECT * FROM
FreshService_M_Requester_Departments
where 
--department_id = 27000111932 and
requester_id = 27002476624

SELECT * FROM
FreshService_M_Requesters
WHERE primary_email LIKE '%sonoo.kumar@herofincorp.com%'
--WHERE ID = 27000374249

SELECT * FROM
FreshService_M_Requester_Departments
WHERE requester_id = 27002476624


--INSERT INTO FreshService_M_Requester_Departments (requester_id, department_id) VALUES (27002476624, 27000109646)


