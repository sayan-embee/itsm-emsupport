SELECT *
FROM [dbo].[CP_T_OTPLog]
WHERE Active = 1
ORDER BY Id DESC

SELECT *
FROM [dbo].[CP_T_EmailLog]
ORDER BY Id DESC

SELECT GETUTCDATE()

SELECT TOP 25 *
FROM CP_T_SignInLog
--WHERE UserId NOT IN ('20001','20002','20003','20004','20005','20006','20007')
ORDER BY LogId DESC

--TRUNCATE TABLE [dbo].[CP_T_OTPLog]
--TRUNCATE TABLE [dbo].[CP_T_EmailLog]
--TRUNCATE TABLE [dbo].[CP_T_SignInLog]



SELECT TOP 1 * FROM [dbo].[CP_T_WebChat_Log] ORDER BY AutoId DESC


--delete FROM [dbo].[FreshService_M_Requesters]  where id = 20006 and primary_email = 'bipul.patra@embee.co.in' 

SELECT * FROM [dbo].[FreshService_M_Requesters] where id = 20006 and primary_email = 'bipul.patra@embee.co.in' 

SELECT * FROM [dbo].[FreshService_M_Requesters]  where primary_email in ('bipul.patra@embee.co.in', 'samuel.pradhan@embee.co.in', 'Dipankar.Das@embee.co.in')
SELECT * FROM [dbo].[FreshService_M_Requesters] WHERE id = 20006

SELECT * FROM [dbo].[FreshService_M_Requesters] where primary_email = 'samuel.pradhan@embee.co.in'

--UPDATE [dbo].[FreshService_M_Requesters] SET id = 20003 where primary_email = 'soumik.hazra@embee.co.in'

--INSERT INTO [dbo].[FreshService_M_Requester_Departments] (requester_id, department_id) VALUES (27000352098, 27000180437)

SELECT * FROM [dbo].[FreshService_M_Requester_Departments] where requester_id = 27000352098

--UPDATE [dbo].[FreshService_M_Requester_Departments] SET requester_id=20002, department_id=27000180437 WHERE requester_id = 999999999

SELECT department_id, COUNT(requester_id) FROM [dbo].[FreshService_M_Requester_Departments] GROUP BY department_id

SELECT * FROM [dbo].[FreshService_M_Requester_Departments] WHERE department_id = 27000112738

SELECT * FROM [dbo].[FreshService_M_Department] D
WHERE id = 27000180437
INNER JOIN [dbo].[FreshService_M_Department_CustomFields] C ON c.department_id = D.id
WHERE
embee_crm_id IS NOT NULL

SELECT * FROM [dbo].[FreshService_M_Department_CustomFields] 
WHERE 
department_id = 27000180437
embee_crm_id IS NOT NULL


SELECT * FROM [dbo].[FreshService_T_Tickets] WHERE id = 745715
WHERE department_id = 27002100003





--update [CP_T_WebChat_Log] set active = 0 where AutoId = 45

SELECT * FROM [dbo].[CP_T_WebChat_UserConversationLog] ORDER BY MessageId DESC
SELECT * FROM [dbo].[CP_T_WebChat_UserConversationFilesLog]

SELECT TOP 1 * FROM [dbo].[TeamsBot_T_UserSearch] Order by MessageId DESC
SELECT * FROM [dbo].[TeamsBot_T_UserSearchFiles]

--INSERT INTO [dbo].[FreshService_M_Requesters]
--           ([id]
--           ,[active]
--           ,[address]
--           ,[first_name]
--           ,[has_logged_in]
--           ,[job_title]
--           ,[language]
--           ,[last_name]
--           ,[mobile_phone_number]
--           ,[primary_email]
--           ,[time_format]
--           ,[time_zone]
--           ,[created_at]
--           ,[created_on])
--     VALUES
--           (20007
--           ,1
--           ,'Mumbai, India'
--           ,'Ashish'
--           ,1
--           ,'Sr. Project Manager'
--           ,'en'
--           ,'Kothari'
--           ,'9051299330'
--           ,'Ashish.Kothari@embee.co.in'
--           ,'12h'
--           ,'Kolkata'
--           ,GETUTCDATE()
--           ,GETUTCDATE())




SELECT * FROM [dbo].[T_BotInstallUninstal_Log]
SELECT * FROM [dbo].[M_Report_UserAccess]




