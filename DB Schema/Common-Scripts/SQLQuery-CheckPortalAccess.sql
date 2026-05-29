EXEC	[dbo].[usp_CP_T_FreshService_CustomerDetails_Get]
		@customerEmail = 'bismillah.shaikh@gebbs.com'

GO

EXEC	[dbo].[usp_CP_CustomerWise_MasterData_Get]
		@embee_crm_ids = 'CG193',
		@DepartmentIds = '27000111932'
