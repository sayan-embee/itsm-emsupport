DECLARE @IsFinancialYear BIT = 0;
WITH OrderedContracts AS (
    SELECT 
        Id,
        CategoryId,
        SubCategoryId,
        TenantId,
        StartDate,
        RIGHT('0' + CAST(MONTH(StartDate) AS VARCHAR(2)), 2) AS StartMonth,
        CASE 
            WHEN @IsFinancialYear = 1 AND MONTH(StartDate) < 4 THEN YEAR(StartDate) - 1
            ELSE YEAR(StartDate)
        END AS ReferenceYear,
        ROW_NUMBER() OVER (
            PARTITION BY TenantId, CategoryId, SubCategoryId, 
                         CASE 
                             WHEN @IsFinancialYear = 1 AND MONTH(StartDate) < 4 THEN YEAR(StartDate) - 1
                             ELSE YEAR(StartDate)
                         END
            ORDER BY StartDate
        ) AS SequenceNo
    FROM CP_T_ContractMaster
)
UPDATE CM
SET CM.ContractNo = C.Code + S.Code + '-' +
                     OC.StartMonth + CAST(OC.ReferenceYear AS VARCHAR) + '-' +
                     T.TenantCode + '-' + 
                     RIGHT('000' + CAST(OC.SequenceNo AS VARCHAR(3)), 3)
FROM CP_T_ContractMaster CM
JOIN OrderedContracts OC ON CM.Id = OC.Id
JOIN CP_M_Category C ON CM.CategoryId = C.Id
JOIN CP_M_SubCategory S ON CM.SubCategoryId = S.Id
JOIN CP_M_Tenant T ON CM.TenantId = T.Id;
