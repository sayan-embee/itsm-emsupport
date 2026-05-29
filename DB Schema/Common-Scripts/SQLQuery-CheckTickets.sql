create  VIEW VW_Site24x7_ServerPerformance AS
SELECT H.zaaid, H.RowId,h.dtStartDate,h.dtEndDate, S.ServerName,M.DISKUSEDPERCENT,M.MEMUSEDPERCENT,M.CPUUSEDPERCENT,M.param_metric_aggregation 
FROM [Site24x7_T_Per_Report_Server_Hdr] H WITH(NOLOCK)
LEFT OUTER JOIN Site24x7_T_Per_Report_Server_Names S WITH(NOLOCK) ON H.RowId=S.RowId
left outer join [Site24x7_T_Per_Report_Server_Metrics] M WITH(NOLOCK) ON M.RowId=S.RowId AND S.RowIndex=M.RowIndex


select CAST(ROUND(min(NULLIF(CPUUSEDPERCENT,0)),2)AS float) AS Average 
from VW_Site24x7_ServerPerformance 
where zaaid=60027079011
and ServerName='HAMAZLAB01.HamiltonIndia.in'
and param_metric_aggregation in (4,5)

select*
from VW_Site24x7_ServerPerformance 
where zaaid=60027079011
and ServerName='HAMAZLAB01.HamiltonIndia.in'
and param_metric_aggregation in (4,5)


EXEC [usp_Site24x7_R_ServerPerformanceReport_sam] @departmentid =27000109788, @zaaid =null, @start_date ='01/02/2025', @end_date ='28/02/2025'
EXEC [usp_Site24x7_R_ServerPerformanceReport] @departmentid =27000109788, @zaaid =null, @start_date ='01/02/2025', @end_date ='01/02/2025'

"CPUUSEDPERCENT": 8.33--
WCAZADC2.RKGROUP.LOCAL


select *
from VW_Site24x7_ServerPerformance 
where zaaid=60027079011
and ServerName='WCAZADC2.RKGROUP.LOCAL'
and param_metric_aggregation=0

order by rowid asc
SELECT * FROM FreshService_M_Department WHERE NAME LIKE '%wonder%'

SELECT * FROM Site24x7_T_Per_Report_Server_Names WHERE ServerName LIKE '%NewGen-Data.RKGROUP.LOCAL%'--281	-- 1445 
SELECT * FROM [dbo].[Site24x7_T_Per_Report_Server_Hdr] WHERE RowId = 281

SELECT * FROM M_CustomerMapping WHERE departmentid_freshservice = 27000109788

SELECT * FROM [Site24x7_T_Per_Report_Server_Hdr_Monthly]
SELECT * FROM [Site24x7_T_Per_Report_Server_Names_Monthly]
SELECT * FROM [Site24x7_T_Per_Report_Server_Availability_Monthly]
SELECT * FROM [Site24x7_T_Per_Report_Server_Metrics_Monthly]

SELECT * FROM Site24x7_M_MSP_Customer



SELECT COUNT(T.id),COUNT(TC.ticket_id) FROM FreshService_T_Tickets T
LEFT JOIN FreshService_T_Ticket_CustomFields TC ON T.id = TC.ticket_id
	WHERE CAST(SWITCHOFFSET(CAST(T.created_at AS DATETIMEOFFSET), '+05:30') AS DATE) 
      BETWEEN CONVERT(DATE, '07/04/2025', 103) AND CONVERT(DATE, '07/04/2025', 103) 

	  --2025-03-06
	  --07/03/2025

select DISTINCT(Tag) from FreshService_T_Ticket_Tags