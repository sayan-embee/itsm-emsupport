USE [AI-Portal-Apps]
GO

DECLARE	@return_value int

EXEC	@return_value = [dbo].[usp_FreshService_R_SummaryLast3Months]
		@DepartmentId = 27000295923,
		@Start_Date = N'01/05/2025',
		@End_Date = N'31/05/2025'

SELECT	'Return Value' = @return_value

GO

DECLARE
@DepartmentId BIGINT=27000295923,
@Start_Date	VARCHAR(10)='01/05/2025',
@End_Date VARCHAR(10)='31/05/2025'
DECLARE @LastDateTobeConsidered DATE = (SELECT TOP 1 LastDate FROM LastDateTobeConsidered WHERE ACTIVE =1);


SELECT * FROM FreshService_T_Tickets T 
INNER JOIN [FreshService_T_Ticket_CustomFields] TC WITH(NOLOCK) ON TC.ticket_id=T.id
WHERE 
    --TYPE = 'Service Request'
	T.[status] IN (4,5)
	 --T.created_at BETWEEN CONVERT(DATE,@Start_Date,103) AND CONVERT(DATE,@End_Date,103)
	AND CAST(SWITCHOFFSET(CAST(T.created_at AS DATETIMEOFFSET), '+05:30') AS DATE) 
      BETWEEN CONVERT(DATE, @Start_Date, 103) AND CONVERT(DATE, @End_Date, 103)
	  AND (CAST(SWITCHOFFSET(CAST(T.created_at AS DATETIMEOFFSET), '+05:30') AS DATE) > @LastDateTobeConsidered OR ISNULL(@LastDateTobeConsidered,'')='')
	AND (T.department_id=@DepartmentId OR ISNULL(@DepartmentId,0)=0)
	AND TC.tenant IN (
		SELECT TenantName 
		FROM MonthlyReport_M_Tenant WITH (NOLOCK) 
		WHERE Active = 1
	)
	AND requester_id NOT IN
	(
	SELECT id from [FreshService_M_Requesters]
	)


--select * from [FreshService_M_Requesters] where id = 27000351986

SELECT * 
FROM [FreshService_T_Change] C WITH(NOLOCK) 
	INNER JOIN [FreshService_M_Department] D WITH(NOLOCK) ON C.department_id=D.id
	WHERE CAST(SWITCHOFFSET(CAST(C.created_at AS DATETIMEOFFSET), '+05:30') AS DATE) 
      BETWEEN CONVERT(DATE, @Start_Date, 103) AND CONVERT(DATE, @End_Date, 103)
	AND (C.department_id=@DepartmentId OR ISNULL(@DepartmentId,0)=0)
	--AND C.[status] IN (4,5)
	AND (CAST(SWITCHOFFSET(CAST(C.created_at AS DATETIMEOFFSET), '+05:30') AS DATE) > @LastDateTobeConsidered OR ISNULL(@LastDateTobeConsidered,'')='')