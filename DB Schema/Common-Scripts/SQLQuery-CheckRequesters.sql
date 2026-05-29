-- @gebbs.com

DECLARE @CurrentIST DATETIME = SWITCHOFFSET(GETUTCDATE(), '+05:30');

-- gebbs  ('kaushik.ghosh@gebbs.com', 'ramdas.mahale@gebbs.com', 'wilson.joseph@gebbs.com', 'hatim.dhorajiwala@gebbs.com', 'jitendra.raut@gebbs.com')

-- dvc ('rajib.saha@dvc.gov.in','amit.singh@dvc.gov.in')

-- Puravankara ('arvind.singh@puravankara.com','vinesh.kurup@puravankara.com')

-- Thermax ('sanjay.abhyankar@thermaxglobal.com')

-- Valvoline ('uday.biswas@valvolinecummins.com','ravi.kumar@valvolinecummins.com')

-- Hero Fincorp ('salahuddin.haque@herofincorp.com','sonoo.kumar@herofincorp.com')

-- Oil India ('manas_bordoloi@oilindia.in','skborchetia@oilindia.in')

-- Pidilite Industries ('saurabh.jain@pidilite.com')

-- Bombay Dyeing ('sivakumar.nair@bombaydyeing.com')

SELECT
	Req.id,
	first_name,
	last_name,
	primary_email,
	Dept.id AS DeptId,
	Dept.name AS DeptName,
	CustomDept.embee_crm_id,
	CustomDept.customer_portal_access,
	CON.ContractNo,
	CASE
		WHEN CON.StartDate >= @CurrentIST THEN 'Upcoming' 
		WHEN @CurrentIST BETWEEN CON.StartDate AND CON.EndDate THEN 'Active'
		WHEN @CurrentIST >= CON.EndDate AND ExtendSupport <> 1 THEN 'Inactive'
		WHEN @CurrentIST >= CON.EndDate AND ExtendSupport = 1 THEN 'Active - Extend Support'
		ELSE NULL
	END AS 'ActiveStatus'
FROM [dbo].[FreshService_M_Requesters] Req
LEFT JOIN [dbo].[FreshService_M_Requester_Departments] ReqDept WITH (NOLOCK) ON ReqDept.requester_id = Req.id
LEFT JOIN [dbo].[FreshService_M_Department] Dept WITH (NOLOCK) ON ReqDept.department_id = Dept.id
LEFT JOIN [dbo].[FreshService_M_Department_CustomFields] CustomDept WITH (NOLOCK) ON ReqDept.department_id = CustomDept.department_id
LEFT JOIN [dbo].[CP_T_ContractMaster] CON ON CON.DepartmentId = CustomDept.department_id

--WHERE primary_email LIKE '%saurabh.jain%'
WHERE 
ISNULL(embee_crm_id,'') <> ''
AND ISNULL(customer_portal_access, 'false') = 'true'
AND primary_email IN ('fahim.khan@gebbs.com','santosh.bhopale@gebbs.com','nagendra.sherugar@gebbs.com','sanjay.kadam@gebbs.com','felix.ranjanc@gebbs.com','salman.pathan@gebbs.com','bismillah.shaikh@gebbs.com')



--select *  FROM [dbo].[FreshService_M_Requesters] Req