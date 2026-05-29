--USE [master]
--GO
--/****** Object:  Database [AI-Portal-Apps]    Script Date: 09/06/2025 10:37:51 ******/
--CREATE DATABASE [AI-Portal-Apps]
-- CONTAINMENT = NONE
-- ON  PRIMARY 
--( NAME = N'AI-Portal-Apps', FILENAME = N'D:\Program Files\Microsoft SQL Server\MSSQL15.MSSQLSERVER\MSSQL\DATA\AI-Portal-Apps.mdf' , SIZE = 401408KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )
-- LOG ON 
--( NAME = N'AI-Portal-Apps_log', FILENAME = N'D:\Program Files\Microsoft SQL Server\MSSQL15.MSSQLSERVER\MSSQL\DATA\AI-Portal-Apps_log.ldf' , SIZE = 2891776KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )
-- WITH CATALOG_COLLATION = DATABASE_DEFAULT
--GO
--ALTER DATABASE [AI-Portal-Apps] SET COMPATIBILITY_LEVEL = 150
--GO
--IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
--begin
--EXEC [AI-Portal-Apps].[dbo].[sp_fulltext_database] @action = 'enable'
--end
--GO
--ALTER DATABASE [AI-Portal-Apps] SET ANSI_NULL_DEFAULT OFF 
--GO
--ALTER DATABASE [AI-Portal-Apps] SET ANSI_NULLS OFF 
--GO
--ALTER DATABASE [AI-Portal-Apps] SET ANSI_PADDING OFF 
--GO
--ALTER DATABASE [AI-Portal-Apps] SET ANSI_WARNINGS OFF 
--GO
--ALTER DATABASE [AI-Portal-Apps] SET ARITHABORT OFF 
--GO
--ALTER DATABASE [AI-Portal-Apps] SET AUTO_CLOSE OFF 
--GO
--ALTER DATABASE [AI-Portal-Apps] SET AUTO_SHRINK OFF 
--GO
--ALTER DATABASE [AI-Portal-Apps] SET AUTO_UPDATE_STATISTICS ON 
--GO
--ALTER DATABASE [AI-Portal-Apps] SET CURSOR_CLOSE_ON_COMMIT OFF 
--GO
--ALTER DATABASE [AI-Portal-Apps] SET CURSOR_DEFAULT  GLOBAL 
--GO
--ALTER DATABASE [AI-Portal-Apps] SET CONCAT_NULL_YIELDS_NULL OFF 
--GO
--ALTER DATABASE [AI-Portal-Apps] SET NUMERIC_ROUNDABORT OFF 
--GO
--ALTER DATABASE [AI-Portal-Apps] SET QUOTED_IDENTIFIER OFF 
--GO
--ALTER DATABASE [AI-Portal-Apps] SET RECURSIVE_TRIGGERS OFF 
--GO
--ALTER DATABASE [AI-Portal-Apps] SET  DISABLE_BROKER 
--GO
--ALTER DATABASE [AI-Portal-Apps] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
--GO
--ALTER DATABASE [AI-Portal-Apps] SET DATE_CORRELATION_OPTIMIZATION OFF 
--GO
--ALTER DATABASE [AI-Portal-Apps] SET TRUSTWORTHY OFF 
--GO
--ALTER DATABASE [AI-Portal-Apps] SET ALLOW_SNAPSHOT_ISOLATION OFF 
--GO
--ALTER DATABASE [AI-Portal-Apps] SET PARAMETERIZATION SIMPLE 
--GO
--ALTER DATABASE [AI-Portal-Apps] SET READ_COMMITTED_SNAPSHOT OFF 
--GO
--ALTER DATABASE [AI-Portal-Apps] SET HONOR_BROKER_PRIORITY OFF 
--GO
--ALTER DATABASE [AI-Portal-Apps] SET RECOVERY FULL 
--GO
--ALTER DATABASE [AI-Portal-Apps] SET  MULTI_USER 
--GO
--ALTER DATABASE [AI-Portal-Apps] SET PAGE_VERIFY CHECKSUM  
--GO
--ALTER DATABASE [AI-Portal-Apps] SET DB_CHAINING OFF 
--GO
--ALTER DATABASE [AI-Portal-Apps] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
--GO
--ALTER DATABASE [AI-Portal-Apps] SET TARGET_RECOVERY_TIME = 60 SECONDS 
--GO
--ALTER DATABASE [AI-Portal-Apps] SET DELAYED_DURABILITY = DISABLED 
--GO
--ALTER DATABASE [AI-Portal-Apps] SET ACCELERATED_DATABASE_RECOVERY = OFF  
--GO
--EXEC sys.sp_db_vardecimal_storage_format N'AI-Portal-Apps', N'ON'
--GO
--ALTER DATABASE [AI-Portal-Apps] SET QUERY_STORE = OFF
--GO
USE [AI-Portal-Apps]
GO
--/****** Object:  User [AIPortalAppUser]    Script Date: 09/06/2025 10:37:51 ******/
--CREATE USER [AIPortalAppUser] FOR LOGIN [AIPortalAppUser] WITH DEFAULT_SCHEMA=[dbo]
--GO
--ALTER ROLE [db_owner] ADD MEMBER [AIPortalAppUser]
--GO
/****** Object:  Table [dbo].[Site24x7_T_Per_Report_Server_Hdr]    Script Date: 09/06/2025 10:37:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Site24x7_T_Per_Report_Server_Hdr](
	[RowId] [bigint] IDENTITY(1,1) NOT NULL,
	[zaaid] [varchar](100) NOT NULL,
	[param_period] [int] NULL,
	[param_metric_aggregation] [int] NULL,
	[param_start_date] [varchar](50) NULL,
	[param_end_date] [varchar](50) NULL,
	[period] [int] NULL,
	[resource_type_name] [varchar](50) NULL,
	[resource_type] [int] NULL,
	[end_time] [varchar](50) NULL,
	[period_name] [varchar](50) NULL,
	[report_type] [int] NULL,
	[start_time] [varchar](50) NULL,
	[metric_aggregation] [int] NULL,
	[resource_name] [varchar](50) NULL,
	[report_name] [varchar](50) NULL,
	[monitor_type] [varchar](50) NULL,
	[CreatedOn] [datetime] NULL,
	[UpdatedOn] [datetime] NULL,
	[dtStartDate] [datetime] NULL,
	[dtEndDate] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[RowId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Site24x7_T_Per_Report_Server_Metrics]    Script Date: 09/06/2025 10:37:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Site24x7_T_Per_Report_Server_Metrics](
	[DetailId] [bigint] IDENTITY(1,1) NOT NULL,
	[RowId] [bigint] NULL,
	[RowIndex] [int] NULL,
	[DISKUSEDPERCENT] [numeric](10, 2) NULL,
	[MEMUSEDPERCENT] [numeric](10, 2) NULL,
	[CPUUSEDPERCENT] [numeric](10, 2) NULL,
	[param_metric_aggregation] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[DetailId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Site24x7_T_Per_Report_Server_Names]    Script Date: 09/06/2025 10:37:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Site24x7_T_Per_Report_Server_Names](
	[DetailId] [bigint] IDENTITY(1,1) NOT NULL,
	[RowId] [bigint] NULL,
	[RowIndex] [int] NULL,
	[ServerName] [varchar](200) NULL,
PRIMARY KEY CLUSTERED 
(
	[DetailId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  View [dbo].[VW_Site24x7_ServerPerformance]    Script Date: 09/06/2025 10:37:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create  VIEW [dbo].[VW_Site24x7_ServerPerformance] AS
SELECT H.zaaid, H.RowId,h.dtStartDate,h.dtEndDate, S.ServerName,M.DISKUSEDPERCENT,M.MEMUSEDPERCENT,M.CPUUSEDPERCENT,M.param_metric_aggregation 
FROM [Site24x7_T_Per_Report_Server_Hdr] H WITH(NOLOCK)
LEFT OUTER JOIN Site24x7_T_Per_Report_Server_Names S WITH(NOLOCK) ON H.RowId=S.RowId
left outer join [Site24x7_T_Per_Report_Server_Metrics] M WITH(NOLOCK) ON M.RowId=S.RowId AND S.RowIndex=M.RowIndex
GO
/****** Object:  Table [dbo].[Site24x7_T_Per_Report_Server_Hdr_Monthly]    Script Date: 09/06/2025 10:37:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Site24x7_T_Per_Report_Server_Hdr_Monthly](
	[RowId] [bigint] IDENTITY(1,1) NOT NULL,
	[zaaid] [varchar](100) NOT NULL,
	[param_period] [int] NULL,
	[param_metric_aggregation] [int] NULL,
	[param_start_date] [varchar](50) NULL,
	[param_end_date] [varchar](50) NULL,
	[period] [int] NULL,
	[resource_type_name] [varchar](50) NULL,
	[resource_type] [int] NULL,
	[end_time] [varchar](50) NULL,
	[period_name] [varchar](50) NULL,
	[report_type] [int] NULL,
	[start_time] [varchar](50) NULL,
	[metric_aggregation] [int] NULL,
	[resource_name] [varchar](50) NULL,
	[report_name] [varchar](50) NULL,
	[monitor_type] [varchar](50) NULL,
	[CreatedOn] [datetime] NULL,
	[UpdatedOn] [datetime] NULL,
	[dtStartDate] [datetime] NULL,
	[dtEndDate] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[RowId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Site24x7_T_Per_Report_Server_Names_Monthly]    Script Date: 09/06/2025 10:37:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Site24x7_T_Per_Report_Server_Names_Monthly](
	[DetailId] [bigint] IDENTITY(1,1) NOT NULL,
	[RowId] [bigint] NULL,
	[RowIndex] [int] NULL,
	[ServerName] [varchar](200) NULL,
PRIMARY KEY CLUSTERED 
(
	[DetailId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Site24x7_T_Per_Report_Server_Metrics_Monthly]    Script Date: 09/06/2025 10:37:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Site24x7_T_Per_Report_Server_Metrics_Monthly](
	[DetailId] [bigint] IDENTITY(1,1) NOT NULL,
	[RowId] [bigint] NULL,
	[RowIndex] [int] NULL,
	[DISKUSEDPERCENT] [numeric](10, 2) NULL,
	[MEMUSEDPERCENT] [numeric](10, 2) NULL,
	[CPUUSEDPERCENT] [numeric](10, 2) NULL,
	[param_metric_aggregation] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[DetailId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  View [dbo].[VW_Site24x7_ServerPerformance_Monthly]    Script Date: 09/06/2025 10:37:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE  VIEW [dbo].[VW_Site24x7_ServerPerformance_Monthly] AS
SELECT H.zaaid, H.RowId,h.dtStartDate,h.dtEndDate, S.ServerName,M.DISKUSEDPERCENT,M.MEMUSEDPERCENT,M.CPUUSEDPERCENT,M.param_metric_aggregation 
FROM [Site24x7_T_Per_Report_Server_Hdr_Monthly] H WITH(NOLOCK)
LEFT OUTER JOIN Site24x7_T_Per_Report_Server_Names_Monthly S WITH(NOLOCK) ON H.RowId=S.RowId
LEFT outer join [Site24x7_T_Per_Report_Server_Metrics_Monthly] M WITH(NOLOCK) ON M.RowId=S.RowId AND S.RowIndex=M.RowIndex
GO
/****** Object:  Table [dbo].[CP_M_Category]    Script Date: 09/06/2025 10:37:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CP_M_Category](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Code] [varchar](50) NULL,
	[Name] [varchar](100) NOT NULL,
	[Active] [bit] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[CP_M_Region]    Script Date: 09/06/2025 10:37:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CP_M_Region](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[RegionCode] [varchar](10) NULL,
	[RegionName] [varchar](50) NOT NULL,
	[Active] [bit] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[CP_M_SubCategory]    Script Date: 09/06/2025 10:37:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CP_M_SubCategory](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[CategoryId] [int] NOT NULL,
	[Code] [varchar](50) NULL,
	[Name] [varchar](100) NOT NULL,
	[Active] [bit] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[CP_M_Tenant]    Script Date: 09/06/2025 10:37:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CP_M_Tenant](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[TenantCode] [varchar](50) NULL,
	[TenantName] [varchar](255) NOT NULL,
	[Active] [bit] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[CP_M_WebChat_Feedback]    Script Date: 09/06/2025 10:37:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CP_M_WebChat_Feedback](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](50) NOT NULL,
	[Active] [bit] NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[CP_M_Webchat_Options]    Script Date: 09/06/2025 10:37:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CP_M_Webchat_Options](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[CategoryId] [int] NOT NULL,
	[SubCategoryId] [int] NOT NULL,
	[Option] [varchar](255) NULL,
	[Active] [bit] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[CP_M_WebChat_SOPIndex]    Script Date: 09/06/2025 10:37:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CP_M_WebChat_SOPIndex](
	[AutoId] [int] IDENTITY(1,1) NOT NULL,
	[CategoryId] [int] NULL,
	[SubCategoryId] [int] NULL,
	[IndexName] [varchar](255) NULL,
	[Active] [bit] NULL,
PRIMARY KEY CLUSTERED 
(
	[AutoId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[CP_T_ContractMaster]    Script Date: 09/06/2025 10:37:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CP_T_ContractMaster](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[ContractNo] [varchar](255) NULL,
	[TenantId] [int] NULL,
	[TenantName] [varchar](255) NULL,
	[CustomerId] [varchar](255) NULL,
	[CustomerName] [varchar](255) NULL,
	[DepartmentId] [bigint] NULL,
	[DepartmentName] [varchar](255) NULL,
	[StartDate] [datetime] NULL,
	[EndDate] [datetime] NULL,
	[Acc_ManagerName] [varchar](255) NULL,
	[Acc_ManagerEmail] [varchar](255) NULL,
	[PONo] [varchar](255) NULL,
	[CategoryId] [int] NULL,
	[SubCategoryId] [int] NULL,
	[RegionId] [int] NULL,
	[Active] [bit] NULL,
	[CreatedByName] [varchar](150) NULL,
	[CreatedByEmail] [varchar](150) NULL,
	[CreatedOnUTC] [datetime] NULL,
	[ModifiedByName] [varchar](150) NULL,
	[ModifiedEmail] [varchar](150) NULL,
	[ModifiedOnUTC] [datetime] NULL,
	[StartMonth] [varchar](25) NULL,
	[StartYear] [int] NULL,
	[EndMonth] [varchar](25) NULL,
	[EndYear] [int] NULL,
	[ReferenceNo] [uniqueidentifier] NULL,
	[ExtendSupport] [bit] NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[CP_T_ContractMaster_BAK_03042025]    Script Date: 09/06/2025 10:37:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CP_T_ContractMaster_BAK_03042025](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[ContractNo] [varchar](255) NULL,
	[TenantId] [int] NULL,
	[TenantName] [varchar](255) NULL,
	[CustomerId] [varchar](255) NULL,
	[CustomerName] [varchar](255) NULL,
	[DepartmentId] [bigint] NULL,
	[DepartmentName] [varchar](255) NULL,
	[StartDate] [datetime] NULL,
	[EndDate] [datetime] NULL,
	[Acc_ManagerName] [varchar](255) NULL,
	[Acc_ManagerEmail] [varchar](255) NULL,
	[PONo] [varchar](255) NULL,
	[CategoryId] [int] NULL,
	[SubCategoryId] [int] NULL,
	[RegionId] [int] NULL,
	[Active] [bit] NULL,
	[CreatedByName] [varchar](150) NULL,
	[CreatedByEmail] [varchar](150) NULL,
	[CreatedOnUTC] [datetime] NULL,
	[ModifiedByName] [varchar](150) NULL,
	[ModifiedEmail] [varchar](150) NULL,
	[ModifiedOnUTC] [datetime] NULL,
	[StartMonth] [varchar](25) NULL,
	[StartYear] [int] NULL,
	[EndMonth] [varchar](25) NULL,
	[EndYear] [int] NULL,
	[ReferenceNo] [uniqueidentifier] NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[CP_T_ContractMaster_Temp2]    Script Date: 09/06/2025 10:37:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CP_T_ContractMaster_Temp2](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[ContractNo] [varchar](255) NULL,
	[TenantId] [int] NULL,
	[TenantName] [varchar](255) NULL,
	[CustomerId] [varchar](255) NULL,
	[CustomerName] [varchar](255) NULL,
	[DepartmentId] [bigint] NULL,
	[DepartmentName] [varchar](255) NULL,
	[StartDate] [datetime] NULL,
	[EndDate] [datetime] NULL,
	[Acc_ManagerName] [varchar](255) NULL,
	[Acc_ManagerEmail] [varchar](255) NULL,
	[PONo] [varchar](255) NULL,
	[CategoryId] [int] NULL,
	[SubCategoryId] [int] NULL,
	[RegionId] [int] NULL,
	[Active] [bit] NULL,
	[CreatedByName] [varchar](150) NULL,
	[CreatedByEmail] [varchar](150) NULL,
	[CreatedOnUTC] [datetime] NULL,
	[ModifiedByName] [varchar](150) NULL,
	[ModifiedEmail] [varchar](150) NULL,
	[ModifiedOnUTC] [datetime] NULL,
	[StartMonth] [varchar](25) NULL,
	[StartYear] [int] NULL,
	[EndMonth] [varchar](25) NULL,
	[EndYear] [int] NULL,
	[ReferenceNo] [uniqueidentifier] NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[CP_T_ContractMasterFiles]    Script Date: 09/06/2025 10:37:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CP_T_ContractMasterFiles](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[ContractId] [bigint] NULL,
	[Name] [nvarchar](255) NULL,
	[InternalName] [nvarchar](255) NULL,
	[ContentType] [varchar](100) NULL,
	[Url] [nvarchar](500) NULL,
	[PhysicalPath] [nvarchar](500) NULL,
	[Active] [bit] NULL,
	[CreatedByName] [varchar](150) NULL,
	[CreatedByEmail] [varchar](150) NULL,
	[CreatedOnUTC] [datetime] NULL,
	[ModifiedByName] [varchar](150) NULL,
	[ModifiedEmail] [varchar](150) NULL,
	[ModifiedOnUTC] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[CP_T_EmailLog]    Script Date: 09/06/2025 10:37:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CP_T_EmailLog](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[From] [varchar](255) NULL,
	[To] [varchar](255) NULL,
	[CC] [varchar](255) NULL,
	[Subject] [nvarchar](255) NULL,
	[Body] [nvarchar](max) NULL,
	[Status] [bit] NULL,
	[Type] [varchar](50) NULL,
	[Message] [varchar](255) NULL,
	[CreatedOn] [datetime] NULL,
	[ReferenceNo] [varchar](255) NULL,
	[OTP_Id] [int] NULL,
	[SessionId] [varchar](255) NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[CP_T_OTPLog]    Script Date: 09/06/2025 10:37:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CP_T_OTPLog](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Code] [varchar](50) NULL,
	[ValidityInSec] [int] NULL,
	[CreatedOn] [datetime] NULL,
	[ExpiredOn] [datetime] NULL,
	[VerifiedOn] [datetime] NULL,
	[InvalidCount] [int] NULL,
	[ResendCount] [int] NULL,
	[Verified] [bit] NULL,
	[Active] [bit] NULL,
	[Recipient] [varchar](255) NULL,
	[ReferenceNo] [varchar](255) NULL,
	[SessionId] [varchar](255) NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[CP_T_SignInLog]    Script Date: 09/06/2025 10:37:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CP_T_SignInLog](
	[LogId] [bigint] IDENTITY(1,1) NOT NULL,
	[UserId] [varchar](100) NULL,
	[UserName] [varchar](255) NULL,
	[UserEmail] [varchar](255) NULL,
	[SigninTimeUTC] [datetime] NULL,
	[SignoutTimeUTC] [datetime] NULL,
	[ClientIP] [varchar](45) NULL,
	[UserAgent] [varchar](500) NULL,
	[DeviceType] [varchar](50) NULL,
	[Location] [varchar](255) NULL,
	[JWTTokenId] [varchar](1000) NULL,
	[JWTTokenExpiredOn] [datetime] NULL,
	[SessionId] [varchar](255) NULL,
	[IsSessionActive] [bit] NULL,
	[SignOutRemarks] [varchar](255) NULL,
	[OTPId] [bigint] NULL,
	[FailedLoginAttempts] [int] NULL,
	[CreatedOn] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[LogId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[CP_T_WebChat_Log]    Script Date: 09/06/2025 10:37:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CP_T_WebChat_Log](
	[AutoId] [bigint] IDENTITY(1,1) NOT NULL,
	[UserId] [varchar](100) NULL,
	[UserName] [varchar](255) NULL,
	[UserEmail] [varchar](255) NULL,
	[LogId] [bigint] NULL,
	[SessionId] [varchar](255) NULL,
	[DirectLineToken] [varchar](2500) NULL,
	[ConversationId] [varchar](1000) NULL,
	[StreamUrl] [varchar](2500) NULL,
	[ExpiredOn] [datetime] NULL,
	[CreatedOn] [datetime] NULL,
	[StartedOn] [datetime] NULL,
	[EndedOn] [datetime] NULL,
	[Active] [bit] NULL,
	[FeedbackRatingId] [int] NULL,
	[AdditionalFeedback] [varchar](500) NULL,
	[SatisfiedWithResolution] [bit] NULL,
	[ConversationType] [varchar](255) NULL,
	[SessionCloseRemarks] [varchar](255) NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[CP_T_WebChat_UserConversationFilesLog]    Script Date: 09/06/2025 10:37:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CP_T_WebChat_UserConversationFilesLog](
	[FileId] [bigint] IDENTITY(1,1) NOT NULL,
	[MessageId] [int] NULL,
	[FileName] [nvarchar](max) NULL,
	[FileURL] [nvarchar](max) NULL,
	[FileContent] [nvarchar](max) NULL,
PRIMARY KEY CLUSTERED 
(
	[FileId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[CP_T_WebChat_UserConversationLog]    Script Date: 09/06/2025 10:37:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CP_T_WebChat_UserConversationLog](
	[MessageId] [bigint] IDENTITY(1,1) NOT NULL,
	[WebChatLogId] [bigint] NULL,
	[UserName] [nvarchar](100) NULL,
	[UserEmail] [nvarchar](50) NULL,
	[UserUPN] [nvarchar](50) NULL,
	[UserADID] [varchar](50) NULL,
	[ChannelId] [nvarchar](50) NULL,
	[ConversationType] [nvarchar](50) NULL,
	[ConversationId] [nvarchar](500) NULL,
	[TenantId] [varchar](50) NULL,
	[ChatId] [nvarchar](50) NULL,
	[LocalTimestamp] [datetimeoffset](7) NULL,
	[Locale] [nvarchar](50) NULL,
	[ServiceUrl] [nvarchar](50) NULL,
	[Text] [nvarchar](max) NULL,
	[TextFormat] [nvarchar](50) NULL,
	[Timestamp] [datetimeoffset](7) NULL,
	[Response] [nvarchar](max) NULL,
	[Intent] [nvarchar](max) NULL,
	[CreatedOnIST] [datetime] NULL,
	[CreatedOnUTC] [datetime] NULL,
	[MessageActivityId] [nvarchar](100) NULL,
	[MessageSentUTC] [datetime] NULL,
	[FeedbackCardActivityId] [nvarchar](100) NULL,
	[FeedbackCardSentUTC] [datetime] NULL,
	[LikeDislike] [bit] NULL,
	[FeedbackReceivedUTC] [datetime] NULL,
	[CategoryId] [int] NULL,
	[SubCategoryId] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[MessageId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[FreshService_M_Department]    Script Date: 09/06/2025 10:37:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[FreshService_M_Department](
	[id] [bigint] NOT NULL,
	[name] [varchar](255) NOT NULL,
	[description] [varchar](max) NULL,
	[head_user_id] [bigint] NULL,
	[head_name] [varchar](255) NULL,
	[prime_user_id] [bigint] NULL,
	[prime_user_name] [varchar](255) NULL,
	[created_at] [datetime] NULL,
	[updated_at] [datetime] NULL,
	[created_on] [datetime] NULL,
	[updated_on] [datetime] NULL,
 CONSTRAINT [PK__FreshSer__3213E83FBB78C7B5] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[FreshService_M_Department_CustomFields]    Script Date: 09/06/2025 10:37:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[FreshService_M_Department_CustomFields](
	[department_id] [bigint] NOT NULL,
	[location] [varchar](255) NULL,
	[tenant] [varchar](255) NULL,
	[embee_crm_id] [varchar](255) NULL,
	[contact_person] [varchar](255) NULL,
	[contact_number] [varchar](255) NULL,
	[contact_email_id] [varchar](255) NULL,
	[embee_account_manager] [varchar](255) NULL,
	[engagement_start_date] [datetime] NULL,
	[engagement_end_date] [datetime] NULL,
	[customer_portal_access] [varchar](255) NULL,
	[sap_customer_name] [varchar](100) NULL,
PRIMARY KEY CLUSTERED 
(
	[department_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[FreshService_M_Priority]    Script Date: 09/06/2025 10:37:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[FreshService_M_Priority](
	[Id] [int] NOT NULL,
	[Name] [varchar](100) NOT NULL,
	[DisplayName] [varchar](100) NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[FreshService_M_Requester_Departments]    Script Date: 09/06/2025 10:37:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[FreshService_M_Requester_Departments](
	[requester_id] [bigint] NOT NULL,
	[department_id] [bigint] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[requester_id] ASC,
	[department_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[FreshService_M_Requesters]    Script Date: 09/06/2025 10:37:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[FreshService_M_Requesters](
	[id] [bigint] NOT NULL,
	[active] [bit] NULL,
	[address] [varchar](1000) NULL,
	[background_information] [text] NULL,
	[can_see_all_changes_from_associated_departments] [bit] NULL,
	[can_see_all_tickets_from_associated_departments] [bit] NULL,
	[external_id] [varchar](255) NULL,
	[first_name] [varchar](255) NULL,
	[has_logged_in] [bit] NULL,
	[is_agent] [bit] NULL,
	[job_title] [varchar](255) NULL,
	[language] [varchar](50) NULL,
	[last_name] [varchar](255) NULL,
	[location_id] [bigint] NULL,
	[location_name] [varchar](255) NULL,
	[mobile_phone_number] [varchar](20) NULL,
	[primary_email] [varchar](255) NULL,
	[reporting_manager_id] [varchar](255) NULL,
	[time_format] [varchar](50) NULL,
	[time_zone] [varchar](50) NULL,
	[vip_user] [bit] NULL,
	[work_phone_number] [varchar](20) NULL,
	[work_schedule_id] [bigint] NULL,
	[employee_id] [varchar](100) NULL,
	[created_at] [datetime] NULL,
	[updated_at] [datetime] NULL,
	[created_on] [datetime] NULL,
	[updated_on] [datetime] NULL,
 CONSTRAINT [PK__FreshSer__3213E83FF1696B9B] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[FreshService_M_Source]    Script Date: 09/06/2025 10:37:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[FreshService_M_Source](
	[SourceId] [int] NOT NULL,
	[SourceName] [varchar](100) NOT NULL,
	[SourceDisplayName] [varchar](100) NULL,
 CONSTRAINT [PK__FreshSer__16E019196ED9385C] PRIMARY KEY CLUSTERED 
(
	[SourceId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[FreshService_M_Status]    Script Date: 09/06/2025 10:37:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[FreshService_M_Status](
	[StatusId] [int] NOT NULL,
	[StatusName] [varchar](100) NOT NULL,
	[StatusDisplayName] [varchar](100) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[StatusId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[FreshService_T_Change]    Script Date: 09/06/2025 10:37:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[FreshService_T_Change](
	[id] [bigint] NOT NULL,
	[agent_id] [bigint] NULL,
	[group_id] [bigint] NULL,
	[priority] [int] NULL,
	[impact] [int] NULL,
	[status] [int] NULL,
	[risk] [int] NULL,
	[change_type] [int] NULL,
	[planned_start_date] [datetime] NULL,
	[planned_end_date] [datetime] NULL,
	[subject] [varchar](255) NULL,
	[department_id] [bigint] NULL,
	[category] [varchar](255) NULL,
	[sub_category] [varchar](255) NULL,
	[item_category] [varchar](255) NULL,
	[description] [text] NULL,
	[planned_effort] [varchar](255) NULL,
	[description_text] [text] NULL,
	[requester_id] [bigint] NULL,
	[approval_status] [int] NULL,
	[change_window_id] [varchar](255) NULL,
	[workspace_id] [int] NULL,
	[tasks_dependency_type] [int] NULL,
	[created_at] [datetime] NULL,
	[updated_at] [datetime] NULL,
	[created_on] [datetime] NULL,
	[updated_on] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[FreshService_T_Change_BKP_17012025]    Script Date: 09/06/2025 10:37:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[FreshService_T_Change_BKP_17012025](
	[id] [bigint] NOT NULL,
	[agent_id] [bigint] NULL,
	[group_id] [bigint] NULL,
	[priority] [int] NULL,
	[impact] [int] NULL,
	[status] [int] NULL,
	[risk] [int] NULL,
	[change_type] [int] NULL,
	[planned_start_date] [datetime] NULL,
	[planned_end_date] [datetime] NULL,
	[subject] [varchar](255) NULL,
	[department_id] [bigint] NULL,
	[category] [varchar](255) NULL,
	[sub_category] [varchar](255) NULL,
	[item_category] [varchar](255) NULL,
	[description] [text] NULL,
	[planned_effort] [varchar](255) NULL,
	[description_text] [text] NULL,
	[requester_id] [bigint] NULL,
	[approval_status] [int] NULL,
	[change_window_id] [varchar](255) NULL,
	[workspace_id] [int] NULL,
	[tasks_dependency_type] [int] NULL,
	[created_at] [datetime] NULL,
	[updated_at] [datetime] NULL,
	[created_on] [datetime] NULL,
	[updated_on] [datetime] NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[FreshService_T_Change_CustomFields]    Script Date: 09/06/2025 10:37:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[FreshService_T_Change_CustomFields](
	[change_id] [int] NOT NULL,
	[tenant] [varchar](255) NULL,
	[elevated_call] [bit] NULL,
	[on_roaster_engineer] [varchar](255) NULL,
	[nsd_member_name] [varchar](255) NULL,
	[resolution_remarks] [varchar](max) NULL,
PRIMARY KEY CLUSTERED 
(
	[change_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[FreshService_T_Change_CustomFields_BKP_17012025]    Script Date: 09/06/2025 10:37:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[FreshService_T_Change_CustomFields_BKP_17012025](
	[change_id] [int] NOT NULL,
	[tenant] [varchar](255) NULL,
	[elevated_call] [bit] NULL,
	[on_roaster_engineer] [varchar](255) NULL,
	[nsd_member_name] [varchar](255) NULL,
	[resolution_remarks] [varchar](max) NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[FreshService_T_Problems]    Script Date: 09/06/2025 10:37:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[FreshService_T_Problems](
	[id] [bigint] NOT NULL,
	[agent_id] [bigint] NULL,
	[group_id] [bigint] NULL,
	[priority] [int] NULL,
	[impact] [int] NULL,
	[status] [int] NULL,
	[due_by] [datetime] NULL,
	[known_error] [bit] NULL,
	[planned_start_date] [datetime] NULL,
	[planned_end_date] [datetime] NULL,
	[subject] [varchar](255) NULL,
	[department_id] [bigint] NULL,
	[category] [varchar](255) NULL,
	[sub_category] [varchar](255) NULL,
	[item_category] [varchar](255) NULL,
	[description] [text] NULL,
	[planned_effort] [varchar](255) NULL,
	[description_text] [text] NULL,
	[requester_id] [bigint] NULL,
	[workspace_id] [int] NULL,
	[tasks_dependency_type] [int] NULL,
	[custom_fields_nsd_member_name] [varchar](255) NULL,
	[custom_fields_on_roaster_engineer] [varchar](255) NULL,
	[created_at] [datetime] NULL,
	[updated_at] [datetime] NULL,
	[created_on] [datetime] NULL,
	[updated_on] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[FreshService_T_Problems_BKP_17012025]    Script Date: 09/06/2025 10:37:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[FreshService_T_Problems_BKP_17012025](
	[id] [bigint] NOT NULL,
	[agent_id] [bigint] NULL,
	[group_id] [bigint] NULL,
	[priority] [int] NULL,
	[impact] [int] NULL,
	[status] [int] NULL,
	[due_by] [datetime] NULL,
	[known_error] [bit] NULL,
	[planned_start_date] [datetime] NULL,
	[planned_end_date] [datetime] NULL,
	[subject] [varchar](255) NULL,
	[department_id] [bigint] NULL,
	[category] [varchar](255) NULL,
	[sub_category] [varchar](255) NULL,
	[item_category] [varchar](255) NULL,
	[description] [text] NULL,
	[planned_effort] [varchar](255) NULL,
	[description_text] [text] NULL,
	[requester_id] [bigint] NULL,
	[workspace_id] [int] NULL,
	[tasks_dependency_type] [int] NULL,
	[custom_fields_nsd_member_name] [varchar](255) NULL,
	[custom_fields_on_roaster_engineer] [varchar](255) NULL,
	[created_at] [datetime] NULL,
	[updated_at] [datetime] NULL,
	[created_on] [datetime] NULL,
	[updated_on] [datetime] NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[FreshService_T_Ticket_CustomFields]    Script Date: 09/06/2025 10:37:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[FreshService_T_Ticket_CustomFields](
	[ticket_id] [bigint] NOT NULL,
	[location] [varchar](255) NULL,
	[major_incident_type] [varchar](255) NULL,
	[nsd_member_name] [varchar](255) NULL,
	[oem_case_id_logged] [varchar](255) NULL,
	[on_roaster_engineer] [varchar](255) NULL,
	[resolution_type] [varchar](255) NULL,
	[support_type] [varchar](255) NULL,
	[tenant] [varchar](255) NULL,
	[ticket_mode] [varchar](255) NULL,
	[ticket_monitoring_owner] [varchar](255) NULL,
	[time_track_mandate] [varchar](255) NULL,
	[user_type] [varchar](255) NULL,
	[parent_ticket_id] [varchar](255) NULL,
	[resolution_remarks] [text] NULL,
	[resource_name] [varchar](255) NULL,
	[problem_statement] [text] NULL,
	[oem_case_idif_any] [varchar](255) NULL,
	[sales_account_manager] [varchar](255) NULL,
	[sl_no] [varchar](255) NULL,
	[pid] [varchar](255) NULL,
	[model] [varchar](255) NULL,
	[product] [varchar](255) NULL,
PRIMARY KEY CLUSTERED 
(
	[ticket_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[FreshService_T_Ticket_CustomFields_BKP_17012025]    Script Date: 09/06/2025 10:37:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[FreshService_T_Ticket_CustomFields_BKP_17012025](
	[ticket_id] [bigint] NOT NULL,
	[location] [varchar](255) NULL,
	[major_incident_type] [varchar](255) NULL,
	[nsd_member_name] [varchar](255) NULL,
	[oem_case_id_logged] [varchar](255) NULL,
	[on_roaster_engineer] [varchar](255) NULL,
	[resolution_type] [varchar](255) NULL,
	[support_type] [varchar](255) NULL,
	[tenant] [varchar](255) NULL,
	[ticket_mode] [varchar](255) NULL,
	[ticket_monitoring_owner] [varchar](255) NULL,
	[time_track_mandate] [varchar](255) NULL,
	[user_type] [varchar](255) NULL,
	[parent_ticket_id] [varchar](255) NULL,
	[resolution_remarks] [text] NULL,
	[resource_name] [varchar](255) NULL,
	[problem_statement] [text] NULL,
	[oem_case_idif_any] [varchar](255) NULL,
	[sales_account_manager] [varchar](255) NULL,
	[sl_no] [varchar](255) NULL,
	[pid] [varchar](255) NULL,
	[model] [varchar](255) NULL,
	[product] [varchar](255) NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[FreshService_T_Ticket_Stats]    Script Date: 09/06/2025 10:37:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[FreshService_T_Ticket_Stats](
	[ticket_id] [int] NOT NULL,
	[created_at] [datetime] NULL,
	[updated_at] [datetime] NULL,
	[opened_at] [datetime] NULL,
	[group_escalated] [bit] NULL,
	[inbound_count] [int] NULL,
	[status_updated_at] [datetime] NULL,
	[outbound_count] [int] NULL,
	[pending_since] [varchar](50) NULL,
	[resolved_at] [datetime] NULL,
	[closed_at] [datetime] NULL,
	[first_assigned_at] [datetime] NULL,
	[assigned_at] [datetime] NULL,
	[agent_responded_at] [datetime] NULL,
	[requester_responded_at] [datetime] NULL,
	[first_responded_at] [datetime] NULL,
	[first_resp_time_in_secs] [int] NULL,
	[resolution_time_in_secs] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[ticket_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[FreshService_T_Ticket_Tags]    Script Date: 09/06/2025 10:37:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[FreshService_T_Ticket_Tags](
	[TagId] [bigint] IDENTITY(1,1) NOT NULL,
	[ticket_id] [bigint] NULL,
	[RowIndex] [int] NULL,
	[Tag] [varchar](1000) NULL,
PRIMARY KEY CLUSTERED 
(
	[TagId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[FreshService_T_TicketRequestedFor]    Script Date: 09/06/2025 10:37:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[FreshService_T_TicketRequestedFor](
	[ticket_id] [bigint] NOT NULL,
	[email] [varchar](255) NULL,
	[mobile] [varchar](20) NULL,
	[name] [varchar](255) NULL,
	[phone] [varchar](20) NULL,
PRIMARY KEY CLUSTERED 
(
	[ticket_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[FreshService_T_TicketRequesters]    Script Date: 09/06/2025 10:37:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[FreshService_T_TicketRequesters](
	[ticket_id] [bigint] NOT NULL,
	[email] [varchar](255) NULL,
	[mobile] [varchar](20) NULL,
	[name] [varchar](255) NULL,
	[phone] [varchar](20) NULL,
PRIMARY KEY CLUSTERED 
(
	[ticket_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[FreshService_T_Tickets]    Script Date: 09/06/2025 10:37:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[FreshService_T_Tickets](
	[id] [bigint] NOT NULL,
	[subject] [varchar](255) NULL,
	[group_id] [bigint] NULL,
	[department_id] [bigint] NULL,
	[category] [varchar](255) NULL,
	[sub_category] [varchar](255) NULL,
	[item_category] [varchar](255) NULL,
	[requester_id] [bigint] NULL,
	[responder_id] [bigint] NULL,
	[due_by] [datetime] NULL,
	[fr_escalated] [bit] NULL,
	[deleted] [bit] NULL,
	[is_escalated] [bit] NULL,
	[fr_due_by] [datetime] NULL,
	[priority] [int] NULL,
	[status] [int] NULL,
	[source] [int] NULL,
	[created_at] [datetime] NULL,
	[updated_at] [datetime] NULL,
	[workspace_id] [int] NULL,
	[requested_for_id] [bigint] NULL,
	[type] [varchar](100) NULL,
	[description_text] [text] NULL,
	[department_name] [varchar](255) NULL,
	[tasks_dependency_type] [int] NULL,
	[created_on] [datetime] NULL,
	[updated_on] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[FreshService_T_Tickets_Bak_soumik12032025]    Script Date: 09/06/2025 10:37:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[FreshService_T_Tickets_Bak_soumik12032025](
	[id] [bigint] NOT NULL,
	[subject] [varchar](255) NULL,
	[group_id] [bigint] NULL,
	[department_id] [bigint] NULL,
	[category] [varchar](255) NULL,
	[sub_category] [varchar](255) NULL,
	[item_category] [varchar](255) NULL,
	[requester_id] [bigint] NULL,
	[responder_id] [bigint] NULL,
	[due_by] [datetime] NULL,
	[fr_escalated] [bit] NULL,
	[deleted] [bit] NULL,
	[is_escalated] [bit] NULL,
	[fr_due_by] [datetime] NULL,
	[priority] [int] NULL,
	[status] [int] NULL,
	[source] [int] NULL,
	[created_at] [datetime] NULL,
	[updated_at] [datetime] NULL,
	[workspace_id] [int] NULL,
	[requested_for_id] [bigint] NULL,
	[type] [varchar](100) NULL,
	[description_text] [text] NULL,
	[department_name] [varchar](255) NULL,
	[tasks_dependency_type] [int] NULL,
	[created_on] [datetime] NULL,
	[updated_on] [datetime] NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[FreshService_T_Tickets_BKP_17012025]    Script Date: 09/06/2025 10:37:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[FreshService_T_Tickets_BKP_17012025](
	[id] [bigint] NOT NULL,
	[subject] [varchar](255) NULL,
	[group_id] [bigint] NULL,
	[department_id] [bigint] NULL,
	[category] [varchar](255) NULL,
	[sub_category] [varchar](255) NULL,
	[item_category] [varchar](255) NULL,
	[requester_id] [bigint] NULL,
	[responder_id] [bigint] NULL,
	[due_by] [datetime] NULL,
	[fr_escalated] [bit] NULL,
	[deleted] [bit] NULL,
	[is_escalated] [bit] NULL,
	[fr_due_by] [datetime] NULL,
	[priority] [int] NULL,
	[status] [int] NULL,
	[source] [int] NULL,
	[created_at] [datetime] NULL,
	[updated_at] [datetime] NULL,
	[workspace_id] [int] NULL,
	[requested_for_id] [bigint] NULL,
	[type] [varchar](100) NULL,
	[description_text] [text] NULL,
	[department_name] [varchar](255) NULL,
	[tasks_dependency_type] [int] NULL,
	[created_on] [datetime] NULL,
	[updated_on] [datetime] NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[LastDateTobeConsidered]    Script Date: 09/06/2025 10:37:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[LastDateTobeConsidered](
	[LastDate] [datetime] NULL,
	[Active] [bit] NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[M_CustomerMapping]    Script Date: 09/06/2025 10:37:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[M_CustomerMapping](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[zaaid_site24x7] [varchar](100) NOT NULL,
	[name_site24x7] [varchar](100) NOT NULL,
	[departmentid_freshservice] [bigint] NOT NULL,
	[name_freshservice] [varchar](100) NOT NULL,
	[active] [bit] NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[M_Report_Sections]    Script Date: 09/06/2025 10:37:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[M_Report_Sections](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Code] [varchar](100) NULL,
	[Name] [varchar](255) NULL,
	[SortOrder] [int] NULL,
	[Active] [bit] NULL,
	[IsOptional] [bit] NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[M_Report_UserAccess]    Script Date: 09/06/2025 10:37:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[M_Report_UserAccess](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[UserName] [varchar](250) NULL,
	[UserEmail] [varchar](250) NULL,
	[Active] [bit] NULL,
	[TeamsTab] [bit] NULL,
	[CreatedBy] [varchar](250) NULL,
	[CreatedOn] [datetime] NULL,
	[ModifiedBy] [varchar](250) NULL,
	[ModifiedOn] [datetime] NULL,
	[UserId] [nvarchar](100) NULL,
	[MonthlyReportTab] [bit] NULL,
	[ContractTab] [bit] NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[M_ReportTypeWiseCustomer]    Script Date: 09/06/2025 10:37:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[M_ReportTypeWiseCustomer](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[departmentid] [bigint] NOT NULL,
	[name] [varchar](100) NOT NULL,
	[reporttype] [varchar](100) NOT NULL,
	[active] [bit] NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[MonthlyReport_M_Tenant]    Script Date: 09/06/2025 10:37:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MonthlyReport_M_Tenant](
	[TenantName] [varchar](255) NULL,
	[Active] [bit] NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Site24x7_M_AccessToken]    Script Date: 09/06/2025 10:37:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Site24x7_M_AccessToken](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ClientId] [varchar](200) NULL,
	[ClientSecret] [varchar](200) NULL,
	[AccessToken] [varchar](200) NULL,
	[RefreshToken] [varchar](200) NULL,
	[Scope] [varchar](max) NULL,
	[API_Domain] [varchar](100) NULL,
	[TokenType] [varchar](100) NULL,
	[ExpiresIn] [int] NULL,
	[ExpiresStarts] [datetime] NULL,
	[ExpiresOn] [datetime] NULL,
	[CreatedOn] [datetime] NULL,
	[UpdatedOn] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Site24x7_M_MSP_Customer]    Script Date: 09/06/2025 10:37:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Site24x7_M_MSP_Customer](
	[zaaid] [varchar](100) NOT NULL,
	[user_id] [varchar](50) NULL,
	[name] [varchar](100) NULL,
	[encodedZaaid] [varchar](100) NULL,
	[CreatedOn] [datetime] NULL,
	[UpdatedOn] [datetime] NULL,
	[Active] [bit] NULL,
PRIMARY KEY CLUSTERED 
(
	[zaaid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Site24x7_T_Per_Report_Server_Availability]    Script Date: 09/06/2025 10:37:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Site24x7_T_Per_Report_Server_Availability](
	[DetailId] [bigint] IDENTITY(1,1) NOT NULL,
	[RowId] [bigint] NULL,
	[RowIndex] [int] NULL,
	[Availability] [varchar](200) NULL,
PRIMARY KEY CLUSTERED 
(
	[DetailId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Site24x7_T_Per_Report_Server_Availability_BKP_17012025]    Script Date: 09/06/2025 10:37:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Site24x7_T_Per_Report_Server_Availability_BKP_17012025](
	[DetailId] [bigint] IDENTITY(1,1) NOT NULL,
	[RowId] [bigint] NULL,
	[RowIndex] [int] NULL,
	[Availability] [varchar](200) NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Site24x7_T_Per_Report_Server_Availability_BKP_19022025]    Script Date: 09/06/2025 10:37:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Site24x7_T_Per_Report_Server_Availability_BKP_19022025](
	[DetailId] [bigint] IDENTITY(1,1) NOT NULL,
	[RowId] [bigint] NULL,
	[RowIndex] [int] NULL,
	[Availability] [varchar](200) NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Site24x7_T_Per_Report_Server_Availability_Monthly]    Script Date: 09/06/2025 10:37:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Site24x7_T_Per_Report_Server_Availability_Monthly](
	[DetailId] [bigint] IDENTITY(1,1) NOT NULL,
	[RowId] [bigint] NULL,
	[RowIndex] [int] NULL,
	[Availability] [varchar](200) NULL,
PRIMARY KEY CLUSTERED 
(
	[DetailId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Site24x7_T_Per_Report_Server_Hdr_BKP_17012025]    Script Date: 09/06/2025 10:37:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Site24x7_T_Per_Report_Server_Hdr_BKP_17012025](
	[RowId] [bigint] IDENTITY(1,1) NOT NULL,
	[zaaid] [varchar](100) NOT NULL,
	[param_period] [int] NULL,
	[param_metric_aggregation] [int] NULL,
	[param_start_date] [varchar](50) NULL,
	[param_end_date] [varchar](50) NULL,
	[period] [int] NULL,
	[resource_type_name] [varchar](50) NULL,
	[resource_type] [int] NULL,
	[end_time] [varchar](50) NULL,
	[period_name] [varchar](50) NULL,
	[report_type] [int] NULL,
	[start_time] [varchar](50) NULL,
	[metric_aggregation] [int] NULL,
	[resource_name] [varchar](50) NULL,
	[report_name] [varchar](50) NULL,
	[monitor_type] [varchar](50) NULL,
	[CreatedOn] [datetime] NULL,
	[UpdatedOn] [datetime] NULL,
	[dtStartDate] [datetime] NULL,
	[dtEndDate] [datetime] NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Site24x7_T_Per_Report_Server_Hdr_BKP_19022025]    Script Date: 09/06/2025 10:37:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Site24x7_T_Per_Report_Server_Hdr_BKP_19022025](
	[RowId] [bigint] IDENTITY(1,1) NOT NULL,
	[zaaid] [varchar](100) NOT NULL,
	[param_period] [int] NULL,
	[param_metric_aggregation] [int] NULL,
	[param_start_date] [varchar](50) NULL,
	[param_end_date] [varchar](50) NULL,
	[period] [int] NULL,
	[resource_type_name] [varchar](50) NULL,
	[resource_type] [int] NULL,
	[end_time] [varchar](50) NULL,
	[period_name] [varchar](50) NULL,
	[report_type] [int] NULL,
	[start_time] [varchar](50) NULL,
	[metric_aggregation] [int] NULL,
	[resource_name] [varchar](50) NULL,
	[report_name] [varchar](50) NULL,
	[monitor_type] [varchar](50) NULL,
	[CreatedOn] [datetime] NULL,
	[UpdatedOn] [datetime] NULL,
	[dtStartDate] [datetime] NULL,
	[dtEndDate] [datetime] NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Site24x7_T_Per_Report_Server_Metrics_BKP_17012025]    Script Date: 09/06/2025 10:37:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Site24x7_T_Per_Report_Server_Metrics_BKP_17012025](
	[DetailId] [bigint] IDENTITY(1,1) NOT NULL,
	[RowId] [bigint] NULL,
	[RowIndex] [int] NULL,
	[DISKUSEDPERCENT] [numeric](10, 2) NULL,
	[MEMUSEDPERCENT] [numeric](10, 2) NULL,
	[CPUUSEDPERCENT] [numeric](10, 2) NULL,
	[param_metric_aggregation] [int] NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Site24x7_T_Per_Report_Server_Metrics_BKP_19022025]    Script Date: 09/06/2025 10:37:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Site24x7_T_Per_Report_Server_Metrics_BKP_19022025](
	[DetailId] [bigint] IDENTITY(1,1) NOT NULL,
	[RowId] [bigint] NULL,
	[RowIndex] [int] NULL,
	[DISKUSEDPERCENT] [numeric](10, 2) NULL,
	[MEMUSEDPERCENT] [numeric](10, 2) NULL,
	[CPUUSEDPERCENT] [numeric](10, 2) NULL,
	[param_metric_aggregation] [int] NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Site24x7_T_Per_Report_Server_Names_BKP_17012025]    Script Date: 09/06/2025 10:37:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Site24x7_T_Per_Report_Server_Names_BKP_17012025](
	[DetailId] [bigint] IDENTITY(1,1) NOT NULL,
	[RowId] [bigint] NULL,
	[RowIndex] [int] NULL,
	[ServerName] [varchar](200) NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Site24x7_T_Per_Report_Server_Names_BKP_19022025]    Script Date: 09/06/2025 10:37:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Site24x7_T_Per_Report_Server_Names_BKP_19022025](
	[DetailId] [bigint] IDENTITY(1,1) NOT NULL,
	[RowId] [bigint] NULL,
	[RowIndex] [int] NULL,
	[ServerName] [varchar](200) NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[T_BotInstallUninstal_Log]    Script Date: 09/06/2025 10:37:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[T_BotInstallUninstal_Log](
	[ConversationId] [nvarchar](200) NOT NULL,
	[UserId] [uniqueidentifier] NOT NULL,
	[UserName] [nvarchar](100) NULL,
	[UserEmail] [nvarchar](100) NULL,
	[ActivityId] [nvarchar](200) NOT NULL,
	[TenantId] [uniqueidentifier] NULL,
	[ServiceUrl] [nvarchar](200) NULL,
	[BotInstalledOn] [datetime] NULL,
	[RecipientId] [nvarchar](200) NULL,
	[RecipientName] [nvarchar](100) NULL,
	[UserPrincipalName] [nvarchar](100) NULL,
	[AppName] [nvarchar](50) NULL,
	[Active] [bit] NULL,
	[BotRemovedOn] [datetime] NULL,
	[ModifiedOn] [datetime] NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TeamsBot_T_UserSearch]    Script Date: 09/06/2025 10:37:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TeamsBot_T_UserSearch](
	[MessageId] [int] IDENTITY(1,1) NOT NULL,
	[UserName] [nvarchar](100) NULL,
	[UserEmail] [nvarchar](50) NULL,
	[UserUPN] [nvarchar](50) NULL,
	[UserADID] [varchar](50) NULL,
	[ChannelId] [nvarchar](50) NULL,
	[ConversationType] [nvarchar](50) NULL,
	[ConversationId] [nvarchar](500) NULL,
	[TenantId] [varchar](50) NULL,
	[ChatId] [nvarchar](50) NULL,
	[LocalTimestamp] [datetimeoffset](7) NULL,
	[Locale] [nvarchar](50) NULL,
	[ServiceUrl] [nvarchar](50) NULL,
	[Text] [nvarchar](max) NULL,
	[TextFormat] [nvarchar](50) NULL,
	[Timestamp] [datetimeoffset](7) NULL,
	[Response] [nvarchar](max) NULL,
	[Intent] [nvarchar](max) NULL,
	[CreatedOnIST] [datetime] NULL,
	[CreatedOnUTC] [datetime] NULL,
	[QuerySucceed] [bit] NULL,
PRIMARY KEY CLUSTERED 
(
	[MessageId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TeamsBot_T_UserSearchFiles]    Script Date: 09/06/2025 10:37:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TeamsBot_T_UserSearchFiles](
	[FileId] [int] IDENTITY(1,1) NOT NULL,
	[MessageId] [int] NULL,
	[FileName] [nvarchar](max) NULL,
	[FileURL] [nvarchar](max) NULL,
	[FileContent] [nvarchar](max) NULL,
PRIMARY KEY CLUSTERED 
(
	[FileId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
ALTER TABLE [dbo].[CP_M_WebChat_Feedback] ADD  DEFAULT ((1)) FOR [Active]
GO
ALTER TABLE [dbo].[CP_T_EmailLog] ADD  DEFAULT ((0)) FOR [Status]
GO
ALTER TABLE [dbo].[CP_T_OTPLog] ADD  DEFAULT ((0)) FOR [InvalidCount]
GO
ALTER TABLE [dbo].[CP_T_OTPLog] ADD  DEFAULT ((0)) FOR [ResendCount]
GO
ALTER TABLE [dbo].[CP_T_OTPLog] ADD  DEFAULT ((0)) FOR [Verified]
GO
ALTER TABLE [dbo].[CP_T_OTPLog] ADD  DEFAULT ((0)) FOR [Active]
GO
ALTER TABLE [dbo].[CP_T_SignInLog] ADD  DEFAULT ((1)) FOR [IsSessionActive]
GO
ALTER TABLE [dbo].[CP_T_SignInLog] ADD  DEFAULT ((0)) FOR [FailedLoginAttempts]
GO
ALTER TABLE [dbo].[CP_T_SignInLog] ADD  DEFAULT (getutcdate()) FOR [CreatedOn]
GO
ALTER TABLE [dbo].[M_Report_UserAccess] ADD  DEFAULT ((0)) FOR [MonthlyReportTab]
GO
ALTER TABLE [dbo].[M_Report_UserAccess] ADD  DEFAULT ((0)) FOR [ContractTab]
GO
ALTER TABLE [dbo].[Site24x7_M_AccessToken] ADD  DEFAULT ((0)) FOR [ExpiresIn]
GO
ALTER TABLE [dbo].[Site24x7_M_MSP_Customer] ADD  DEFAULT ((1)) FOR [Active]
GO
ALTER TABLE [dbo].[Site24x7_T_Per_Report_Server_Metrics] ADD  DEFAULT (NULL) FOR [DISKUSEDPERCENT]
GO
ALTER TABLE [dbo].[Site24x7_T_Per_Report_Server_Metrics] ADD  DEFAULT (NULL) FOR [MEMUSEDPERCENT]
GO
ALTER TABLE [dbo].[Site24x7_T_Per_Report_Server_Metrics] ADD  DEFAULT (NULL) FOR [CPUUSEDPERCENT]
GO
ALTER TABLE [dbo].[Site24x7_T_Per_Report_Server_Metrics_Monthly] ADD  DEFAULT (NULL) FOR [DISKUSEDPERCENT]
GO
ALTER TABLE [dbo].[Site24x7_T_Per_Report_Server_Metrics_Monthly] ADD  DEFAULT (NULL) FOR [MEMUSEDPERCENT]
GO
ALTER TABLE [dbo].[Site24x7_T_Per_Report_Server_Metrics_Monthly] ADD  DEFAULT (NULL) FOR [CPUUSEDPERCENT]
GO
/****** Object:  StoredProcedure [dbo].[FreshService_R_SummaryReport]    Script Date: 09/06/2025 10:37:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE   PROC [dbo].[FreshService_R_SummaryReport]
(
@DepartmentId BIGINT=NULL,
@Start_Date	VARCHAR(10)='01/11/2024',
@End_Date VARCHAR(10)='30/11/2024'
)
AS
BEGIN
	SELECT  D.[name], T.department_id,T.[type],ISNULL(T.category,'-') AS Category,count(T.id) as NoOfTickets
	FROM [FreshService_T_Tickets] T WITH(NOLOCK) 
	INNER JOIN [FreshService_M_Department] D WITH(NOLOCK) ON T.department_id=D.id
	INNER JOIN [FreshService_T_Ticket_CustomFields] TC WITH(NOLOCK) ON TC.ticket_id=T.id
	WHERE T.created_at between CONVERT(DATE,@Start_Date,103) AND CONVERT(DATE,@End_Date,103)
	AND T.department_id=@DepartmentId OR ISNULL(@DepartmentId,0)=0
	AND TC.tenant IN (
		SELECT TenantName 
		FROM MonthlyReport_M_Tenant WITH (NOLOCK) 
		WHERE Active = 1
	)
	GROUP BY D.[name], T.department_id,T.[type],ISNULL(T.category,'-')
	
	UNION ALL 
	SELECT  D.[name], C.department_id,'Change' as [type],ISNULL(C.category,'-') AS Category,count(C.id) as NoOfTickets
	FROM [FreshService_T_Change] C WITH(NOLOCK) 
	INNER JOIN [FreshService_M_Department] D WITH(NOLOCK) ON C.department_id=D.id
	WHERE C.created_at between CONVERT(DATE,@Start_Date,103) AND CONVERT(DATE,@End_Date,103)
	AND C.department_id=@DepartmentId OR ISNULL(@DepartmentId,0)=0
	GROUP BY D.[name], C.department_id,ISNULL(C.category,'-')
	

	ORDER BY [name],[type],category
END
GO
/****** Object:  StoredProcedure [dbo].[FreshService_R_SummaryReport_PIVOT]    Script Date: 09/06/2025 10:37:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--exec [FreshService_R_SummaryReport_PIVOT] 27000180437,'01/01/2025','31/01/2025'
CREATE   PROC [dbo].[FreshService_R_SummaryReport_PIVOT]
(@DepartmentId BIGINT=27000109788,
@Start_Date	VARCHAR(10)='01/11/2024',
@End_Date VARCHAR(10)='30/11/2024'
)
AS
BEGIN


DROP TABLE IF EXISTS #FreshService_R_SummaryReport  
  
CREATE TABLE #FreshService_R_SummaryReport  
(  
	departmentid bigint,
	[name] varchar(100),
	[type] varchar(100),
	[category] varchar(100),
	[categoryId] int,
	nooftickets int 
)  
INSERT INTO #FreshService_R_SummaryReport ([name],departmentid,[type],[category],nooftickets)

  SELECT  D.[name], T.department_id,T.[type],ISNULL(T.category,'-') AS Category,count(T.id) as NoOfTickets
	FROM [FreshService_T_Tickets] T WITH(NOLOCK) 
	INNER JOIN [FreshService_M_Department] D WITH(NOLOCK) ON T.department_id=D.id
	INNER JOIN [FreshService_T_Ticket_CustomFields] TC WITH(NOLOCK) ON TC.ticket_id=T.id
	WHERE CONVERT(DATE,DATEADD(MINUTE,330,T.created_at),103) between CONVERT(DATE,@Start_Date,103) AND CONVERT(DATE,@End_Date,103)
	AND (T.department_id=@DepartmentId OR ISNULL(@DepartmentId,0)=0)
	AND TC.tenant IN (
		SELECT TenantName 
		FROM MonthlyReport_M_Tenant WITH (NOLOCK) 
		WHERE Active = 1
	)
	GROUP BY D.[name], T.department_id,T.[type],ISNULL(T.category,'-')
	
	UNION ALL 
	SELECT  D.[name], C.department_id,'Change' as [type],ISNULL(C.category,'-') AS Category,count(C.id) as NoOfTickets
	FROM [FreshService_T_Change] C WITH(NOLOCK) 
	INNER JOIN [FreshService_M_Department] D WITH(NOLOCK) ON C.department_id=D.id
	WHERE CONVERT(DATE,DATEADD(MINUTE,330,C.created_at),103)between CONVERT(DATE,@Start_Date,103) AND CONVERT(DATE,@End_Date,103)
	AND (C.department_id=@DepartmentId OR ISNULL(@DepartmentId,0)=0)
	GROUP BY D.[name], C.department_id,ISNULL(C.category,'-')
	

	ORDER BY [name],[type],category
  
  INSERT INTO #FreshService_R_SummaryReport ([name],departmentid,[type],[category],nooftickets)
  SELECT [NAME], departmentid,'ZZZ-Grand Total',[category],sum(nooftickets)  FROM #FreshService_R_SummaryReport
  GROUP BY departmentid,[NAME], [category]

  INSERT INTO #FreshService_R_SummaryReport ([name],departmentid,[type],[category],nooftickets)
  SELECT [NAME], departmentid,[type],'ZZZ-Grand Total',sum(nooftickets)  FROM #FreshService_R_SummaryReport
  GROUP BY departmentid,[NAME], [type]
  

DECLARE @COLUMNS AS NVARCHAR(MAX)  
DECLARE @QUERY  AS NVARCHAR(MAX)  
  
SET @COLUMNS = STUFF((SELECT distinct ',' + QUOTENAME([category])   
            FROM #FreshService_R_SummaryReport  
			--ORDER BY categoryId
            FOR XML PATH(''), TYPE  
            ).value('.', 'NVARCHAR(MAX)')   
        ,1,1,'')  
  

set @query = 'SELECT	departmentid,[name],[type],' + @COLUMNS + '   
			FROM	#FreshService_R_SummaryReport  
			pivot   
			(  
				min([nooftickets]) 
				for [category] in (' + @COLUMNS + ')  
			) p   
			ORDER BY [type] ASC'   
	--SELECT @query
execute(@query)  

select * from #FreshService_R_SummaryReport

END
GO
/****** Object:  StoredProcedure [dbo].[FreshService_R_SummaryResponsePriotySLA]    Script Date: 09/06/2025 10:37:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE   PROC [dbo].[FreshService_R_SummaryResponsePriotySLA]
(
@DepartmentId INT=NULL,
@Start_Date	VARCHAR(10)='01/11/2024',
@End_Date VARCHAR(10)='30/11/2024'
)
AS
BEGIN
	SELECT  D.[name], T.department_id,T.[type],T.[priority],isnull(t.fr_escalated,0),count(T.id) as NoOfTickets
	FROM [FreshService_T_Tickets] T WITH(NOLOCK) 
	INNER JOIN [FreshService_M_Department] D WITH(NOLOCK) ON T.department_id=D.id
	WHERE CONVERT(DATE,DATEADD(MINUTE,330,T.created_at),103) between CONVERT(DATE,@Start_Date,103) AND CONVERT(DATE,@End_Date,103)
	AND (T.department_id=@DepartmentId OR ISNULL(@DepartmentId,0)=0)
	GROUP BY D.[name], T.department_id,T.[type],T.[priority],isnull(t.fr_escalated,0)
	
	

	--ORDER BY [name],[type],category
END
GO
/****** Object:  StoredProcedure [dbo].[Usp_bot_Conversation_InsUp]    Script Date: 09/06/2025 10:37:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[Usp_bot_Conversation_InsUp]
	@ActivityId NVARCHAR(200),
	@ConversationId  NVARCHAR(200),
	@RecipientId  NVARCHAR(200),
	@RecipientName  NVARCHAR(200),
	@ServiceUrl NVARCHAR(200),
	@UserEmail  NVARCHAR(100),
	@TenantId  UNIQUEIDENTIFIER,
	@UserId   UNIQUEIDENTIFIER,
	@UserName NVARCHAR(100),
	@UserPrincipalName NVARCHAR(100),
	@BotActiveInactive BIT,	-- 0 -> Bot Uninstall, 1 -> Bot Install,
	@AppName NVARCHAR(100)
AS
----- CREATED BY Soumik 10-01-2025
BEGIN


	IF EXISTS (SELECT 1 FROM T_BotInstallUninstal_Log WHERE UserId = @UserId AND ConversationId=@ConversationId AND AppName=@AppName)
	BEGIN
			UPDATE T_BotInstallUninstal_Log
			SET 
			BotRemovedOn	=	CASE WHEN @BotActiveInactive = 0 THEN GETDATE() ELSE NULL END,
			BotInstalledOn	=	CASE WHEN @BotActiveInactive = 1 THEN GETDATE() ELSE BotInstalledOn END,
			Active = @BotActiveInactive,
			ModifiedOn = GETDATE()
			WHERE ConversationId=@ConversationId AND AppName=@AppName
			AND UserId=@UserId
	END
	
	ELSE
	BEGIN
		INSERT INTO T_BotInstallUninstal_Log
		(
			ActivityId,ConversationId,RecipientId,RecipientName,ServiceUrl,UserEmail,TenantId,UserId,UserName,BotInstalledOn,UserPrincipalName,AppName,Active,ModifiedOn
		)
		VALUES
		(
			@ActivityId,@ConversationId,@RecipientId,@RecipientName,@ServiceUrl,@UserEmail,@TenantId,@UserId,@UserName,GETDATE(),
			@UserPrincipalName,@AppName,@BotActiveInactive,GETDATE()
		)
	END

	EXEC Usp_Report_UserAccess_InsUp 	
	@UserName	= @UserName,
	@UserEmail	= @UserPrincipalName,
	@Active		= @BotActiveInactive,
	@CreatedBy	= @UserEmail,
	@UserId		= @UserId

	IF @@ERROR<>0
	BEGIN
		SELECT 
			'Something went wrong, unable to insert Conversation data'	AS [Message],
			''						AS ErrorMessage,
			0						AS [Status],
			0						AS Id,
			''						AS ReferenceNo
		RETURN 
	END

	SELECT 
		'Conversation data saved successfully!'			AS	[Message],
		''								AS ErrorMessage,
		1								AS [Status],
		1					AS Id,
		@ConversationId					AS ReferenceNo
END

GO
/****** Object:  StoredProcedure [dbo].[usp_CP_CustomerWise_MasterData_Get]    Script Date: 09/06/2025 10:37:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_CP_CustomerWise_MasterData_Get]
(
	@embee_crm_ids VARCHAR(500) = NULL,
	@DepartmentIds VARCHAR(500) = NULL
)
AS
BEGIN
    SET NOCOUNT ON;

    -- Get Current IST Time
	DECLARE @CurrentIST DATETIME = SWITCHOFFSET(GETUTCDATE(), '+05:30');

	-- Department Temp Table
	CREATE TABLE #CustomerWise_MasterData_DepartmentIdTable (DepartmentId BIGINT);

	IF @DepartmentIds IS NOT NULL
	BEGIN
		INSERT INTO #CustomerWise_MasterData_DepartmentIdTable (DepartmentId)
		SELECT CAST(value AS BIGINT) 
		FROM STRING_SPLIT(@DepartmentIds, ',');

		-- Index for performance
		CREATE INDEX idx_DepartmentId ON #CustomerWise_MasterData_DepartmentIdTable(DepartmentId);
	END;

	-- CustomerId Temp Table
	CREATE TABLE #CustomerWise_MasterData_CustomerIdTable (CustomerId VARCHAR(225));

	IF @embee_crm_ids IS NOT NULL
	BEGIN
		INSERT INTO #CustomerWise_MasterData_CustomerIdTable (CustomerId)
		SELECT value 
		FROM STRING_SPLIT(@embee_crm_ids, ',');

		-- Index for performance
		CREATE INDEX idx_CustomerId ON #CustomerWise_MasterData_CustomerIdTable(CustomerId);
	END;

	-- Get Active Contract Details
	SELECT
        CM.Id,
        CM.CustomerId,
		CM.DepartmentId,
        CM.CategoryId,
        CM.SubCategoryId
	INTO #CustomerWise_MasterData_ActiveContracts
	FROM CP_T_ContractMaster CM WITH(NOLOCK)
		INNER JOIN #CustomerWise_MasterData_DepartmentIdTable DIT ON CM.DepartmentId = DIT.DepartmentId
		INNER JOIN #CustomerWise_MasterData_CustomerIdTable CIT ON CM.CustomerId = CIT.CustomerId
	WHERE (@CurrentIST BETWEEN CM.StartDate AND CM.EndDate) OR ExtendSupport = 1;
	
	-- Index on Active Contracts Temp Table
	CREATE INDEX idx_ActiveContracts_CategoryId ON #CustomerWise_MasterData_ActiveContracts(CategoryId);
	CREATE INDEX idx_ActiveContracts_SubCategoryId ON #CustomerWise_MasterData_ActiveContracts(SubCategoryId);

	-- Get Active Categories
	SELECT DISTINCT 
		C.[Id],
		C.[Code] AS [CategoryCode],
		C.[Name] AS [CategoryName],
		C.[Active]
	FROM [dbo].[CP_M_Category] C
	INNER JOIN #CustomerWise_MasterData_ActiveContracts AC ON C.Id = AC.CategoryId
	WHERE C.[Active] = 1;

	-- Get Active SubCategories
	SELECT DISTINCT 
		S.[Id],
		S.[CategoryId],
		S.[Code] AS [SubCategoryCode],
		S.[Name] AS [SubCategoryName],
		S.[Active],
		IND.IndexName
	FROM [dbo].[CP_M_SubCategory] S
	INNER JOIN #CustomerWise_MasterData_ActiveContracts AC ON S.Id = AC.SubCategoryId
	LEFT JOIN [CP_M_WebChat_SOPIndex] IND ON IND.CategoryId = S.CategoryId AND IND.SubCategoryId = S.Id
	WHERE S.[Active] = 1;

	-- Clear Temp Tables
	DROP TABLE #CustomerWise_MasterData_DepartmentIdTable;
	DROP TABLE #CustomerWise_MasterData_CustomerIdTable;
	DROP TABLE #CustomerWise_MasterData_ActiveContracts;

END
GO
/****** Object:  StoredProcedure [dbo].[usp_CP_FreshServiceData_Get]    Script Date: 09/06/2025 10:37:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_CP_FreshServiceData_Get]
(
    @tenant NVARCHAR(255) = NULL,
    @embee_crm_id NVARCHAR(50) = NULL
)
AS
BEGIN
    SET NOCOUNT ON;

    -- Query 1: Retrieve distinct tenant values (no parameter filtering required)
    SELECT DISTINCT DC.tenant
    FROM dbo.[FreshService_M_Department] D WITH(NOLOCK)
        INNER JOIN dbo.[FreshService_M_Department_CustomFields] DC WITH(NOLOCK) ON D.id = DC.department_id
	WHERE DC.tenant IS NOT NULL
    ORDER BY DC.tenant;


    -- Query 2: Retrieve distinct SAP customer name and CRM ID (filter by tenant if provided)
    IF @Tenant IS NOT NULL
    BEGIN
        SELECT DISTINCT DC.sap_customer_name, DC.embee_crm_id
        FROM dbo.[FreshService_M_Department] D WITH(NOLOCK)
            INNER JOIN dbo.[FreshService_M_Department_CustomFields] DC WITH(NOLOCK) ON D.id = DC.department_id
        WHERE DC.sap_customer_name IS NOT NULL
            AND DC.tenant = @tenant
        ORDER BY DC.sap_customer_name;
    END


    -- Query 3: Retrieve department details (filter by tenant and CRM ID if provided)
    IF @Tenant IS NOT NULL AND @embee_crm_id IS NOT NULL
    BEGIN
        SELECT d.id, d.name
        FROM dbo.[FreshService_M_Department] D WITH(NOLOCK)
            INNER JOIN dbo.[FreshService_M_Department_CustomFields] DC WITH(NOLOCK) ON D.id = DC.department_id
        WHERE DC.sap_customer_name IS NOT NULL
            AND DC.tenant = @tenant
            AND DC.embee_crm_id = @embee_crm_id
        ORDER BY DC.sap_customer_name;
    END


END
GO
/****** Object:  StoredProcedure [dbo].[usp_CP_M_WebChat_Feedback_Get]    Script Date: 09/06/2025 10:37:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_CP_M_WebChat_Feedback_Get]
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT TOP (1000) [Id]
      ,[Name]
      ,[Active]
	FROM [dbo].[CP_M_WebChat_Feedback]
    WHERE Active = 1;

END
GO
/****** Object:  StoredProcedure [dbo].[usp_CP_M_WebChat_Options_Get]    Script Date: 09/06/2025 10:37:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_CP_M_WebChat_Options_Get]
(
    @CategoryIdList VARCHAR(255) = NULL,
    @SubCategoryIdList VARCHAR(255) = NULL,
	@Top INT = 5
)
AS
BEGIN
	
    SET NOCOUNT ON;

	DECLARE @CategoryIds TABLE (CategoryId INT);
    DECLARE @SubCategoryIds TABLE (SubCategoryId INT);

	IF(@CategoryIdList IS NOT NULL)
	BEGIN
		INSERT INTO @CategoryIds (CategoryId)
		SELECT value FROM STRING_SPLIT(@CategoryIdList, ',') WHERE ISNUMERIC(value) = 1;
	END

	IF(@SubCategoryIdList IS NOT NULL)
	BEGIN
		INSERT INTO @SubCategoryIds (SubCategoryId)
		SELECT value FROM STRING_SPLIT(@SubCategoryIdList, ',') WHERE ISNUMERIC(value) = 1;
	END


	SELECT TOP (@Top)
        O.[Id] AS 'OptionId',
        O.[CategoryId],
        O.[SubCategoryId],
        O.[Option],
        O.[Active]
    FROM [dbo].[CP_M_Webchat_Options] O WITH(NOLOCK)
    INNER JOIN @CategoryIds C ON O.CategoryId = C.CategoryId
    INNER JOIN @SubCategoryIds S ON O.SubCategoryId = S.SubCategoryId
    WHERE O.Active = 1

    ORDER BY NEWID();

END
GO
/****** Object:  StoredProcedure [dbo].[usp_CP_M_WebChat_SOPIndex_Get]    Script Date: 09/06/2025 10:37:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_CP_M_WebChat_SOPIndex_Get]
(
    @CategoryId INT = 1,
    @SubCategoryId INT = 1
)
AS
BEGIN	
    SET NOCOUNT ON;

	SELECT TOP 1 [AutoId]
      ,[CategoryId]
      ,[SubCategoryId]
      ,[IndexName]
      ,[Active]
	FROM [dbo].[CP_M_WebChat_SOPIndex] WITH(NOLOCK)
	WHERE [CategoryId] = @CategoryId
	AND [SubCategoryId] = @SubCategoryId
	AND Active = 1

END
GO
/****** Object:  StoredProcedure [dbo].[usp_CP_MasterData_Get]    Script Date: 09/06/2025 10:37:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_CP_MasterData_Get]
(
	@CategoryId INT = NULL,
	@SubCategoryId INT = NULL,
	@TenantId INT = NULL,
	@RegionId INT = NULL
)
AS
BEGIN
    SET NOCOUNT ON;

	SELECT [Id]
		,[Code] AS 'CategoryCode'
		,[Name] AS 'CategoryName'
		,[Active]
	FROM [dbo].[CP_M_Category] C
	WHERE (@CategoryId IS NULL OR C.Id = @CategoryId)
	AND Active = 1;


	SELECT S.[Id]
		,S.[CategoryId]
		,CONCAT(C.Code,S.[Code]) AS 'SubCategoryCode'
		,S.[Name] AS 'SubCategoryName'
		,S.[Active]
	FROM [dbo].[CP_M_SubCategory] S
	INNER JOIN [dbo].[CP_M_Category] C ON C.Id = S.CategoryId
	WHERE (@CategoryId IS NULL OR S.CategoryId = @CategoryId)
	AND (@SubCategoryId IS NULL OR S.Id = @SubCategoryId)
	AND S.Active = 1;


	SELECT [Id]
		,[TenantCode]
		,[TenantName]
		,[Active]
		,DT.tenant AS 'Dept_Tenant'
	FROM [dbo].[CP_M_Tenant] T
	LEFT JOIN 
	(
		SELECT DISTINCT DC.tenant
		FROM dbo.[FreshService_M_Department] D WITH(NOLOCK)
		INNER JOIN dbo.[FreshService_M_Department_CustomFields] DC WITH(NOLOCK) ON D.id = DC.department_id
		WHERE DC.tenant IS NOT NULL
	) DT ON DT.tenant = T.TenantName
	WHERE (@TenantId IS NULL OR T.Id = @TenantId)
	AND Active = 1
	ORDER BY T.TenantName;


	SELECT [Id]
		,[RegionCode]
		,[RegionName]
		,[Active]
	FROM [dbo].[CP_M_Region] R
	WHERE (@RegionId IS NULL OR R.Id = @RegionId)
	AND Active = 1;


	SELECT DISTINCT 
		DC.sap_customer_name
		,DC.embee_crm_id
		,DC.tenant
    FROM dbo.[FreshService_M_Department] D WITH(NOLOCK)
        INNER JOIN dbo.[FreshService_M_Department_CustomFields] DC WITH(NOLOCK) ON D.id = DC.department_id
    WHERE DC.sap_customer_name IS NOT NULL AND DC.embee_crm_id IS NOT NULL
	ORDER BY DC.sap_customer_name;


	SELECT
		d.id AS 'departmentId'
		,d.[name] AS 'departmentName'
		,DC.sap_customer_name
		,DC.embee_crm_id
		,DC.tenant
    FROM dbo.[FreshService_M_Department] D WITH(NOLOCK)
       INNER JOIN dbo.[FreshService_M_Department_CustomFields] DC WITH(NOLOCK) ON D.id = DC.department_id
    WHERE DC.sap_customer_name IS NOT NULL AND DC.embee_crm_id IS NOT NULL
    ORDER BY DC.sap_customer_name;

END
GO
/****** Object:  StoredProcedure [dbo].[usp_CP_R_BasicReports]    Script Date: 09/06/2025 10:37:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_CP_R_BasicReports]
(
	@UserId VARCHAR(100) = NULL,
    @UserEmail VARCHAR(255) = NULL,
	@DepartmentIds VARCHAR(500) = NULL, -- (1234,5678)
	@Start_Date VARCHAR(10) = NULL, -- '07/04/2025'
	@End_Date VARCHAR(10) = NULL -- '07/04/2025'
)
AS
BEGIN

    SET NOCOUNT ON;
	DECLARE @CurrentIST DATETIME = SWITCHOFFSET(GETUTCDATE(), '+05:30');
	DECLARE @CurrentUTC DATETIME = GETUTCDATE();



	-- Department Temp Table
	CREATE TABLE #CP_R_BasicReports_DepartmentIdTable (DepartmentId BIGINT);
	-- UserId Temp Table
	CREATE TABLE #CP_R_BasicReports_UserEmailTable (UserEmail VARCHAR(255));

	IF @DepartmentIds IS NOT NULL
	BEGIN
		INSERT INTO #CP_R_BasicReports_DepartmentIdTable (DepartmentId)
		SELECT value 
		FROM STRING_SPLIT(@DepartmentIds, ',');
    
		-- index for performance
		CREATE INDEX idx_DepartmentId ON #CP_R_BasicReports_DepartmentIdTable(DepartmentId);

		IF EXISTS (SELECT * FROM #CP_R_BasicReports_DepartmentIdTable)
		BEGIN
			INSERT INTO #CP_R_BasicReports_UserEmailTable (UserEmail)
			SELECT TRIM(primary_email)
			FROM dbo.[FreshService_M_Requesters] req
			INNER JOIN dbo.[FreshService_M_Requester_Departments] dept ON dept.requester_id = req.id
			INNER JOIN #CP_R_BasicReports_DepartmentIdTable dept_filter ON dept_filter.DepartmentId = dept.department_id
			WHERE ISNULL(@UserEmail, '') = '' OR TRIM(primary_email) = TRIM(@UserEmail)
    
			-- index for performance
			CREATE INDEX idx_UserEmail ON #CP_R_BasicReports_UserEmailTable(UserEmail);
		END
	END



	-- SIGN IN LOG
	SELECT
		[LogId]
		,[UserId]
		,[UserName]
		,[UserEmail]
		,[SigninTimeUTC]
		,CONVERT(DATETIME, (SWITCHOFFSET([SigninTimeUTC], '+05:30'))) AS [SigninTimeIST]		
		,[SignoutTimeUTC]
		,CONVERT(DATETIME, (SWITCHOFFSET([SignoutTimeUTC], '+05:30'))) AS [SignoutTimeIST]
		,CASE 
			WHEN SigninTimeUTC IS NULL OR SignoutTimeUTC IS NULL THEN '00:00:00'
			ELSE FORMAT(SignoutTimeUTC - SigninTimeUTC, 'HH:mm:ss')
		END AS Duration
		,[ClientIP]
		,[UserAgent]
		,[DeviceType]
		,[SessionId]
		,[SignOutRemarks]
		,[FailedLoginAttempts]
		,[CreatedOn]
		,CONVERT(DATETIME, (SWITCHOFFSET([CreatedOn], '+05:30'))) AS [CreatedOnIST]
		,(SELECT COUNT(*) 
		  FROM [dbo].[CP_T_WebChat_Log] innerLog WITH(NOLOCK)
		  WHERE innerLog.SessionId = outerLog.SessionId
		) AS ChatInitiatedCount
	FROM [dbo].[CP_T_SignInLog] outerLog WITH(NOLOCK)
	--INNER JOIN dbo.[FreshService_M_Department]
	WHERE 
	--ISNULL(@UserEmail, '') = '' OR TRIM(UserEmail) = TRIM(@UserEmail)
	(
		(ISNULL(@DepartmentIds, '') = '' AND (ISNULL(@UserEmail, '') = '' OR TRIM(UserEmail) = TRIM(@UserEmail)))
		OR
		(ISNULL(@DepartmentIds, '') != '' AND TRIM(UserEmail) IN (SELECT UserEmail FROM #CP_R_BasicReports_UserEmailTable))
	)
	AND (ISNULL(@UserId, '') = '' OR TRIM(UserId) = TRIM(@UserId))
	AND (@Start_Date IS NULL OR CONVERT(DATE, (SWITCHOFFSET([CreatedOn], '+05:30'))) >= CONVERT(DATE, @Start_Date, 103))
	AND (@End_Date IS NULL OR CONVERT(DATE, (SWITCHOFFSET([CreatedOn], '+05:30'))) <= CONVERT(DATE, @End_Date, 103))
	ORDER BY
		LogId;

	-- WEB CHAT LOG
	SELECT 
		[AutoId]
		,[UserId]
		,[UserName]
		,[UserEmail]
		,[StartedOn]
		,[EndedOn]
		,CASE 
			WHEN StartedOn IS NULL OR EndedOn IS NULL THEN '00:00:00'
			ELSE FORMAT(EndedOn - StartedOn, 'HH:mm:ss')
		END AS Duration
		,[SessionCloseRemarks]
		,(SELECT COUNT(*) 
		  FROM [dbo].[CP_T_WebChat_UserConversationLog] innerLog WITH(NOLOCK)
		  WHERE innerLog.WebChatLogId = outerLog.AutoId
		) AS QueryCount
	FROM [dbo].[CP_T_WebChat_Log] outerLog WITH(NOLOCK)
	WHERE
	--ISNULL(@UserEmail, '') = '' OR TRIM(UserEmail) = TRIM(@UserEmail)
	(
		(ISNULL(@DepartmentIds,'') = '' AND (ISNULL(@UserEmail, '') = '' OR TRIM(UserEmail) = TRIM(@UserEmail)))
		OR
		(ISNULL(@DepartmentIds,'') != '' AND TRIM(UserEmail) IN (SELECT UserEmail FROM #CP_R_BasicReports_UserEmailTable))
	)
	AND (ISNULL(@UserId, '') = '' OR TRIM(UserId) = TRIM(@UserId))
	AND (@Start_Date IS NULL OR CONVERT(DATE, (SWITCHOFFSET([CreatedOn], '+05:30'))) >= CONVERT(DATE, @Start_Date, 103))
	AND (@End_Date IS NULL OR CONVERT(DATE, (SWITCHOFFSET([CreatedOn], '+05:30'))) <= CONVERT(DATE, @End_Date, 103))
	ORDER BY
		AutoId;

	-- WEB CHAT CONVERSATION LOG
	SELECT 
		[MessageId]
      ,[WebChatLogId]
      ,L.[UserName]
      ,L.[UserEmail]
      ,[LocalTimestamp]
      ,[Text]
      ,[Timestamp]
      ,[Response]
      ,[CreatedOnIST]
      ,[CreatedOnUTC]
      ,[MessageSentUTC]
      ,[FeedbackCardSentUTC]
      ,[LikeDislike]
      ,[FeedbackReceivedUTC]
      ,L.[CategoryId]
	  ,C.Code AS 'CategoryCode'
	  ,C.Name AS 'CategoryName'
      ,L.[SubCategoryId]
	  ,S.Code AS 'SubCategoryCode'
	  ,S.Name AS 'SubCategoryName'
	FROM [dbo].[CP_T_WebChat_UserConversationLog] L WITH(NOLOCK)
	INNER JOIN [dbo].[CP_T_WebChat_Log] W ON W.AutoId = L.WebChatLogId
	LEFT JOIN dbo.[CP_M_Category] C ON C.Id = L.CategoryId
	LEFT JOIN dbo.[CP_M_SubCategory] S ON S.Id = L.SubCategoryId AND S.CategoryId = L.CategoryId
	WHERE 
	--ISNULL(@UserEmail, '') = '' OR TRIM(L.UserEmail) = TRIM(@UserEmail)
	(
		(ISNULL(@DepartmentIds,'') = '' AND (ISNULL(@UserEmail, '') = '' OR TRIM(L.UserEmail) = TRIM(@UserEmail)))
		OR
		(ISNULL(@DepartmentIds,'') != '' AND TRIM(L.UserEmail) IN (SELECT UserEmail FROM #CP_R_BasicReports_UserEmailTable))
	)
	AND (ISNULL(@UserId, '') = '' OR (TRIM(W.UserId) = TRIM(@UserId) AND TRIM(L.UserEmail) = TRIM(@UserEmail)))
	AND (@Start_Date IS NULL OR CONVERT(DATE, CreatedOnIST) >= CONVERT(DATE, @Start_Date, 103))
	AND (@End_Date IS NULL OR CONVERT(DATE, CreatedOnIST) <= CONVERT(DATE, @End_Date, 103))
	ORDER BY
		MessageId;




	-- Clear Temp Tables
	DROP TABLE #CP_R_BasicReports_DepartmentIdTable;
	DROP TABLE #CP_R_BasicReports_UserEmailTable;

END;
GO
/****** Object:  StoredProcedure [dbo].[usp_CP_T_ContractMaster_Get]    Script Date: 09/06/2025 10:37:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_CP_T_ContractMaster_Get]
(
    @Id BIGINT = NULL,
    @ContractNo VARCHAR(255) = NULL,
	@TenantId INT = NULL,
    @TenantName VARCHAR(255) = NULL,
    @CustomerId VARCHAR(255) = NULL,
    @CustomerName VARCHAR(255) = NULL,
	@DepartmentId BIGINT = NULL,
    @DepartmentName VARCHAR(255) = NULL,
    @CategoryId INT = NULL,
    @SubCategoryId INT = NULL,
    @StartDate DATETIME = NULL,
    @EndDate DATETIME = NULL,
    @RegionId INT = NULL,
    @Acc_ManagerName VARCHAR(255) = NULL,
    @Acc_ManagerEmail VARCHAR(255) = NULL,
	@PONo VARCHAR(255) = NULL,
    @Active BIT = NULL,
	@CreatedByEmail VARCHAR(150) = NULL
)
AS
BEGIN
    SET NOCOUNT ON;
	DECLARE @CurrentIST DATETIME = SWITCHOFFSET(GETUTCDATE(), '+05:30');

    IF @Id = 0
    BEGIN
        -- Get all rows from CP_T_ContractMaster
        SELECT
			ROW_NUMBER() OVER (ORDER BY CM.CustomerName, CM.DepartmentName, CM.TenantName, C.[Name], SC.[Name]) AS 'SlNo',
            CM.Id,
			TRY_CAST(CM.ReferenceNo AS VARCHAR(255)) AS 'ReferenceNo',
            ContractNo,
            TenantId,
            TenantName,
            CustomerId,
            CustomerName,
			DepartmentId,
            DepartmentName,
            CM.CategoryId,
			C.[Name] AS 'CategoryName',
			C.[Code] AS 'CategoryCode',
            CM.SubCategoryId,
			SC.[Name] AS 'SubCategoryName',
			SC.[Code] AS 'SubCategoryCode',
            StartDate,
            EndDate,
            CM.RegionId,
			R.RegionCode,
			R.RegionName,
            Acc_ManagerName,
            Acc_ManagerEmail,
			PONo,
            CM.Active,
			CM.CreatedByName,
			CM.CreatedByEmail,
			CAST(SWITCHOFFSET(CM.CreatedOnUTC, '+05:30') AS DATETIME) AS 'CreatedOn',
			CM.ModifiedByName,
			CM.ModifiedEmail,
			CAST(SWITCHOFFSET(CM.ModifiedOnUTC, '+05:30') AS DATETIME) AS 'ModifiedOn',
			D.contact_person AS 'ContactPersonName',
			D.contact_email_id AS 'ContactPersonEmail',
			D.contact_number AS 'ContactPersonPhone',
			CASE
				WHEN CM.StartDate >= @CurrentIST THEN 1
				WHEN @CurrentIST BETWEEN CM.StartDate AND CM.EndDate THEN 2
				WHEN @CurrentIST >= CM.EndDate AND ISNULL(ExtendSupport,0) <> 1 THEN 3
				WHEN @CurrentIST >= CM.EndDate AND ISNULL(ExtendSupport,0) = 1 THEN 4
				ELSE NULL
			END AS 'ActiveStatusId',
			CASE
				WHEN CM.StartDate >= @CurrentIST THEN 'Upcoming' 
				WHEN @CurrentIST BETWEEN CM.StartDate AND CM.EndDate THEN 'Active'
				WHEN @CurrentIST >= CM.EndDate AND ISNULL(ExtendSupport,0) <> 1 THEN 'Inactive'
				WHEN @CurrentIST >= CM.EndDate AND ISNULL(ExtendSupport,0) = 1 THEN 'Active - Extend Support'
				ELSE ''
			END AS 'ActiveStatus',
			CM.ExtendSupport
        FROM CP_T_ContractMaster CM WITH(NOLOCK)
		INNER JOIN CP_M_Category C WITH(NOLOCK) ON C.Id = CM.CategoryId
		INNER JOIN CP_M_SubCategory SC WITH(NOLOCK) ON SC.Id = CM.SubCategoryId
		INNER JOIN CP_M_Region R WITH(NOLOCK) ON R.Id = CM.RegionId
		LEFT JOIN [dbo].[FreshService_M_Department_CustomFields] D WITH (NOLOCK) ON D.embee_crm_id = CM.CustomerId AND D.department_id = CM.DepartmentId
        WHERE
            (@ContractNo IS NULL OR ContractNo = @ContractNo)
            AND (@TenantId IS NULL OR TenantId = @TenantId)
            AND (@TenantName IS NULL OR TenantName LIKE '%' + @TenantName + '%')
            AND (@CustomerId IS NULL OR CustomerId LIKE '%' + @CustomerId + '%')
            AND (@CustomerName IS NULL OR CustomerName LIKE '%' + @CustomerName + '%')
			AND (@DepartmentId IS NULL OR DepartmentId = @DepartmentId)
            AND (@DepartmentName IS NULL OR DepartmentName LIKE '%' + @DepartmentName + '%')
            AND (@CategoryId IS NULL OR CM.CategoryId = @CategoryId)
            AND (@SubCategoryId IS NULL OR SubCategoryId = @SubCategoryId)
            AND (@StartDate IS NULL OR StartDate >= @StartDate)
            AND (@EndDate IS NULL OR EndDate <= @EndDate)
            AND (@RegionId IS NULL OR RegionId = @RegionId)
            AND (@Acc_ManagerName IS NULL OR Acc_ManagerName LIKE '%' + @Acc_ManagerName + '%')
            AND (@Acc_ManagerEmail IS NULL OR Acc_ManagerEmail LIKE '%' + @Acc_ManagerEmail + '%')
            AND (@PONo IS NULL OR PONo LIKE '%' + @PONo + '%')
            AND (@Active IS NULL OR CM.Active = @Active)
			AND (@CreatedByEmail IS NULL OR CreatedByEmail = @CreatedByEmail);
    END
    ELSE
    BEGIN
        -- Get the specific row from CP_T_ContractMaster by Id
        SELECT
			ROW_NUMBER() OVER (ORDER BY CM.CreatedOnUTC ASC) AS 'SlNo',
            CM.Id,
			TRY_CAST(CM.ReferenceNo AS VARCHAR(255)) AS 'ReferenceNo',
            ContractNo,
            TenantId,
            TenantName,
            CustomerId,
            CustomerName,
			DepartmentId,
            DepartmentName,
            CM.CategoryId,
			C.[Name] AS 'CategoryName',
			C.[Code] AS 'CategoryCode',
            CM.SubCategoryId,
			SC.[Name] AS 'SubCategoryName',
			SC.[Code] AS 'SubCategoryCode',
            StartDate,
            EndDate,
            CM.RegionId,
			R.RegionCode,
			R.RegionName,
            Acc_ManagerName,
            Acc_ManagerEmail,
			PONo,
            CM.Active,
			CM.CreatedByName,
			CM.CreatedByEmail,
			CAST(SWITCHOFFSET(CM.CreatedOnUTC, '+05:30') AS DATETIME) AS 'CreatedOn',
			CM.ModifiedByName,
			CM.ModifiedEmail,
			CAST(SWITCHOFFSET(CM.ModifiedOnUTC, '+05:30') AS DATETIME) AS 'ModifiedOn',
			D.contact_person AS 'ContactPersonName',
			D.contact_email_id AS 'ContactPersonEmail',
			D.contact_number AS 'ContactPersonPhone',
			CASE
				WHEN CM.StartDate >= @CurrentIST THEN 1
				WHEN @CurrentIST BETWEEN CM.StartDate AND CM.EndDate THEN 2
				WHEN @CurrentIST >= CM.EndDate AND ISNULL(ExtendSupport,0) <> 1 THEN 3
				WHEN @CurrentIST >= CM.EndDate AND ISNULL(ExtendSupport,0) = 1 THEN 4
				ELSE NULL
			END AS 'ActiveStatusId',
			CASE
				WHEN CM.StartDate >= @CurrentIST THEN 'Upcoming' 
				WHEN @CurrentIST BETWEEN CM.StartDate AND CM.EndDate THEN 'Active'
				WHEN @CurrentIST >= CM.EndDate AND ISNULL(ExtendSupport,0) <> 1 THEN 'Inactive'
				WHEN @CurrentIST >= CM.EndDate AND ISNULL(ExtendSupport,0) = 1 THEN 'Active - Extend Support'
				ELSE ''
			END AS 'ActiveStatus',
			CM.ExtendSupport
        FROM CP_T_ContractMaster CM WITH(NOLOCK)
		INNER JOIN CP_M_Category C WITH(NOLOCK) ON C.Id = CM.CategoryId
		INNER JOIN CP_M_SubCategory SC WITH(NOLOCK) ON SC.Id = CM.SubCategoryId
		INNER JOIN CP_M_Region R WITH(NOLOCK) ON R.Id = CM.RegionId
		LEFT JOIN [dbo].[FreshService_M_Department_CustomFields] D WITH (NOLOCK) ON D.embee_crm_id = CM.CustomerId AND D.department_id = CM.DepartmentId
        WHERE CM.Id = @Id;

        -- Get related rows from CP_T_ContractMasterFiles
        SELECT
            Id,
            ContractId,
            [Name],
            InternalName,
            ContentType,
            [Url],
            PhysicalPath,
            Active,
			CreatedByName,
			CreatedByEmail,
            CAST(SWITCHOFFSET(CreatedOnUTC, '+05:30') AS DATETIME) AS 'CreatedOn',
			ModifiedByName,
			ModifiedEmail,
            CAST(SWITCHOFFSET(ModifiedOnUTC, '+05:30') AS DATETIME) AS 'ModifiedOn'
        FROM CP_T_ContractMasterFiles WITH(NOLOCK)
        WHERE ContractId = @Id;
    END

END;
GO
/****** Object:  StoredProcedure [dbo].[usp_CP_T_ContractMaster_InsertUpdate]    Script Date: 09/06/2025 10:37:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_CP_T_ContractMaster_InsertUpdate]
(
    @TransactionType VARCHAR(10),

    @Id BIGINT = NULL,
    @ContractNo VARCHAR(255) = NULL,
    @TenantId INT = NULL,
    @TenantName VARCHAR(255) = NULL,
    @CustomerId VARCHAR(255) = NULL,
    @CustomerName VARCHAR(255) = NULL,
	@DepartmentId BIGINT = NULL,
    @DepartmentName VARCHAR(255) = NULL,
    @StartDate DATETIME = NULL,
    @EndDate DATETIME = NULL,
    @Acc_ManagerName VARCHAR(255) = NULL,
    @Acc_ManagerEmail VARCHAR(255) = NULL,
	@PONo VARCHAR(255) = NULL,
	@CategoryId INT = NULL,
	@SubCategoryId INT = NULL,
	@RegionId INT = NULL,
    @Active BIT = NULL,

	@CreatedByName VARCHAR(150) = NULL,
	@CreatedByEmail VARCHAR(150) = NULL,
	@ModifiedByName VARCHAR(150) = NULL,
	@ModifiedEmail VARCHAR(150) = NULL,

	@ExtendSupport BIT = NULL,

    @FilesJSONInput NVARCHAR(MAX) = NULL
)
AS
BEGIN
    SET NOCOUNT ON;

	DECLARE @IsFinancialYear BIT = 0 -- Flag to determine if financial year should be considered
	DECLARE @UseFinancialYearInContractNo BIT = 0 -- Flag to decide whether to use financial year or start date year in Contract No

	DECLARE @ReferenceStartDate DATETIME;
	DECLARE @ReferenceEndDate DATETIME;

	IF @IsFinancialYear = 1
	BEGIN
		SET @ReferenceStartDate = DATEFROMPARTS(YEAR(DATEADD(MONTH, -3, @StartDate)), 4, 1);
		SET @ReferenceEndDate = DATEFROMPARTS(YEAR(@ReferenceStartDate) + 1, 3, 31);
	END
	ELSE
	BEGIN
		SET @ReferenceStartDate = DATEFROMPARTS(YEAR(@StartDate), 1, 1);
		SET @ReferenceEndDate = DATEFROMPARTS(YEAR(@StartDate), 12, 31);
	END

	DECLARE @YearForContractNo INT;
    IF @UseFinancialYearInContractNo = 1
        SET @YearForContractNo = YEAR(@ReferenceStartDate);
    ELSE
        SET @YearForContractNo = YEAR(@StartDate);


    IF @TransactionType = 'I'
    BEGIN

		SELECT TOP 1 @Id = Id, @ContractNo = ContractNo
		FROM CP_T_ContractMaster
		WHERE TenantId = @TenantId
		  AND DepartmentId = @DepartmentId
		  AND CategoryId = @CategoryId
		  AND SubCategoryId = @SubCategoryId
		  AND (
			  (@StartDate >= StartDate AND @StartDate < EndDate) -- This checks if the StartDate of the new contract is inside an existing contract's date range.
			  OR (@EndDate > StartDate AND @EndDate <= EndDate) -- This checks if the EndDate of the new contract is inside an existing contract's date range.
			  OR (StartDate >= @StartDate AND EndDate <= @EndDate) -- This checks if the existing contract is completely within the new contract's date range.
			  OR (StartDate <= @StartDate AND EndDate >= @EndDate) -- This checks if the new contract is completely within an existing contract's date range.
		  )
		ORDER BY StartDate DESC; -- Ensures the latest contract is checked first

		IF (@Id IS NOT NULL AND @Id > 0 AND @ContractNo IS NOT NULL)
		BEGIN
			SELECT 
				'A contract with similar details already exists. Please refer Contract no. '+@ContractNo AS [Message],
				''																						AS ErrorMessage,
				0																						AS [Status],
				@Id																						AS Id,
				@ContractNo																				AS ReferenceNo
			RETURN
		END


        BEGIN TRANSACTION;
        BEGIN TRY

		-- Insert into CP_T_ContractMaster
        INSERT INTO CP_T_ContractMaster
            (
				ReferenceNo,
				ContractNo,
				TenantId,
				TenantName,
				CustomerId,
				CustomerName,
				DepartmentId,
				DepartmentName,
				CategoryId,
				SubCategoryId,
				StartDate,
				EndDate,
				RegionId,
				Acc_ManagerName,
				Acc_ManagerEmail,
				PONo,
				Active,
				CreatedOnUTC,
				StartMonth,
				StartYear,
				EndMonth,
				EndYear,
				CreatedByName,
				CreatedByEmail
            )
        VALUES
            (
				NEWID(),
                @ContractNo,
                @TenantId,
                @TenantName,
                @CustomerId,
                @CustomerName,
				@DepartmentId,
                @DepartmentName,
                @CategoryId,
                @SubCategoryId,
                @StartDate,
                @EndDate,
                @RegionId,
                @Acc_ManagerName,
                @Acc_ManagerEmail,
				@PONo,
                @Active,
                CAST(SWITCHOFFSET(GETUTCDATE(), '+05:30') AS DATETIME),
				RIGHT('0' + CAST(MONTH(@StartDate) AS VARCHAR(2)), 2),
				YEAR(@StartDate),
				RIGHT('0' + CAST(MONTH(@EndDate) AS VARCHAR(2)), 2),
				YEAR(@EndDate),
				@CreatedByName,
				@CreatedByEmail
		);

        SET @Id = SCOPE_IDENTITY();


		-- GENERATE ContractNo START

		DECLARE @CategoryCode VARCHAR(50);
		DECLARE @SubCategoryCode VARCHAR(50);
		DECLARE @TenantCode VARCHAR(50);
		DECLARE @StartMonth VARCHAR(2);
		DECLARE @StartYear INT;
		DECLARE @NextSequence INT;
		DECLARE @NextSequenceStr VARCHAR(3);

		-- Get CategoryCode and SubCategoryCode
		SELECT @CategoryCode = Code FROM CP_M_Category WHERE Id = @CategoryId;
		SELECT @SubCategoryCode = Code FROM CP_M_SubCategory WHERE Id = @SubCategoryId;

		-- Get TenantCode
		SELECT @TenantCode = TenantCode FROM CP_M_Tenant WHERE Id = @TenantId;

		-- Extract StartMonth and StartYear
		SET @StartMonth = RIGHT('0' + CAST(MONTH(@StartDate) AS VARCHAR(2)), 2);
		SET @StartYear = YEAR(@StartDate);

		-- Get Next 3-digit Sequence Number
		SELECT @NextSequence = ISNULL(MAX(CAST(RIGHT(ContractNo, 3) AS INT)), 0) + 1
		FROM CP_T_ContractMaster
		WHERE CategoryId = @CategoryId
		  AND SubCategoryId = @SubCategoryId
		  AND StartDate BETWEEN @ReferenceStartDate AND @ReferenceEndDate
		  AND TenantId = @TenantId;

		-- Format sequence as 3-digit number
		SET @NextSequenceStr = RIGHT('000' + CAST(@NextSequence AS VARCHAR(3)), 3);

		-- Construct the ContractNo
		SET @ContractNo = @CategoryCode + @SubCategoryCode + '-' +
						  @StartMonth + CAST(@YearForContractNo AS VARCHAR) + '-' +
						  @TenantCode + '-' + @NextSequenceStr;

		-- Update ContractNo in the inserted record
		UPDATE CP_T_ContractMaster
		SET ContractNo = @ContractNo
		WHERE Id = @Id;

		-- GENERATE ContractNo END


        -- Insert into CP_T_ContractMasterFiles from JSON input
    --    IF @FilesJSONInput IS NOT NULL
    --    BEGIN
    --        INSERT INTO CP_T_ContractMasterFiles
    --            (
				--	ContractId,
				--	[Name],
				--	InternalName,
				--	ContentType,
				--	[Url],
				--	PhysicalPath,
				--	Active,
				--	CreatedOnUTC,
				--	CreatedByName,
				--	CreatedByEmail
    --            )
    --        SELECT
    --            @Id AS ContractId,
    --            JSON_VALUE(value, '$.Name') AS [Name],
    --            JSON_VALUE(value, '$.InternalName') AS InternalName,
    --            JSON_VALUE(value, '$.ContentType') AS ContentType,
    --            JSON_VALUE(value, '$.Url') AS [Url],
    --            JSON_VALUE(value, '$.PhysicalPath') AS PhysicalPath,
    --            JSON_VALUE(value, '$.Active') AS Active,
    --            CAST(SWITCHOFFSET(GETUTCDATE(), '+05:30') AS DATETIME) AS CreatedOnUTC,
				--@CreatedByName,
				--@CreatedByEmail
    --        FROM OPENJSON(@FilesJSONInput);
    --    END


		END TRY
		BEGIN CATCH
			IF @@TRANCOUNT > 0
				ROLLBACK TRANSACTION;

			SELECT
            'There was an error processing your request.'					AS [Message],
            ERROR_MESSAGE()													AS ErrorMessage,
            0																AS [Status],
            0																AS Id,
            0																AS ReferenceNo
			RETURN
		END CATCH

        IF @@TRANCOUNT > 0
        COMMIT TRANSACTION;

        SELECT
            'Contract created successfully!'									AS [Message],
            ''																AS ErrorMessage,
            1																AS [Status],
            @Id																AS Id,
            @ContractNo														AS ReferenceNo
    END

    ELSE IF @TransactionType = 'U'
    BEGIN

		DECLARE @ExistingId INT = NULL;
		DECLARE @ExistingContractNo VARCHAR(255) = NULL;
		DECLARE @IsActive BIT = NULL;

		SELECT TOP 1 @ExistingId = Id, @ExistingContractNo = ContractNo 
		FROM CP_T_ContractMaster WITH(NOLOCK)
		WHERE TenantId = @TenantId
		  AND DepartmentId = @DepartmentId
		  AND CategoryId = @CategoryId
		  AND SubCategoryId = @SubCategoryId
		  AND Id <> @Id
		  AND (@StartDate BETWEEN StartDate AND EndDate 
			   OR @EndDate BETWEEN StartDate AND EndDate);

		IF (@ExistingId IS NOT NULL AND @ExistingContractNo IS NOT NULL)
		BEGIN
			SELECT 
				'A contract with similar details already exists. Please refer Contract no. '+@ContractNo AS [Message],
				''																						AS ErrorMessage,
				0																						AS [Status],
				@Id																						AS Id,
				@ContractNo																				AS ReferenceNo
			RETURN
		END


		--DECLARE @CurrentIST DATETIME = SWITCHOFFSET(GETUTCDATE(), '+05:30');
		--SELECT @IsActive = CASE 
		--	WHEN @CurrentIST BETWEEN StartDate AND EndDate 
		--	THEN 1 ELSE 0 
		--END
		--FROM CP_T_ContractMaster WITH(NOLOCK) 
		--WHERE Id = @Id;
		--IF (@ExistingId IS NOT NULL AND @ExistingContractNo IS NOT NULL)
		--BEGIN
		--	SELECT 
		--		'A contract with these details already exists. Please refer Contract no. '+@ContractNo AS [Message],
		--		''																						AS ErrorMessage,
		--		0																						AS [Status],
		--		@Id																						AS Id,
		--		@ContractNo																				AS ReferenceNo
		--	RETURN
		--END


        BEGIN TRANSACTION;
        BEGIN TRY

        -- Update CP_T_ContractMaster
        UPDATE CP_T_ContractMaster
        SET
			--ReferenceNo = NEWID(),
            --ContractNo = @ContractNo,
			TenantId = @TenantId,
            TenantName = @TenantName,
            CustomerId = @CustomerId,
            CustomerName = @CustomerName,
			DepartmentId = @DepartmentId,
            DepartmentName = @DepartmentName,
            CategoryId = @CategoryId,
            SubCategoryId = @SubCategoryId,
            StartDate = @StartDate,
            EndDate = @EndDate,
            RegionId = @RegionId,
            Acc_ManagerName = @Acc_ManagerName,
            Acc_ManagerEmail = @Acc_ManagerEmail,
			PONo = @PONo,
            Active = @Active,
            ModifiedOnUTC = CAST(SWITCHOFFSET(GETUTCDATE(), '+05:30') AS DATETIME),
			StartMonth = RIGHT('0' + CAST(MONTH(@StartDate) AS VARCHAR(2)), 2),
			StartYear = YEAR(@StartDate),
			EndMonth = RIGHT('0' + CAST(MONTH(@EndDate) AS VARCHAR(2)), 2),
			EndYear = YEAR(@EndDate),
			ModifiedByName = @ModifiedByName,
			ModifiedEmail = @ModifiedEmail
        WHERE Id = @Id;

        -- Insert updated files from JSON input
        IF @FilesJSONInput IS NOT NULL
        BEGIN
    --        INSERT INTO CP_T_ContractMasterFiles
    --            (
				--	ContractId,
				--	[Name],
				--	InternalName,
				--	ContentType,
				--	[Url],
				--	PhysicalPath,
				--	Active,
				--	CreatedOnUTC,
				--	CreatedByName,
				--	CreatedByEmail
    --            )
    --        SELECT
    --            @Id AS ContractId,
    --            JSON_VALUE(value, '$.Name') AS [Name],
    --            JSON_VALUE(value, '$.InternalName') AS InternalName,
    --            JSON_VALUE(value, '$.ContentType') AS ContentType,
    --            JSON_VALUE(value, '$.Url') AS [Url],
    --            JSON_VALUE(value, '$.PhysicalPath') AS PhysicalPath,
    --            JSON_VALUE(value, '$.Active') AS Active,
    --            CAST(SWITCHOFFSET(GETUTCDATE(), '+05:30') AS DATETIME) AS CreatedOnUTC,
				--@CreatedByName,
				--@CreatedByEmail
    --        FROM OPENJSON(@FilesJSONInput)
    --        WHERE JSON_VALUE(value, '$.Id') = 0
				--AND JSON_VALUE(value, '$.Active') = 1;

            UPDATE F
            SET
            F.Active = 0,
            F.ModifiedOnUTC = CAST(SWITCHOFFSET(GETUTCDATE(), '+05:30') AS DATETIME),
			ModifiedByName = @ModifiedByName,
			ModifiedEmail = @ModifiedEmail
            FROM CP_T_ContractMasterFiles F
                INNER JOIN OPENJSON(@FilesJSONInput) J ON F.Id = JSON_VALUE(J.value, '$.Id')
            WHERE JSON_VALUE(J.value, '$.Id') > 0
                AND JSON_VALUE(J.value, '$.Active') = 0;
        END

		END TRY
		BEGIN CATCH
			IF @@TRANCOUNT > 0
				ROLLBACK TRANSACTION;

			SELECT
            'There was an error processing your request.'					AS [Message],
            ERROR_MESSAGE()													AS ErrorMessage,
            0																AS [Status],
            0																AS Id,
            0																AS ReferenceNo
			RETURN
		END CATCH

        IF @@TRANCOUNT > 0
        COMMIT TRANSACTION;

        SELECT
            'Contract successfully updated!'								AS [Message],
            ''																AS ErrorMessage,
            1																AS [Status],
            @Id																AS Id,
            @ContractNo														AS ReferenceNo
    END
	ELSE IF @TransactionType = 'FILES'
    BEGIN
        BEGIN TRANSACTION;
        BEGIN TRY

        -- Insert into CP_T_ContractMasterFiles from JSON input
        IF @FilesJSONInput IS NOT NULL
        BEGIN
            INSERT INTO CP_T_ContractMasterFiles
                (
					ContractId,
					[Name],
					InternalName,
					ContentType,
					[Url],
					PhysicalPath,
					Active,
					CreatedOnUTC,
					CreatedByName,
					CreatedByEmail
                )
            SELECT
                @Id AS ContractId,
                JSON_VALUE(value, '$.Name') AS [Name],
                JSON_VALUE(value, '$.InternalName') AS InternalName,
                JSON_VALUE(value, '$.ContentType') AS ContentType,
                JSON_VALUE(value, '$.Url') AS [Url],
                JSON_VALUE(value, '$.PhysicalPath') AS PhysicalPath,
                JSON_VALUE(value, '$.Active') AS Active,
                CAST(SWITCHOFFSET(GETUTCDATE(), '+05:30') AS DATETIME) AS CreatedOnUTC,
				@CreatedByName,
				@CreatedByEmail
            FROM OPENJSON(@FilesJSONInput);
        END

		END TRY
		BEGIN CATCH
			IF @@TRANCOUNT > 0
				ROLLBACK TRANSACTION;

			SELECT
            'There was an error processing your request.'					AS [Message],
            ERROR_MESSAGE()													AS ErrorMessage,
            0																AS [Status],
            0																AS Id,
            0																AS ReferenceNo
			RETURN
		END CATCH

        IF @@TRANCOUNT > 0
        COMMIT TRANSACTION;

        SELECT
            'Contract successfully updated!'								AS [Message],
            ''																AS ErrorMessage,
            1																AS [Status],
            @Id																AS Id,
            @ContractNo														AS ReferenceNo
    END
	ELSE IF @TransactionType = 'E'
    BEGIN

		DECLARE @UpdateId BIGINT = NULL;

		SELECT TOP 1 @UpdateId = Id
		FROM CP_T_ContractMaster WITH(NOLOCK)
		WHERE Id = @Id;

		IF (@UpdateId IS NULL)
		BEGIN
			SELECT 
				'Something went wrong, Unable to find contract details'									AS [Message],
				''																						AS ErrorMessage,
				0																						AS [Status],
				@Id																						AS Id,
				''																						AS ReferenceNo
			RETURN
		END


        BEGIN TRANSACTION;
        BEGIN TRY

        -- Update CP_T_ContractMaster
        UPDATE CP_T_ContractMaster
        SET
			ExtendSupport = @ExtendSupport,
            ModifiedOnUTC = CAST(SWITCHOFFSET(GETUTCDATE(), '+05:30') AS DATETIME),
			ModifiedByName = @ModifiedByName,
			ModifiedEmail = @ModifiedEmail
        WHERE Id = @Id;

		DECLARE @UpdateMessage VARCHAR(150) = 'Contract updated successfully!';
		IF(@ExtendSupport = 1)
		BEGIN
			SET @UpdateMessage = 'Extended support has been activated';
		END
		ELSE IF(@ExtendSupport = 0)
		BEGIN
			SET @UpdateMessage = 'Extended support has been deactivated';
		END

		END TRY
		BEGIN CATCH
			IF @@TRANCOUNT > 0
				ROLLBACK TRANSACTION;

			SELECT
            'There was an error processing your request.'					AS [Message],
            ERROR_MESSAGE()													AS ErrorMessage,
            0																AS [Status],
            0																AS Id,
            0																AS ReferenceNo
			RETURN
		END CATCH

        IF @@TRANCOUNT > 0
        COMMIT TRANSACTION;

        SELECT
            @UpdateMessage													AS [Message],
            ''																AS ErrorMessage,
            1																AS [Status],
            @Id																AS Id,
            ''																AS ReferenceNo
    END

END;
GO
/****** Object:  StoredProcedure [dbo].[usp_CP_T_EmailLog_InsertUpdate]    Script Date: 09/06/2025 10:37:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_CP_T_EmailLog_InsertUpdate]
(
    @From VARCHAR(255) = NULL,
    @To VARCHAR(255) = NULL,
    @CC VARCHAR(255) = NULL,
    @Subject NVARCHAR(255) = NULL,
    @Body NVARCHAR(MAX) = NULL,
    @Status BIT = 0,
    @Type VARCHAR(50) = NULL,
    @Message VARCHAR(255) = NULL,
    @CreatedOn DATETIME = NULL,
    @ReferenceNo VARCHAR(255) = NULL,
    @OTP_Id INT = NULL,
	@SessionId VARCHAR(255) = NULL
)
AS
BEGIN
    SET NOCOUNT ON;
	DECLARE @NewItemId	BIGINT = 0;

	BEGIN TRANSACTION;
    BEGIN TRY

		INSERT INTO CP_T_EmailLog
		(
			[From],
			[To],
			CC,
			[Subject],
			Body,
			[Status],
			[Type],
			[Message],
			CreatedOn,
			ReferenceNo,
			OTP_Id,
			SessionId
		)
		VALUES
		(
			@From, 
			@To, 
			@CC, 
			@Subject, 
			@Body, 
			@Status, 
			@Type, 
			@Message, 
			@CreatedOn, 
			@ReferenceNo, 
			@OTP_Id,
			@SessionId
		);

		SET @NewItemId = SCOPE_IDENTITY();

    END TRY
		BEGIN CATCH
			IF @@TRANCOUNT > 0
				ROLLBACK TRANSACTION;

			SELECT
            'DB execution failed'											AS [Message],
            ERROR_MESSAGE()													AS ErrorMessage,
            0																AS [Status],
            0																AS Id,
            0																AS ReferenceNo
			RETURN
		END CATCH

        IF @@TRANCOUNT > 0
        COMMIT TRANSACTION;

        SELECT
            'DB execution successful'										AS [Message],
            ''																AS ErrorMessage,
            1																AS [Status],
            @NewItemId														AS Id,
            @ReferenceNo													AS ReferenceNo
END;
GO
/****** Object:  StoredProcedure [dbo].[usp_CP_T_FreshService_CustomerDetails_Get]    Script Date: 09/06/2025 10:37:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_CP_T_FreshService_CustomerDetails_Get]
(
	@customerEmail VARCHAR(255) = NULL,
	@department_id BIGINT = NULL,
	@tenant VARCHAR(255) = NULL,
	@embee_crm_id VARCHAR(255) = NULL,
	@engagement_start_date DATETIME = NULL,
	@engagement_end_date DATETIME = NULL,
	@customer_portal_access VARCHAR(255) = NULL,
	@sap_customer_name VARCHAR(255) = NULL
)
AS
BEGIN
    SET NOCOUNT ON;

	SELECT Req.[id] AS 'CustomerId'
		,Dept.[id] AS 'department_id'
		,Dept.[name] AS 'department_name'
		,CustomDept.[tenant]
		,CustomDept.[embee_crm_id]
		,CustomDept.[customer_portal_access]
		,CustomDept.[sap_customer_name]
		,[active]
		,[address] AS 'CustomerAddress'
		,LTRIM(RTRIM(([first_name] + ' ' + ISNULL([last_name], '')))) AS 'CustomerName'
		,[external_id]
		,[first_name]
		,[is_agent]
		,[job_title]
		,[language]
		,[last_name]
		,[location_id]
		,[location_name]
		,[mobile_phone_number] AS 'CustomerPhone'
		,[primary_email] AS 'CustomerEmail'
		,[reporting_manager_id]
		,[time_format]
		,[time_zone]
		,[vip_user]
		,[work_phone_number]
		,[work_schedule_id]
		,[employee_id]
		,CustomDept.[embee_account_manager]
		,CustomDept.[engagement_start_date]
		,CustomDept.[engagement_end_date]
	FROM [dbo].[FreshService_M_Requesters] Req WITH (NOLOCK)
		INNER JOIN [dbo].[FreshService_M_Requester_Departments] ReqDept WITH (NOLOCK) ON ReqDept.requester_id = Req.id
		INNER JOIN [dbo].[FreshService_M_Department] Dept WITH (NOLOCK) ON ReqDept.department_id = Dept.id
		INNER JOIN [dbo].[FreshService_M_Department_CustomFields] CustomDept WITH (NOLOCK) ON ReqDept.department_id = CustomDept.department_id

	WHERE TRIM(Req.primary_email) = TRIM(@customerEmail)
	AND Req.active = 1
	AND COALESCE(CustomDept.[customer_portal_access], 'false') = 'true'
	AND (@tenant IS NULL OR CustomDept.tenant = @tenant)
	AND (@embee_crm_id IS NULL OR CustomDept.embee_crm_id = @embee_crm_id)
	AND (@engagement_start_date IS NULL OR CustomDept.engagement_start_date >= @engagement_start_date)
	AND (@engagement_end_date IS NULL OR CustomDept.engagement_end_date <= @engagement_end_date)
	AND (@sap_customer_name IS NULL OR CustomDept.sap_customer_name LIKE @sap_customer_name + '%');
		

	IF(ISNULL(@embee_crm_id, '') != '' AND ISNULL(@department_id, '') != '')
	BEGIN
		DECLARE @CurrentIST DATETIME = SWITCHOFFSET(GETUTCDATE(), '+05:30');

		-- Get the specific row from CP_T_ContractMaster by Id
		SELECT
			ROW_NUMBER() OVER (ORDER BY CM.CustomerName, CM.DepartmentName, CM.TenantName, C.[Name], SC.[Name]) AS 'SlNo',
            CM.Id,
			TRY_CAST(CM.ReferenceNo AS VARCHAR(255)) AS 'ReferenceNo',
            ContractNo,
            TenantId,
            TenantName,
            CustomerId,
            CustomerName,
			DepartmentId,
            DepartmentName,
            CM.CategoryId,
			C.[Name] AS 'CategoryName',
			C.[Code] AS 'CategoryCode',
            CM.SubCategoryId,
			SC.[Name] AS 'SubCategoryName',
			SC.[Code] AS 'SubCategoryCode',
            StartDate,
            EndDate,
            CM.RegionId,
			R.RegionCode,
			R.RegionName,
            Acc_ManagerName,
            Acc_ManagerEmail,
			PONo,
            CM.Active,
			CM.CreatedByName,
			CM.CreatedByEmail,
			CAST(SWITCHOFFSET(CM.CreatedOnUTC, '+05:30') AS DATETIME) AS 'CreatedOn',
			CM.ModifiedByName,
			CM.ModifiedEmail,
			CAST(SWITCHOFFSET(CM.ModifiedOnUTC, '+05:30') AS DATETIME) AS 'ModifiedOn',
			D.contact_person AS 'ContactPersonName',
			D.contact_email_id AS 'ContactPersonEmail',
			D.contact_number AS 'ContactPersonPhone',
			CASE
				WHEN CM.StartDate >= @CurrentIST THEN 1
				WHEN @CurrentIST BETWEEN CM.StartDate AND CM.EndDate THEN 2
				WHEN @CurrentIST >= CM.EndDate AND ISNULL(ExtendSupport,0) <> 1 THEN 3
				WHEN @CurrentIST >= CM.EndDate AND ISNULL(ExtendSupport,0) = 1 THEN 4
				ELSE NULL
			END AS 'ActiveStatusId',
			CASE
				WHEN CM.StartDate >= @CurrentIST THEN 'Upcoming' 
				WHEN @CurrentIST BETWEEN CM.StartDate AND CM.EndDate THEN 'Active'
				WHEN @CurrentIST >= CM.EndDate AND ISNULL(ExtendSupport,0) <> 1 THEN 'Inactive'
				WHEN @CurrentIST >= CM.EndDate AND ISNULL(ExtendSupport,0) = 1 THEN 'Active - Extend Support'
				ELSE ''
			END AS 'ActiveStatus',
			CM.ExtendSupport
        FROM CP_T_ContractMaster CM WITH(NOLOCK)
			INNER JOIN CP_M_Category C WITH(NOLOCK) ON C.Id = CM.CategoryId
			INNER JOIN CP_M_SubCategory SC WITH(NOLOCK) ON SC.Id = CM.SubCategoryId
			INNER JOIN CP_M_Region R WITH(NOLOCK) ON R.Id = CM.RegionId
			LEFT JOIN [dbo].[FreshService_M_Department_CustomFields] D WITH (NOLOCK) ON D.embee_crm_id = CM.CustomerId AND D.department_id = CM.DepartmentId
		WHERE (@embee_crm_id IS NULL OR CM.CustomerId = @embee_crm_id)
			AND (@department_id IS NULL OR CM.DepartmentId = @department_id)
			AND @CurrentIST BETWEEN CM.StartDate AND CM.EndDate;
	END

END;
GO
/****** Object:  StoredProcedure [dbo].[usp_CP_T_FreshServiceTickets_Get]    Script Date: 09/06/2025 10:37:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_CP_T_FreshServiceTickets_Get]
 (
	@PageNumber INT = 1,
    @PageSize INT = 10,
	@TransactionType VARCHAR(25) = 'List',
	@DepartmentIds VARCHAR(500) = NULL,
	@TicketIds VARCHAR(500) = NULL,
	@Start_Date VARCHAR(10) = NULL,
	@End_Date VARCHAR(10) = NULL,
	@StatusId INT = NULL,
	@TimePeriod VARCHAR(10) = 'Monthly'
 )
 AS
 BEGIN
	SET NOCOUNT ON;

	--DECLARE @DepartmentIdTable TABLE (DepartmentId BIGINT);
	--INSERT INTO @DepartmentIdTable (DepartmentId)
	--SELECT value FROM STRING_SPLIT(@DepartmentIds, ',');

	DECLARE @LastDateTobeConsidered DATETIME = (SELECT TOP 1 LastDate FROM LastDateTobeConsidered WHERE ACTIVE = 1);

	-- Department Temp Table
	CREATE TABLE #FreshServiceTickets_Get_DepartmentIdTable (DepartmentId BIGINT);

	IF @DepartmentIds IS NOT NULL
	BEGIN
		INSERT INTO #FreshServiceTickets_Get_DepartmentIdTable (DepartmentId)
		SELECT value 
		FROM STRING_SPLIT(@DepartmentIds, ',');
    
		-- index for performance
		CREATE INDEX idx_DepartmentId ON #FreshServiceTickets_Get_DepartmentIdTable(DepartmentId);
	END

	
	-- Count
	IF(@TransactionType = 'Count')
	BEGIN

		SELECT 
			COUNT(T.id) AS TotalTickets,  -- Total Tickets
			ISNULL(SUM(CASE WHEN T.[status] = 5 THEN 1 ELSE 0 END), 0) AS ClosedTickets,  -- Closed (Status = 5)
			ISNULL(SUM(CASE WHEN T.[status] = 2 THEN 1 ELSE 0 END), 0) AS OpenTickets,    -- Open (Status = 2)
			ISNULL(SUM(CASE WHEN T.fr_escalated = 1 THEN 1 ELSE 0 END), 0) AS ResponseViolated,    -- (fr_escalated = 1)
			ISNULL(SUM(CASE WHEN T.is_escalated = 1 THEN 1 ELSE 0 END), 0) AS ResolutionViolated    -- (is_escalated = 1)

		FROM [FreshService_T_Tickets] T WITH(NOLOCK)
		INNER JOIN #FreshServiceTickets_Get_DepartmentIdTable DIT ON T.department_id = DIT.DepartmentId

		WHERE 
			(
				(@Start_Date IS NULL OR CAST(SWITCHOFFSET(CAST(T.created_at AS DATETIMEOFFSET), '+05:30') AS DATE) 
				>= CONVERT(DATE, @Start_Date, 103))
			AND 
				(@End_Date IS NULL OR CAST(SWITCHOFFSET(CAST(T.created_at AS DATETIMEOFFSET), '+05:30') AS DATE) 
				<= CONVERT(DATE, @End_Date, 103))
			)
			AND (CAST(SWITCHOFFSET(CAST(T.created_at AS DATETIMEOFFSET), '+05:30') AS DATETIME) > @LastDateTobeConsidered OR ISNULL(@LastDateTobeConsidered,'')='');

	END


	-- List
	ELSE IF(@TransactionType = 'List')
	BEGIN

		-- 0FFSET
		DECLARE @Offset INT = (@PageNumber - 1) * @PageSize;

		IF(@PageNumber > 0 AND @PageSize > 0)
		BEGIN
			SET @Offset = (@PageNumber - 1) * @PageSize;
		END
		ELSE
		BEGIN
			SET @Offset = 0;
		END;


		-- Ticket Temp Table
		CREATE TABLE #FreshServiceTickets_Get_TicketIdTable (TicketId BIGINT);

		IF ISNULL(@TicketIds,'') != ''
		BEGIN
			INSERT INTO #FreshServiceTickets_Get_TicketIdTable (TicketId)
			SELECT value 
			FROM STRING_SPLIT(@TicketIds, ',');
    
			-- index for performance
			CREATE INDEX idx_TicketId ON #FreshServiceTickets_Get_TicketIdTable(TicketId);
		END
		ELSE SET @TicketIds = NULL


		SELECT COUNT(T.id) AS TotalRecords
		INTO #FreshServiceTickets_Get_TotalCount
		FROM [FreshService_T_Tickets] T WITH(NOLOCK)
		INNER JOIN #FreshServiceTickets_Get_DepartmentIdTable DIT ON T.department_id = DIT.DepartmentId
		LEFT JOIN #FreshServiceTickets_Get_TicketIdTable TIT ON T.id = TIT.TicketId --optional filter
		WHERE
			--CAST(SWITCHOFFSET(CAST(T.created_at AS DATETIMEOFFSET), '+05:30') AS DATETIME) 
			--BETWEEN CONVERT(DATETIME, @Start_Date, 103) AND CONVERT(DATETIME, @End_Date, 103)
			(
				(@Start_Date IS NULL OR CAST(SWITCHOFFSET(CAST(T.created_at AS DATETIMEOFFSET), '+05:30') AS DATE) 
				>= CONVERT(DATE, @Start_Date, 103))
			AND 
				(@End_Date IS NULL OR CAST(SWITCHOFFSET(CAST(T.created_at AS DATETIMEOFFSET), '+05:30') AS DATE) 
				<= CONVERT(DATE, @End_Date, 103))
			)
			AND (CAST(SWITCHOFFSET(CAST(T.created_at AS DATETIMEOFFSET), '+05:30') AS DATETIME) > @LastDateTobeConsidered OR ISNULL(@LastDateTobeConsidered,'')='')
			AND (@StatusId IS NULL OR T.[status] = @StatusId) --optional filter
			AND (@TicketIds IS NULL OR TIT.TicketId IS NOT NULL); --optional filter

		SELECT 
			ROW_NUMBER() OVER (ORDER BY T.created_at ASC) AS SlNo,
			T.id,
			D.[name], 
			T.department_id,
			T.category,
			T.sub_category,
			T.created_at,
			FORMAT(SWITCHOFFSET(CAST(T.created_at AS DATETIMEOFFSET), '+05:30'), 'dd-MM-yyyy HH:mm:ss') AS 'created_at_display',
			T.[type],
			T.[subject],
			T.[status],
			S.StatusName,
			TR.[email] AS RequesterEmail,
			TR.[name] as RequesterName,
			Tr.[mobile] as RequesterMobile,
			TC.[location],
			tc.tenant,
			TC.nsd_member_name,
			TC.on_roaster_engineer,
			TC.resolution_remarks,
			tc.resource_name,
			tc.oem_case_idif_any,
			T.[priority],
			p.[Name] as priorityname,
			CASE WHEN ISNULL(T.is_escalated,0)=1 THEN 'SLA Violated' ELSE 'Within SLA' END AS ResolutionStatus,
			CASE WHEN ISNULL(T.fr_escalated,0)=1 THEN 'SLA Violated' ELSE 'Within SLA' END AS ResponseStatus,
			FORMAT(SWITCHOFFSET(CAST(TS.resolved_at AS DATETIMEOFFSET), '+05:30'), 'dd-MM-yyyy HH:mm:ss') AS 'resolved_at_display',
			FORMAT(SWITCHOFFSET(CAST(TS.closed_at AS DATETIMEOFFSET), '+05:30'), 'dd-MM-yyyy HH:mm:ss') AS 'closed_at_display',
			FORMAT(SWITCHOFFSET(CAST(TS.status_updated_at AS DATETIMEOFFSET), '+05:30'), 'dd-MM-yyyy HH:mm:ss') AS 'status_updated_at_display',
			ts.first_resp_time_in_secs,
			ts.resolution_time_in_secs,
			FORMAT(SWITCHOFFSET(CAST(TS.first_assigned_at AS DATETIMEOFFSET), '+05:30'), 'dd-MM-yyyy HH:mm:ss') AS 'first_assigned_at_display',
			FORMAT(SWITCHOFFSET(CAST(TS.first_responded_at AS DATETIMEOFFSET), '+05:30'), 'dd-MM-yyyy HH:mm:ss') AS 'first_responded_at_display',
			FORMAT(SWITCHOFFSET(CAST(TS.assigned_at AS DATETIMEOFFSET), '+05:30'), 'dd-MM-yyyy HH:mm:ss') AS 'assigned_at_display'

		FROM [FreshService_T_Tickets] T WITH(NOLOCK)
		INNER JOIN #FreshServiceTickets_Get_DepartmentIdTable DIT ON T.department_id = DIT.DepartmentId
		INNER JOIN [FreshService_M_Department] D WITH(NOLOCK) ON T.department_id = D.id
		LEFT JOIN [FreshService_T_Ticket_CustomFields] TC WITH(NOLOCK) ON TC.ticket_id = T.id

		LEFT OUTER JOIN FreshService_T_TicketRequesters TR WITH(NOLOCK) ON TR.ticket_id = T.id
		LEFT OUTER JOIN FreshService_T_Ticket_Stats TS WITH(NOLOCK) ON TS.ticket_id = T.id
		LEFT OUTER JOIN [FreshService_M_Status] S WITH(NOLOCK) ON S.StatusId = T.[status]
		LEFT OUTER JOIN FreshService_M_Priority P WITH(NOLOCK) ON P.Id = T.[priority]

		LEFT JOIN #FreshServiceTickets_Get_TicketIdTable TIT ON T.id = TIT.TicketId --optional filter

		WHERE 
			--CAST(SWITCHOFFSET(CAST(T.created_at AS DATETIMEOFFSET), '+05:30') AS DATETIME) 
			--BETWEEN CONVERT(DATETIME, @Start_Date, 103) AND CONVERT(DATETIME, @End_Date, 103)
			(
				(@Start_Date IS NULL OR CAST(SWITCHOFFSET(CAST(T.created_at AS DATETIMEOFFSET), '+05:30') AS DATE) 
				>= CONVERT(DATE, @Start_Date, 103))
			AND 
				(@End_Date IS NULL OR CAST(SWITCHOFFSET(CAST(T.created_at AS DATETIMEOFFSET), '+05:30') AS DATE) 
				<= CONVERT(DATE, @End_Date, 103))
			)
			AND (CAST(SWITCHOFFSET(CAST(T.created_at AS DATETIMEOFFSET), '+05:30') AS DATETIME) > @LastDateTobeConsidered OR ISNULL(@LastDateTobeConsidered,'')='')
			AND (@StatusId IS NULL OR T.[status] = @StatusId) --optional filter
			AND (@TicketIds IS NULL OR TIT.TicketId IS NOT NULL) --optional filter

		ORDER BY T.created_at ASC

		OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;

		SELECT TotalRecords FROM #FreshServiceTickets_Get_TotalCount;

		-- Clear Temp Tables
        DROP TABLE #FreshServiceTickets_Get_TotalCount;
        DROP TABLE #FreshServiceTickets_Get_TicketIdTable;

	END


	-- Percenyage Change
	ELSE IF(@TransactionType = 'PercentageChange')
	BEGIN

		DECLARE @DateStart DATETIME, @DateEnd DATETIME, @PrevDateStart DATETIME, @PrevDateEnd DATETIME;
		--DECLARE @Today DATETIME = CAST(SWITCHOFFSET(CAST(GETUTCDATE() AS DATETIMEOFFSET), '+05:30') AS DATETIME);
		DECLARE @Today DATETIME = CAST(SWITCHOFFSET(CAST('2024-11-30T00:00:00Z' AS DATETIMEOFFSET), '+05:30') AS DATETIME);
		IF @TimePeriod = 'Weekly'
		BEGIN			

			SET @DateStart = DATEADD(WEEK, DATEDIFF(WEEK, 0, @Today), 0);
			SET @DateEnd = @Today;
        
			SET @PrevDateStart = DATEADD(WEEK, DATEDIFF(WEEK, 0, @Today) - 1, 0);
			SET @PrevDateEnd = DATEADD(WEEK, DATEDIFF(WEEK, 0, @Today) - 1, @Today);
		END
		ELSE IF @TimePeriod = 'Monthly'
		BEGIN

			SET @DateStart = DATEADD(MONTH, DATEDIFF(MONTH, 0, @Today), 0);
			SET @DateEnd = @Today;
        
			SET @PrevDateStart = DATEADD(MONTH, DATEDIFF(MONTH, 0, @Today) - 1, 0);
			SET @PrevDateEnd = DATEADD(MONTH, DATEDIFF(MONTH, 0, @Today) - 1, @Today);
		END
		ELSE IF @TimePeriod = 'Quarterly'
		BEGIN

			SET @DateStart = DATEADD(QUARTER, DATEDIFF(QUARTER, 0, @Today), 0);
			SET @DateEnd = @Today;
        
			SET @PrevDateStart = DATEADD(QUARTER, DATEDIFF(QUARTER, 0, @Today) - 1, 0);
			SET @PrevDateEnd = DATEADD(QUARTER, DATEDIFF(QUARTER, 0, @Today) - 1, @Today);
		END
		ELSE IF @TimePeriod = 'Yearly'
		BEGIN

			SET @DateStart = DATEADD(YEAR, DATEDIFF(YEAR, 0, @Today), 0);
			SET @DateEnd = @Today;
        
			SET @PrevDateStart = DATEADD(YEAR, DATEDIFF(YEAR, 0, @Today) - 1, 0);
			SET @PrevDateEnd = DATEADD(YEAR, DATEDIFF(YEAR, 0, @Today) - 1, @Today);
		END

		SELECT 
			COUNT(T.id) AS TotalTickets,  
			ISNULL(SUM(CASE WHEN T.[status] = 5 THEN 1 ELSE 0 END), 0) AS ClosedTickets,  
			ISNULL(SUM(CASE WHEN T.[status] <> 5 THEN 1 ELSE 0 END), 0) AS OpenTickets,    
			ISNULL(SUM(CASE WHEN T.fr_escalated = 1 THEN 1 ELSE 0 END), 0) AS ResponseViolated,    
			ISNULL(SUM(CASE WHEN T.is_escalated = 1 THEN 1 ELSE 0 END), 0) AS ResolutionViolated    
		INTO #FreshServiceTickets_Get_CurrentPeriodTickets
		FROM [FreshService_T_Tickets] T WITH(NOLOCK)
		INNER JOIN #FreshServiceTickets_Get_DepartmentIdTable DIT ON T.department_id = DIT.DepartmentId
		WHERE 
			(
				(@Start_Date IS NULL OR CAST(SWITCHOFFSET(CAST(T.created_at AS DATETIMEOFFSET), '+05:30') AS DATE) 
				>= CONVERT(DATE, @Start_Date, 103))
			AND 
				(@End_Date IS NULL OR CAST(SWITCHOFFSET(CAST(T.created_at AS DATETIMEOFFSET), '+05:30') AS DATE) 
				<= CONVERT(DATE, @End_Date, 103))
			)
			AND (CAST(SWITCHOFFSET(CAST(T.created_at AS DATETIMEOFFSET), '+05:30') AS DATETIME) > @LastDateTobeConsidered OR ISNULL(@LastDateTobeConsidered,'')='')
			AND T.created_at BETWEEN @DateStart AND @DateEnd;

		--SELECT * FROM #FreshServiceTickets_Get_CurrentPeriodTickets;

		SELECT 
			COUNT(T.id) AS TotalTickets,  
			ISNULL(SUM(CASE WHEN T.[status] = 5 THEN 1 ELSE 0 END), 0) AS ClosedTickets,  
			ISNULL(SUM(CASE WHEN T.[status] <> 5 THEN 1 ELSE 0 END), 0) AS OpenTickets,    
			ISNULL(SUM(CASE WHEN T.fr_escalated = 1 THEN 1 ELSE 0 END), 0) AS ResponseViolated,    
			ISNULL(SUM(CASE WHEN T.is_escalated = 1 THEN 1 ELSE 0 END), 0) AS ResolutionViolated    
		INTO #FreshServiceTickets_Get_PreviousPeriodTickets
		FROM [FreshService_T_Tickets] T WITH(NOLOCK)
		INNER JOIN #FreshServiceTickets_Get_DepartmentIdTable DIT ON T.department_id = DIT.DepartmentId
		WHERE 
			(@Start_Date IS NULL OR CAST(SWITCHOFFSET(CAST(T.created_at AS DATETIMEOFFSET), '+05:30') AS DATETIME) 
				>= @PrevDateStart)
			AND (@End_Date IS NULL OR CAST(SWITCHOFFSET(CAST(T.created_at AS DATETIMEOFFSET), '+05:30') AS DATETIME) 
				<= @PrevDateEnd)
			AND T.created_at BETWEEN @PrevDateStart AND @PrevDateEnd;

		--SELECT * FROM #FreshServiceTickets_Get_PreviousPeriodTickets;

		SELECT 
		--C.TotalTickets, 
		--C.ClosedTickets, 
		--C.OpenTickets, 
		--C.ResponseViolated, 
		--C.ResolutionViolated,

		-- Total Tickets Percentage Change
		CAST(CASE 
			WHEN P.TotalTickets = 0 AND C.TotalTickets = 0 THEN 0  -- No change if both are 0
			WHEN P.TotalTickets = 0 THEN 0  -- No previous data
			ELSE ((C.TotalTickets - P.TotalTickets) * 1.0 / P.TotalTickets) * 100 
		END AS DECIMAL(18, 2)) AS TotalTicketsPercentageChange,

		-- Closed Tickets Percentage Change
		CAST(CASE 
			WHEN P.ClosedTickets = 0 AND C.ClosedTickets = 0 THEN 0  -- No change if both are 0
			WHEN P.ClosedTickets = 0 THEN 0  -- No previous data
			ELSE ((C.ClosedTickets - P.ClosedTickets) * 1.0 / P.ClosedTickets) * 100 
		END AS DECIMAL(18, 2)) AS ClosedTicketsPercentageChange,

		-- Open Tickets Percentage Change
		CAST(CASE 
			WHEN P.OpenTickets = 0 AND C.OpenTickets = 0 THEN 0  -- No change if both are 0
			WHEN P.OpenTickets = 0 THEN 0  -- No previous data
			ELSE ((C.OpenTickets - P.OpenTickets) * 1.0 / P.OpenTickets) * 100 
		END AS DECIMAL(18, 2)) AS OpenTicketsPercentageChange,

		-- Response Violated Percentage Change
		CAST(CASE 
			WHEN P.ResponseViolated = 0 AND C.ResponseViolated = 0 THEN 0  -- No change if both are 0
			WHEN P.ResponseViolated = 0 THEN 0  -- No previous data
			ELSE ((C.ResponseViolated - P.ResponseViolated) * 1.0 / P.ResponseViolated) * 100 
		END AS DECIMAL(18, 2)) AS ResponseViolatedPercentageChange,

		-- Resolution Violated Percentage Change
		CAST(CASE 
			WHEN P.ResolutionViolated = 0 AND C.ResolutionViolated = 0 THEN 0  -- No change if both are 0
			WHEN P.ResolutionViolated = 0 THEN 0  -- No previous data
			ELSE ((C.ResolutionViolated - P.ResolutionViolated) * 1.0 / P.ResolutionViolated) * 100 
		END AS DECIMAL(18, 2)) AS ResolutionViolatedPercentageChange 

	FROM #FreshServiceTickets_Get_CurrentPeriodTickets C
	JOIN #FreshServiceTickets_Get_PreviousPeriodTickets P ON 1 = 1;

		DROP TABLE #FreshServiceTickets_Get_CurrentPeriodTickets;
		DROP TABLE #FreshServiceTickets_Get_PreviousPeriodTickets;

	END


	-- Clear Temp Tables
	DROP TABLE #FreshServiceTickets_Get_DepartmentIdTable;

 END
GO
/****** Object:  StoredProcedure [dbo].[usp_CP_T_OTPLog_InsertUpdate]    Script Date: 09/06/2025 10:37:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_CP_T_OTPLog_InsertUpdate]
(
    @TransactionType VARCHAR(10),

    @ReferenceNo VARCHAR(255) = NULL,
    @Code VARCHAR(50) = NULL,
    @ValidityInSec INT = NULL,
    @CreatedOn DATETIME = NULL,
    @ExpiredOn DATETIME = NULL,
    @Recipient VARCHAR(150) = NULL,
	@SessionId VARCHAR(255) = NULL
)
AS
BEGIN
    SET NOCOUNT ON;
    
	DECLARE @OutPutId	BIGINT = NULL 
	DECLARE @OutPutMsg	VARCHAR(255) = NULL
	DECLARE @OutPutMsg2	VARCHAR(255) = ''

	DECLARE @NewItemId	BIGINT = 0;

    SET @OutPutId = 0;
    SET @OutPutMsg = 'Something went wrong';

    IF(@TransactionType = 'I')
    BEGIN

			IF (
			SELECT COUNT(*)
			FROM CP_T_OTPLog WITH(NOLOCK)
			WHERE Recipient = @Recipient
			  AND CreatedOn >= DATEADD(MINUTE, -5, CAST(SWITCHOFFSET(CONVERT(DATETIMEOFFSET, GETUTCDATE()), '+05:30') AS DATETIME))
		) >= 6
		BEGIN
			SET @OutPutId = 0;
			SET @OutPutMsg = 'The maximum number of OTP requests has been reached. Please try again after 5 minutes.';
			SET @OutPutMsg2 = '';

			SELECT
				@OutPutMsg														AS [Message],
				@OutPutMsg2														AS ErrorMessage,
				@OutPutId														AS [Status],
				0																AS Id,
				@Recipient														AS ReferenceNo
			RETURN
		END

		BEGIN TRANSACTION;
        BEGIN TRY

			-- Inactive previous active OTPs
			UPDATE [dbo].[CP_T_OTPLog]
			SET 
				Active = 0
			WHERE Recipient = @Recipient 
				AND Active = 1;


			INSERT INTO [dbo].[CP_T_OTPLog] (
				ReferenceNo,
				Recipient,
				Code,
				ValidityInSec,
				CreatedOn,
				ExpiredOn,
				Verified,
				Active,
				SessionId
			)
			VALUES (
				@ReferenceNo,
				@Recipient,
				@Code,
				@ValidityInSec,
				ISNULL(@CreatedOn, CAST(SWITCHOFFSET(CONVERT(DATETIMEOFFSET, GETUTCDATE()), '+05:30') AS DATETIME)),
				@ExpiredOn,
				0,
				1,
				@SessionId
			);

			SET @NewItemId = SCOPE_IDENTITY();
			SET @OutPutId = 1;
			SET @OutPutMsg = 'OTP generated successfully';

		END TRY
		BEGIN CATCH
			IF @@TRANCOUNT > 0
				ROLLBACK TRANSACTION;

			SELECT
            'DB execution failed'											AS [Message],
            ERROR_MESSAGE()													AS ErrorMessage,
            0																AS [Status],
            0																AS Id,
            0																AS ReferenceNo
			RETURN
		END CATCH

        IF @@TRANCOUNT > 0
        COMMIT TRANSACTION;

        SELECT
            @OutPutMsg														AS [Message],
            ''																AS ErrorMessage,
            @OutPutId														AS [Status],
            @NewItemId														AS Id,
            @Recipient														AS ReferenceNo

    END

    IF(@TransactionType = 'U')
    BEGIN
		BEGIN TRANSACTION;
        BEGIN TRY

			DECLARE @otp_code VARCHAR(50) = NULL;
			DECLARE @otp_id INT = 0;
			DECLARE @invalid_count INT = 0;
			DECLARE @active BIT = 0;
			DECLARE @expiryonDatetime DATETIME = NULL;

			SELECT TOP 1 @otp_id = Id, @otp_code = Code, @invalid_count = InvalidCount, @active = Active, @expiryonDatetime = ExpiredOn
			FROM [dbo].[CP_T_OTPLog] WITH(NOLOCK)
			WHERE Recipient = @Recipient
			AND Verified = 0
			ORDER BY Id DESC;

			IF(@otp_id IS NULL OR @otp_code IS NULL)
			BEGIN
				SET @OutPutId = 0;
				SET @OutPutMsg = 'Please click on ''Resend'' to generate a new OTP.';
			END

			ELSE IF(@otp_id IS NOT NULL AND ((CAST(SWITCHOFFSET(CONVERT(DATETIMEOFFSET, GETUTCDATE()), '+05:30') AS DATETIME)) >= @expiryonDatetime))
			BEGIN
				SET @OutPutId = 0;
				SET @OutPutMsg = 'OTP has been expired, Please click on ''Resend'' to generate a new OTP.';
				SET @OutPutMsg2 = '';

				UPDATE T
					SET T.Active = 0
					FROM [dbo].[CP_T_OTPLog] T
					WHERE T.id = @otp_id;
			END

			ELSE IF(@otp_id IS NOT NULL AND @active = 0 AND @invalid_count >= 3)
			BEGIN
				SET @OutPutId = 0;
				SET @OutPutMsg = 'Maximum no. of retry exceeds, Please click on ''Resend'' to generate a new OTP.';
				SET @OutPutMsg2 = 'Invalid Count: ' + CAST(@invalid_count AS VARCHAR(10));
			END

			ELSE
			BEGIN
				IF(@otp_code = @code AND @otp_id > 0)
				BEGIN
					SET @OutPutId = 1;
					SET @OutPutMsg = 'OTP Verified Successfully.';
					UPDATE [dbo].[CP_T_OTPLog]
					SET Verified = 1, 
					VerifiedOn = CAST(SWITCHOFFSET(CONVERT(DATETIMEOFFSET, GETUTCDATE()), '+05:30') AS DATETIME),
					active = 0
					WHERE id = @otp_id
				END
				ELSE
				BEGIN
					SET @OutPutId = 0;
					SET @OutPutMsg = 'Invalid OTP, Please try again.';

					UPDATE T
					SET T.InvalidCount = T.InvalidCount + 1,
					T.active = CASE 
					  WHEN T.InvalidCount + 1 >= 3 THEN 0
					  ELSE 1
					END
					FROM [dbo].[CP_T_OTPLog] T
					WHERE T.id = @otp_id;


					DECLARE @UpdatedInvalidCount INT;
					SELECT @UpdatedInvalidCount = InvalidCount 
					FROM [dbo].[CP_T_OTPLog] WITH(NOLOCK)
					WHERE id = @otp_id;

					-- Update active sessions
					UPDATE CP
					SET	
						CP.SignoutTimeUTC = CASE WHEN @UpdatedInvalidCount >= 3 THEN GETUTCDATE() ELSE CP.SignoutTimeUTC END,
						CP.IsSessionActive = CASE WHEN @UpdatedInvalidCount >= 3 THEN 0 ELSE CP.IsSessionActive END,
						CP.SignOutRemarks = CASE WHEN @UpdatedInvalidCount >= 3 THEN 'Force-SignOut' ELSE CP.SignOutRemarks END,
						CP.FailedLoginAttempts = @UpdatedInvalidCount
					FROM CP_T_SignInLog CP
					WHERE CP.UserEmail = @Recipient
						AND CP.IsSessionActive = 1
						AND CP.OTPId = @otp_id;


					SET @OutPutMsg2 = 'Invalid Count: ' + CAST(@UpdatedInvalidCount AS VARCHAR(10));
				END
			END

		END TRY
		BEGIN CATCH
			IF @@TRANCOUNT > 0
				ROLLBACK TRANSACTION;

			SELECT
            'DB execution failed'											AS [Message],
            ERROR_MESSAGE()													AS ErrorMessage,
            0																AS [Status],
            0																AS Id,
            0																AS ReferenceNo
			RETURN
		END CATCH

        IF @@TRANCOUNT > 0
        COMMIT TRANSACTION;

        SELECT
            @OutPutMsg														AS [Message],
            @OutPutMsg2														AS ErrorMessage,
            @OutPutId														AS [Status],
            @otp_id															AS Id,
            @Recipient														AS ReferenceNo

    END
END
GO
/****** Object:  StoredProcedure [dbo].[usp_CP_T_SignInLog_InsertUpdate]    Script Date: 09/06/2025 10:37:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_CP_T_SignInLog_InsertUpdate]
(
    @TransactionType VARCHAR(10), -- 'I' for Insert, 'U' for Update
    @LogId BIGINT = NULL,
    @UserId VARCHAR(100) = NULL,
    @UserName VARCHAR(255) = NULL,
    @UserEmail VARCHAR(255) = NULL,
    --@SigninTimeUTC DATETIME = NULL,
    --@SignoutTimeUTC DATETIME = NULL,
    @ClientIP VARCHAR(45) = NULL,
    @UserAgent VARCHAR(500) = NULL,
    @DeviceType VARCHAR(50) = NULL,
    @Location VARCHAR(255) = NULL,
    @JWTTokenId VARCHAR(1000) = NULL,
	@JWTTokenExpiredOn DATETIME = NULL,
    @SessionId VARCHAR(255) = NULL,
    @IsSessionActive BIT = NULL,
    @SignOutRemarks VARCHAR(255) = NULL,
    @OTPId BIGINT = NULL
    --@FailedLoginAttempts INT = NULL
)
AS
BEGIN
    SET NOCOUNT ON;
	DECLARE @NewItemId	BIGINT = 0;

    IF @TransactionType = 'I'  -- INSERT
    BEGIN
		BEGIN TRANSACTION;
		BEGIN TRY

			-- Inactive active sessions
			UPDATE L
			SET 
				SignoutTimeUTC = L.JWTTokenExpiredOn,
				IsSessionActive = 0,
				SignOutRemarks = 'Session-Expired'
			FROM CP_T_SignInLog L
			WHERE UserEmail = @UserEmail
				AND IsSessionActive = 1
				AND SignoutTimeUTC IS NULL;


			UPDATE [dbo].[CP_T_WebChat_Log]
			SET 
				Active = 0,
				EndedOn = GETUTCDATE(),
				SessionCloseRemarks = 'Session-Expired'
			WHERE @UserEmail IS NOT NULL
				AND UserEmail = @UserEmail
				AND Active = 1;


			INSERT INTO CP_T_SignInLog
			(
				UserId,
				UserName,
				UserEmail,
				SigninTimeUTC,
				ClientIP,
				UserAgent,
				DeviceType,
				[Location],
				JWTTokenId,
				SessionId,
				IsSessionActive,
				SignOutRemarks,
				OTPId,
				CreatedOn
			)
			VALUES
			(	@UserId,
				 @UserName,
				 @UserEmail,
				 GETUTCDATE(),
				 @ClientIP,
				 @UserAgent,
				 @DeviceType,
				 @Location,
				 @JWTTokenId,
				 @SessionId,
				 1,
				 @SignOutRemarks,
				 @OTPId,
				 GETUTCDATE()
			);

			SET @NewItemId = SCOPE_IDENTITY();

		END TRY
		BEGIN CATCH
		IF @@TRANCOUNT > 0
			ROLLBACK TRANSACTION;

		SELECT
		'DB execution failed'											AS [Message],
		ERROR_MESSAGE()													AS ErrorMessage,
		0																AS [Status],
		0																AS Id,
		0																AS ReferenceNo
		RETURN
		END CATCH

		IF @@TRANCOUNT > 0
		COMMIT TRANSACTION;

		SELECT
			'DB execution successful'										AS [Message],
			''																AS ErrorMessage,
			1																AS [Status],
			@NewItemId														AS Id,
			''																AS ReferenceNo
    END

    ELSE IF @TransactionType = 'U' -- UPDATE
    BEGIN

		BEGIN TRANSACTION;
		BEGIN TRY
			
			IF(@IsSessionActive = 1 AND @JWTTokenId IS NOT NULL)
			BEGIN
				UPDATE CP_T_SignInLog
				SET 
					JWTTokenId = @JWTTokenId,
					JWTTokenExpiredOn = @JWTTokenExpiredOn
				WHERE 
					IsSessionActive = 1 
					AND (
						(@SessionId IS NOT NULL AND @UserEmail IS NOT NULL AND SessionId = @SessionId AND UserEmail = @UserEmail)
						OR 
						(@SessionId IS NULL AND UserEmail = @UserEmail)
					);
			END

			ELSE IF(@IsSessionActive = 0)
			BEGIN
				UPDATE CP_T_SignInLog
				SET 
					SignoutTimeUTC = GETUTCDATE(),
					IsSessionActive = @IsSessionActive,
					SignOutRemarks = @SignOutRemarks
				WHERE 
					IsSessionActive = 1 
					AND (
						(@SessionId IS NOT NULL AND @UserEmail IS NOT NULL AND SessionId = @SessionId AND UserEmail = @UserEmail)
						OR 
						(@SessionId IS NULL AND UserEmail = @UserEmail)
					);


					UPDATE [dbo].[CP_T_WebChat_Log]
					SET 
						Active = 0,
						EndedOn = GETUTCDATE(),
						SessionCloseRemarks = 'Session-Expired'
					WHERE @UserEmail IS NOT NULL
						AND UserEmail = @UserEmail
						AND Active = 1;

			END

		END TRY
		BEGIN CATCH
		IF @@TRANCOUNT > 0
			ROLLBACK TRANSACTION;

		SELECT
		'DB execution failed'											AS [Message],
		ERROR_MESSAGE()													AS ErrorMessage,
		0																AS [Status],
		0																AS Id,
		0																AS ReferenceNo
		RETURN
		END CATCH

		IF @@TRANCOUNT > 0
		COMMIT TRANSACTION;

		SELECT
			'DB execution successful'										AS [Message],
			''																AS ErrorMessage,
			1																AS [Status],
			@NewItemId														AS Id,
			''																AS ReferenceNo
    END
END;
GO
/****** Object:  StoredProcedure [dbo].[usp_CP_T_WebChat_Log_Get]    Script Date: 09/06/2025 10:37:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_CP_T_WebChat_Log_Get]
(
    @UserId VARCHAR(100) = NULL,
    @UserEmail VARCHAR(255) = NULL,
    @SessionId VARCHAR(255) = NULL,
	@ConversationType VARCHAR(255) = NULL
)
AS
BEGIN
    SET NOCOUNT ON;
	--DECLARE @CurrentIST DATETIME = SWITCHOFFSET(GETUTCDATE(), '+05:30');
	DECLARE @CurrentUTC DATETIME = GETUTCDATE();

    SELECT TOP 1
        [AutoId] AS 'WebChatLogId',
        [UserId], 
        [UserName], 
        [UserEmail], 
        [LogId], 
        [SessionId], 
        [DirectLineToken], 
        [ConversationId],
		[StreamUrl], 
        [ExpiredOn], 
        [CreatedOn],
		CASE 
            WHEN DATEDIFF(MINUTE, @CurrentUTC, ExpiredOn) <= 15 THEN 1 
            ELSE 0 
        END AS NeedsRefresh
    FROM 
        [dbo].[CP_T_WebChat_Log]
    
	WHERE
        UserEmail = @UserEmail
        AND SessionId = @SessionId
        AND ExpiredOn > @CurrentUTC
		AND StartedOn IS NOT NULL
		AND ConversationType = @ConversationType
		AND Active = 1
    
	ORDER BY 
        AutoId DESC;

END;
GO
/****** Object:  StoredProcedure [dbo].[usp_CP_T_WebChat_Log_InsertUpdate]    Script Date: 09/06/2025 10:37:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_CP_T_WebChat_Log_InsertUpdate]
(
    @TransactionType VARCHAR(10), -- 'I' for Insert, 'U' for Update
    @UserId VARCHAR(100) = NULL,
    @UserName VARCHAR(255) = NULL,
    @UserEmail VARCHAR(255) = NULL,
    @LogId BIGINT = NULL,
    @SessionId VARCHAR(255) = NULL,
    @DirectLineToken VARCHAR(2500) = NULL,
    @ConversationId VARCHAR(1000) = NULL,
	@StreamUrl VARCHAR(2500) = NULL,
    @ExpiredOn DATETIME = NULL,
	@CreatedOn DATETIME = NULL,

	@StartedOn DATETIME = NULL,
	@EndedOn DATETIME = NULL,
	@Active BIT = NULL,

	@FeedbackRatingId INT = NULL,
    @AdditionalFeedback VARCHAR(500) = NULL,
    @SatisfiedWithResolution BIT = NULL,

	@ConversationType VARCHAR(255) = NULL,
	@SessionCloseRemarks VARCHAR(255) = NULL
)
AS
BEGIN
    SET NOCOUNT ON;
	DECLARE @NewItemId	BIGINT = 0;

    IF @TransactionType = 'I'  -- INSERT
    BEGIN
		BEGIN TRANSACTION;
		BEGIN TRY

			UPDATE [dbo].[CP_T_WebChat_Log]
			SET Active = 0,
			EndedOn = GETUTCDATE(),
			SessionCloseRemarks = 'Session-Expired'
			WHERE UserEmail = @UserEmail
			AND Active = 1;

			INSERT INTO [dbo].[CP_T_WebChat_Log]
			(
				[UserId], 
				[UserName], 
				[UserEmail], 
				[LogId], 
				[SessionId], 
				[DirectLineToken], 
				[ConversationId],
				[StreamUrl],
				[ExpiredOn], 
				[CreatedOn],
				Active,
				ConversationType
			)
			VALUES 
			(
				@UserId, 
				@UserName, 
				@UserEmail, 
				@LogId, 
				@SessionId, 
				@DirectLineToken, 
				@ConversationId,
				@StreamUrl,
				@ExpiredOn, 
				@CreatedOn,
				1,
				@ConversationType
			);

			SET @NewItemId = SCOPE_IDENTITY();

		END TRY
		BEGIN CATCH
		IF @@TRANCOUNT > 0
			ROLLBACK TRANSACTION;

		SELECT
		'DB execution failed'											AS [Message],
		ERROR_MESSAGE()													AS ErrorMessage,
		0																AS [Status],
		0																AS Id,
		0																AS ReferenceNo
		RETURN
		END CATCH

		IF @@TRANCOUNT > 0
		COMMIT TRANSACTION;

		SELECT
			'DB execution successful'										AS [Message],
			''																AS ErrorMessage,
			1																AS [Status],
			@NewItemId														AS Id,
			''																AS ReferenceNo
    END

    ELSE IF @TransactionType = 'U' -- UPDATE
    BEGIN

		BEGIN TRANSACTION;
		BEGIN TRY
			
			UPDATE L
			SET 
				L.Active = ISNULL(@Active, L.Active),
				L.StartedOn = ISNULL(@StartedOn, L.StartedOn),
				L.EndedOn = ISNULL(@EndedOn, L.EndedOn),
				L.FeedbackRatingId = ISNULL(@FeedbackRatingId, L.FeedbackRatingId),
				L.AdditionalFeedback = ISNULL(@AdditionalFeedback, L.AdditionalFeedback),
				L.SatisfiedWithResolution = ISNULL(@SatisfiedWithResolution, L.SatisfiedWithResolution),
				L.SessionCloseRemarks = ISNULL(@SessionCloseRemarks, SessionCloseRemarks)
			FROM [dbo].[CP_T_WebChat_Log] L
			WHERE L.UserEmail = @UserEmail
			AND L.Active = 1;

		END TRY
		BEGIN CATCH
		IF @@TRANCOUNT > 0
			ROLLBACK TRANSACTION;

		SELECT
		'DB execution failed'											AS [Message],
		ERROR_MESSAGE()													AS ErrorMessage,
		0																AS [Status],
		0																AS Id,
		0																AS ReferenceNo
		RETURN
		END CATCH

		IF @@TRANCOUNT > 0
		COMMIT TRANSACTION;

		SELECT
			'DB execution successful'										AS [Message],
			''																AS ErrorMessage,
			1																AS [Status],
			@NewItemId														AS Id,
			''																AS ReferenceNo
    END
END;
GO
/****** Object:  StoredProcedure [dbo].[usp_CP_T_WebChat_UserConversationLog_InsertUpdate]    Script Date: 09/06/2025 10:37:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_CP_T_WebChat_UserConversationLog_InsertUpdate]
(
    @TransactionType VARCHAR(10), -- 'I' for Insert, 'U' for Update

	@MessageId BIGINT = NULL,
	@Name NVARCHAR(100) = NULL,
    @Email NVARCHAR(50) = NULL,
    @UPN NVARCHAR(50) = NULL,
    @ADID NVARCHAR(50) = NULL,
    @ChannelId NVARCHAR(50) = NULL,
    @ConversationType NVARCHAR(50) = NULL,
    @ConversationId NVARCHAR(500) = NULL,
    @TenantId NVARCHAR(50) = NULL,
    @ChatId NVARCHAR(50) = NULL,
    @LocalTimestamp DATETIMEOFFSET = NULL,
    @Locale NVARCHAR(50) = NULL,
    @ServiceUrl NVARCHAR(50) = NULL,
    @Text NVARCHAR(MAX) = NULL,
    @TextFormat NVARCHAR(50) = NULL,
    @Timestamp DATETIMEOFFSET = NULL,
    @Response NVARCHAR(MAX) = NULL,
    @Intent NVARCHAR(MAX) = NULL,

	@CategoryId INT = NULL,
	@SubCategoryId INT = NULL,

	@WebChatLogId BIGINT = NULL,
    @MessageActivityId NVARCHAR(100) = NULL,
    @MessageSentUTC DATETIME = NULL,
    @FeedbackCardActivityId NVARCHAR(100) = NULL,
    @FeedbackCardSentUTC DATETIME = NULL,
    @LikeDislike BIT = NULL,
    @FeedbackReceivedUTC DATETIME = NULL,

	@FileJSONInput VARCHAR(MAX)
)
AS
BEGIN
    SET NOCOUNT ON;
	DECLARE @NewItemId	BIGINT = 0;
	DECLARE @Message VARCHAR(255) = NULL;

    IF @TransactionType = 'I'  -- INSERT
    BEGIN
		BEGIN TRANSACTION;
		BEGIN TRY

			INSERT INTO CP_T_WebChat_UserConversationLog 
			(
				[UserName],
				[UserEmail],
				[UserUPN],
				[UserADID],
				[ChannelId],
				[ConversationType],
				[ConversationId],
				[TenantId],
				[ChatId],
				[LocalTimestamp],
				[Locale],
				[ServiceUrl],
				[Text],
				[TextFormat],
				[Timestamp],
				[Response],
				[Intent],
				CreatedOnIST,
				CreatedOnUTC,
				WebChatLogId,
				MessageActivityId,
				MessageSentUTC, 
				FeedbackCardActivityId,
				FeedbackCardSentUTC, 
				LikeDislike,
				FeedbackReceivedUTC,
				CategoryId,
				SubCategoryId
			)
			VALUES 
			(
				@Name,
				@Email,
				@UPN,
				@ADID,
				@ChannelId,
				@ConversationType,
				@ConversationId,
				@TenantId,
				@ChatId,
				@LocalTimestamp,
				@Locale,
				@ServiceUrl,
				@Text,
				@TextFormat,
				@Timestamp,
				@Response,
				@Intent,
				CAST(SWITCHOFFSET(GETUTCDATE(), '+05:30') AS DATETIME),
				GETUTCDATE(),
				@WebChatLogId,
				@MessageActivityId,
				@MessageSentUTC, 
				@FeedbackCardActivityId,
				@FeedbackCardSentUTC, 
				@LikeDislike,
				@FeedbackReceivedUTC,
				@CategoryId,
				@SubCategoryId
			);

			SET @NewItemId = SCOPE_IDENTITY();

			IF(@FileJSONInput IS NOT NULL AND @FileJSONInput != 'null')
			BEGIN
				INSERT INTO [CP_T_WebChat_UserConversationFilesLog] (
					[MessageId],
					[FileName],
					[FileURL],
					[FileContent]
				)
				SELECT 
					@MessageId,
					JSON_VALUE([File].value, '$.FileName') AS [FileName],
					JSON_VALUE([File].value, '$.FileURL') AS FileURL,
					JSON_VALUE([File].value, '$.FileContent') AS FileContent
				FROM OPENJSON(@FileJSONInput) AS [File];
			END

		END TRY
		BEGIN CATCH
		IF @@TRANCOUNT > 0
			ROLLBACK TRANSACTION;

		SELECT
		'DB execution failed'											AS [Message],
		ERROR_MESSAGE()													AS ErrorMessage,
		0																AS [Status],
		0																AS Id,
		0																AS ReferenceNo
		RETURN
		END CATCH

		IF @@TRANCOUNT > 0
		COMMIT TRANSACTION;

		SELECT
			'DB execution successful'										AS [Message],
			''																AS ErrorMessage,
			1																AS [Status],
			@NewItemId														AS Id,
			''																AS ReferenceNo
    END

    ELSE IF @TransactionType = 'U' -- UPDATE
    BEGIN

		BEGIN TRANSACTION;
		BEGIN TRY

			IF EXISTS (
			SELECT 1 FROM [dbo].[CP_T_WebChat_UserConversationLog]
			WHERE MessageActivityId = @MessageActivityId AND LikeDislike IS NOT NULL
			)
			BEGIN
				SET @NewItemId = 0;
				SET @Message = 'You have already provided feedback for this message.';
			END
			ELSE
			BEGIN
				UPDATE L
				SET 
					L.LikeDislike = @LikeDislike,
					L.FeedbackReceivedUTC = @FeedbackReceivedUTC
				FROM [dbo].[CP_T_WebChat_UserConversationLog] L
				WHERE L.MessageActivityId = @MessageActivityId;

				SELECT @FeedbackCardActivityId = FeedbackCardActivityId, @NewItemId = L.MessageId FROM [dbo].[CP_T_WebChat_UserConversationLog] L
				WHERE L.MessageActivityId = @MessageActivityId;

				SET @Message = 'Thank you for your feedback.';
			END

		END TRY
		BEGIN CATCH
		IF @@TRANCOUNT > 0
			ROLLBACK TRANSACTION;

		SELECT
		@Message														AS [Message],
		ERROR_MESSAGE()													AS ErrorMessage,
		0																AS [Status],
		0																AS Id,
		0																AS ReferenceNo
		RETURN
		END CATCH

		IF @@TRANCOUNT > 0
		COMMIT TRANSACTION;

		SELECT
			@Message														AS [Message],
			@Message														AS ErrorMessage,
			1																AS [Status],
			@NewItemId														AS Id,
			@FeedbackCardActivityId											AS ReferenceNo
    END
END;
GO
/****** Object:  StoredProcedure [dbo].[usp_FreshService_G_GetDepartment]    Script Date: 09/06/2025 10:37:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE   PROC [dbo].[usp_FreshService_G_GetDepartment]
(
	@name VARCHAR(100)=null,
	@id bigint=null,
	@STARTROWINDEX INT=0,
	@MAXIMUMROWS INT=100	
)
AS
BEGIN

	SELECT 
		D.id
		,D.[name]
		,D.[description]
		,D.head_user_id
		,D.head_name
		,D.prime_user_id
		,D.prime_user_name
		,D.created_at
		,D.updated_at
		,D.Created_On
		,D.Updated_On
		,DC.[location]
		,DC.tenant
		,DC.embee_crm_id
		,DC.contact_person
		,DC.contact_number
		,DC.contact_email_id
		,DC.embee_account_manager
		,DC.engagement_start_date
		,DC.engagement_end_date
	FROM FreshService_M_Department D WITH(NOLOCK)
	LEFT OUTER JOIN FreshService_M_Department_CustomFields DC WITH(NOLOCK)
	ON D.id=DC.department_id
	WHERE (D.id=@id or ISNULL(@id,0) =0)
	AND  (D.[name] LIKE ISNULL(@name,'')+'%' or ISNULL(@name,'') ='')
	ORDER BY D.[NAME] ASC
	OFFSET @STARTROWINDEX * @MAXIMUMROWS ROWS
	FETCH NEXT @MAXIMUMROWS ROWS ONLY
END
GO
/****** Object:  StoredProcedure [dbo].[usp_FreshService_M_Department_InsertUpdate]    Script Date: 09/06/2025 10:37:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE   PROCEDURE [dbo].[usp_FreshService_M_Department_InsertUpdate]
(
 @jsonInput   VARCHAR(MAX)
)  
AS
BEGIN
    
    SET NOCOUNT ON;  
    
 declare @OutPutId   BIGINT=NULL
 declare @OutPutMsg   VARCHAR(255)=NULL

 SET @OutPutId = 0
 SET @OutPutMsg='Sorry something went wrong.'

 
 IF ISNULL(@jsonInput,'')=''
 BEGIN
  SET @OutPutId = 0
  SET @OutPutMsg='Invalid details, please check your entry.' 
  RETURN
 END

 BEGIN TRY 
  
  BEGIN TRANSACTION 
   
   DECLARE @Id BIGINT

   IF  (@jsonInput IS NOT NULL AND iSNULL(@jsonInput,'') <> '') 
   BEGIN
    
    DECLARE @MaxRowIndex INT, @LoopIndex INT=0  
    
    SELECT @MaxRowIndex=MAX(cast([key] as int)) FROM OPENJSON(@jsonInput, '$.departments')

    WHILE @LoopIndex <= ISNULL(@MaxRowIndex,-1)
    BEGIN     
     
     SET @Id=TRY_PARSE(JSON_VALUE(@jsonInput, '$.departments['+CAST(@LoopIndex AS VARCHAR)+'].id') as BIGINT)
     IF(@Id IS NOT NULL)
     BEGIN
      IF NOT EXISTS (SELECT * FROM FreshService_M_Department WITH(NOLOCK) WHERE ID=@Id)
      BEGIN
       INSERT INTO FreshService_M_Department
       (
        id
        ,[name]
        ,[description]
        ,head_user_id
        ,head_name
        ,prime_user_id
        ,prime_user_name
        ,created_at
        ,updated_at
        ,created_on
        ,updated_on
       )
       SELECT @Id
        ,JSON_VALUE(@jsonInput, '$.departments['+CAST(@LoopIndex AS VARCHAR)+'].name')
        ,JSON_VALUE(@jsonInput, '$.departments['+CAST(@LoopIndex AS VARCHAR)+'].description')
        ,TRY_PARSE(JSON_VALUE(@jsonInput, '$.departments['+CAST(@LoopIndex AS VARCHAR)+'].head_user_id') AS BIGINT)
        ,JSON_VALUE(@jsonInput, '$.departments['+CAST(@LoopIndex AS VARCHAR)+'].head_name')
        ,TRY_PARSE(JSON_VALUE(@jsonInput, '$.departments['+CAST(@LoopIndex AS VARCHAR)+'].prime_user_id') AS BIGINT)
        ,JSON_VALUE(@jsonInput, '$.departments['+CAST(@LoopIndex AS VARCHAR)+'].prime_user_name')
        ,TRY_PARSE(JSON_VALUE(@jsonInput, '$.departments['+CAST(@LoopIndex AS VARCHAR)+'].created_at') AS DATETIMEOFFSET)
        ,TRY_PARSE(JSON_VALUE(@jsonInput, '$.departments['+CAST(@LoopIndex AS VARCHAR)+'].updated_at') AS DATETIMEOFFSET)
        ,GETUTCDATE()
        ,GETUTCDATE()
      END
      ELSE
      BEGIN
       UPDATE FreshService_M_Department
       SET 
        [name]=JSON_VALUE(@jsonInput, '$.departments['+CAST(@LoopIndex AS VARCHAR)+'].name')
        ,[description]=JSON_VALUE(@jsonInput, '$.departments['+CAST(@LoopIndex AS VARCHAR)+'].description')
        ,head_user_id=TRY_PARSE(JSON_VALUE(@jsonInput, '$.departments['+CAST(@LoopIndex AS VARCHAR)+'].head_user_id') AS BIGINT)
        ,head_name=JSON_VALUE(@jsonInput, '$.departments['+CAST(@LoopIndex AS VARCHAR)+'].head_name')
        ,prime_user_id=TRY_PARSE(JSON_VALUE(@jsonInput, '$.departments['+CAST(@LoopIndex AS VARCHAR)+'].prime_user_id') AS BIGINT)
        ,prime_user_name=JSON_VALUE(@jsonInput, '$.departments['+CAST(@LoopIndex AS VARCHAR)+'].prime_user_name')
        ,updated_at=TRY_PARSE(JSON_VALUE(@jsonInput, '$.departments['+CAST(@LoopIndex AS VARCHAR)+'].updated_at') AS DATETIMEOFFSET)
        ,updated_on=GETUTCDATE()
       WHERE ID=@Id
      END

      IF NOT EXISTS (SELECT * FROM FreshService_M_Department_CustomFields WITH(NOLOCK) WHERE department_id=@Id)
      BEGIN
       INSERT INTO FreshService_M_Department_CustomFields
       (
        department_id
        ,[location]
        ,tenant
        ,embee_crm_id
        ,contact_person
        ,contact_number
        ,contact_email_id
        ,embee_account_manager
        ,engagement_start_date
        ,engagement_end_date
        ,customer_portal_access
        ,sap_customer_name
       )
       SELECT @Id
        ,JSON_VALUE(@jsonInput, '$.departments['+CAST(@LoopIndex AS VARCHAR)+'].custom_fields.location')
        ,JSON_VALUE(@jsonInput, '$.departments['+CAST(@LoopIndex AS VARCHAR)+'].custom_fields.tenant')
        ,JSON_VALUE(@jsonInput, '$.departments['+CAST(@LoopIndex AS VARCHAR)+'].custom_fields.embee_crm_id')
        ,JSON_VALUE(@jsonInput, '$.departments['+CAST(@LoopIndex AS VARCHAR)+'].custom_fields.contact_person')
        ,JSON_VALUE(@jsonInput, '$.departments['+CAST(@LoopIndex AS VARCHAR)+'].custom_fields.contact_number')
        ,JSON_VALUE(@jsonInput, '$.departments['+CAST(@LoopIndex AS VARCHAR)+'].custom_fields.contact_email_id')
        ,JSON_VALUE(@jsonInput, '$.departments['+CAST(@LoopIndex AS VARCHAR)+'].custom_fields.embee_account_manager')
        ,TRY_PARSE(JSON_VALUE(@jsonInput, '$.departments['+CAST(@LoopIndex AS VARCHAR)+'].custom_fields.engagement_start_date') AS DATETIMEOFFSET)
        ,TRY_PARSE(JSON_VALUE(@jsonInput, '$.departments['+CAST(@LoopIndex AS VARCHAR)+'].custom_fields.engagement_end_date') AS DATETIMEOFFSET)
        ,JSON_VALUE(@jsonInput, '$.departments['+CAST(@LoopIndex AS VARCHAR)+'].custom_fields.customer_portal_access')
        ,JSON_VALUE(@jsonInput, '$.departments['+CAST(@LoopIndex AS VARCHAR)+'].custom_fields.sap_customer_name')
      END
      ELSE
      BEGIN
       UPDATE FreshService_M_Department_CustomFields
       SET 
        [location]=JSON_VALUE(@jsonInput, '$.departments['+CAST(@LoopIndex AS VARCHAR)+'].custom_fields.location')
        ,tenant=JSON_VALUE(@jsonInput, '$.departments['+CAST(@LoopIndex AS VARCHAR)+'].custom_fields.tenant')
        ,embee_crm_id=JSON_VALUE(@jsonInput, '$.departments['+CAST(@LoopIndex AS VARCHAR)+'].custom_fields.embee_crm_id')
        ,contact_person=JSON_VALUE(@jsonInput, '$.departments['+CAST(@LoopIndex AS VARCHAR)+'].custom_fields.contact_person')
        ,contact_number=JSON_VALUE(@jsonInput, '$.departments['+CAST(@LoopIndex AS VARCHAR)+'].custom_fields.contact_number')
        ,contact_email_id=JSON_VALUE(@jsonInput, '$.departments['+CAST(@LoopIndex AS VARCHAR)+'].custom_fields.contact_email_id')
        ,embee_account_manager=JSON_VALUE(@jsonInput, '$.departments['+CAST(@LoopIndex AS VARCHAR)+'].custom_fields.embee_account_manager')
        ,engagement_start_date=TRY_PARSE(JSON_VALUE(@jsonInput, '$.departments['+CAST(@LoopIndex AS VARCHAR)+'].custom_fields.engagement_start_date') AS DATETIMEOFFSET)
        ,engagement_end_date=TRY_PARSE(JSON_VALUE(@jsonInput, '$.departments['+CAST(@LoopIndex AS VARCHAR)+'].custom_fields.engagement_end_date') AS DATETIMEOFFSET)
        ,customer_portal_access=JSON_VALUE(@jsonInput, '$.departments['+CAST(@LoopIndex AS VARCHAR)+'].custom_fields.customer_portal_access')
        ,sap_customer_name=JSON_VALUE(@jsonInput, '$.departments['+CAST(@LoopIndex AS VARCHAR)+'].custom_fields.sap_customer_name')
       WHERE department_id =@Id
      END

     END
     
     SET @LoopIndex=@LoopIndex+1
    END

    SET @OutPutId = 1
    SET @OutPutMsg='Data saved successfully.' 

   END
   ELSE
   BEGIN
    SET @OutPutId = 0
    SET @OutPutMsg='Data already exists' 
   END
  
   
  
 END TRY
 BEGIN CATCH
 -----------------
  ROLLBACK TRANSACTION
  DECLARE @error int, @message varchar(4000), @xstate int;  
  Select @error = ERROR_NUMBER(), @message = ERROR_MESSAGE(), @xstate = XACT_STATE();  
  RAISERROR ('usp_FS_M_Dept_IU: %d: %s', 16, 1, @error, @message) ;  
  SET @OutPutId = 0
  SET @OutPutMsg=@message  

  SELECT '' AS [Message],
   @OutPutMsg AS ErrorMessage,
   @OutPutId AS [Status],
   '0' AS Id

  RETURN
 END CATCH
 -----------------
 
 SELECT @OutPutMsg AS [Message],
 '' AS ErrorMessage,
 @OutPutId AS [Status],
 '1' AS Id


 COMMIT TRANSACTION
 
  
  
END
GO
/****** Object:  StoredProcedure [dbo].[usp_FreshService_M_Requester_InsertUpdate]    Script Date: 09/06/2025 10:37:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE   PROCEDURE [dbo].[usp_FreshService_M_Requester_InsertUpdate]
(
	@jsonInput			VARCHAR(MAX)
)  
AS
BEGIN
    
    SET NOCOUNT ON;  
    
	declare @OutPutId			BIGINT=NULL
	declare @OutPutMsg			VARCHAR(255)=NULL

	SET @OutPutId = 0
	SET @OutPutMsg='Sorry something went wrong.'

	
	IF ISNULL(@jsonInput,'')=''
	BEGIN
		SET @OutPutId = 0
		SET @OutPutMsg='Invalid details, please check your entry.' 
		RETURN
	END

	BEGIN TRY 
		
		
			
			DECLARE @Id BIGINT

			IF  (@jsonInput IS NOT NULL AND iSNULL(@jsonInput,'') <> '') 
			BEGIN				
				
				DECLARE @MaxRowIndex INT, @LoopIndex INT=0		
				
				SELECT @MaxRowIndex=MAX(cast([key] as int)) FROM OPENJSON(@jsonInput, '$.requesters')

				WHILE @LoopIndex <= ISNULL(@MaxRowIndex,-1)
				BEGIN					
					BEGIN TRY 
						BEGIN TRANSACTION 

						SET @Id=TRY_PARSE(JSON_VALUE(@jsonInput, '$.requesters['+CAST(@LoopIndex AS VARCHAR)+'].id') as BIGINT)
						IF(@Id IS NOT NULL)
						BEGIN
							IF NOT EXISTS (SELECT * FROM FreshService_M_Requesters WITH(NOLOCK) WHERE ID=@Id)
							BEGIN
								INSERT INTO FreshService_M_Requesters
								(
									id
									,active
									,[address]
									,background_information
									,can_see_all_changes_from_associated_departments
									,can_see_all_tickets_from_associated_departments
									,external_id
									,first_name
									,has_logged_in
									,is_agent
									,job_title
									,[language]
									,last_name
									,location_id
									,location_name
									,mobile_phone_number
									,primary_email
									,reporting_manager_id
									,time_format
									,time_zone
									,vip_user
									,work_phone_number
									,work_schedule_id
									,employee_id
									,created_at
									,updated_at
									,created_on
									,updated_on
								)
								SELECT @Id
									,JSON_VALUE(@jsonInput, '$.requesters['+CAST(@LoopIndex AS VARCHAR)+'].active')
									,JSON_VALUE(@jsonInput, '$.requesters['+CAST(@LoopIndex AS VARCHAR)+'].address')
									,JSON_VALUE(@jsonInput, '$.requesters['+CAST(@LoopIndex AS VARCHAR)+'].background_information')
									,JSON_VALUE(@jsonInput, '$.requesters['+CAST(@LoopIndex AS VARCHAR)+'].can_see_all_changes_from_associated_departments')
									,JSON_VALUE(@jsonInput, '$.requesters['+CAST(@LoopIndex AS VARCHAR)+'].can_see_all_tickets_from_associated_departments')
									,JSON_VALUE(@jsonInput, '$.requesters['+CAST(@LoopIndex AS VARCHAR)+'].external_id')
									,JSON_VALUE(@jsonInput, '$.requesters['+CAST(@LoopIndex AS VARCHAR)+'].first_name') 
									,JSON_VALUE(@jsonInput, '$.requesters['+CAST(@LoopIndex AS VARCHAR)+'].has_logged_in') 
									,JSON_VALUE(@jsonInput, '$.requesters['+CAST(@LoopIndex AS VARCHAR)+'].is_agent') 
									,JSON_VALUE(@jsonInput, '$.requesters['+CAST(@LoopIndex AS VARCHAR)+'].job_title') 
									,JSON_VALUE(@jsonInput, '$.requesters['+CAST(@LoopIndex AS VARCHAR)+'].language') 
									,JSON_VALUE(@jsonInput, '$.requesters['+CAST(@LoopIndex AS VARCHAR)+'].last_name') 
									,TRY_PARSE(JSON_VALUE(@jsonInput, '$.requesters['+CAST(@LoopIndex AS VARCHAR)+'].location_id') AS BIGINT) 
									,JSON_VALUE(@jsonInput, '$.requesters['+CAST(@LoopIndex AS VARCHAR)+'].location_name') 
									,JSON_VALUE(@jsonInput, '$.requesters['+CAST(@LoopIndex AS VARCHAR)+'].mobile_phone_number') 
									,JSON_VALUE(@jsonInput, '$.requesters['+CAST(@LoopIndex AS VARCHAR)+'].primary_email') 
									,JSON_VALUE(@jsonInput, '$.requesters['+CAST(@LoopIndex AS VARCHAR)+'].reporting_manager_id') 
									,JSON_VALUE(@jsonInput, '$.requesters['+CAST(@LoopIndex AS VARCHAR)+'].time_format') 
									,JSON_VALUE(@jsonInput, '$.requesters['+CAST(@LoopIndex AS VARCHAR)+'].time_zone') 
									,JSON_VALUE(@jsonInput, '$.requesters['+CAST(@LoopIndex AS VARCHAR)+'].vip_user') 
									,JSON_VALUE(@jsonInput, '$.requesters['+CAST(@LoopIndex AS VARCHAR)+'].work_phone_number') 
									,TRY_PARSE(JSON_VALUE(@jsonInput, '$.requesters['+CAST(@LoopIndex AS VARCHAR)+'].work_schedule_id') AS BIGINT) 
									,JSON_VALUE(@jsonInput, '$.requesters['+CAST(@LoopIndex AS VARCHAR)+'].custom_fields.employee_id') 
									,TRY_PARSE(JSON_VALUE(@jsonInput, '$.requesters['+CAST(@LoopIndex AS VARCHAR)+'].created_at') AS DATETIMEOFFSET)
									,TRY_PARSE(JSON_VALUE(@jsonInput, '$.requesters['+CAST(@LoopIndex AS VARCHAR)+'].updated_at') AS DATETIMEOFFSET)
									,GETUTCDATE()
									,GETUTCDATE()
							END
							ELSE
							BEGIN
								UPDATE FreshService_M_Requesters
								SET 
									 [active]=JSON_VALUE(@jsonInput, '$.requesters['+CAST(@LoopIndex AS VARCHAR)+'].active')
									,[address]=JSON_VALUE(@jsonInput, '$.requesters['+CAST(@LoopIndex AS VARCHAR)+'].address')
									,background_information=JSON_VALUE(@jsonInput, '$.requesters['+CAST(@LoopIndex AS VARCHAR)+'].background_information')
									,can_see_all_changes_from_associated_departments=JSON_VALUE(@jsonInput, '$.requesters['+CAST(@LoopIndex AS VARCHAR)+'].can_see_all_changes_from_associated_departments')
									,can_see_all_tickets_from_associated_departments=JSON_VALUE(@jsonInput, '$.requesters['+CAST(@LoopIndex AS VARCHAR)+'].can_see_all_tickets_from_associated_departments')
									,external_id=JSON_VALUE(@jsonInput, '$.requesters['+CAST(@LoopIndex AS VARCHAR)+'].external_id')
									,first_name=JSON_VALUE(@jsonInput, '$.requesters['+CAST(@LoopIndex AS VARCHAR)+'].first_name') 
									,has_logged_in=JSON_VALUE(@jsonInput, '$.requesters['+CAST(@LoopIndex AS VARCHAR)+'].has_logged_in') 
									,is_agent=JSON_VALUE(@jsonInput, '$.requesters['+CAST(@LoopIndex AS VARCHAR)+'].is_agent') 
									,job_title=JSON_VALUE(@jsonInput, '$.requesters['+CAST(@LoopIndex AS VARCHAR)+'].job_title') 
									,[language]=JSON_VALUE(@jsonInput, '$.requesters['+CAST(@LoopIndex AS VARCHAR)+'].language') 
									,last_name=JSON_VALUE(@jsonInput, '$.requesters['+CAST(@LoopIndex AS VARCHAR)+'].last_name') 
									,location_id=TRY_PARSE(JSON_VALUE(@jsonInput, '$.requesters['+CAST(@LoopIndex AS VARCHAR)+'].location_id') AS BIGINT) 
									,location_name=JSON_VALUE(@jsonInput, '$.requesters['+CAST(@LoopIndex AS VARCHAR)+'].location_name') 
									,mobile_phone_number=JSON_VALUE(@jsonInput, '$.requesters['+CAST(@LoopIndex AS VARCHAR)+'].mobile_phone_number') 
									,primary_email=JSON_VALUE(@jsonInput, '$.requesters['+CAST(@LoopIndex AS VARCHAR)+'].primary_email') 
									,reporting_manager_id=JSON_VALUE(@jsonInput, '$.requesters['+CAST(@LoopIndex AS VARCHAR)+'].reporting_manager_id') 
									,time_format=JSON_VALUE(@jsonInput, '$.requesters['+CAST(@LoopIndex AS VARCHAR)+'].time_format') 
									,time_zone=JSON_VALUE(@jsonInput, '$.requesters['+CAST(@LoopIndex AS VARCHAR)+'].time_zone') 
									,vip_user=JSON_VALUE(@jsonInput, '$.requesters['+CAST(@LoopIndex AS VARCHAR)+'].vip_user') 
									,work_phone_number=JSON_VALUE(@jsonInput, '$.requesters['+CAST(@LoopIndex AS VARCHAR)+'].work_phone_number') 
									,work_schedule_id=TRY_PARSE(JSON_VALUE(@jsonInput, '$.requesters['+CAST(@LoopIndex AS VARCHAR)+'].work_schedule_id') AS BIGINT) 
									,employee_id=JSON_VALUE(@jsonInput, '$.requesters['+CAST(@LoopIndex AS VARCHAR)+'].custom_fields.employee_id') 
									,updated_at=TRY_PARSE(JSON_VALUE(@jsonInput, '$.requesters['+CAST(@LoopIndex AS VARCHAR)+'].updated_at') AS DATETIMEOFFSET)
									,updated_on=GETUTCDATE()
								WHERE ID=@Id
							END

							--Department
							IF NOT EXISTS (SELECT * FROM FreshService_M_Requester_Departments WITH(NOLOCK)
							WHERE requester_id=@Id)
							BEGIN
								INSERT INTO FreshService_M_Requester_Departments
								(
									requester_id
									,department_id									
								)
								SELECT @Id,
								TRY_PARSE([VALUE] AS BIGINT)
								FROM OPENJSON(@jsonInput, '$.requesters['+CAST(@LoopIndex AS VARCHAR)+'].department_ids')
								WHERE TRY_PARSE([VALUE] AS BIGINT) IS NOT NULL
									
							END
						

						END

						COMMIT TRANSACTION
					END TRY
					BEGIN CATCH
						ROLLBACK TRANSACTION
					END CATCH
					
					SET @LoopIndex=@LoopIndex+1
				END

				SET @OutPutId = 1
				SET @OutPutMsg='Data saved successfully.' 

			END
			ELSE
			BEGIN
				SET @OutPutId = 0
				SET @OutPutMsg='Data already exists' 
			END
		
			
		
	END TRY
	BEGIN CATCH
	-----------------
		--ROLLBACK TRANSACTION
		DECLARE @error int, @message varchar(4000), @xstate int;  
		Select @error = ERROR_NUMBER(), @message = ERROR_MESSAGE(), @xstate = XACT_STATE();  
		RAISERROR ('usp_FS_M_Request_IU: %d: %s', 16, 1, @error, @message) ;  
		SET @OutPutId = 0
		SET @OutPutMsg=@message  

		SELECT '' AS [Message],
			@OutPutMsg AS ErrorMessage,
			@OutPutId AS [Status],
			'0' AS Id

		RETURN
	END CATCH
	-----------------
	
	SELECT @OutPutMsg AS [Message],
	'' AS ErrorMessage,
	@OutPutId AS [Status],
	'1' AS Id


	--COMMIT TRANSACTION
	
  
		
END
GO
/****** Object:  StoredProcedure [dbo].[usp_FreshService_R_ServiceRequestByUsers]    Script Date: 09/06/2025 10:37:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE PROC [dbo].[usp_FreshService_R_ServiceRequestByUsers]
(
@DepartmentId BIGINT=NULL,
@Start_Date	VARCHAR(10)='01/09/2024',
@End_Date VARCHAR(10)='30/11/2024'
)
AS

BEGIN

DECLARE @TicketsByUser TABLE (
    Requester NVARCHAR(250),
    [Count] INT DEFAULT(0)
);

DECLARE @LastDateTobeConsidered DATE = (SELECT TOP 1 LastDate FROM LastDateTobeConsidered WHERE ACTIVE =1);

INSERT INTO @TicketsByUser (Requester, [Count])
SELECT 
    R.primary_email AS Requester,
    COUNT(T.id) AS [Count]
FROM 
    FreshService_T_Tickets T WITH (NOLOCK)
--INNER JOIN [FreshService_T_Ticket_CustomFields] TC WITH(NOLOCK) ON TC.ticket_id=T.id
INNER JOIN 
    FreshService_M_Requesters R WITH (NOLOCK)
    ON T.requester_id = R.id
WHERE 
    --TYPE = 'Service Request'
	T.[status] IN (4,5)
	 --T.created_at BETWEEN CONVERT(DATE,@Start_Date,103) AND CONVERT(DATE,@End_Date,103)
	AND CAST(SWITCHOFFSET(CAST(T.created_at AS DATETIMEOFFSET), '+05:30') AS DATE) 
      BETWEEN CONVERT(DATE, @Start_Date, 103) AND CONVERT(DATE, @End_Date, 103)
	  AND (CAST(SWITCHOFFSET(CAST(T.created_at AS DATETIMEOFFSET), '+05:30') AS DATE) > @LastDateTobeConsidered OR ISNULL(@LastDateTobeConsidered,'')='')
	AND (T.department_id=@DepartmentId OR ISNULL(@DepartmentId,0)=0)
	--AND TC.tenant IN (
	--	SELECT TenantName 
	--	FROM MonthlyReport_M_Tenant WITH (NOLOCK) 
	--	WHERE Active = 1
	--)
GROUP BY 
    R.primary_email

INSERT INTO @TicketsByUser (Requester, [Count])
SELECT 
    'ZZZ-Grand Total',
    SUM(ISNULL([Count],0))
FROM 
    @TicketsByUser;

SELECT * 
FROM @TicketsByUser
ORDER BY 
    CASE 
        WHEN Requester = 'ZZZ-Grand Total' THEN 1 
        ELSE 0 
    END,
    [Count] DESC;

END



GO
/****** Object:  StoredProcedure [dbo].[usp_FreshService_R_SummaryLast3Months]    Script Date: 09/06/2025 10:37:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE   PROC [dbo].[usp_FreshService_R_SummaryLast3Months]
(
@DepartmentId bigINT=27000109788,
@Start_Date	VARCHAR(10)='01/11/2024',
@End_Date VARCHAR(10)='31/01/2025'
)
AS
/*
Rev 1.0		Soumik		02-01-2025		Added Problem column,Only Closed/Resolved,Category total not required in row, WITHOUT CATEGORY 
*/
BEGIN

	--Rev 1.0
	DECLARE @IncludeStatus AS TABLE
	(
		StatusID INT
	)
	INSERT INTO @IncludeStatus (StatusID) VALUES (4);
	INSERT INTO @IncludeStatus (StatusID) VALUES (5);

	DECLARE @LastDateTobeConsidered DATE = (SELECT TOP 1 LastDate FROM LastDateTobeConsidered WHERE ACTIVE =1);
	--End of Rev

	DECLARE @TBL AS TABLE
	(
		departmentId bigint,
		[name]  varchar(300),
		[category] varchar(100) default(''),
		[monthname] varchar(100) default(''),
		[monthstartdate] date,
		[ChangeRequest] int default(0),
		[Incident] int default(0),
		[ServiceRequest] int default(0),
		--Rev 1.0
		[Problem] int default(0),
		--End of Rev 1.0
		GrandTotal int default(0),
		RowType int default(0)
	)
	INSERT INTO @TBL (departmentId,[name],[category],[MonthName],MonthStartDate)
	SELECT 
	distinct T.department_id,D.[name],ISNULL(T.category,'-') as category ,DT.[MonthName],DT.MonthStartDate
	FROM [FreshService_T_Tickets] T WITH(NOLOCK) 
	INNER JOIN [FreshService_M_Department] D WITH(NOLOCK) ON T.department_id=D.id
	INNER JOIN [FreshService_T_Ticket_CustomFields] TC WITH(NOLOCK) ON TC.ticket_id=T.id
	LEFT OUTER JOIN [FreshService_M_Status] S WITH(NOLOCK) ON S.StatusId=T.[status]
	CROSS JOIN (SELECT  DATENAME(MONTH, DATEADD(MONTH, x.number, convert(date,@Start_Date,103))) AS [MonthName],
			DATEADD(MONTH, x.number, convert(date,@Start_Date,103)) AS MonthStartDate
			FROM    [master].dbo.spt_values x
			WHERE   x.type = 'P'        
			AND     x.number <= DATEDIFF(MONTH, convert(date,@Start_Date,103), convert(date,@End_Date,103))
			) DT
	--WHERE T.created_at between CONVERT(DATE,@Start_Date,103) AND CONVERT(DATE,@End_Date,103)
	WHERE CAST(SWITCHOFFSET(CAST(T.created_at AS DATETIMEOFFSET), '+05:30') AS DATE) 
      BETWEEN CONVERT(DATE, @Start_Date, 103) AND CONVERT(DATE, @End_Date, 103)
	AND (T.department_id=@DepartmentId OR ISNULL(@DepartmentId,0)=0)
	--Rev 1.0
	AND T.[status] IN (SELECT StatusID FROM @IncludeStatus)
	AND (CAST(SWITCHOFFSET(CAST(T.created_at AS DATETIMEOFFSET), '+05:30') AS DATE) > @LastDateTobeConsidered OR ISNULL(@LastDateTobeConsidered,'')='')
	AND TC.tenant IN (
		SELECT TenantName 
		FROM MonthlyReport_M_Tenant WITH (NOLOCK) 
		WHERE Active = 1
	)
	--End of Rev 1.0
	UNION ALL
	SELECT  C.department_id,D.[name], ISNULL(C.category,'-') AS Category,DT.[MonthName],DT.MonthStartDate
	FROM [FreshService_T_Change] C WITH(NOLOCK) 
	INNER JOIN [FreshService_M_Department] D WITH(NOLOCK) ON C.department_id=D.id
	CROSS JOIN (SELECT  DATENAME(MONTH, DATEADD(MONTH, x.number, convert(date,@Start_Date,103))) AS [MonthName],
		DATEADD(MONTH, x.number, convert(date,@Start_Date,103)) AS MonthStartDate
		FROM    [master].dbo.spt_values x
		WHERE   x.type = 'P'        
		AND     x.number <= DATEDIFF(MONTH, convert(date,@Start_Date,103), convert(date,@End_Date,103))
		) DT
	--WHERE C.created_at between CONVERT(DATE,@Start_Date,103) AND CONVERT(DATE,@End_Date,103)
	WHERE CAST(SWITCHOFFSET(CAST(C.created_at AS DATETIMEOFFSET), '+05:30') AS DATE) 
      BETWEEN CONVERT(DATE, @Start_Date, 103) AND CONVERT(DATE, @End_Date, 103)
	AND (C.department_id=@DepartmentId OR ISNULL(@DepartmentId,0)=0)
	--Rev 1.0
	AND C.[status] IN (SELECT StatusID FROM @IncludeStatus)
	AND (CAST(SWITCHOFFSET(CAST(C.created_at AS DATETIMEOFFSET), '+05:30') AS DATE) > @LastDateTobeConsidered OR ISNULL(@LastDateTobeConsidered,'')='')
	--End of Rev 1.

	--Rev 1.0

	UNION ALL
	SELECT  P.department_id,D.[name], ISNULL(P.category,'-') AS Category,DT.[MonthName],DT.MonthStartDate
	FROM [FreshService_T_Problems] P WITH(NOLOCK) 
	INNER JOIN [FreshService_M_Department] D WITH(NOLOCK) ON P.department_id=D.id
	CROSS JOIN (SELECT  DATENAME(MONTH, DATEADD(MONTH, x.number, convert(date,@Start_Date,103))) AS [MonthName],
		DATEADD(MONTH, x.number, convert(date,@Start_Date,103)) AS MonthStartDate
		FROM    [master].dbo.spt_values x
		WHERE   x.type = 'P'        
		AND     x.number <= DATEDIFF(MONTH, convert(date,@Start_Date,103), convert(date,@End_Date,103))
		) DT
	--WHERE P.created_at between CONVERT(DATE,@Start_Date,103) AND CONVERT(DATE,@End_Date,103)
	WHERE CAST(SWITCHOFFSET(CAST(P.created_at AS DATETIMEOFFSET), '+05:30') AS DATE) 
      BETWEEN CONVERT(DATE, @Start_Date, 103) AND CONVERT(DATE, @End_Date, 103)
	AND (P.department_id=@DepartmentId OR ISNULL(@DepartmentId,0)=0)
	--Rev 1.0
	AND P.[status] IN (SELECT StatusID FROM @IncludeStatus)
	AND (CAST(SWITCHOFFSET(CAST(P.created_at AS DATETIMEOFFSET), '+05:30') AS DATE) > @LastDateTobeConsidered OR ISNULL(@LastDateTobeConsidered,'')='')
	--End of Rev 1.0
	
	SELECT * INTO #TEMP_FreshService_R_SummaryLast3Months 
	FROM 
	(SELECT 
	T.department_id,D.[name]
	,T.[type]
	,ISNULL(T.category,'-') as category 
	--,DATENAME(MONTH,T.created_at) AS [MonthName]
	,DATENAME(MONTH,CAST(SWITCHOFFSET(CAST(T.created_at AS DATETIMEOFFSET), '+05:30') AS DATE)) AS [MonthName]
	--,cast( DATEADD(day,-DATEPART(day,T.created_at)+1 ,T.created_at) as date) as MonthStartDate
	,CAST(DATEADD(DAY, -DATEPART(DAY, SWITCHOFFSET(CAST(T.created_at AS DATETIMEOFFSET), '+05:30')) + 1, 
             SWITCHOFFSET(CAST(T.created_at AS DATETIMEOFFSET), '+05:30')) AS DATE) AS MonthStartDate
	,COUNT(T.ID) AS NoOfTickets
	FROM [FreshService_T_Tickets] T WITH(NOLOCK) 
	INNER JOIN [FreshService_M_Department] D WITH(NOLOCK) ON T.department_id=D.id
	INNER JOIN [FreshService_T_Ticket_CustomFields] TC WITH(NOLOCK) ON TC.ticket_id=T.id
	LEFT OUTER JOIN [FreshService_M_Status] S WITH(NOLOCK) ON S.StatusId=T.[status]
	--WHERE T.created_at between CONVERT(DATE,@Start_Date,103) AND CONVERT(DATE,@End_Date,103)
	WHERE CAST(SWITCHOFFSET(CAST(T.created_at AS DATETIMEOFFSET), '+05:30') AS DATE) 
      BETWEEN CONVERT(DATE, @Start_Date, 103) AND CONVERT(DATE, @End_Date, 103)
	AND (T.department_id=@DepartmentId OR ISNULL(@DepartmentId,0)=0)
	--Rev 1.0
	AND T.[status] IN (SELECT StatusID FROM @IncludeStatus)
	AND (CAST(SWITCHOFFSET(CAST(T.created_at AS DATETIMEOFFSET), '+05:30') AS DATE) > @LastDateTobeConsidered OR ISNULL(@LastDateTobeConsidered,'')='')
	AND TC.tenant IN (
		SELECT TenantName 
		FROM MonthlyReport_M_Tenant WITH (NOLOCK) 
		WHERE Active = 1
	)
	--End of Rev 1.0
	GROUP BY T.department_id,D.[name],T.[type], ISNULL(T.category,'-'),DATENAME(MONTH,CAST(SWITCHOFFSET(CAST(T.created_at AS DATETIMEOFFSET), '+05:30') AS DATE))
	,CAST(DATEADD(DAY, -DATEPART(DAY, SWITCHOFFSET(CAST(T.created_at AS DATETIMEOFFSET), '+05:30')) + 1, 
             SWITCHOFFSET(CAST(T.created_at AS DATETIMEOFFSET), '+05:30')) AS DATE)

	UNION ALL
	SELECT  C.department_id,D.[name]
	,'Change Request' as [type]
	, ISNULL(C.category,'-') AS Category
	--,DATENAME(MONTH,c.created_at) AS [MonthName]
	,DATENAME(MONTH,CAST(SWITCHOFFSET(CAST(c.created_at AS DATETIMEOFFSET), '+05:30') AS DATE)) AS [MonthName]
	--,cast( DATEADD(day,-DATEPART(day,c.created_at)+1 ,c.created_at) as date) as MonthStartDate
		,CAST(DATEADD(DAY, -DATEPART(DAY, SWITCHOFFSET(CAST(c.created_at AS DATETIMEOFFSET), '+05:30')) + 1, 
             SWITCHOFFSET(CAST(c.created_at AS DATETIMEOFFSET), '+05:30')) AS DATE) AS MonthStartDate
	,COUNT(C.ID) AS NoOfTickets
	FROM [FreshService_T_Change] C WITH(NOLOCK) 
	INNER JOIN [FreshService_M_Department] D WITH(NOLOCK) ON C.department_id=D.id
	--WHERE C.created_at between CONVERT(DATE,@Start_Date,103) AND CONVERT(DATE,@End_Date,103)
	WHERE CAST(SWITCHOFFSET(CAST(C.created_at AS DATETIMEOFFSET), '+05:30') AS DATE) 
      BETWEEN CONVERT(DATE, @Start_Date, 103) AND CONVERT(DATE, @End_Date, 103)
	AND (C.department_id=@DepartmentId OR ISNULL(@DepartmentId,0)=0)
	--Rev 1.0
	AND C.[status] IN (SELECT StatusID FROM @IncludeStatus)
	AND (CAST(SWITCHOFFSET(CAST(C.created_at AS DATETIMEOFFSET), '+05:30') AS DATE) > @LastDateTobeConsidered OR ISNULL(@LastDateTobeConsidered,'')='')
	--End of Rev 1.0
	GROUP BY C.department_id,D.[name],ISNULL(C.category,'-'),DATENAME(MONTH,CAST(SWITCHOFFSET(CAST(c.created_at AS DATETIMEOFFSET), '+05:30') AS DATE))
	,CAST(DATEADD(DAY, -DATEPART(DAY, SWITCHOFFSET(CAST(c.created_at AS DATETIMEOFFSET), '+05:30')) + 1, 
     SWITCHOFFSET(CAST(c.created_at AS DATETIMEOFFSET), '+05:30')) AS DATE)
	
	--Rev 1.0
	UNION ALL
	SELECT  P.department_id,D.[name]
	,'Problem' as [type]
	, ISNULL(P.category,'-') AS Category
	--,DATENAME(MONTH,P.created_at) AS [MonthName]
	,DATENAME(MONTH,CAST(SWITCHOFFSET(CAST(P.created_at AS DATETIMEOFFSET), '+05:30') AS DATE)) AS [MonthName]
	--,cast( DATEADD(day,-DATEPART(day,P.created_at)+1 ,P.created_at) as date) as MonthStartDate
		,CAST(DATEADD(DAY, -DATEPART(DAY, SWITCHOFFSET(CAST(p.created_at AS DATETIMEOFFSET), '+05:30')) + 1, 
             SWITCHOFFSET(CAST(p.created_at AS DATETIMEOFFSET), '+05:30')) AS DATE) AS MonthStartDate
	,COUNT(P.ID) AS NoOfTickets
	FROM [FreshService_T_Problems] P WITH(NOLOCK) 
	INNER JOIN [FreshService_M_Department] D WITH(NOLOCK) ON P.department_id=D.id
	--WHERE P.created_at between CONVERT(DATE,@Start_Date,103) AND CONVERT(DATE,@End_Date,103)
	WHERE CAST(SWITCHOFFSET(CAST(P.created_at AS DATETIMEOFFSET), '+05:30') AS DATE) 
      BETWEEN CONVERT(DATE, @Start_Date, 103) AND CONVERT(DATE, @End_Date, 103)
	AND (P.department_id=@DepartmentId OR ISNULL(@DepartmentId,0)=0)
	AND P.[status] IN (SELECT StatusID FROM @IncludeStatus)
	AND (CAST(SWITCHOFFSET(CAST(P.created_at AS DATETIMEOFFSET), '+05:30') AS DATE) > @LastDateTobeConsidered OR ISNULL(@LastDateTobeConsidered,'')='')
	GROUP BY P.department_id,D.[name],ISNULL(P.category,'-'),DATENAME(MONTH,CAST(SWITCHOFFSET(CAST(P.created_at AS DATETIMEOFFSET), '+05:30') AS DATE)) 
	,CAST(DATEADD(DAY, -DATEPART(DAY, SWITCHOFFSET(CAST(p.created_at AS DATETIMEOFFSET), '+05:30')) + 1, 
             SWITCHOFFSET(CAST(p.created_at AS DATETIMEOFFSET), '+05:30')) AS DATE)
	--End of Rev 1.0
	) X


	UPDATE A
	SET Incident=(ISNULL(B.NOOFTICKETS,0))
	FROM @TBL A,
	(select x.department_id,x.category,x.MonthName,sum(ISNULL(x.NOOFTICKETS,0)) as NOOFTICKETS
	from
	#TEMP_FreshService_R_SummaryLast3Months x
	where x.[type]='Incident'
	group by x.department_id,x.category,x.MonthName
	) B
	WHERE A.departmentId=B.department_id
	AND A.category=B.category
	AND A.[MonthName]=B.[MonthName]
	

	UPDATE A
	SET ServiceRequest=(ISNULL(B.NOOFTICKETS,0))
	FROM @TBL A,
	(select x.department_id,x.category,x.MonthName,sum(ISNULL(x.NOOFTICKETS,0)) as NOOFTICKETS
	from
	#TEMP_FreshService_R_SummaryLast3Months x
	where x.[type]='Service Request'
	group by x.department_id,x.category,x.MonthName
	) B
	WHERE A.departmentId=B.department_id
	AND A.category=B.category
	AND A.[MonthName]=B.[MonthName]


	UPDATE A
	SET ChangeRequest=(ISNULL(B.NOOFTICKETS,0))
	FROM @TBL A,
	(select x.department_id,x.category,x.MonthName,sum(ISNULL(x.NOOFTICKETS,0)) as NOOFTICKETS
	from
	#TEMP_FreshService_R_SummaryLast3Months x
	where x.[type]='Change Request'
	group by x.department_id,x.category,x.MonthName
	) B
	WHERE A.departmentId=B.department_id
	AND A.category=B.category
	AND A.[MonthName]=B.[MonthName]

	--Rev 1.0
	UPDATE A
	SET Problem =(ISNULL(B.NOOFTICKETS,0))
	FROM @TBL A,
	(select x.department_id,x.category,x.MonthName,sum(ISNULL(x.NOOFTICKETS,0)) as NOOFTICKETS
	from
	#TEMP_FreshService_R_SummaryLast3Months x
	where x.[type]='Problem'
	group by x.department_id,x.category,x.MonthName
	) B
	WHERE A.departmentId=B.department_id
	AND A.category=B.category
	AND A.[MonthName]=B.[MonthName]
	--End of Rev 1.0

	--Rev 1.0
	--INSERT INTO @TBL (departmentId,[name],category,ChangeRequest,Incident,ServiceRequest,Problem,RowType)
	--SELECT departmentId,[name],category+ ' Total' as category,SUM(ChangeRequest) AS ChangeRequest
	--,SUM(Incident) AS Incident
	--,SUM(ServiceRequest) AS ServiceRequest
	----Rev 1.0
	--,SUM(Problem) AS Problem
	----End of Rev 1.0
	--,1
	--FROM @TBL
	--GROUP BY departmentId,[name],category
	--End of Rev 1.0



	INSERT INTO @TBL (departmentId,[name],category,ChangeRequest,Incident,ServiceRequest,Problem,RowType)
	SELECT departmentId,[name],'ZZZ-Grand Total' AS category,SUM(ChangeRequest) AS ChangeRequest
	,SUM(Incident) AS Incident
	,SUM(ServiceRequest) AS ServiceRequest
	--Rev 1.0
	,SUM(Problem) AS Problem
	--End of Rev 1.0
	,2
	FROM @TBL
	where RowType=0
	GROUP BY departmentId,[name]


    UPDATE @TBL SET GrandTotal=ISNULL(ChangeRequest,0)+ISNULL(Incident,0)+ISNULL(ServiceRequest,0)+ISNULL(Problem,0)
	
	
	SELECT 
		departmentId,
		[name],
		[category],
		[monthname] ,
		[monthstartdate] ,
		[ChangeRequest],
		[Incident],
		[ServiceRequest],
		--Rev 1.0
		Problem,
		--End of Rev 1.0
		GrandTotal,
		RowType
	
	FROM @TBL
	ORDER BY [name],[category],[monthstartdate]


	SELECT * INTO #TEMP_FreshService_R_DetailsLast3Months
	from(
	select x.department_id,x.[name],x.[type],x.[category]
	,case when x.[MonthName]=dt.[MonthName] then  x.NoOfTickets else 0 end as NoOfTickets
	,dt.[MonthName],dt.MonthStartDate ,0 as RowType
	from 
	(select distinct  T.department_id,T.[name],T.[type],t.category,t.[MonthName],t.MonthStartDate,t.NoOfTickets
	from #TEMP_FreshService_R_SummaryLast3Months t
	)x
	cross join (SELECT  DATENAME(MONTH, DATEADD(MONTH, x.number, convert(date,@Start_Date,103))) AS [MonthName],
			DATEADD(MONTH, x.number, convert(date,@Start_Date,103)) AS MonthStartDate
			FROM    [master].dbo.spt_values x
			WHERE   x.type = 'P'        
			AND     x.number <= DATEDIFF(MONTH, convert(date,@Start_Date,103), convert(date,@End_Date,103))
			) DT) y

	INSERT INTO #TEMP_FreshService_R_DetailsLast3Months(department_id,[name],[type],[category],NoOfTickets,[MonthName],MonthStartDate,RowType)
	SELECT department_id,[name],[type],[category],SUM(ISNULL(NoOfTickets,0)) AS NoOfTickets,'Grand Total', MAX(MonthStartDate) as MonthStartDate,1 AS RowType
	FROM #TEMP_FreshService_R_DetailsLast3Months 
	GROUP BY department_id,[name],[type],[category]


	select * from #TEMP_FreshService_R_DetailsLast3Months
	ORDER BY [TYPE],[category],[MonthStartDate],RowType

	--Rev 1.0
	UPDATE @TBL
	SET [monthname] = category
	WHERE category = 'ZZZ-Grand Total'

		SELECT 
		[departmentId],
		[name],
		[monthname] ,
		[monthstartdate],
		SUM([ChangeRequest]) AS ChangeRequest,
		SUM([Incident]) AS Incident,
		SUM([ServiceRequest]) AS ServiceRequest,
		SUM([Problem]) AS Problem,
		SUM([GrandTotal]) AS GrandTotal,
		[RowType]
		
	FROM @TBL
	GROUP BY [departmentId],[name],[monthname],[monthstartdate],[RowType]
	ORDER BY 
    CASE 
        WHEN [monthname] = 'ZZZ-Grand Total' THEN 1 
        ELSE 0 
    END,
    [name],[monthstartdate];
	--End of Rev 1.0

			
	DROP TABLE #TEMP_FreshService_R_SummaryLast3Months

	
	DROP TABLE #TEMP_FreshService_R_DetailsLast3Months
	
END
GO
/****** Object:  StoredProcedure [dbo].[usp_FreshService_R_SummaryReport_PIVOT]    Script Date: 09/06/2025 10:37:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE   PROC [dbo].[usp_FreshService_R_SummaryReport_PIVOT]
(@DepartmentId BIGINT=27000586401,
@Start_Date	VARCHAR(10)='01/01/2025',
@End_Date VARCHAR(10)='30/01/2025'
)
AS
BEGIN
/*
Rev 1.0		Soumik		02-01-2025		Added Problem column, display columns as rows,Only Closed/Resolved,GMT TO IST, 
*/

DROP TABLE IF EXISTS #FreshService_R_SummaryReport  

	--Rev 1.0
	DECLARE @IncludeStatus AS TABLE
	(
		StatusID INT
	)
	--Only Closed / Resolved 
	INSERT INTO @IncludeStatus (StatusID) VALUES (4);
	INSERT INTO @IncludeStatus (StatusID) VALUES (5);

	DECLARE @LastDateTobeConsidered DATE = (SELECT TOP 1 LastDate FROM LastDateTobeConsidered WHERE ACTIVE =1);
	--End of Rev
  
CREATE TABLE #FreshService_R_SummaryReport  
(  
	departmentid bigint,
	[name] varchar(100),
	[type] varchar(100),
	[category] varchar(100),
	[categoryId] int,
	nooftickets int ,
	RowType INT DEFAULT(0)
)  
INSERT INTO #FreshService_R_SummaryReport ([name],departmentid,[type],[category],nooftickets)

  SELECT  D.[name], T.department_id,T.[type],ISNULL(T.category,'-') AS Category,count(T.id) as NoOfTickets
	FROM [FreshService_T_Tickets] T WITH(NOLOCK) 
	INNER JOIN [FreshService_M_Department] D WITH(NOLOCK) ON T.department_id=D.id
	INNER JOIN [FreshService_T_Ticket_CustomFields] TC WITH(NOLOCK) ON TC.ticket_id=T.id
	--Rev 1.0
	--WHERE T.created_at between CONVERT(DATE,@Start_Date,103) AND CONVERT(DATE,@End_Date,103)
	WHERE CAST(SWITCHOFFSET(CAST(T.created_at AS DATETIMEOFFSET), '+05:30') AS DATE) 
      BETWEEN CONVERT(DATE, @Start_Date, 103) AND CONVERT(DATE, @End_Date, 103)
	  AND T.[status] IN (SELECT StatusID FROM @IncludeStatus)
	  AND (CAST(SWITCHOFFSET(CAST(T.created_at AS DATETIMEOFFSET), '+05:30') AS DATE) > @LastDateTobeConsidered OR ISNULL(@LastDateTobeConsidered,'')='')
	--End of Rev 1.0
	AND (T.department_id=@DepartmentId OR ISNULL(@DepartmentId,0)=0)
	AND TC.tenant IN (
		SELECT TenantName 
		FROM MonthlyReport_M_Tenant WITH (NOLOCK) 
		WHERE Active = 1
	)
	GROUP BY D.[name], T.department_id,T.[type],ISNULL(T.category,'-')
	
	UNION  all
	SELECT  D.[name], C.department_id,'Change Request' as [type],ISNULL(C.category,'-') AS Category,count(C.id) as NoOfTickets
	FROM [FreshService_T_Change] C WITH(NOLOCK) 
	INNER JOIN [FreshService_M_Department] D WITH(NOLOCK) ON C.department_id=D.id
	--Rev 1.0
	--WHERE C.created_at between CONVERT(DATE,@Start_Date,103) AND CONVERT(DATE,@End_Date,103)
	WHERE CAST(SWITCHOFFSET(CAST(C.created_at AS DATETIMEOFFSET), '+05:30') AS DATE) 
      BETWEEN CONVERT(DATE, @Start_Date, 103) AND CONVERT(DATE, @End_Date, 103)
	  AND C.[status] IN (SELECT StatusID FROM @IncludeStatus) 
	  AND (CAST(SWITCHOFFSET(CAST(C.created_at AS DATETIMEOFFSET), '+05:30') AS DATE) > @LastDateTobeConsidered OR ISNULL(@LastDateTobeConsidered,'')='')
	--End of Rev 1.0
	AND (C.department_id=@DepartmentId OR ISNULL(@DepartmentId,0)=0)
	GROUP BY D.[name], C.department_id,ISNULL(C.category,'-')

	-- Rev 1.0
	UNION ALL
	SELECT D.[name],p.department_id,'Problem' as [type],ISNULL(P.category,'-') AS Category,count(P.id) as NoOfTickets 
	FROM [FreshService_T_Problems] P WITH(NOLOCK)
	INNER JOIN [FreshService_M_Department] D WITH(NOLOCK) ON P.department_id=D.id
	--Rev 1.0
	--WHERE P.created_at between CONVERT(DATE,@Start_Date,103) AND CONVERT(DATE,@End_Date,103)
	WHERE CAST(SWITCHOFFSET(CAST(P.created_at AS DATETIMEOFFSET), '+05:30') AS DATE) 
      BETWEEN CONVERT(DATE, @Start_Date, 103) AND CONVERT(DATE, @End_Date, 103)
	  AND P.[status] IN (SELECT StatusID FROM @IncludeStatus)
	  AND (CAST(SWITCHOFFSET(CAST(P.created_at AS DATETIMEOFFSET), '+05:30') AS DATE) > @LastDateTobeConsidered OR ISNULL(@LastDateTobeConsidered,'')='')
	--End of Rev 1.0
	AND (P.department_id=@DepartmentId OR ISNULL(@DepartmentId,0)=0)
	GROUP BY D.[name], P.department_id,ISNULL(P.category,'-')
	-- End of Rev 1.0
	

	ORDER BY [name],[type],category
  
  INSERT INTO #FreshService_R_SummaryReport ([name],departmentid,[type],[category],nooftickets)
  SELECT [NAME], departmentid,'ZZZ-Grand Total',[category],sum(nooftickets) FROM #FreshService_R_SummaryReport
  GROUP BY departmentid,[NAME], [category]

  --update #FreshService_R_SummaryReport set RowType=1 where [type]='ZZZ-Grand Total'
  INSERT INTO #FreshService_R_SummaryReport ([name],departmentid,[type],[category],nooftickets)
  SELECT [NAME], departmentid,[type],'ZZZ-Grand Total',sum(nooftickets)FROM #FreshService_R_SummaryReport
  GROUP BY departmentid,[NAME], [type]
   --update #FreshService_R_SummaryReport set RowType=2 where [type]='ZZZ-Grand Total' and [category]='ZZZ-Grand Total'
  
 

DECLARE @COLUMNS AS NVARCHAR(MAX)  
DECLARE @QUERY  AS NVARCHAR(MAX)  
 
 --Rev 1.0
 --SET @COLUMNS = STUFF((SELECT distinct ',' + QUOTENAME([category])   
 --           FROM #FreshService_R_SummaryReport  
	--		--ORDER BY categoryId
 --           FOR XML PATH(''), TYPE  
 --           ).value('.', 'NVARCHAR(MAX)')   
 --       ,1,1,'')  

 SET @COLUMNS = STUFF((SELECT distinct ',' + QUOTENAME([TYPE])   
            FROM #FreshService_R_SummaryReport  
			--ORDER BY categoryId
            FOR XML PATH(''), TYPE  
            ).value('.', 'NVARCHAR(MAX)')   
        ,1,1,'')

		--set @query = 'SELECT	departmentid,[name],[type],' + @COLUMNS + '   
		--	FROM	#FreshService_R_SummaryReport  
		--	pivot   
		--	( min([nooftickets]) 
		--		for [category] in (' + @COLUMNS + ')  
		--	) p   
		--	ORDER BY [type] ASC' 


		set @query = 'SELECT	departmentid,[name],[category],' + @COLUMNS + '   
			FROM	#FreshService_R_SummaryReport  
			pivot   
			( min([nooftickets]) 
				for [TYPE] in (' + @COLUMNS + ')  
			) p   
			ORDER BY [category] ASC'   
 --End of Rev 1.0
  

  
	--SELECT @query
execute(@query)  

 UPDATE a
  SET categoryId=B.Slno
  FROM #FreshService_R_SummaryReport A,
  (SELECT ROW_NUMBER() OVER(ORDER BY X.category ASC) AS Slno,x.category
  FROM (
  SELECT DISTINCT category FROM #FreshService_R_SummaryReport
  )  X) B
  WHERE A.category=B.category

select departmentid,[name],[type], [category],categoryId,nooftickets, case when ([category]='ZZZ-Grand Total') then 1  else 0 end as RowType from #FreshService_R_SummaryReport
ORDER BY  [NAME],[TYPE],category

END
GO
/****** Object:  StoredProcedure [dbo].[usp_FreshService_R_SummaryResolutionPrioritySLA]    Script Date: 09/06/2025 10:37:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO



CREATE   PROC [dbo].[usp_FreshService_R_SummaryResolutionPrioritySLA]
(
@DepartmentId bigINT=27000112738,
@Start_Date	VARCHAR(10)='01/11/2024',
@End_Date VARCHAR(10)='30/12/2024'
)
AS
/*
Rev 1.0		Soumik		02-01-2025		CALCULATE ACHIVED PERCENTANGE,Only Closed/Resolved TICKETS, GMT - IST
*/
BEGIN

	--Rev 1.0
	DECLARE @IncludeStatus AS TABLE
	(
		StatusID INT
	)
	--Only Closed / Resolved 
	INSERT INTO @IncludeStatus (StatusID) VALUES (4);
	INSERT INTO @IncludeStatus (StatusID) VALUES (5);

	DECLARE @LastDateTobeConsidered DATE = (SELECT TOP 1 LastDate FROM LastDateTobeConsidered WHERE ACTIVE =1);

	--End of Rev

	DECLARE @TBL AS TABLE
	(
		departmentId bigint,
		[name]  varchar(300),
		[type] varchar(100),
		[statustype] varchar(100),
		[Urgent] int default(0),
		[High] int default(0),
		[Medium] int default(0),
		[Low] int default(0),
		GrandTotal int default(0),
		AchievedPercentage varchar(100)
	)
	INSERT INTO @TBL (departmentId,[name],[type],statustype)
	SELECT d.id,d.[name], 'Incident' as [type],'SLA Violated' [statustype] 
	FROM [FreshService_M_Department] D WITH(NOLOCK) 
	WHERE (D.id=@DepartmentId OR  ISNULL(@DepartmentId,0)=0)
	UNION
	SELECT d.id,d.[name], 'Incident' as [type],'Within SLA' [statustype] 
	FROM [FreshService_M_Department] D WITH(NOLOCK) 
	WHERE (D.id=@DepartmentId OR  ISNULL(@DepartmentId,0)=0)
	UNION
	SELECT d.id,d.[name], 'Service Request' as [type],'SLA Violated' [statustype] 
	FROM [FreshService_M_Department] D WITH(NOLOCK) 
	WHERE (D.id=@DepartmentId OR  ISNULL(@DepartmentId,0)=0)
	UNION
	SELECT d.id,d.[name], 'Service Request' as [type],'Within SLA' [statustype] 
	FROM [FreshService_M_Department] D WITH(NOLOCK) 
	WHERE (D.id=@DepartmentId OR  ISNULL(@DepartmentId,0)=0)

	UNION
	SELECT d.id,d.[name], 'ZZZ-Grand Total' as [type],'' [statustype] 
	FROM [FreshService_M_Department] D WITH(NOLOCK) 
	WHERE (D.id=@DepartmentId OR  ISNULL(@DepartmentId,0)=0)

	SELECT * INTO #TEMP_TABLE_FreshService_T_Tickets
	from
	(SELECT  D.[name], T.department_id,T.[type],T.[priority],isnull(t.is_escalated,0) AS is_escalated,count(T.id) as NoOfTickets
	FROM [FreshService_T_Tickets] T WITH(NOLOCK) 
	INNER JOIN [FreshService_M_Department] D WITH(NOLOCK) ON T.department_id=D.id
	--Rev 1.0
	--WHERE T.created_at between CONVERT(DATE,@Start_Date,103) AND CONVERT(DATE,@End_Date,103)
	INNER JOIN [FreshService_T_Ticket_CustomFields] TC WITH(NOLOCK) ON T.id = TC.ticket_id
	WHERE CAST(SWITCHOFFSET(CAST(T.created_at AS DATETIMEOFFSET), '+05:30') AS DATE) 
      BETWEEN CONVERT(DATE, @Start_Date, 103) AND CONVERT(DATE, @End_Date, 103)
	AND T.[status] IN (SELECT StatusID FROM @IncludeStatus)
	AND (CAST(SWITCHOFFSET(CAST(T.created_at AS DATETIMEOFFSET), '+05:30') AS DATE) > @LastDateTobeConsidered OR ISNULL(@LastDateTobeConsidered,'')='')
	--End of Rev 1.0 
	AND (T.department_id=@DepartmentId OR ISNULL(@DepartmentId,0)=0)
	AND TC.tenant IN (
		SELECT TenantName 
		FROM MonthlyReport_M_Tenant WITH (NOLOCK) 
		WHERE Active = 1
	)
	GROUP BY D.[name], T.department_id,T.[type],T.[priority],isnull(t.is_escalated,0))
	x

	UPDATE @TBL
	SET Urgent=b.NoOfTickets
	FROM @TBL A,
	#TEMP_TABLE_FreshService_T_Tickets B
	WHERE A.departmentId=B.department_id AND A.[type]=B.[type] AND A.statustype='Within SLA'
	and b.[priority]=4
	AND B.is_escalated=0
	

	UPDATE @TBL
	SET Urgent=b.NoOfTickets
	FROM @TBL A,
	#TEMP_TABLE_FreshService_T_Tickets B
	WHERE A.departmentId=B.department_id AND A.[type]=B.[type] AND A.statustype='SLA Violated'
	and b.[priority]=4
	AND B.is_escalated=1

	UPDATE @TBL
	SET [high]=b.NoOfTickets
	FROM @TBL A,
	#TEMP_TABLE_FreshService_T_Tickets B
	WHERE A.departmentId=B.department_id AND A.[type]=B.[type] AND A.statustype='Within SLA'
	and b.[priority]=3
	AND B.is_escalated=0
	

	UPDATE @TBL
	SET [high]=b.NoOfTickets
	FROM @TBL A,
	#TEMP_TABLE_FreshService_T_Tickets B
	WHERE A.departmentId=B.department_id AND A.[type]=B.[type] AND A.statustype='SLA Violated'
	and b.[priority]=3
	AND B.is_escalated=1

	UPDATE @TBL
	SET [Medium]=b.NoOfTickets
	FROM @TBL A,
	#TEMP_TABLE_FreshService_T_Tickets B
	WHERE A.departmentId=B.department_id AND A.[type]=B.[type] AND A.statustype='Within SLA'
	and b.[priority]=2
	AND B.is_escalated=0
	

	UPDATE @TBL
	SET [Medium]=b.NoOfTickets
	FROM @TBL A,
	#TEMP_TABLE_FreshService_T_Tickets B
	WHERE A.departmentId=B.department_id AND A.[type]=B.[type] AND A.statustype='SLA Violated'
	and b.[priority]=2
	AND B.is_escalated=1

	UPDATE @TBL
	SET [Low]=b.NoOfTickets
	FROM @TBL A,
	#TEMP_TABLE_FreshService_T_Tickets B
	WHERE A.departmentId=B.department_id AND A.[type]=B.[type] AND A.statustype='Within SLA'
	and b.[priority]=1
	AND B.is_escalated=0
	

	UPDATE @TBL
	SET [Low]=b.NoOfTickets
	FROM @TBL A,
	#TEMP_TABLE_FreshService_T_Tickets B
	WHERE A.departmentId=B.department_id AND A.[type]=B.[type] AND A.statustype='SLA Violated'
	and b.[priority]=1
	AND B.is_escalated=1

	update @TBL
	set GrandTotal=isnull(urgent,0)+isnull([high],0)+isnull([medium],0)+isnull([low],0)
	
	update @TBL
	SET Urgent=B.urgent,
		[High]=B.[high],
		[Medium]=B.[medium],
		[Low]=b.[Low],
		[GrandTotal]=b.GrandTotal
	from  @TBL a,

	(select departmentId,sum(isnull(urgent,0)) as urgent,sum(isnull([high],0)) as [high],sum(isnull([medium],0)) as [medium],sum(isnull([low],0)) as [low],sum(isnull([GrandTotal],0)) as [GrandTotal]
	from @TBL group by departmentId) B
	WHERE A.departmentId=B.departmentId
	AND A.[type]='ZZZ-Grand Total'

	--Rev 1.0
			;WITH CTE_Aggregate AS
				(
					SELECT 
						departmentId,
						[type],
						SUM(CASE WHEN statustype IN ('SLA Violated', 'Within SLA') THEN GrandTotal ELSE 0 END) AS Total, -- SLA Violated + Within SLA
						SUM(CASE WHEN statustype = 'Within SLA' THEN GrandTotal ELSE 0 END) AS WithinSLA
					FROM @TBL
					GROUP BY departmentId, [type]
				),
				-- Calculate totals for Within SLA and ZZZ-Grand Total
				CTE_Combined AS
				(
					SELECT 
						departmentId,
						[type],
						SUM(CASE WHEN [type] IN ('Incident', 'Service Request') AND statustype = 'Within SLA' THEN GrandTotal ELSE 0 END) OVER (PARTITION BY departmentId) AS TotalWithinSLA,
						SUM(CASE WHEN [type] = 'ZZZ-Grand Total' THEN GrandTotal ELSE 0 END) OVER (PARTITION BY departmentId) AS ZZZGrandTotal
					FROM @TBL
				)
				-- Update AchievedPercentage for all rows
				UPDATE T
				SET AchievedPercentage = 
					CASE 
						WHEN T.[type] IN ('Incident', 'Service Request') THEN 
							CASE 
								WHEN A.Total = 0 THEN '0'
								ELSE CAST(ROUND((CAST(A.WithinSLA AS DECIMAL) / A.Total * 100.0), 0) AS INT)
							END
						WHEN T.[type] = 'ZZZ-Grand Total' THEN
							CASE 
								WHEN C.ZZZGrandTotal = 0 THEN '0'
								ELSE CAST(ROUND((CAST(C.TotalWithinSLA AS DECIMAL) / C.ZZZGrandTotal * 100.0), 0) AS INT)
							END
						ELSE NULL
					END
				FROM @TBL T
				LEFT JOIN CTE_Aggregate A
					ON T.departmentId = A.departmentId AND T.[type] = A.[type]
				LEFT JOIN CTE_Combined C
					ON T.departmentId = C.departmentId AND T.[type] = C.[type];
	--End of Rev 1.0 



	select * from @TBL
	order by [name],[type],[statustype]

	

	SELECT 
	ROW_NUMBER() OVER(ORDER BY T.created_at ASC) AS SlNo
	,T.id
	,T.created_at
	--Rev 1.0
	--,FORMAT(T.created_at, 'dd-MM-yyyy HH:mm:ss') AS 'created_at_display'
	,FORMAT(SWITCHOFFSET(CAST(T.created_at AS DATETIMEOFFSET), '+05:30'), 'dd-MM-yyyy HH:mm:ss') AS 'created_at_display'
	--End of Rev 1.0
	, T.[type], T.[subject],T.[status],S.StatusName,TC.on_roaster_engineer,TC.resolution_remarks, D.[name], T.department_id
	FROM [FreshService_T_Tickets] T WITH(NOLOCK) 
	INNER JOIN [FreshService_M_Department] D WITH(NOLOCK) ON T.department_id=D.id
	INNER JOIN [FreshService_T_Ticket_CustomFields] TC WITH(NOLOCK) ON TC.ticket_id=T.id
	LEFT OUTER JOIN [FreshService_M_Status] S WITH(NOLOCK) ON S.StatusId=T.[status]
	--Rev 1.0
	--WHERE T.created_at between CONVERT(DATE,@Start_Date,103) AND CONVERT(DATE,@End_Date,103)
	WHERE CAST(SWITCHOFFSET(CAST(T.created_at AS DATETIMEOFFSET), '+05:30') AS DATE) 
      BETWEEN CONVERT(DATE, @Start_Date, 103) AND CONVERT(DATE, @End_Date, 103)
	AND T.[status] IN (SELECT StatusID FROM @IncludeStatus)
	AND ISNULL(T.is_escalated,0)=1
	AND (CAST(SWITCHOFFSET(CAST(T.created_at AS DATETIMEOFFSET), '+05:30') AS DATE) > @LastDateTobeConsidered OR ISNULL(@LastDateTobeConsidered,'')='')
	--End of Rev 1.0
	AND (T.department_id=@DepartmentId OR ISNULL(@DepartmentId,0)=0)
	--AND ISNULL(T.is_escalated,0)=1
	AND TC.tenant IN (
		SELECT TenantName 
		FROM MonthlyReport_M_Tenant WITH (NOLOCK) 
		WHERE Active = 1
	)
	
	
    ORDER BY T.created_at ASC
	DROP TABLE #TEMP_TABLE_FreshService_T_Tickets
END
GO
/****** Object:  StoredProcedure [dbo].[usp_FreshService_R_SummaryResponsePrioritySLA]    Script Date: 09/06/2025 10:37:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO




CREATE   PROC [dbo].[usp_FreshService_R_SummaryResponsePrioritySLA]
(
@DepartmentId bigINT=27000111932,
@Start_Date	VARCHAR(10)='01/01/2024',
@End_Date VARCHAR(10)='30/11/2024'
)
AS
/*
Rev 1.0		Soumik		02-01-2025		CALCULATE ACHIVED PERCENTANGE,Only Closed/Resolved TICKETS,GMT TO IST
*/
BEGIN

	--Rev 1.0
	DECLARE @IncludeStatus AS TABLE
	(
		StatusID INT
	)
	--Only Closed / Resolved 
	INSERT INTO @IncludeStatus (StatusID) VALUES (4);
	INSERT INTO @IncludeStatus (StatusID) VALUES (5);
	DECLARE @LastDateTobeConsidered DATE = (SELECT TOP 1 LastDate FROM LastDateTobeConsidered WHERE ACTIVE =1);
	--End of Rev

	DECLARE @TBL AS TABLE
	(
		departmentId bigint,
		[name]  varchar(300),
		[type] varchar(100),
		[statustype] varchar(100),
		[Urgent] int default(0),
		[High] int default(0),
		[Medium] int default(0),
		[Low] int default(0),
		GrandTotal int default(0),
		AchievedPercentage varchar(100)
	)
	INSERT INTO @TBL (departmentId,[name],[type],statustype)
	SELECT d.id,d.[name], 'Incident' as [type],'SLA Violated' [statustype] 
	FROM [FreshService_M_Department] D WITH(NOLOCK) 
	WHERE (D.id=@DepartmentId OR  ISNULL(@DepartmentId,0)=0)
	UNION
	SELECT d.id,d.[name], 'Incident' as [type],'Within SLA' [statustype] 
	FROM [FreshService_M_Department] D WITH(NOLOCK) 
	WHERE (D.id=@DepartmentId OR  ISNULL(@DepartmentId,0)=0)
	UNION
	SELECT d.id,d.[name], 'Service Request' as [type],'SLA Violated' [statustype] 
	FROM [FreshService_M_Department] D WITH(NOLOCK) 
	WHERE (D.id=@DepartmentId OR  ISNULL(@DepartmentId,0)=0)
	UNION
	SELECT d.id,d.[name], 'Service Request' as [type],'Within SLA' [statustype] 
	FROM [FreshService_M_Department] D WITH(NOLOCK) 
	WHERE (D.id=@DepartmentId OR  ISNULL(@DepartmentId,0)=0)

	UNION
	SELECT d.id,d.[name], 'ZZZ-Grand Total' as [type],'' [statustype] 
	FROM [FreshService_M_Department] D WITH(NOLOCK) 
	WHERE (D.id=@DepartmentId OR  ISNULL(@DepartmentId,0)=0)

	SELECT * INTO #TEMP_TABLE_FreshService_T_Tickets
	from
	(SELECT  D.[name], T.department_id,T.[type],T.[priority],isnull(t.fr_escalated,0) AS fr_escalated,count(T.id) as NoOfTickets
	FROM [FreshService_T_Tickets] T WITH(NOLOCK) 
	INNER JOIN [FreshService_M_Department] D WITH(NOLOCK) ON T.department_id=D.id
	INNER JOIN [FreshService_T_Ticket_CustomFields] TC WITH(NOLOCK) ON TC.ticket_id=T.id
	--Rev 1.0 
	--WHERE T.created_at between CONVERT(DATE,@Start_Date,103) AND CONVERT(DATE,@End_Date,103)
	WHERE CAST(SWITCHOFFSET(CAST(T.created_at AS DATETIMEOFFSET), '+05:30') AS DATE) 
      BETWEEN CONVERT(DATE, @Start_Date, 103) AND CONVERT(DATE, @End_Date, 103)
	  AND T.[status] IN (SELECT StatusID FROM @IncludeStatus)
	  AND (CAST(SWITCHOFFSET(CAST(T.created_at AS DATETIMEOFFSET), '+05:30') AS DATE) > @LastDateTobeConsidered OR ISNULL(@LastDateTobeConsidered,'')='')
	--End of Rev 1.0
	AND (T.department_id=@DepartmentId OR ISNULL(@DepartmentId,0)=0)
	AND TC.tenant IN (
		SELECT TenantName 
		FROM MonthlyReport_M_Tenant WITH (NOLOCK) 
		WHERE Active = 1
	)

	GROUP BY D.[name], T.department_id,T.[type],T.[priority],isnull(t.fr_escalated,0))
	x

	UPDATE @TBL
	SET Urgent=b.NoOfTickets
	FROM @TBL A,
	#TEMP_TABLE_FreshService_T_Tickets B
	WHERE A.departmentId=B.department_id AND A.[type]=B.[type] AND A.statustype='Within SLA'
	and b.[priority]=4
	AND B.fr_escalated=0
	

	UPDATE @TBL
	SET Urgent=b.NoOfTickets
	FROM @TBL A,
	#TEMP_TABLE_FreshService_T_Tickets B
	WHERE A.departmentId=B.department_id AND A.[type]=B.[type] AND A.statustype='SLA Violated'
	and b.[priority]=4
	AND B.fr_escalated=1

	UPDATE @TBL
	SET [high]=b.NoOfTickets
	FROM @TBL A,
	#TEMP_TABLE_FreshService_T_Tickets B
	WHERE A.departmentId=B.department_id AND A.[type]=B.[type] AND A.statustype='Within SLA'
	and b.[priority]=3
	AND B.fr_escalated=0
	

	UPDATE @TBL
	SET [high]=b.NoOfTickets
	FROM @TBL A,
	#TEMP_TABLE_FreshService_T_Tickets B
	WHERE A.departmentId=B.department_id AND A.[type]=B.[type] AND A.statustype='SLA Violated'
	and b.[priority]=3
	AND B.fr_escalated=1

	UPDATE @TBL
	SET [Medium]=b.NoOfTickets
	FROM @TBL A,
	#TEMP_TABLE_FreshService_T_Tickets B
	WHERE A.departmentId=B.department_id AND A.[type]=B.[type] AND A.statustype='Within SLA'
	and b.[priority]=2
	AND B.fr_escalated=0
	

	UPDATE @TBL
	SET [Medium]=b.NoOfTickets
	FROM @TBL A,
	#TEMP_TABLE_FreshService_T_Tickets B
	WHERE A.departmentId=B.department_id AND A.[type]=B.[type] AND A.statustype='SLA Violated'
	and b.[priority]=2
	AND B.fr_escalated=1

	UPDATE @TBL
	SET [Low]=b.NoOfTickets
	FROM @TBL A,
	#TEMP_TABLE_FreshService_T_Tickets B
	WHERE A.departmentId=B.department_id AND A.[type]=B.[type] AND A.statustype='Within SLA'
	and b.[priority]=1
	AND B.fr_escalated=0
	

	UPDATE @TBL
	SET [Low]=b.NoOfTickets
	FROM @TBL A,
	#TEMP_TABLE_FreshService_T_Tickets B
	WHERE A.departmentId=B.department_id AND A.[type]=B.[type] AND A.statustype='SLA Violated'
	and b.[priority]=1
	AND B.fr_escalated=1

	update @TBL
	set GrandTotal=isnull(urgent,0)+isnull([high],0)+isnull([medium],0)+isnull([low],0)
	
	update @TBL
	SET Urgent=B.urgent,
		[High]=B.[high],
		[Medium]=B.[medium],
		[Low]=b.[Low],
		[GrandTotal]=b.GrandTotal
	from  @TBL a,

	(select departmentId,sum(isnull(urgent,0)) as urgent,sum(isnull([high],0)) as [high],sum(isnull([medium],0)) as [medium],sum(isnull([low],0)) as [low],sum(isnull([GrandTotal],0)) as [GrandTotal]
	from @TBL group by departmentId) B
	WHERE A.departmentId=B.departmentId
	AND A.[type]='ZZZ-Grand Total'


	--Rev 1.0
			;WITH CTE_Aggregate AS
				(
					SELECT 
						departmentId,
						[type],
						SUM(CASE WHEN statustype IN ('SLA Violated', 'Within SLA') THEN GrandTotal ELSE 0 END) AS Total, -- SLA Violated + Within SLA
						SUM(CASE WHEN statustype = 'Within SLA' THEN GrandTotal ELSE 0 END) AS WithinSLA
					FROM @TBL
					GROUP BY departmentId, [type]
				),
				
				CTE_Combined AS
				(
					SELECT 
						departmentId,
						[type],
						SUM(CASE WHEN [type] IN ('Incident', 'Service Request') AND statustype = 'Within SLA' THEN GrandTotal ELSE 0 END) OVER (PARTITION BY departmentId) AS TotalWithinSLA,
						SUM(CASE WHEN [type] = 'ZZZ-Grand Total' THEN GrandTotal ELSE 0 END) OVER (PARTITION BY departmentId) AS ZZZGrandTotal
					FROM @TBL
				)
				-- Update AchievedPercentage
				UPDATE T
				SET AchievedPercentage = 
					CASE 
						WHEN T.[type] IN ('Incident', 'Service Request') THEN 
							CASE 
								WHEN A.Total = 0 THEN '0'
								ELSE CAST(ROUND((CAST(A.WithinSLA AS DECIMAL) / A.Total * 100.0), 0) AS INT)
							END
						WHEN T.[type] = 'ZZZ-Grand Total' THEN
							CASE 
								WHEN C.ZZZGrandTotal = 0 THEN '0'
								ELSE CAST(ROUND((CAST(C.TotalWithinSLA AS DECIMAL) / C.ZZZGrandTotal * 100.0), 0) AS INT)
							END
						ELSE NULL
					END
				FROM @TBL T
				LEFT JOIN CTE_Aggregate A
					ON T.departmentId = A.departmentId AND T.[type] = A.[type]
				LEFT JOIN CTE_Combined C
					ON T.departmentId = C.departmentId AND T.[type] = C.[type];
	--End of Rev 1.0 



	select * from @TBL
	order by [name],[type],[statustype]

	

	SELECT 
	ROW_NUMBER() OVER(ORDER BY T.created_at ASC) AS SlNo
	,T.id
	,T.created_at
	--Rev 1.0
	--,FORMAT(T.created_at, 'dd-MM-yyyy HH:mm:ss') AS 'created_at_display'
	,FORMAT(SWITCHOFFSET(CAST(T.created_at AS DATETIMEOFFSET), '+05:30'), 'dd-MM-yyyy HH:mm:ss') AS 'created_at_display'
	--End of Rev 1.0
	, T.[type], T.[subject],T.[status],S.StatusName,TC.on_roaster_engineer,TC.resolution_remarks, D.[name], T.department_id
	FROM [FreshService_T_Tickets] T WITH(NOLOCK) 
	INNER JOIN [FreshService_M_Department] D WITH(NOLOCK) ON T.department_id=D.id
	INNER JOIN [FreshService_T_Ticket_CustomFields] TC WITH(NOLOCK) ON TC.ticket_id=T.id
	LEFT OUTER JOIN [FreshService_M_Status] S WITH(NOLOCK) ON S.StatusId=T.[status]
	--Rev 1.0 
	--WHERE T.created_at between CONVERT(DATE,@Start_Date,103) AND CONVERT(DATE,@End_Date,103)
	WHERE CAST(SWITCHOFFSET(CAST(T.created_at AS DATETIMEOFFSET), '+05:30') AS DATE) 
      BETWEEN CONVERT(DATE, @Start_Date, 103) AND CONVERT(DATE, @End_Date, 103)
	  AND (CAST(SWITCHOFFSET(CAST(T.created_at AS DATETIMEOFFSET), '+05:30') AS DATE) > @LastDateTobeConsidered OR ISNULL(@LastDateTobeConsidered,'')='')
	--End of Rev 1.0
	AND (T.department_id=@DepartmentId OR ISNULL(@DepartmentId,0)=0)
	AND ISNULL(T.fr_escalated,0)=1
	AND T.[status] IN (SELECT StatusID FROM @IncludeStatus)
	AND TC.tenant IN (
		SELECT TenantName 
		FROM MonthlyReport_M_Tenant WITH (NOLOCK) 
		WHERE Active = 1
	)

    ORDER BY T.created_at ASC
	DROP TABLE #TEMP_TABLE_FreshService_T_Tickets
END
GO
/****** Object:  StoredProcedure [dbo].[usp_FreshService_R_TickesExcel]    Script Date: 09/06/2025 10:37:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO



CREATE     PROC [dbo].[usp_FreshService_R_TickesExcel]
(
@DepartmentId bigINT=27000109788,
@Start_Date	VARCHAR(10)='01/11/2024',
@End_Date VARCHAR(10)='30/11/2024'
)
AS
BEGIN

	
	DECLARE @LastDateTobeConsidered DATE = (SELECT TOP 1 LastDate FROM LastDateTobeConsidered WHERE ACTIVE =1);
	

	SELECT 
	ROW_NUMBER() OVER(ORDER BY T.created_at ASC) AS SlNo
	,T.id
	,T.category
	,T.sub_category
	,T.created_at
	--,FORMAT(T.created_at, 'dd-MM-yyyy HH:mm:ss') AS 'created_at_display'
	,FORMAT(SWITCHOFFSET(CAST(T.created_at AS DATETIMEOFFSET), '+05:30'), 'dd-MM-yyyy HH:mm:ss') AS 'created_at_display'
	, T.[type], T.[subject],T.[status],S.StatusName
	,TR.[email] AS RequesterEmail
	,TR.[name] as RequesterName
	,Tr.[mobile] as RequesterMobile
	,TC.[location]
	,tc.tenant
	,TC.nsd_member_name
	,TC.on_roaster_engineer
	,TC.resolution_remarks
	,tc.resource_name
	,tc.oem_case_idif_any
	, D.[name], T.department_id
	,T.[priority]
	,p.[Name] as priorityname
	,CASE WHEN ISNULL(T.is_escalated,0)=1 THEN 'SLA Violated' ELSE 'Within SLA' END AS ResolutionStatus
	,CASE WHEN ISNULL(T.fr_escalated,0)=1 THEN 'SLA Violated' ELSE 'Within SLA' END AS ResponseStatus
	--,FORMAT(TS.resolved_at, 'dd-MM-yyyy HH:mm:ss') AS 'resolved_at_display'
	,FORMAT(SWITCHOFFSET(CAST(TS.resolved_at AS DATETIMEOFFSET), '+05:30'), 'dd-MM-yyyy HH:mm:ss') AS 'resolved_at_display'
	--,FORMAT(TS.closed_at, 'dd-MM-yyyy HH:mm:ss') AS 'closed_at_display'
	,FORMAT(SWITCHOFFSET(CAST(TS.closed_at AS DATETIMEOFFSET), '+05:30'), 'dd-MM-yyyy HH:mm:ss') AS 'closed_at_display'
	--,FORMAT(TS.status_updated_at, 'dd-MM-yyyy HH:mm:ss') AS 'status_updated_at_display'
	,FORMAT(SWITCHOFFSET(CAST(TS.status_updated_at AS DATETIMEOFFSET), '+05:30'), 'dd-MM-yyyy HH:mm:ss') AS 'status_updated_at_display'
	,ts.first_resp_time_in_secs
	,ts.resolution_time_in_secs
	--,FORMAT(TS.first_assigned_at, 'dd-MM-yyyy HH:mm:ss') AS 'first_assigned_at_display'
	,FORMAT(SWITCHOFFSET(CAST(TS.first_assigned_at AS DATETIMEOFFSET), '+05:30'), 'dd-MM-yyyy HH:mm:ss') AS 'first_assigned_at_display'
	--,FORMAT(TS.first_responded_at, 'dd-MM-yyyy HH:mm:ss') AS 'first_responded_at_display'
	,FORMAT(SWITCHOFFSET(CAST(TS.first_responded_at AS DATETIMEOFFSET), '+05:30'), 'dd-MM-yyyy HH:mm:ss') AS 'first_responded_at_display'
	--,FORMAT(TS.assigned_at, 'dd-MM-yyyy HH:mm:ss') AS 'assigned_at_display'
	,FORMAT(SWITCHOFFSET(CAST(TS.assigned_at AS DATETIMEOFFSET), '+05:30'), 'dd-MM-yyyy HH:mm:ss') AS 'assigned_at_display'

	
	FROM [FreshService_T_Tickets] T WITH(NOLOCK) 
	INNER JOIN [FreshService_M_Department] D WITH(NOLOCK) ON T.department_id=D.id
	INNER JOIN [FreshService_T_Ticket_CustomFields] TC WITH(NOLOCK) ON TC.ticket_id=T.id
	LEFT OUTER JOIN FreshService_T_TicketRequesters TR WITH(NOLOCK) ON TR.ticket_id=T.id
	LEFT OUTER JOIN FreshService_T_Ticket_Stats TS WITH(NOLOCK) ON TS.ticket_id=T.id
	LEFT OUTER JOIN [FreshService_M_Status] S WITH(NOLOCK) ON S.StatusId=T.[status]
	LEFT OUTER JOIN FreshService_M_Priority P WITH(NOLOCK) ON P.Id=T.[priority]
	--WHERE T.created_at between CONVERT(DATE,@Start_Date,103) AND CONVERT(DATE,@End_Date,103)
	WHERE CAST(SWITCHOFFSET(CAST(T.created_at AS DATETIMEOFFSET), '+05:30') AS DATE) 
      BETWEEN CONVERT(DATE, @Start_Date, 103) AND CONVERT(DATE, @End_Date, 103)
	AND (T.department_id=@DepartmentId OR ISNULL(@DepartmentId,0)=0)
	AND (CAST(SWITCHOFFSET(CAST(T.created_at AS DATETIMEOFFSET), '+05:30') AS DATE) > @LastDateTobeConsidered OR ISNULL(@LastDateTobeConsidered,'')='')
	--AND ISNULL(T.is_escalated,0)=1	
    ORDER BY T.created_at ASC
END
GO
/****** Object:  StoredProcedure [dbo].[usp_FreshService_R_TicketByCategoryAndPriority]    Script Date: 09/06/2025 10:37:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE     PROC [dbo].[usp_FreshService_R_TicketByCategoryAndPriority]
(
@DepartmentId bigINT=27002310005,
@Start_Date	VARCHAR(10)='01/02/2025',
@End_Date VARCHAR(10)='28/02/2025'
)
AS
/*
Rev 1.0		Soumik		02-01-2025		Added Problem column,Only Closed/Resolved,Category total not required in row 
*/
BEGIN

	--Rev 1.0
	DECLARE @IncludeStatus AS TABLE
	(
		StatusID INT
	)
	--Only Closed / Resolved 
	INSERT INTO @IncludeStatus (StatusID) VALUES (4);
	INSERT INTO @IncludeStatus (StatusID) VALUES (5);

	DECLARE @LastDateTobeConsidered DATE = (SELECT TOP 1 LastDate FROM LastDateTobeConsidered WHERE ACTIVE =1);
	--End of Rev

	DECLARE @TBL AS TABLE
	(
		departmentId bigint,
		[name]  varchar(300),
		[category] varchar(100) default(''),
		[sub_category] varchar(100) default(''),
		[Urgent] int default(0),
		[High] int default(0),
		[Medium] int default(0),
		[Low] int default(0),
		GrandTotal int default(0),
		RowType int default(0)
	)
	INSERT INTO @TBL (departmentId,[name],[category],[sub_category])
	SELECT 
	distinct T.department_id,D.[name],ISNULL(T.category,'-') as category ,ISNULL(T.sub_category,'-') as sub_category
	FROM [FreshService_T_Tickets] T WITH(NOLOCK) 
	INNER JOIN [FreshService_M_Department] D WITH(NOLOCK) ON T.department_id=D.id
	INNER JOIN [FreshService_T_Ticket_CustomFields] TC WITH(NOLOCK) ON TC.ticket_id=T.id
	LEFT OUTER JOIN [FreshService_M_Status] S WITH(NOLOCK) ON S.StatusId=T.[status]
	--Rev 1.0 
	--WHERE T.created_at between CONVERT(DATE,@Start_Date,103) AND CONVERT(DATE,@End_Date,103)
	WHERE CAST(SWITCHOFFSET(CAST(T.created_at AS DATETIMEOFFSET), '+05:30') AS DATE) 
      BETWEEN CONVERT(DATE, @Start_Date, 103) AND CONVERT(DATE, @End_Date, 103)
	AND T.[status] IN (SELECT StatusID FROM @IncludeStatus)
	AND (CAST(SWITCHOFFSET(CAST(T.created_at AS DATETIMEOFFSET), '+05:30') AS DATE) > @LastDateTobeConsidered OR ISNULL(@LastDateTobeConsidered,'')='')
	--End of Rev 1.0
	AND (T.department_id=@DepartmentId OR ISNULL(@DepartmentId,0)=0)
	AND TC.tenant IN (
		SELECT TenantName 
		FROM MonthlyReport_M_Tenant WITH (NOLOCK) 
		WHERE Active = 1
	)

	UNION ALL
	SELECT  C.department_id,D.[name], ISNULL(C.category,'-') AS Category,ISNULL(c.sub_category,'-') as sub_category
	FROM [FreshService_T_Change] C WITH(NOLOCK) 
	INNER JOIN [FreshService_M_Department] D WITH(NOLOCK) ON C.department_id=D.id
	--Rev 1.0 
	--WHERE C.created_at between CONVERT(DATE,@Start_Date,103) AND CONVERT(DATE,@End_Date,103)
	WHERE CAST(SWITCHOFFSET(CAST(C.created_at AS DATETIMEOFFSET), '+05:30') AS DATE) 
      BETWEEN CONVERT(DATE, @Start_Date, 103) AND CONVERT(DATE, @End_Date, 103)
	AND C.[status] IN (SELECT StatusID FROM @IncludeStatus)
	AND (CAST(SWITCHOFFSET(CAST(C.created_at AS DATETIMEOFFSET), '+05:30') AS DATE) > @LastDateTobeConsidered OR ISNULL(@LastDateTobeConsidered,'')='')
	--End of Rev 1.0
	AND (C.department_id=@DepartmentId OR ISNULL(@DepartmentId,0)=0)


	-- Rev 1.0
	UNION ALL
	SELECT P.department_id,D.[name], ISNULL(P.category,'-') AS Category,ISNULL(P.sub_category,'-') as sub_category
	FROM [FreshService_T_Problems] P WITH(NOLOCK)
	INNER JOIN [FreshService_M_Department] D WITH(NOLOCK) ON P.department_id=D.id
	--WHERE P.created_at between CONVERT(DATE,@Start_Date,103) AND CONVERT(DATE,@End_Date,103)
	WHERE CAST(SWITCHOFFSET(CAST(P.created_at AS DATETIMEOFFSET), '+05:30') AS DATE) 
      BETWEEN CONVERT(DATE, @Start_Date, 103) AND CONVERT(DATE, @End_Date, 103)
	AND (P.department_id=@DepartmentId OR ISNULL(@DepartmentId,0)=0)
	AND P.[status] IN (SELECT StatusID FROM @IncludeStatus)
	AND (CAST(SWITCHOFFSET(CAST(P.created_at AS DATETIMEOFFSET), '+05:30') AS DATE) > @LastDateTobeConsidered OR ISNULL(@LastDateTobeConsidered,'')='')
	-- End of Rev 1.0
	
	SELECT * INTO #TEMP_FreshService_R_TicketByCategoryAndPriority 
	FROM 
	(SELECT 
	T.department_id,D.[name],ISNULL(T.category,'-') as category ,ISNULL(T.sub_category,'-') as sub_category
	,t.[priority], COUNT(T.ID) AS NoOfTickets
	FROM [FreshService_T_Tickets] T WITH(NOLOCK) 
	INNER JOIN [FreshService_M_Department] D WITH(NOLOCK) ON T.department_id=D.id
	INNER JOIN [FreshService_T_Ticket_CustomFields] TC WITH(NOLOCK) ON TC.ticket_id=T.id
	LEFT OUTER JOIN [FreshService_M_Status] S WITH(NOLOCK) ON S.StatusId=T.[status]
	--Rev 1.0
	--WHERE T.created_at between CONVERT(DATE,@Start_Date,103) AND CONVERT(DATE,@End_Date,103)
	WHERE CAST(SWITCHOFFSET(CAST(T.created_at AS DATETIMEOFFSET), '+05:30') AS DATE) 
      BETWEEN CONVERT(DATE, @Start_Date, 103) AND CONVERT(DATE, @End_Date, 103)
	AND T.[status] IN (SELECT StatusID FROM @IncludeStatus)
	AND (CAST(SWITCHOFFSET(CAST(T.created_at AS DATETIMEOFFSET), '+05:30') AS DATE) > @LastDateTobeConsidered OR ISNULL(@LastDateTobeConsidered,'')='')
	--End of Rev 1.0
	AND (T.department_id=@DepartmentId OR ISNULL(@DepartmentId,0)=0)
	AND TC.tenant IN (
		SELECT TenantName 
		FROM MonthlyReport_M_Tenant WITH (NOLOCK) 
		WHERE Active = 1
	)

	GROUP BY T.department_id,D.[name],ISNULL(T.category,'-'),ISNULL(T.sub_category,'-'), T.[priority]

	UNION ALL
	SELECT  C.department_id,D.[name], ISNULL(C.category,'-') AS Category,ISNULL(c.sub_category,'-') as sub_category
	,c.[priority],COUNT(C.ID) AS NoOfTickets
	FROM [FreshService_T_Change] C WITH(NOLOCK) 
	INNER JOIN [FreshService_M_Department] D WITH(NOLOCK) ON C.department_id=D.id
	--Rev 1.0
	--WHERE C.created_at between CONVERT(DATE,@Start_Date,103) AND CONVERT(DATE,@End_Date,103)
	WHERE CAST(SWITCHOFFSET(CAST(C.created_at AS DATETIMEOFFSET), '+05:30') AS DATE) 
      BETWEEN CONVERT(DATE, @Start_Date, 103) AND CONVERT(DATE, @End_Date, 103)
	AND C.[status] IN (SELECT StatusID FROM @IncludeStatus)
	AND (CAST(SWITCHOFFSET(CAST(C.created_at AS DATETIMEOFFSET), '+05:30') AS DATE) > @LastDateTobeConsidered OR ISNULL(@LastDateTobeConsidered,'')='')
	--End of Rev 1.0
	AND (C.department_id=@DepartmentId OR ISNULL(@DepartmentId,0)=0)

	GROUP BY C.department_id,D.[name],ISNULL(C.category,'-'),ISNULL(C.sub_category,'-'),c.[priority]

	-- Rev 1.0
	UNION ALL
	SELECT 
	P.department_id,D.[name], ISNULL(P.category,'-') AS Category,ISNULL(P.sub_category,'-') as sub_category
	,p.[priority],COUNT(P.ID) AS NoOfTickets
	FROM [FreshService_T_Problems] P WITH(NOLOCK)
	INNER JOIN [FreshService_M_Department] D WITH(NOLOCK) ON P.department_id=D.id
		--Rev 1.0 
	--WHERE P.created_at between CONVERT(DATE,@Start_Date,103) AND CONVERT(DATE,@End_Date,103)
	WHERE CAST(SWITCHOFFSET(CAST(P.created_at AS DATETIMEOFFSET), '+05:30') AS DATE) 
      BETWEEN CONVERT(DATE, @Start_Date, 103) AND CONVERT(DATE, @End_Date, 103)
	  AND (CAST(SWITCHOFFSET(CAST(P.created_at AS DATETIMEOFFSET), '+05:30') AS DATE) > @LastDateTobeConsidered OR ISNULL(@LastDateTobeConsidered,'')='')
	--End of Rev 1.0
	AND (P.department_id=@DepartmentId OR ISNULL(@DepartmentId,0)=0)
	AND P.[status] IN (SELECT StatusID FROM @IncludeStatus)
	GROUP BY D.[name], P.department_id,ISNULL(P.category,'-'),ISNULL(P.sub_category,'-'),p.[priority]
	-- End of Rev 1.0
	) X


	UPDATE A
	SET [Low]=(ISNULL(B.NOOFTICKETS,0))
	FROM @TBL A,
	#TEMP_FreshService_R_TicketByCategoryAndPriority B
	WHERE A.departmentId=B.department_id
	AND A.category=B.category
	AND A.sub_category=B.sub_category
	AND B.[priority]=1

	UPDATE A
	SET [medium]=(ISNULL(B.NOOFTICKETS,0))
	FROM @TBL A,
	#TEMP_FreshService_R_TicketByCategoryAndPriority B
	WHERE A.departmentId=B.department_id
	AND A.category=B.category
	AND A.sub_category=B.sub_category
	AND  B.[priority]=2


	UPDATE A
	SET [high]=(ISNULL(B.NOOFTICKETS,0))
	FROM @TBL A,
	#TEMP_FreshService_R_TicketByCategoryAndPriority B
	WHERE A.departmentId=B.department_id
	AND A.category=B.category
	AND A.sub_category=B.sub_category
	AND B.[priority]=3

	UPDATE A
	SET [Urgent]=(ISNULL(B.NOOFTICKETS,0))
	FROM @TBL A,
	#TEMP_FreshService_R_TicketByCategoryAndPriority B
	WHERE A.departmentId=B.department_id
	AND A.category=B.category
	AND A.sub_category=B.sub_category
	AND B.[priority]=4

	--Rev 1.0
	--INSERT INTO @TBL (departmentId,[name],category,Urgent,[high],[medium],[Low],RowType)
	--SELECT departmentId,[name],category+ ' Total' as category,SUM(Urgent) AS Urgent
	--,SUM([high]) AS [high]
	--,SUM([medium]) AS [medium]
	--,SUM([Low]) AS [Low]
	--,1
	--FROM @TBL
	--GROUP BY departmentId,[name],category
	--End of Rev 1.0


	INSERT INTO @TBL (departmentId,[name],category,Urgent,[high],[medium],[Low],RowType)
	SELECT departmentId,[name],'ZZZ-Grand Total' AS category,SUM(Urgent) AS Urgent
	,SUM([high]) AS [high]
	,SUM([medium]) AS [medium]
	,SUM([Low]) AS [Low]
	,2
	FROM @TBL
	where RowType=0
	GROUP BY departmentId,[name]


    UPDATE @TBL SET GrandTotal=ISNULL(Urgent,0)+ISNULL([high],0)+ISNULL([medium],0)+ISNULL([Low],0)
	
	
	SELECT 
		departmentId,
		[name],
		[category],
		[sub_category] ,
		Urgent,
		[high],
		[medium],
		[Low],
		GrandTotal ,
		RowType
	
	FROM @TBL
ORDER BY [name],[category],[sub_category]


	DROP TABLE #TEMP_FreshService_R_TicketByCategoryAndPriority
	
END
GO
/****** Object:  StoredProcedure [dbo].[usp_FreshService_R_TicketByCategoryAndType]    Script Date: 09/06/2025 10:37:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE     PROC [dbo].[usp_FreshService_R_TicketByCategoryAndType]
(
@DepartmentId bigINT=27000586401,
@Start_Date	VARCHAR(10)='01/11/2024',
@End_Date VARCHAR(10)='30/11/2024'
)
AS
/*
Rev 1.0		Soumik		02-01-2025		Added Problem column,Only Closed/Resolved,Category total not required in row 
*/
BEGIN

	--Rev 1.0
	DECLARE @IncludeStatus AS TABLE
	(
		StatusID INT
	)
	--Only Closed / Resolved 
	INSERT INTO @IncludeStatus (StatusID) VALUES (4);
	INSERT INTO @IncludeStatus (StatusID) VALUES (5);

	DECLARE @LastDateTobeConsidered DATE = (SELECT TOP 1 LastDate FROM LastDateTobeConsidered WHERE ACTIVE =1);
	--End of Rev

	DECLARE @TBL AS TABLE
	(
		departmentId bigint,
		[name]  varchar(300),
		[category] varchar(100) default(''),
		[sub_category] varchar(100) default(''),
		[ChangeRequest] int default(0),
		[Incident] int default(0),
		[ServiceRequest] int default(0),
		--Rev 1.0
		[Problem] int default(0),
		--End of Rev 1.0
		GrandTotal int default(0),
		RowType int default(0)
	)
	INSERT INTO @TBL (departmentId,[name],[category],[sub_category])
	SELECT 
	distinct T.department_id,D.[name],ISNULL(T.category,'-') as category ,ISNULL(T.sub_category,'-') as sub_category
	FROM [FreshService_T_Tickets] T WITH(NOLOCK) 
	INNER JOIN [FreshService_M_Department] D WITH(NOLOCK) ON T.department_id=D.id
	INNER JOIN [FreshService_T_Ticket_CustomFields] TC WITH(NOLOCK) ON TC.ticket_id=T.id
	LEFT OUTER JOIN [FreshService_M_Status] S WITH(NOLOCK) ON S.StatusId=T.[status]
	--Rev 1.0
	--WHERE T.created_at between CONVERT(DATE,@Start_Date,103) AND CONVERT(DATE,@End_Date,103)
	WHERE CAST(SWITCHOFFSET(CAST(T.created_at AS DATETIMEOFFSET), '+05:30') AS DATE) 
      BETWEEN CONVERT(DATE, @Start_Date, 103) AND CONVERT(DATE, @End_Date, 103)
	AND T.[status] IN (SELECT StatusID FROM @IncludeStatus)
	AND (CAST(SWITCHOFFSET(CAST(T.created_at AS DATETIMEOFFSET), '+05:30') AS DATE) > @LastDateTobeConsidered OR ISNULL(@LastDateTobeConsidered,'')='')
	--End of Rev 1.0
	AND (T.department_id=@DepartmentId OR ISNULL(@DepartmentId,0)=0)
	AND TC.tenant IN (
		SELECT TenantName 
		FROM MonthlyReport_M_Tenant WITH (NOLOCK) 
		WHERE Active = 1
	)

	UNION ALL
	SELECT  C.department_id,D.[name], ISNULL(C.category,'-') AS Category,ISNULL(c.sub_category,'-') as sub_category
	FROM [FreshService_T_Change] C WITH(NOLOCK) 
	INNER JOIN [FreshService_M_Department] D WITH(NOLOCK) ON C.department_id=D.id
	--Rev 1.0
	--WHERE C.created_at between CONVERT(DATE,@Start_Date,103) AND CONVERT(DATE,@End_Date,103)
	WHERE CAST(SWITCHOFFSET(CAST(C.created_at AS DATETIMEOFFSET), '+05:30') AS DATE) 
    BETWEEN CONVERT(DATE, @Start_Date, 103) AND CONVERT(DATE, @End_Date, 103)
	AND C.[status] IN (SELECT StatusID FROM @IncludeStatus)
	AND (CAST(SWITCHOFFSET(CAST(C.created_at AS DATETIMEOFFSET), '+05:30') AS DATE) > @LastDateTobeConsidered OR ISNULL(@LastDateTobeConsidered,'')='')
	--End of Rev 1.0
	AND (C.department_id=@DepartmentId OR ISNULL(@DepartmentId,0)=0)


	-- Rev 1.0
	UNION ALL
	SELECT P.department_id,D.[name], ISNULL(P.category,'-') AS Category,ISNULL(P.sub_category,'-') as sub_category
	FROM [FreshService_T_Problems] P WITH(NOLOCK)
	INNER JOIN [FreshService_M_Department] D WITH(NOLOCK) ON P.department_id=D.id
	--WHERE P.created_at between CONVERT(DATE,@Start_Date,103) AND CONVERT(DATE,@End_Date,103)
	WHERE CAST(SWITCHOFFSET(CAST(P.created_at AS DATETIMEOFFSET), '+05:30') AS DATE) 
      BETWEEN CONVERT(DATE, @Start_Date, 103) AND CONVERT(DATE, @End_Date, 103)
	AND (P.department_id=@DepartmentId OR ISNULL(@DepartmentId,0)=0)
	AND P.[status] IN (SELECT StatusID FROM @IncludeStatus) 
	AND (CAST(SWITCHOFFSET(CAST(P.created_at AS DATETIMEOFFSET), '+05:30') AS DATE) > @LastDateTobeConsidered OR ISNULL(@LastDateTobeConsidered,'')='')
	-- End of Rev 1.0
	
	
	SELECT * INTO #TEMP_FreshService_R_TicketByCategoryAndType 
	FROM 
	(SELECT 
	T.department_id,D.[name],ISNULL(T.category,'-') as category ,ISNULL(T.sub_category,'-') as sub_category
	,T.[type],COUNT(T.ID) AS NoOfTickets
	FROM [FreshService_T_Tickets] T WITH(NOLOCK) 
	INNER JOIN [FreshService_M_Department] D WITH(NOLOCK) ON T.department_id=D.id
	INNER JOIN [FreshService_T_Ticket_CustomFields] TC WITH(NOLOCK) ON TC.ticket_id=T.id
	LEFT OUTER JOIN [FreshService_M_Status] S WITH(NOLOCK) ON S.StatusId=T.[status]
	--Rev 1.0 
	--WHERE T.created_at between CONVERT(DATE,@Start_Date,103) AND CONVERT(DATE,@End_Date,103)
	WHERE CAST(SWITCHOFFSET(CAST(T.created_at AS DATETIMEOFFSET), '+05:30') AS DATE) 
      BETWEEN CONVERT(DATE, @Start_Date, 103) AND CONVERT(DATE, @End_Date, 103)
	AND T.[status] IN (SELECT StatusID FROM @IncludeStatus)
	AND (CAST(SWITCHOFFSET(CAST(T.created_at AS DATETIMEOFFSET), '+05:30') AS DATE) > @LastDateTobeConsidered OR ISNULL(@LastDateTobeConsidered,'')='')
	--End of Rev 1.0
	AND (T.department_id=@DepartmentId OR ISNULL(@DepartmentId,0)=0)
	AND TC.tenant IN (
		SELECT TenantName 
		FROM MonthlyReport_M_Tenant WITH (NOLOCK) 
		WHERE Active = 1
	)

	GROUP BY T.department_id,D.[name],ISNULL(T.category,'-'),ISNULL(T.sub_category,'-'), T.[type]

	UNION ALL
	SELECT  C.department_id,D.[name], ISNULL(C.category,'-') AS Category,ISNULL(c.sub_category,'-') as sub_category
	,'Change Request' AS [type],COUNT(C.ID) AS NoOfTickets
	FROM [FreshService_T_Change] C WITH(NOLOCK) 
	INNER JOIN [FreshService_M_Department] D WITH(NOLOCK) ON C.department_id=D.id
	--Rev 1.0
	--WHERE C.created_at between CONVERT(DATE,@Start_Date,103) AND CONVERT(DATE,@End_Date,103)
	WHERE CAST(SWITCHOFFSET(CAST(C.created_at AS DATETIMEOFFSET), '+05:30') AS DATE) 
      BETWEEN CONVERT(DATE, @Start_Date, 103) AND CONVERT(DATE, @End_Date, 103)
	AND C.[status] IN (SELECT StatusID FROM @IncludeStatus)
	AND (CAST(SWITCHOFFSET(CAST(C.created_at AS DATETIMEOFFSET), '+05:30') AS DATE) > @LastDateTobeConsidered OR ISNULL(@LastDateTobeConsidered,'')='')
	--End of Rev 1.0
	AND (C.department_id=@DepartmentId OR ISNULL(@DepartmentId,0)=0)
	GROUP BY C.department_id,D.[name],ISNULL(C.category,'-'),ISNULL(C.sub_category,'-')

	-- Rev 1.0
	UNION ALL
	SELECT 
	P.department_id,D.[name], ISNULL(P.category,'-') AS Category,ISNULL(P.sub_category,'-') as sub_category
	,'Problem' AS [type],COUNT(P.ID) AS NoOfTickets
	FROM [FreshService_T_Problems] P WITH(NOLOCK)
	INNER JOIN [FreshService_M_Department] D WITH(NOLOCK) ON P.department_id=D.id
	--WHERE P.created_at between CONVERT(DATE,@Start_Date,103) AND CONVERT(DATE,@End_Date,103)
	WHERE CAST(SWITCHOFFSET(CAST(P.created_at AS DATETIMEOFFSET), '+05:30') AS DATE) 
      BETWEEN CONVERT(DATE, @Start_Date, 103) AND CONVERT(DATE, @End_Date, 103)
	  AND (CAST(SWITCHOFFSET(CAST(P.created_at AS DATETIMEOFFSET), '+05:30') AS DATE) > @LastDateTobeConsidered OR ISNULL(@LastDateTobeConsidered,'')='')
	AND (P.department_id=@DepartmentId OR ISNULL(@DepartmentId,0)=0)
	AND P.[status] IN (SELECT StatusID FROM @IncludeStatus) 
	GROUP BY D.[name], P.department_id,ISNULL(P.category,'-'),ISNULL(P.sub_category,'-')
	-- End of Rev 1.0


	) X


	UPDATE A
	SET Incident=(ISNULL(B.NOOFTICKETS,0))
	FROM @TBL A,
	#TEMP_FreshService_R_TicketByCategoryAndType B
	WHERE A.departmentId=B.department_id
	AND A.category=B.category
	AND A.sub_category=B.sub_category
	AND B.[TYPE]='Incident'

	UPDATE A
	SET ServiceRequest=(ISNULL(B.NOOFTICKETS,0))
	FROM @TBL A,
	#TEMP_FreshService_R_TicketByCategoryAndType B
	WHERE A.departmentId=B.department_id
	AND A.category=B.category
	AND A.sub_category=B.sub_category
	AND B.[TYPE]='Service Request'


	UPDATE A
	SET ChangeRequest=(ISNULL(B.NOOFTICKETS,0))
	FROM @TBL A,
	#TEMP_FreshService_R_TicketByCategoryAndType B
	WHERE A.departmentId=B.department_id
	AND A.category=B.category
	AND A.sub_category=B.sub_category
	AND B.[TYPE]='Change Request'

	--Rev 1.0
	UPDATE A
	SET Problem=(ISNULL(B.NOOFTICKETS,0))
	FROM @TBL A,
	#TEMP_FreshService_R_TicketByCategoryAndType B
	WHERE A.departmentId=B.department_id
	AND A.category=B.category
	AND A.sub_category=B.sub_category
	AND B.[TYPE]='Problem'
	--End of Rev 1.0

	--Rev 1.0 --Category total not required in row

	--INSERT INTO @TBL (departmentId,[name],category,ChangeRequest,Incident,ServiceRequest,RowType)
	--SELECT departmentId,[name],category+ ' Total' as category,SUM(ChangeRequest) AS ChangeRequest
	--,SUM(Incident) AS Incident
	--,SUM(ServiceRequest) AS ServiceRequest
	--,1
	--FROM @TBL
	--GROUP BY departmentId,[name],category
	--End of Rev 1.0



	INSERT INTO @TBL (departmentId,[name],category,ChangeRequest,Incident,ServiceRequest,Problem,RowType)
	SELECT departmentId,[name],'ZZZ-Grand Total' AS category,SUM(ChangeRequest) AS ChangeRequest
	,SUM(Incident) AS Incident
	,SUM(ServiceRequest) AS ServiceRequest,
	--Rev 1.0
	SUM(Problem) AS Problem
	--End of Rev 1.0
	,2
	FROM @TBL
	WHERE RowType=0
	GROUP BY departmentId,[name]

	--Rev 1.0
    --UPDATE @TBL SET GrandTotal=ISNULL(ChangeRequest,0)+ISNULL(Incident,0)+ISNULL(ServiceRequest,0)
    UPDATE @TBL SET GrandTotal=ISNULL(ChangeRequest,0)+ISNULL(Incident,0)+ISNULL(ServiceRequest,0)+ISNULL(Problem,0)
	--End of Rev 1.0

	
	
	SELECT 
		departmentId,
		[name],
		[category],
		[sub_category] ,
		[ChangeRequest],
		[Incident],
		[ServiceRequest],
		--Rev 1.0
		Problem,
		--End of Rev 1.0
		GrandTotal ,
		RowType
	
	FROM @TBL
	ORDER BY [name],[category],[sub_category]


	DROP TABLE #TEMP_FreshService_R_TicketByCategoryAndType
	
	END

GO
/****** Object:  StoredProcedure [dbo].[usp_FreshService_R_TicketByResourceName]    Script Date: 09/06/2025 10:37:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE     PROC [dbo].[usp_FreshService_R_TicketByResourceName]
(
@DepartmentId bigINT=27000109788,
@Start_Date	VARCHAR(10)='01/11/2024',
@End_Date VARCHAR(10)='30/11/2024'
)
AS
BEGIN

	DECLARE @LastDateTobeConsidered DATE = (SELECT TOP 1 LastDate FROM LastDateTobeConsidered WHERE ACTIVE =1);

	SELECT TOP 20
	 T.department_id,D.[name],TC.resource_name,COUNT(T.ID) AS NoOfTickets
	FROM [FreshService_T_Tickets] T WITH(NOLOCK) 
	INNER JOIN [FreshService_M_Department] D WITH(NOLOCK) ON T.department_id=D.id
	INNER JOIN [FreshService_T_Ticket_CustomFields] TC WITH(NOLOCK) ON TC.ticket_id=T.id
	LEFT OUTER JOIN [FreshService_M_Status] S WITH(NOLOCK) ON S.StatusId=T.[status]
	--Rev 1.0 
	--WHERE T.created_at between CONVERT(DATE,@Start_Date,103) AND CONVERT(DATE,@End_Date,103)
	WHERE CAST(SWITCHOFFSET(CAST(T.created_at AS DATETIMEOFFSET), '+05:30') AS DATE) 
      BETWEEN CONVERT(DATE, @Start_Date, 103) AND CONVERT(DATE, @End_Date, 103)
	  AND (CAST(SWITCHOFFSET(CAST(T.created_at AS DATETIMEOFFSET), '+05:30') AS DATE) > @LastDateTobeConsidered OR ISNULL(@LastDateTobeConsidered,'')='')
	--End of Rev 1.0
	AND (T.department_id=@DepartmentId OR ISNULL(@DepartmentId,0)=0)
	AND ISNULL(TC.resource_name,'')<>'' AND  TC.resource_name<>'NA'
	AND TC.tenant IN (
		SELECT TenantName 
		FROM MonthlyReport_M_Tenant WITH (NOLOCK) 
		WHERE Active = 1
	)
	GROUP BY T.department_id,D.[name],TC.resource_name
	ORDER BY T.department_id,D.[name],COUNT(T.ID)  DESC,TC.resource_name
	
END
GO
/****** Object:  StoredProcedure [dbo].[usp_FreshService_R_TicketNotClosed]    Script Date: 09/06/2025 10:37:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO





CREATE     PROC [dbo].[usp_FreshService_R_TicketNotClosed]
(
@DepartmentId bigINT=27000109788,
@Start_Date	VARCHAR(10)='01/11/2024',
@End_Date VARCHAR(10)='30/11/2024'
)
AS
/*
Rev 1.0		Soumik		02-01-2025		Only Closed/Resolved TICKETS,GMT - IST
*/
BEGIN

	DECLARE @LastDateTobeConsidered DATE = (SELECT TOP 1 LastDate FROM LastDateTobeConsidered WHERE ACTIVE =1);

	SELECT 
	ROW_NUMBER() OVER(ORDER BY T.created_at ASC) AS SlNo
	,T.id
	,T.created_at
	--Rev 1.0
	--,FORMAT(T.created_at, 'dd-MM-yyyy HH:mm:ss') AS 'created_at_display'
	,FORMAT(SWITCHOFFSET(CAST(T.created_at AS DATETIMEOFFSET), '+05:30'), 'dd-MM-yyyy HH:mm:ss') AS 'created_at_display'
	--End of Rev 1.0
	, T.[type], T.[subject],T.[status],TR.[name] AS RequesterName,tr.email as RequesterEmail, S.StatusName,TC.on_roaster_engineer,TC.resolution_remarks, D.[name], T.department_id
	FROM [FreshService_T_Tickets] T WITH(NOLOCK) 
	INNER JOIN [FreshService_M_Department] D WITH(NOLOCK) ON T.department_id=D.id
	INNER JOIN [FreshService_T_Ticket_CustomFields] TC WITH(NOLOCK) ON TC.ticket_id=T.id
	INNER JOIN FreshService_T_TicketRequesters TR WITH(NOLOCK) ON TR.ticket_id=T.id
	LEFT OUTER JOIN [FreshService_M_Status] S WITH(NOLOCK) ON S.StatusId=T.[status]
	WHERE 
	--Rev 1.0
	--T.created_at between CONVERT(DATE,@Start_Date,103) AND CONVERT(DATE,@End_Date,103)
	--AND ISNULL(T.[status],0)not in (7)
	CAST(SWITCHOFFSET(CAST(T.created_at AS DATETIMEOFFSET), '+05:30') AS DATE) 
      BETWEEN CONVERT(DATE, @Start_Date, 103) AND CONVERT(DATE, @End_Date, 103)
	  AND T.[status] NOT IN (4,5,7) -- Exclude Status Cancel / Resolved / Closed
	  AND (CAST(SWITCHOFFSET(CAST(T.created_at AS DATETIMEOFFSET), '+05:30') AS DATE) > @LastDateTobeConsidered OR ISNULL(@LastDateTobeConsidered,'')='')
	--End of Rev 1.0
	AND (T.department_id=@DepartmentId OR ISNULL(@DepartmentId,0)=0)
	AND TC.tenant IN (
		SELECT TenantName 
		FROM MonthlyReport_M_Tenant WITH (NOLOCK) 
		WHERE Active = 1
	)
	
    ORDER BY T.created_at ASC


END

GO
/****** Object:  StoredProcedure [dbo].[usp_FreshService_T_Changes_InsertUpdate]    Script Date: 09/06/2025 10:37:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE     PROCEDURE [dbo].[usp_FreshService_T_Changes_InsertUpdate]
(
	@jsonInput			VARCHAR(MAX)
)  
AS
BEGIN
    
    SET NOCOUNT ON;  
    
	declare @OutPutId			BIGINT=NULL
	declare @OutPutMsg			VARCHAR(255)=NULL

	SET @OutPutId = 0
	SET @OutPutMsg='Sorry something went wrong.'

	
	IF ISNULL(@jsonInput,'')=''
	BEGIN
		SET @OutPutId = 0
		SET @OutPutMsg='Invalid details, please check your entry.' 
		RETURN
	END

	BEGIN TRY 
		
		
			
			DECLARE @Id BIGINT

			IF  (@jsonInput IS NOT NULL AND iSNULL(@jsonInput,'') <> '') 
			BEGIN				
				
				DECLARE @MaxRowIndex INT, @LoopIndex INT=0		
				
				SELECT @MaxRowIndex=MAX(cast([key] as int)) FROM OPENJSON(@jsonInput, '$.changes')

				WHILE @LoopIndex <= ISNULL(@MaxRowIndex,-1)
				BEGIN					
					BEGIN TRY 
						BEGIN TRANSACTION 

						SET @Id=TRY_PARSE(JSON_VALUE(@jsonInput, '$.changes['+CAST(@LoopIndex AS VARCHAR)+'].id') as BIGINT)
						IF(@Id IS NOT NULL)
						BEGIN
							IF NOT EXISTS (SELECT * FROM FreshService_T_Change WITH(NOLOCK) WHERE ID=@Id)
							BEGIN
								INSERT INTO FreshService_T_Change
								(
									id
									,agent_id
									,group_id
									,[priority]
									,impact
									,[status]
									,risk
									,change_type
									,planned_start_date
									,planned_end_date
									,[subject]
									,department_id
									,category
									,sub_category
									,item_category
									--,[description]
									,planned_effort
									,description_text
									,requester_id
									,approval_status
									,change_window_id
									,workspace_id
									,tasks_dependency_type
									,created_at
									,updated_at
									,created_on
									,updated_on
								)
								SELECT @Id
									,TRY_PARSE(JSON_VALUE(@jsonInput, '$.changes['+CAST(@LoopIndex AS VARCHAR)+'].agent_id') AS BIGINT)
									,TRY_PARSE(JSON_VALUE(@jsonInput, '$.changes['+CAST(@LoopIndex AS VARCHAR)+'].group_id') AS BIGINT)
									,TRY_PARSE(JSON_VALUE(@jsonInput, '$.changes['+CAST(@LoopIndex AS VARCHAR)+'].priority')AS INT)
									,TRY_PARSE(JSON_VALUE(@jsonInput, '$.changes['+CAST(@LoopIndex AS VARCHAR)+'].impact') AS INT)
									,TRY_PARSE(JSON_VALUE(@jsonInput, '$.changes['+CAST(@LoopIndex AS VARCHAR)+'].status') AS INT)
									,TRY_PARSE(JSON_VALUE(@jsonInput, '$.changes['+CAST(@LoopIndex AS VARCHAR)+'].risk') AS INT)
									,TRY_PARSE(JSON_VALUE(@jsonInput, '$.changes['+CAST(@LoopIndex AS VARCHAR)+'].change_type') AS INT)
									,TRY_PARSE(JSON_VALUE(@jsonInput, '$.changes['+CAST(@LoopIndex AS VARCHAR)+'].planned_start_date') AS DATETIMEOFFSET)
									,TRY_PARSE(JSON_VALUE(@jsonInput, '$.changes['+CAST(@LoopIndex AS VARCHAR)+'].planned_end_date') AS DATETIMEOFFSET)
									,JSON_VALUE(@jsonInput, '$.changes['+CAST(@LoopIndex AS VARCHAR)+'].subject') 
									,TRY_PARSE(JSON_VALUE(@jsonInput, '$.changes['+CAST(@LoopIndex AS VARCHAR)+'].department_id') AS BIGINT)
									,JSON_VALUE(@jsonInput, '$.changes['+CAST(@LoopIndex AS VARCHAR)+'].category') 
									,JSON_VALUE(@jsonInput, '$.changes['+CAST(@LoopIndex AS VARCHAR)+'].sub_category') 
									,JSON_VALUE(@jsonInput, '$.changes['+CAST(@LoopIndex AS VARCHAR)+'].item_category') 
									--,JSON_VALUE(@jsonInput, '$.changes['+CAST(@LoopIndex AS VARCHAR)+'].description') 
									,JSON_VALUE(@jsonInput, '$.changes['+CAST(@LoopIndex AS VARCHAR)+'].planned_effort') 
									,JSON_VALUE(@jsonInput, '$.changes['+CAST(@LoopIndex AS VARCHAR)+'].description_text') 
									,TRY_PARSE(JSON_VALUE(@jsonInput, '$.changes['+CAST(@LoopIndex AS VARCHAR)+'].requester_id') AS BIGINT) 
									,TRY_PARSE(JSON_VALUE(@jsonInput, '$.changes['+CAST(@LoopIndex AS VARCHAR)+'].approval_status') AS INT) 
									,JSON_VALUE(@jsonInput, '$.changes['+CAST(@LoopIndex AS VARCHAR)+'].change_window_id') 
									,TRY_PARSE(JSON_VALUE(@jsonInput, '$.changes['+CAST(@LoopIndex AS VARCHAR)+'].workspace_id') AS INT) 
									,TRY_PARSE(JSON_VALUE(@jsonInput, '$.changes['+CAST(@LoopIndex AS VARCHAR)+'].tasks_dependency_type') AS INT)  
									,TRY_PARSE(JSON_VALUE(@jsonInput, '$.changes['+CAST(@LoopIndex AS VARCHAR)+'].created_at') AS DATETIMEOFFSET)
									,TRY_PARSE(JSON_VALUE(@jsonInput, '$.changes['+CAST(@LoopIndex AS VARCHAR)+'].updated_at') AS DATETIMEOFFSET)
									,GETUTCDATE()
									,GETUTCDATE()
							END
							ELSE
							BEGIN
								UPDATE FreshService_T_Change
								SET 
									 agent_id=TRY_PARSE(JSON_VALUE(@jsonInput, '$.changes['+CAST(@LoopIndex AS VARCHAR)+'].agent_id') AS BIGINT)
									,group_id=TRY_PARSE(JSON_VALUE(@jsonInput, '$.changes['+CAST(@LoopIndex AS VARCHAR)+'].group_id') AS BIGINT)
									,[priority]=TRY_PARSE(JSON_VALUE(@jsonInput, '$.changes['+CAST(@LoopIndex AS VARCHAR)+'].priority')AS INT)
									,impact=TRY_PARSE(JSON_VALUE(@jsonInput, '$.changes['+CAST(@LoopIndex AS VARCHAR)+'].impact') AS INT)
									,[status]=TRY_PARSE(JSON_VALUE(@jsonInput, '$.changes['+CAST(@LoopIndex AS VARCHAR)+'].status') AS INT)
									,risk=TRY_PARSE(JSON_VALUE(@jsonInput, '$.changes['+CAST(@LoopIndex AS VARCHAR)+'].risk') AS INT)
									,change_type=TRY_PARSE(JSON_VALUE(@jsonInput, '$.changes['+CAST(@LoopIndex AS VARCHAR)+'].change_type') AS INT)
									,planned_start_date=TRY_PARSE(JSON_VALUE(@jsonInput, '$.changes['+CAST(@LoopIndex AS VARCHAR)+'].planned_start_date') AS DATETIMEOFFSET)
									,planned_end_date=TRY_PARSE(JSON_VALUE(@jsonInput, '$.changes['+CAST(@LoopIndex AS VARCHAR)+'].planned_end_date') AS DATETIMEOFFSET)
									,[subject]=JSON_VALUE(@jsonInput, '$.changes['+CAST(@LoopIndex AS VARCHAR)+'].subject') 
									,department_id=TRY_PARSE(JSON_VALUE(@jsonInput, '$.changes['+CAST(@LoopIndex AS VARCHAR)+'].department_id') AS BIGINT)
									,category=JSON_VALUE(@jsonInput, '$.changes['+CAST(@LoopIndex AS VARCHAR)+'].category') 
									,sub_category=JSON_VALUE(@jsonInput, '$.changes['+CAST(@LoopIndex AS VARCHAR)+'].sub_category') 
									,item_category=JSON_VALUE(@jsonInput, '$.changes['+CAST(@LoopIndex AS VARCHAR)+'].item_category') 
									--,description=JSON_VALUE(@jsonInput, '$.changes['+CAST(@LoopIndex AS VARCHAR)+'].description') 
									,planned_effort=JSON_VALUE(@jsonInput, '$.changes['+CAST(@LoopIndex AS VARCHAR)+'].planned_effort') 
									,description_text=JSON_VALUE(@jsonInput, '$.changes['+CAST(@LoopIndex AS VARCHAR)+'].description_text') 
									,requester_id=TRY_PARSE(JSON_VALUE(@jsonInput, '$.changes['+CAST(@LoopIndex AS VARCHAR)+'].requester_id') AS BIGINT) 
									,approval_status=TRY_PARSE(JSON_VALUE(@jsonInput, '$.changes['+CAST(@LoopIndex AS VARCHAR)+'].approval_status') AS INT) 
									,change_window_id=JSON_VALUE(@jsonInput, '$.changes['+CAST(@LoopIndex AS VARCHAR)+'].change_window_id') 
									,workspace_id=TRY_PARSE(JSON_VALUE(@jsonInput, '$.changes['+CAST(@LoopIndex AS VARCHAR)+'].workspace_id') AS INT) 
									,tasks_dependency_type=TRY_PARSE(JSON_VALUE(@jsonInput, '$.changes['+CAST(@LoopIndex AS VARCHAR)+'].tasks_dependency_type') AS INT)
									,updated_at=TRY_PARSE(JSON_VALUE(@jsonInput, '$.changes['+CAST(@LoopIndex AS VARCHAR)+'].updated_at') AS DATETIMEOFFSET)
									,updated_on=GETUTCDATE()
								WHERE ID=@Id
							END

							
							IF NOT EXISTS (SELECT * FROM FreshService_T_Change_CustomFields WITH(NOLOCK)
							WHERE change_id=@Id)
							BEGIN
								INSERT INTO FreshService_T_Change_CustomFields
								(
									change_id
									,tenant
									,elevated_call
									,on_roaster_engineer
									,nsd_member_name
									,resolution_remarks
								)
								SELECT @Id
								,JSON_VALUE(@jsonInput, '$.changes['+CAST(@LoopIndex AS VARCHAR)+'].custom_fields.tenant')
								,JSON_VALUE(@jsonInput, '$.changes['+CAST(@LoopIndex AS VARCHAR)+'].custom_fields.elevated_call')
								,JSON_VALUE(@jsonInput, '$.changes['+CAST(@LoopIndex AS VARCHAR)+'].custom_fields.on_roaster_engineer')
								,JSON_VALUE(@jsonInput, '$.changes['+CAST(@LoopIndex AS VARCHAR)+'].custom_fields.nsd_member_name')
								,JSON_VALUE(@jsonInput, '$.changes['+CAST(@LoopIndex AS VARCHAR)+'].custom_fields.resolution_remarks')
								
									
							END
						

						END

						COMMIT TRANSACTION
					END TRY
					BEGIN CATCH
						ROLLBACK TRANSACTION
					END CATCH
					
					SET @LoopIndex=@LoopIndex+1
				END

				SET @OutPutId = 1
				SET @OutPutMsg='Data saved successfully.' 

			END
			ELSE
			BEGIN
				SET @OutPutId = 0
				SET @OutPutMsg='Data already exists' 
			END
		
			
		
	END TRY
	BEGIN CATCH
	-----------------
		--ROLLBACK TRANSACTION
		DECLARE @error int, @message varchar(4000), @xstate int;  
		Select @error = ERROR_NUMBER(), @message = ERROR_MESSAGE(), @xstate = XACT_STATE();  
		RAISERROR ('usp_FS_T_Change_IU: %d: %s', 16, 1, @error, @message) ;  
		SET @OutPutId = 0
		SET @OutPutMsg=@message  

		SELECT '' AS [Message],
			@OutPutMsg AS ErrorMessage,
			@OutPutId AS [Status],
			'0' AS Id

		RETURN
	END CATCH
	-----------------
	
	SELECT @OutPutMsg AS [Message],
	'' AS ErrorMessage,
	@OutPutId AS [Status],
	'1' AS Id


	--COMMIT TRANSACTION
	
  
		
END
GO
/****** Object:  StoredProcedure [dbo].[usp_FreshService_T_Problems_InsertUpdate]    Script Date: 09/06/2025 10:37:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO



CREATE   PROCEDURE [dbo].[usp_FreshService_T_Problems_InsertUpdate]
(
	@jsonInput			VARCHAR(MAX)
)  
AS
BEGIN
    
    SET NOCOUNT ON;  
    
	declare @OutPutId			BIGINT=NULL
	declare @OutPutMsg			VARCHAR(255)=NULL

	SET @OutPutId = 0
	SET @OutPutMsg='Sorry something went wrong.'

	
	IF ISNULL(@jsonInput,'')=''
	BEGIN
		SET @OutPutId = 0
		SET @OutPutMsg='Invalid details, please check your entry.' 
		RETURN
	END

	BEGIN TRY 
		
		
			
			DECLARE @Id BIGINT

			IF  (@jsonInput IS NOT NULL AND iSNULL(@jsonInput,'') <> '') 
			BEGIN				
				
				DECLARE @MaxRowIndex INT, @LoopIndex INT=0		
				
				SELECT @MaxRowIndex=MAX(cast([key] as int)) FROM OPENJSON(@jsonInput, '$.problems')

				WHILE @LoopIndex <= ISNULL(@MaxRowIndex,-1)
				BEGIN					
					BEGIN TRY 
						BEGIN TRANSACTION 

						SET @Id=TRY_PARSE(JSON_VALUE(@jsonInput, '$.problems['+CAST(@LoopIndex AS VARCHAR)+'].id') as BIGINT)
						IF(@Id IS NOT NULL)
						BEGIN
							IF NOT EXISTS (SELECT * FROM FreshService_T_Problems WITH(NOLOCK) WHERE ID=@Id)
							BEGIN
								INSERT INTO FreshService_T_Problems
								(
									id
									,agent_id
									,group_id
									,[priority]
									,impact
									,[status]
									,due_by
									,known_error
									,planned_start_date
									,planned_end_date
									,[subject]
									,department_id
									,category
									,sub_category
									,item_category
									--,[description]
									,planned_effort
									,description_text
									,requester_id
									,workspace_id
									,tasks_dependency_type
									--,custom_fields_nsd_member_name
									--,custom_fields_on_roaster_engineer
									,created_at
									,updated_at
									,created_on
									,updated_on
								)
								SELECT @Id
									,TRY_PARSE(JSON_VALUE(@jsonInput, '$.problems['+CAST(@LoopIndex AS VARCHAR)+'].agent_id') AS BIGINT)
									,TRY_PARSE(JSON_VALUE(@jsonInput, '$.problems['+CAST(@LoopIndex AS VARCHAR)+'].group_id') AS BIGINT)
									,TRY_PARSE(JSON_VALUE(@jsonInput, '$.problems['+CAST(@LoopIndex AS VARCHAR)+'].priority')AS INT)
									,TRY_PARSE(JSON_VALUE(@jsonInput, '$.problems['+CAST(@LoopIndex AS VARCHAR)+'].impact') AS INT)
									,TRY_PARSE(JSON_VALUE(@jsonInput, '$.problems['+CAST(@LoopIndex AS VARCHAR)+'].status') AS INT)
									,TRY_PARSE(JSON_VALUE(@jsonInput, '$.problems['+CAST(@LoopIndex AS VARCHAR)+'].due_by') AS DATETIMEOFFSET)
									,JSON_VALUE(@jsonInput, '$.problems['+CAST(@LoopIndex AS VARCHAR)+'].known_error')
									,TRY_PARSE(JSON_VALUE(@jsonInput, '$.problems['+CAST(@LoopIndex AS VARCHAR)+'].planned_start_date') AS DATETIMEOFFSET)
									,TRY_PARSE(JSON_VALUE(@jsonInput, '$.problems['+CAST(@LoopIndex AS VARCHAR)+'].planned_end_date') AS DATETIMEOFFSET)
									,JSON_VALUE(@jsonInput, '$.problems['+CAST(@LoopIndex AS VARCHAR)+'].subject') 
									,TRY_PARSE(JSON_VALUE(@jsonInput, '$.problems['+CAST(@LoopIndex AS VARCHAR)+'].department_id') AS BIGINT)
									,JSON_VALUE(@jsonInput, '$.problems['+CAST(@LoopIndex AS VARCHAR)+'].category') 
									,JSON_VALUE(@jsonInput, '$.problems['+CAST(@LoopIndex AS VARCHAR)+'].sub_category') 
									,JSON_VALUE(@jsonInput, '$.problems['+CAST(@LoopIndex AS VARCHAR)+'].item_category') 
									--,JSON_VALUE(@jsonInput, '$.problems['+CAST(@LoopIndex AS VARCHAR)+'].description') 
									,JSON_VALUE(@jsonInput, '$.problems['+CAST(@LoopIndex AS VARCHAR)+'].planned_effort') 
									,JSON_VALUE(@jsonInput, '$.problems['+CAST(@LoopIndex AS VARCHAR)+'].description_text') 
									,TRY_PARSE(JSON_VALUE(@jsonInput, '$.problems['+CAST(@LoopIndex AS VARCHAR)+'].requester_id') AS BIGINT) 
									,TRY_PARSE(JSON_VALUE(@jsonInput, '$.problems['+CAST(@LoopIndex AS VARCHAR)+'].workspace_id') AS INT) 
									,TRY_PARSE(JSON_VALUE(@jsonInput, '$.problems['+CAST(@LoopIndex AS VARCHAR)+'].tasks_dependency_type') AS INT) 
									--,JSON_VALUE(@jsonInput, '$.problems['+CAST(@LoopIndex AS VARCHAR)+'].custom_fields.nsd_member_name') 
									--,JSON_VALUE(@jsonInput, '$.problems['+CAST(@LoopIndex AS VARCHAR)+'].custom_fields.on_roaster_engineer') 
									,TRY_PARSE(JSON_VALUE(@jsonInput, '$.problems['+CAST(@LoopIndex AS VARCHAR)+'].created_at') AS DATETIMEOFFSET)
									,TRY_PARSE(JSON_VALUE(@jsonInput, '$.problems['+CAST(@LoopIndex AS VARCHAR)+'].updated_at') AS DATETIMEOFFSET)
									,GETUTCDATE()
									,GETUTCDATE()
							END
							ELSE
							BEGIN
								UPDATE FreshService_T_Problems
								SET 
									 agent_id=TRY_PARSE(JSON_VALUE(@jsonInput, '$.problems['+CAST(@LoopIndex AS VARCHAR)+'].agent_id') AS BIGINT)
									,group_id=TRY_PARSE(JSON_VALUE(@jsonInput, '$.problems['+CAST(@LoopIndex AS VARCHAR)+'].group_id') AS BIGINT)
									,[priority]=TRY_PARSE(JSON_VALUE(@jsonInput, '$.problems['+CAST(@LoopIndex AS VARCHAR)+'].priority')AS INT)
									,impact=TRY_PARSE(JSON_VALUE(@jsonInput, '$.problems['+CAST(@LoopIndex AS VARCHAR)+'].impact') AS INT)
									,[status]=TRY_PARSE(JSON_VALUE(@jsonInput, '$.problems['+CAST(@LoopIndex AS VARCHAR)+'].status') AS INT)
									,due_by=TRY_PARSE(JSON_VALUE(@jsonInput, '$.problems['+CAST(@LoopIndex AS VARCHAR)+'].due_by') AS DATETIMEOFFSET)
									,known_error=JSON_VALUE(@jsonInput, '$.problems['+CAST(@LoopIndex AS VARCHAR)+'].known_error')
									,planned_start_date=TRY_PARSE(JSON_VALUE(@jsonInput, '$.problems['+CAST(@LoopIndex AS VARCHAR)+'].planned_start_date') AS DATETIMEOFFSET)
									,planned_end_date=TRY_PARSE(JSON_VALUE(@jsonInput, '$.problems['+CAST(@LoopIndex AS VARCHAR)+'].planned_end_date') AS DATETIMEOFFSET)
									,[subject]=JSON_VALUE(@jsonInput, '$.problems['+CAST(@LoopIndex AS VARCHAR)+'].subject') 
									,department_id=TRY_PARSE(JSON_VALUE(@jsonInput, '$.problems['+CAST(@LoopIndex AS VARCHAR)+'].department_id') AS BIGINT)
									,category=JSON_VALUE(@jsonInput, '$.problems['+CAST(@LoopIndex AS VARCHAR)+'].category') 
									,sub_category=JSON_VALUE(@jsonInput, '$.problems['+CAST(@LoopIndex AS VARCHAR)+'].sub_category') 
									,item_category=JSON_VALUE(@jsonInput, '$.problems['+CAST(@LoopIndex AS VARCHAR)+'].item_category') 
									--,description=JSON_VALUE(@jsonInput, '$.problems['+CAST(@LoopIndex AS VARCHAR)+'].description') 
									,planned_effort=JSON_VALUE(@jsonInput, '$.problems['+CAST(@LoopIndex AS VARCHAR)+'].planned_effort') 
									,description_text=JSON_VALUE(@jsonInput, '$.problems['+CAST(@LoopIndex AS VARCHAR)+'].description_text') 
									,requester_id=TRY_PARSE(JSON_VALUE(@jsonInput, '$.problems['+CAST(@LoopIndex AS VARCHAR)+'].requester_id') AS BIGINT) 
									,workspace_id=TRY_PARSE(JSON_VALUE(@jsonInput, '$.problems['+CAST(@LoopIndex AS VARCHAR)+'].workspace_id') AS INT) 
									,tasks_dependency_type=TRY_PARSE(JSON_VALUE(@jsonInput, '$.problems['+CAST(@LoopIndex AS VARCHAR)+'].tasks_dependency_type') AS INT)
									--,custom_fields_nsd_member_name=JSON_VALUE(@jsonInput, '$.problems['+CAST(@LoopIndex AS VARCHAR)+'].custom_fields.nsd_member_name') 
									--,custom_fields_on_roaster_engineer=JSON_VALUE(@jsonInput, '$.problems['+CAST(@LoopIndex AS VARCHAR)+'].custom_fields.on_roaster_engineer') 
									,updated_at=TRY_PARSE(JSON_VALUE(@jsonInput, '$.problems['+CAST(@LoopIndex AS VARCHAR)+'].updated_at') AS DATETIMEOFFSET)
									,updated_on=GETUTCDATE()
								WHERE ID=@Id
							END

							

						END

						COMMIT TRANSACTION
					END TRY
					BEGIN CATCH
						ROLLBACK TRANSACTION
					END CATCH
					
					SET @LoopIndex=@LoopIndex+1
				END

				SET @OutPutId = 1
				SET @OutPutMsg='Data saved successfully.' 

			END
			ELSE
			BEGIN
				SET @OutPutId = 0
				SET @OutPutMsg='Data already exists' 
			END
		
			
		
	END TRY
	BEGIN CATCH
	-----------------
		--ROLLBACK TRANSACTION
		DECLARE @error int, @message varchar(4000), @xstate int;  
		Select @error = ERROR_NUMBER(), @message = ERROR_MESSAGE(), @xstate = XACT_STATE();  
		RAISERROR ('usp_FS_T_Problem_IU: %d: %s', 16, 1, @error, @message) ;  
		SET @OutPutId = 0
		SET @OutPutMsg=@message  

		SELECT '' AS [Message],
			@OutPutMsg AS ErrorMessage,
			@OutPutId AS [Status],
			'0' AS Id

		RETURN
	END CATCH
	-----------------
	
	SELECT @OutPutMsg AS [Message],
	'' AS ErrorMessage,
	@OutPutId AS [Status],
	'1' AS Id


	--COMMIT TRANSACTION
	
  
		
END
GO
/****** Object:  StoredProcedure [dbo].[usp_FreshService_T_Tickets_InsertUpdate]    Script Date: 09/06/2025 10:37:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE   PROCEDURE [dbo].[usp_FreshService_T_Tickets_InsertUpdate]
(
	@jsonInput			VARCHAR(MAX)
)  
AS
BEGIN
    
    SET NOCOUNT ON;  
    
	declare @OutPutId			BIGINT=NULL
	declare @OutPutMsg			VARCHAR(255)=NULL

	SET @OutPutId = 0
	SET @OutPutMsg='Sorry something went wrong.'

	
	IF ISNULL(@jsonInput,'')=''
	BEGIN
		SET @OutPutId = 0
		SET @OutPutMsg='Invalid details, please check your entry.' 
		RETURN
	END

	BEGIN TRY 
		
		
			
			DECLARE @Id BIGINT

			IF  (@jsonInput IS NOT NULL AND iSNULL(@jsonInput,'') <> '') 
			BEGIN
				
				DECLARE @MaxRowIndex INT, @LoopIndex INT=0		
				
				SELECT @MaxRowIndex=MAX(cast([key] as int)) FROM OPENJSON(@jsonInput, '$.tickets')

				WHILE @LoopIndex <= ISNULL(@MaxRowIndex,-1)
				BEGIN					
					BEGIN TRY 
						BEGIN TRANSACTION 
						SET @Id=TRY_PARSE(JSON_VALUE(@jsonInput, '$.tickets['+CAST(@LoopIndex AS VARCHAR)+'].id') as BIGINT)
						IF(@Id IS NOT NULL)
						BEGIN
							IF NOT EXISTS (SELECT * FROM FreshService_T_Tickets WITH(NOLOCK) WHERE ID=@Id)
							BEGIN
								INSERT INTO FreshService_T_Tickets
								(
									id
									,[subject]
									,[group_id]
									,department_id
									,category
									,sub_category
									,item_category
									,requester_id
									,responder_id
									,due_by
									,fr_escalated
									,deleted
									,is_escalated
									,fr_due_by
									,[priority]
									,[status]
									,[source]
									,workspace_id
									,requested_for_id
									,[type]
									,description_text
									,department_name
									,tasks_dependency_type
									,created_at
									,updated_at
									,created_on
									,updated_on
								)
								SELECT @Id
									,JSON_VALUE(@jsonInput, '$.tickets['+CAST(@LoopIndex AS VARCHAR)+'].subject')
									,TRY_PARSE(JSON_VALUE(@jsonInput, '$.tickets['+CAST(@LoopIndex AS VARCHAR)+'].group_id') AS BIGINT)
									,TRY_PARSE(JSON_VALUE(@jsonInput, '$.tickets['+CAST(@LoopIndex AS VARCHAR)+'].department_id') AS BIGINT)
									,JSON_VALUE(@jsonInput, '$.tickets['+CAST(@LoopIndex AS VARCHAR)+'].category')
									,JSON_VALUE(@jsonInput, '$.tickets['+CAST(@LoopIndex AS VARCHAR)+'].sub_category')
									,JSON_VALUE(@jsonInput, '$.tickets['+CAST(@LoopIndex AS VARCHAR)+'].item_category')
									,TRY_PARSE(JSON_VALUE(@jsonInput, '$.tickets['+CAST(@LoopIndex AS VARCHAR)+'].requester_id') AS BIGINT)
									,TRY_PARSE(JSON_VALUE(@jsonInput, '$.tickets['+CAST(@LoopIndex AS VARCHAR)+'].responder_id') AS BIGINT)
									,TRY_PARSE(JSON_VALUE(@jsonInput, '$.tickets['+CAST(@LoopIndex AS VARCHAR)+'].due_by') AS DATETIMEOFFSET)
									,JSON_VALUE(@jsonInput, '$.tickets['+CAST(@LoopIndex AS VARCHAR)+'].fr_escalated')
									,JSON_VALUE(@jsonInput, '$.tickets['+CAST(@LoopIndex AS VARCHAR)+'].deleted')
									,JSON_VALUE(@jsonInput, '$.tickets['+CAST(@LoopIndex AS VARCHAR)+'].is_escalated')
									,TRY_PARSE(JSON_VALUE(@jsonInput, '$.tickets['+CAST(@LoopIndex AS VARCHAR)+'].fr_due_by') AS DATETIMEOFFSET)
									,TRY_PARSE(JSON_VALUE(@jsonInput, '$.tickets['+CAST(@LoopIndex AS VARCHAR)+'].priority') AS INT)
									,TRY_PARSE(JSON_VALUE(@jsonInput, '$.tickets['+CAST(@LoopIndex AS VARCHAR)+'].status') AS INT)
									,TRY_PARSE(JSON_VALUE(@jsonInput, '$.tickets['+CAST(@LoopIndex AS VARCHAR)+'].source') AS INT)
									,TRY_PARSE(JSON_VALUE(@jsonInput, '$.tickets['+CAST(@LoopIndex AS VARCHAR)+'].workspace_id') AS INT)
									,TRY_PARSE(JSON_VALUE(@jsonInput, '$.tickets['+CAST(@LoopIndex AS VARCHAR)+'].requested_for_id') AS BIGINT)
									,JSON_VALUE(@jsonInput, '$.tickets['+CAST(@LoopIndex AS VARCHAR)+'].type')
									,JSON_VALUE(@jsonInput, '$.tickets['+CAST(@LoopIndex AS VARCHAR)+'].description_text')
									,JSON_VALUE(@jsonInput, '$.tickets['+CAST(@LoopIndex AS VARCHAR)+'].department_name')
									,TRY_PARSE(JSON_VALUE(@jsonInput, '$.tickets['+CAST(@LoopIndex AS VARCHAR)+'].tasks_dependency_type') AS INT)
									,TRY_PARSE(JSON_VALUE(@jsonInput, '$.tickets['+CAST(@LoopIndex AS VARCHAR)+'].created_at') AS DATETIMEOFFSET)
									,TRY_PARSE(JSON_VALUE(@jsonInput, '$.tickets['+CAST(@LoopIndex AS VARCHAR)+'].updated_at') AS DATETIMEOFFSET)
									,GETUTCDATE()
									,GETUTCDATE()
							END
							ELSE
							BEGIN
								UPDATE FreshService_T_Tickets
								SET 
									[subject]=JSON_VALUE(@jsonInput, '$.tickets['+CAST(@LoopIndex AS VARCHAR)+'].subject')
									,group_id=TRY_PARSE(JSON_VALUE(@jsonInput, '$.tickets['+CAST(@LoopIndex AS VARCHAR)+'].group_id') AS BIGINT)
									,department_id=TRY_PARSE(JSON_VALUE(@jsonInput, '$.tickets['+CAST(@LoopIndex AS VARCHAR)+'].department_id') AS BIGINT)
									,category=JSON_VALUE(@jsonInput, '$.tickets['+CAST(@LoopIndex AS VARCHAR)+'].category')
									,sub_category=JSON_VALUE(@jsonInput, '$.tickets['+CAST(@LoopIndex AS VARCHAR)+'].sub_category')
									,item_category=JSON_VALUE(@jsonInput, '$.tickets['+CAST(@LoopIndex AS VARCHAR)+'].item_category')
									,requester_id=TRY_PARSE(JSON_VALUE(@jsonInput, '$.tickets['+CAST(@LoopIndex AS VARCHAR)+'].requester_id') AS BIGINT)
									,responder_id=TRY_PARSE(JSON_VALUE(@jsonInput, '$.tickets['+CAST(@LoopIndex AS VARCHAR)+'].responder_id') AS BIGINT)
									,due_by=TRY_PARSE(JSON_VALUE(@jsonInput, '$.tickets['+CAST(@LoopIndex AS VARCHAR)+'].due_by') AS DATETIMEOFFSET)
									,fr_escalated=JSON_VALUE(@jsonInput, '$.tickets['+CAST(@LoopIndex AS VARCHAR)+'].fr_escalated') 
									,deleted=JSON_VALUE(@jsonInput, '$.tickets['+CAST(@LoopIndex AS VARCHAR)+'].deleted') 
									,is_escalated=JSON_VALUE(@jsonInput, '$.tickets['+CAST(@LoopIndex AS VARCHAR)+'].is_escalated')
									,fr_due_by=TRY_PARSE(JSON_VALUE(@jsonInput, '$.tickets['+CAST(@LoopIndex AS VARCHAR)+'].fr_due_by') AS DATETIMEOFFSET)
									,[priority]=TRY_PARSE(JSON_VALUE(@jsonInput, '$.tickets['+CAST(@LoopIndex AS VARCHAR)+'].priority') AS INT)
									,[status]=TRY_PARSE(JSON_VALUE(@jsonInput, '$.tickets['+CAST(@LoopIndex AS VARCHAR)+'].status') AS INT)
									,[source]=TRY_PARSE(JSON_VALUE(@jsonInput, '$.tickets['+CAST(@LoopIndex AS VARCHAR)+'].source') AS INT)
									,workspace_id=TRY_PARSE(JSON_VALUE(@jsonInput, '$.tickets['+CAST(@LoopIndex AS VARCHAR)+'].workspace_id') AS INT)
									,requested_for_id=TRY_PARSE(JSON_VALUE(@jsonInput, '$.tickets['+CAST(@LoopIndex AS VARCHAR)+'].requested_for_id') AS BIGINT)
									,[type]=JSON_VALUE(@jsonInput, '$.tickets['+CAST(@LoopIndex AS VARCHAR)+'].type')
									,description_text=JSON_VALUE(@jsonInput, '$.tickets['+CAST(@LoopIndex AS VARCHAR)+'].description_text')
									,department_name=JSON_VALUE(@jsonInput, '$.tickets['+CAST(@LoopIndex AS VARCHAR)+'].department_name')
									,updated_at=TRY_PARSE(JSON_VALUE(@jsonInput, '$.tickets['+CAST(@LoopIndex AS VARCHAR)+'].updated_at') AS DATETIMEOFFSET)
									,updated_on=GETUTCDATE()
								WHERE ID=@Id
							END

							--Custom Fields
							IF NOT EXISTS (SELECT * FROM FreshService_T_Ticket_CustomFields WITH(NOLOCK) WHERE ticket_id=@Id)
							BEGIN
								INSERT INTO FreshService_T_Ticket_CustomFields
								(
									ticket_id
									,[location]
									,major_incident_type
									,nsd_member_name
									,oem_case_id_logged
									,on_roaster_engineer
									,resolution_type
									,support_type
									,tenant
									,ticket_mode
									,ticket_monitoring_owner
									,time_track_mandate
									,user_type
									,parent_ticket_id
									,resolution_remarks
									,resource_name
									,problem_statement
									,oem_case_idif_any
									,sales_account_manager
									,sl_no
									,pid
									,model
									,product
								)
								SELECT @Id
									,JSON_VALUE(@jsonInput, '$.tickets['+CAST(@LoopIndex AS VARCHAR)+'].custom_fields.location')
									,JSON_VALUE(@jsonInput, '$.tickets['+CAST(@LoopIndex AS VARCHAR)+'].custom_fields.major_incident_type')
									,JSON_VALUE(@jsonInput, '$.tickets['+CAST(@LoopIndex AS VARCHAR)+'].custom_fields.nsd_member_name')
									,JSON_VALUE(@jsonInput, '$.tickets['+CAST(@LoopIndex AS VARCHAR)+'].custom_fields.oem_case_id_logged')
									,JSON_VALUE(@jsonInput, '$.tickets['+CAST(@LoopIndex AS VARCHAR)+'].custom_fields.on_roaster_engineer')
									,JSON_VALUE(@jsonInput, '$.tickets['+CAST(@LoopIndex AS VARCHAR)+'].custom_fields.resolution_type')
									,JSON_VALUE(@jsonInput, '$.tickets['+CAST(@LoopIndex AS VARCHAR)+'].custom_fields.support_type')
									,JSON_VALUE(@jsonInput, '$.tickets['+CAST(@LoopIndex AS VARCHAR)+'].custom_fields.tenant')
									,JSON_VALUE(@jsonInput, '$.tickets['+CAST(@LoopIndex AS VARCHAR)+'].custom_fields.ticket_mode')
									,JSON_VALUE(@jsonInput, '$.tickets['+CAST(@LoopIndex AS VARCHAR)+'].custom_fields.ticket_monitoring_owner')
									,JSON_VALUE(@jsonInput, '$.tickets['+CAST(@LoopIndex AS VARCHAR)+'].custom_fields.time_track_mandate')
									,JSON_VALUE(@jsonInput, '$.tickets['+CAST(@LoopIndex AS VARCHAR)+'].custom_fields.user_type')
									,JSON_VALUE(@jsonInput, '$.tickets['+CAST(@LoopIndex AS VARCHAR)+'].custom_fields.parent_ticket_id')
									,JSON_VALUE(@jsonInput, '$.tickets['+CAST(@LoopIndex AS VARCHAR)+'].custom_fields.resolution_remarks')
									,JSON_VALUE(@jsonInput, '$.tickets['+CAST(@LoopIndex AS VARCHAR)+'].custom_fields.resource_name')
									,JSON_VALUE(@jsonInput, '$.tickets['+CAST(@LoopIndex AS VARCHAR)+'].custom_fields.problem_statement')
									,JSON_VALUE(@jsonInput, '$.tickets['+CAST(@LoopIndex AS VARCHAR)+'].custom_fields.oem_case_idif_any')
									,JSON_VALUE(@jsonInput, '$.tickets['+CAST(@LoopIndex AS VARCHAR)+'].custom_fields.sales_account_manager')
									,JSON_VALUE(@jsonInput, '$.tickets['+CAST(@LoopIndex AS VARCHAR)+'].custom_fields.sl_no')
									,JSON_VALUE(@jsonInput, '$.tickets['+CAST(@LoopIndex AS VARCHAR)+'].custom_fields.pid')
									,JSON_VALUE(@jsonInput, '$.tickets['+CAST(@LoopIndex AS VARCHAR)+'].custom_fields.model')
									,JSON_VALUE(@jsonInput, '$.tickets['+CAST(@LoopIndex AS VARCHAR)+'].custom_fields.product')
								
							END
							ELSE
							BEGIN
								UPDATE FreshService_T_Ticket_CustomFields
								SET 
									[location]=JSON_VALUE(@jsonInput, '$.tickets['+CAST(@LoopIndex AS VARCHAR)+'].custom_fields.location')
									,major_incident_type=JSON_VALUE(@jsonInput, '$.tickets['+CAST(@LoopIndex AS VARCHAR)+'].custom_fields.major_incident_type')
									,nsd_member_name=JSON_VALUE(@jsonInput, '$.tickets['+CAST(@LoopIndex AS VARCHAR)+'].custom_fields.nsd_member_name')
									,oem_case_id_logged=JSON_VALUE(@jsonInput, '$.tickets['+CAST(@LoopIndex AS VARCHAR)+'].custom_fields.oem_case_id_logged')
									,on_roaster_engineer=JSON_VALUE(@jsonInput, '$.tickets['+CAST(@LoopIndex AS VARCHAR)+'].custom_fields.on_roaster_engineer')
									,resolution_type=JSON_VALUE(@jsonInput, '$.tickets['+CAST(@LoopIndex AS VARCHAR)+'].custom_fields.resolution_type')
									,support_type=JSON_VALUE(@jsonInput, '$.tickets['+CAST(@LoopIndex AS VARCHAR)+'].custom_fields.support_type')
									,tenant=JSON_VALUE(@jsonInput, '$.tickets['+CAST(@LoopIndex AS VARCHAR)+'].custom_fields.tenant')
									,ticket_mode=JSON_VALUE(@jsonInput, '$.tickets['+CAST(@LoopIndex AS VARCHAR)+'].custom_fields.ticket_mode')
									,ticket_monitoring_owner=JSON_VALUE(@jsonInput, '$.tickets['+CAST(@LoopIndex AS VARCHAR)+'].custom_fields.ticket_monitoring_owner')
									,time_track_mandate=JSON_VALUE(@jsonInput, '$.tickets['+CAST(@LoopIndex AS VARCHAR)+'].custom_fields.time_track_mandate')
									,user_type=JSON_VALUE(@jsonInput, '$.tickets['+CAST(@LoopIndex AS VARCHAR)+'].custom_fields.user_type')
									,parent_ticket_id=JSON_VALUE(@jsonInput, '$.tickets['+CAST(@LoopIndex AS VARCHAR)+'].custom_fields.parent_ticket_id')
									,resolution_remarks=JSON_VALUE(@jsonInput, '$.tickets['+CAST(@LoopIndex AS VARCHAR)+'].custom_fields.resolution_remarks')
									,resource_name=JSON_VALUE(@jsonInput, '$.tickets['+CAST(@LoopIndex AS VARCHAR)+'].custom_fields.resource_name')
									,problem_statement=JSON_VALUE(@jsonInput, '$.tickets['+CAST(@LoopIndex AS VARCHAR)+'].custom_fields.problem_statement')
									,oem_case_idif_any=JSON_VALUE(@jsonInput, '$.tickets['+CAST(@LoopIndex AS VARCHAR)+'].custom_fields.oem_case_idif_any')
									,sales_account_manager=JSON_VALUE(@jsonInput, '$.tickets['+CAST(@LoopIndex AS VARCHAR)+'].custom_fields.sales_account_manager')
									,sl_no=JSON_VALUE(@jsonInput, '$.tickets['+CAST(@LoopIndex AS VARCHAR)+'].custom_fields.sl_no')
									,pid=JSON_VALUE(@jsonInput, '$.tickets['+CAST(@LoopIndex AS VARCHAR)+'].custom_fields.pid')
									,model=JSON_VALUE(@jsonInput, '$.tickets['+CAST(@LoopIndex AS VARCHAR)+'].custom_fields.model')
									,product=JSON_VALUE(@jsonInput, '$.tickets['+CAST(@LoopIndex AS VARCHAR)+'].custom_fields.product')
								WHERE ticket_id =@Id
							END


							--Stats
							IF NOT EXISTS (SELECT * FROM FreshService_T_Ticket_Stats WITH(NOLOCK) WHERE ticket_id=@Id)
							BEGIN

								INSERT INTO FreshService_T_Ticket_Stats
								(
									ticket_id
									,opened_at
									,group_escalated
									,inbound_count
									,status_updated_at
									,outbound_count
									,pending_since
									,resolved_at
									,closed_at
									,first_assigned_at
									,assigned_at
									,agent_responded_at
									,requester_responded_at
									,first_responded_at
									,first_resp_time_in_secs
									,resolution_time_in_secs
									,created_at
									,updated_at
								)
								SELECT @Id
									,TRY_PARSE(JSON_VALUE(@jsonInput, '$.tickets['+CAST(@LoopIndex AS VARCHAR)+'].stats.opened_at') AS DATETIME)
									,JSON_VALUE(@jsonInput, '$.tickets['+CAST(@LoopIndex AS VARCHAR)+'].stats.group_escalated')
									,TRY_PARSE(JSON_VALUE(@jsonInput, '$.tickets['+CAST(@LoopIndex AS VARCHAR)+'].stats.inbound_count') AS INT)
									,TRY_PARSE(JSON_VALUE(@jsonInput, '$.tickets['+CAST(@LoopIndex AS VARCHAR)+'].stats.status_updated_at') AS DATETIME)
									,TRY_PARSE(JSON_VALUE(@jsonInput, '$.tickets['+CAST(@LoopIndex AS VARCHAR)+'].stats.outbound_count') AS INT)
									,JSON_VALUE(@jsonInput, '$.tickets['+CAST(@LoopIndex AS VARCHAR)+'].stats.pending_since')
									,TRY_PARSE(JSON_VALUE(@jsonInput, '$.tickets['+CAST(@LoopIndex AS VARCHAR)+'].stats.resolved_at') AS DATETIME)
									,TRY_PARSE(JSON_VALUE(@jsonInput, '$.tickets['+CAST(@LoopIndex AS VARCHAR)+'].stats.closed_at') AS DATETIME)
									,TRY_PARSE(JSON_VALUE(@jsonInput, '$.tickets['+CAST(@LoopIndex AS VARCHAR)+'].stats.first_assigned_at') AS DATETIME)
									,TRY_PARSE(JSON_VALUE(@jsonInput, '$.tickets['+CAST(@LoopIndex AS VARCHAR)+'].stats.assigned_at') AS DATETIME)
									,TRY_PARSE(JSON_VALUE(@jsonInput, '$.tickets['+CAST(@LoopIndex AS VARCHAR)+'].stats.agent_responded_at') AS DATETIME)
									,TRY_PARSE(JSON_VALUE(@jsonInput, '$.tickets['+CAST(@LoopIndex AS VARCHAR)+'].stats.requester_responded_at') AS DATETIME)
									,TRY_PARSE(JSON_VALUE(@jsonInput, '$.tickets['+CAST(@LoopIndex AS VARCHAR)+'].stats.first_responded_at') AS DATETIME)
									,TRY_PARSE(JSON_VALUE(@jsonInput, '$.tickets['+CAST(@LoopIndex AS VARCHAR)+'].stats.first_resp_time_in_secs') AS INT)
									,TRY_PARSE(JSON_VALUE(@jsonInput, '$.tickets['+CAST(@LoopIndex AS VARCHAR)+'].stats.resolution_time_in_secs') AS INT)
									,TRY_PARSE(JSON_VALUE(@jsonInput, '$.tickets['+CAST(@LoopIndex AS VARCHAR)+'].stats.created_at') AS DATETIMEOFFSET)
									,TRY_PARSE(JSON_VALUE(@jsonInput, '$.tickets['+CAST(@LoopIndex AS VARCHAR)+'].stats.updated_at') AS DATETIMEOFFSET)
								
							END
							ELSE
							BEGIN
								UPDATE FreshService_T_Ticket_Stats
								SET 
									opened_at=TRY_PARSE(JSON_VALUE(@jsonInput, '$.tickets['+CAST(@LoopIndex AS VARCHAR)+'].stats.opened_at') AS DATETIME)
									,group_escalated=JSON_VALUE(@jsonInput, '$.tickets['+CAST(@LoopIndex AS VARCHAR)+'].stats.group_escalated')
									,inbound_count=TRY_PARSE(JSON_VALUE(@jsonInput, '$.tickets['+CAST(@LoopIndex AS VARCHAR)+'].stats.inbound_count') AS INT)
									,status_updated_at=TRY_PARSE(JSON_VALUE(@jsonInput, '$.tickets['+CAST(@LoopIndex AS VARCHAR)+'].stats.status_updated_at') AS DATETIME)
									,outbound_count=TRY_PARSE(JSON_VALUE(@jsonInput, '$.tickets['+CAST(@LoopIndex AS VARCHAR)+'].stats.outbound_count') AS INT)
									,pending_since=JSON_VALUE(@jsonInput, '$.tickets['+CAST(@LoopIndex AS VARCHAR)+'].stats.pending_since')
									,resolved_at=TRY_PARSE(JSON_VALUE(@jsonInput, '$.tickets['+CAST(@LoopIndex AS VARCHAR)+'].stats.resolved_at') AS DATETIME)
									,closed_at=TRY_PARSE(JSON_VALUE(@jsonInput, '$.tickets['+CAST(@LoopIndex AS VARCHAR)+'].stats.closed_at') AS DATETIME)
									,first_assigned_at=TRY_PARSE(JSON_VALUE(@jsonInput, '$.tickets['+CAST(@LoopIndex AS VARCHAR)+'].stats.first_assigned_at') AS DATETIME)
									,assigned_at=TRY_PARSE(JSON_VALUE(@jsonInput, '$.tickets['+CAST(@LoopIndex AS VARCHAR)+'].stats.assigned_at') AS DATETIME)
									,agent_responded_at=TRY_PARSE(JSON_VALUE(@jsonInput, '$.tickets['+CAST(@LoopIndex AS VARCHAR)+'].stats.agent_responded_at') AS DATETIME)
									,requester_responded_at=TRY_PARSE(JSON_VALUE(@jsonInput, '$.tickets['+CAST(@LoopIndex AS VARCHAR)+'].stats.requester_responded_at') AS DATETIME)
									,first_responded_at=TRY_PARSE(JSON_VALUE(@jsonInput, '$.tickets['+CAST(@LoopIndex AS VARCHAR)+'].stats.first_responded_at') AS DATETIME)
									,first_resp_time_in_secs=TRY_PARSE(JSON_VALUE(@jsonInput, '$.tickets['+CAST(@LoopIndex AS VARCHAR)+'].stats.first_resp_time_in_secs') AS INT)
									,resolution_time_in_secs=TRY_PARSE(JSON_VALUE(@jsonInput, '$.tickets['+CAST(@LoopIndex AS VARCHAR)+'].stats.resolution_time_in_secs') AS INT)
									,created_at=TRY_PARSE(JSON_VALUE(@jsonInput, '$.tickets['+CAST(@LoopIndex AS VARCHAR)+'].stats.created_at') AS DATETIMEOFFSET)
									,updated_at=TRY_PARSE(JSON_VALUE(@jsonInput, '$.tickets['+CAST(@LoopIndex AS VARCHAR)+'].stats.updated_at') AS DATETIMEOFFSET)
								WHERE ticket_id =@Id
							END


							--Requesters
							IF NOT EXISTS (SELECT * FROM FreshService_T_TicketRequesters WITH(NOLOCK) WHERE ticket_id=@Id)
							BEGIN
								INSERT INTO FreshService_T_TicketRequesters
								(
									ticket_id
									,email
									,mobile
									,[name]
									,phone
								)
								SELECT @Id
									,JSON_VALUE(@jsonInput, '$.tickets['+CAST(@LoopIndex AS VARCHAR)+'].requester.email')
									,JSON_VALUE(@jsonInput, '$.tickets['+CAST(@LoopIndex AS VARCHAR)+'].requester.mobile')
									,JSON_VALUE(@jsonInput, '$.tickets['+CAST(@LoopIndex AS VARCHAR)+'].requester.name')
									,JSON_VALUE(@jsonInput, '$.tickets['+CAST(@LoopIndex AS VARCHAR)+'].requester.phone')
								
							END

							--RequestedFor
							IF NOT EXISTS (SELECT * FROM FreshService_T_TicketRequestedFor WITH(NOLOCK) WHERE ticket_id=@Id)
							BEGIN
								INSERT INTO FreshService_T_TicketRequestedFor
								(
									ticket_id
									,email
									,mobile
									,[name]
									,phone
								)
								SELECT @Id
									,JSON_VALUE(@jsonInput, '$.tickets['+CAST(@LoopIndex AS VARCHAR)+'].requester.email')
									,JSON_VALUE(@jsonInput, '$.tickets['+CAST(@LoopIndex AS VARCHAR)+'].requester.mobile')
									,JSON_VALUE(@jsonInput, '$.tickets['+CAST(@LoopIndex AS VARCHAR)+'].requester.name')
									,JSON_VALUE(@jsonInput, '$.tickets['+CAST(@LoopIndex AS VARCHAR)+'].requester.phone')
								
							END
						
							--Soumik Rev start
								DELETE FROM FreshService_T_Ticket_Tags WHERE ticket_id=@Id
				
								INSERT INTO FreshService_T_Ticket_Tags
								(
									[ticket_id],
									[RowIndex],
									[Tag]
								)
								SELECT @Id,[key],[value] 
								FROM OPENJSON(@jsonInput, '$.tickets['+CAST(@LoopIndex AS VARCHAR)+'].tags')
								order by [key] asc
							--Soumik Rev End

						END

						COMMIT TRANSACTION
					END TRY
					BEGIN CATCH
						ROLLBACK TRANSACTION
					END CATCH
					
					SET @LoopIndex=@LoopIndex+1
				END

				SET @OutPutId = 1
				SET @OutPutMsg='Data saved successfully.' 

			END
			ELSE
			BEGIN
				SET @OutPutId = 0
				SET @OutPutMsg='Data already exists' 
			END
		
			
		
	END TRY
	BEGIN CATCH
	-----------------
		--ROLLBACK TRANSACTION
		DECLARE @error int, @message varchar(4000), @xstate int;  
		Select @error = ERROR_NUMBER(), @message = ERROR_MESSAGE(), @xstate = XACT_STATE();  
		RAISERROR ('usp_FS_T_Tickets_IU: %d: %s', 16, 1, @error, @message) ;  
		SET @OutPutId = 0
		SET @OutPutMsg=@message  

		SELECT '' AS [Message],
			@OutPutMsg AS ErrorMessage,
			@OutPutId AS [Status],
			'0' AS Id

		RETURN
	END CATCH
	-----------------
	
	SELECT @OutPutMsg AS [Message],
	'' AS ErrorMessage,
	@OutPutId AS [Status],
	'1' AS Id


	--COMMIT TRANSACTION
	
  
		
END
GO
/****** Object:  StoredProcedure [dbo].[usp_FreshService_T_TicketsByCreatedDate_InsertUpdate]    Script Date: 09/06/2025 10:37:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE PROCEDURE [dbo].[usp_FreshService_T_TicketsByCreatedDate_InsertUpdate]
(
	@jsonInput			VARCHAR(MAX) = ''
)  
AS
BEGIN
    
    SET NOCOUNT ON;  
    
	declare @OutPutId			BIGINT=NULL
	declare @OutPutMsg			VARCHAR(255)=NULL

	SET @OutPutId = 0
	SET @OutPutMsg='Sorry something went wrong.'

	
	IF ISNULL(@jsonInput,'')=''
	BEGIN
		SET @OutPutId = 0
		SET @OutPutMsg='Invalid details, please check your entry.' 
		RETURN
	END

	BEGIN TRY 
		
		
			
			DECLARE @Id BIGINT

			IF  (@jsonInput IS NOT NULL AND iSNULL(@jsonInput,'') <> '') 
			BEGIN
				
				DECLARE @MaxRowIndex INT, @LoopIndex INT=0		
				
				SELECT @MaxRowIndex=MAX(cast([key] as int)) FROM OPENJSON(@jsonInput, '$.tickets')

				WHILE @LoopIndex <= ISNULL(@MaxRowIndex,-1)
				BEGIN					
					BEGIN TRY 
						BEGIN TRANSACTION 
						SET @Id=TRY_PARSE(JSON_VALUE(@jsonInput, '$.tickets['+CAST(@LoopIndex AS VARCHAR)+'].id') as BIGINT)
						IF(@Id IS NOT NULL)
						BEGIN
							IF NOT EXISTS (SELECT * FROM FreshService_T_Tickets WITH(NOLOCK) WHERE ID=@Id)
							BEGIN
								INSERT INTO FreshService_T_Tickets
								(
									id
									,[subject]
									,[group_id]
									,department_id
									,category
									,sub_category
									,item_category
									,requester_id
									,responder_id
									,due_by
									,fr_escalated
									,deleted
									,is_escalated
									,fr_due_by
									,[priority]
									,[status]
									,[source]
									,workspace_id
									,requested_for_id
									,[type]
									,description_text
									,department_name
									,tasks_dependency_type
									,created_at
									,updated_at
									,created_on
									,updated_on
								)
								SELECT @Id
									,JSON_VALUE(@jsonInput, '$.tickets['+CAST(@LoopIndex AS VARCHAR)+'].subject')
									,TRY_PARSE(JSON_VALUE(@jsonInput, '$.tickets['+CAST(@LoopIndex AS VARCHAR)+'].group_id') AS BIGINT)
									,TRY_PARSE(JSON_VALUE(@jsonInput, '$.tickets['+CAST(@LoopIndex AS VARCHAR)+'].department_id') AS BIGINT)
									,JSON_VALUE(@jsonInput, '$.tickets['+CAST(@LoopIndex AS VARCHAR)+'].category')
									,JSON_VALUE(@jsonInput, '$.tickets['+CAST(@LoopIndex AS VARCHAR)+'].sub_category')
									,JSON_VALUE(@jsonInput, '$.tickets['+CAST(@LoopIndex AS VARCHAR)+'].item_category')
									,TRY_PARSE(JSON_VALUE(@jsonInput, '$.tickets['+CAST(@LoopIndex AS VARCHAR)+'].requester_id') AS BIGINT)
									,TRY_PARSE(JSON_VALUE(@jsonInput, '$.tickets['+CAST(@LoopIndex AS VARCHAR)+'].responder_id') AS BIGINT)
									,TRY_PARSE(JSON_VALUE(@jsonInput, '$.tickets['+CAST(@LoopIndex AS VARCHAR)+'].due_by') AS DATETIMEOFFSET)
									,JSON_VALUE(@jsonInput, '$.tickets['+CAST(@LoopIndex AS VARCHAR)+'].fr_escalated')
									,JSON_VALUE(@jsonInput, '$.tickets['+CAST(@LoopIndex AS VARCHAR)+'].deleted')
									,JSON_VALUE(@jsonInput, '$.tickets['+CAST(@LoopIndex AS VARCHAR)+'].is_escalated')
									,TRY_PARSE(JSON_VALUE(@jsonInput, '$.tickets['+CAST(@LoopIndex AS VARCHAR)+'].fr_due_by') AS DATETIMEOFFSET)
									,TRY_PARSE(JSON_VALUE(@jsonInput, '$.tickets['+CAST(@LoopIndex AS VARCHAR)+'].priority') AS INT)
									,TRY_PARSE(JSON_VALUE(@jsonInput, '$.tickets['+CAST(@LoopIndex AS VARCHAR)+'].status') AS INT)
									,TRY_PARSE(JSON_VALUE(@jsonInput, '$.tickets['+CAST(@LoopIndex AS VARCHAR)+'].source') AS INT)
									,TRY_PARSE(JSON_VALUE(@jsonInput, '$.tickets['+CAST(@LoopIndex AS VARCHAR)+'].workspace_id') AS INT)
									,TRY_PARSE(JSON_VALUE(@jsonInput, '$.tickets['+CAST(@LoopIndex AS VARCHAR)+'].requested_for_id') AS BIGINT)
									,JSON_VALUE(@jsonInput, '$.tickets['+CAST(@LoopIndex AS VARCHAR)+'].type')
									,JSON_VALUE(@jsonInput, '$.tickets['+CAST(@LoopIndex AS VARCHAR)+'].description_text')
									,JSON_VALUE(@jsonInput, '$.tickets['+CAST(@LoopIndex AS VARCHAR)+'].department_name')
									,TRY_PARSE(JSON_VALUE(@jsonInput, '$.tickets['+CAST(@LoopIndex AS VARCHAR)+'].tasks_dependency_type') AS INT)
									,TRY_PARSE(JSON_VALUE(@jsonInput, '$.tickets['+CAST(@LoopIndex AS VARCHAR)+'].created_at') AS DATETIMEOFFSET)
									,TRY_PARSE(JSON_VALUE(@jsonInput, '$.tickets['+CAST(@LoopIndex AS VARCHAR)+'].updated_at') AS DATETIMEOFFSET)
									,GETUTCDATE()
									,GETUTCDATE()
							END
							ELSE
							BEGIN
								UPDATE FreshService_T_Tickets
								SET 
									[subject]=JSON_VALUE(@jsonInput, '$.tickets['+CAST(@LoopIndex AS VARCHAR)+'].subject')
									,group_id=TRY_PARSE(JSON_VALUE(@jsonInput, '$.tickets['+CAST(@LoopIndex AS VARCHAR)+'].group_id') AS BIGINT)
									,department_id=TRY_PARSE(JSON_VALUE(@jsonInput, '$.tickets['+CAST(@LoopIndex AS VARCHAR)+'].department_id') AS BIGINT)
									,category=JSON_VALUE(@jsonInput, '$.tickets['+CAST(@LoopIndex AS VARCHAR)+'].category')
									,sub_category=JSON_VALUE(@jsonInput, '$.tickets['+CAST(@LoopIndex AS VARCHAR)+'].sub_category')
									,item_category=JSON_VALUE(@jsonInput, '$.tickets['+CAST(@LoopIndex AS VARCHAR)+'].item_category')
									,requester_id=TRY_PARSE(JSON_VALUE(@jsonInput, '$.tickets['+CAST(@LoopIndex AS VARCHAR)+'].requester_id') AS BIGINT)
									,responder_id=TRY_PARSE(JSON_VALUE(@jsonInput, '$.tickets['+CAST(@LoopIndex AS VARCHAR)+'].responder_id') AS BIGINT)
									,due_by=TRY_PARSE(JSON_VALUE(@jsonInput, '$.tickets['+CAST(@LoopIndex AS VARCHAR)+'].due_by') AS DATETIMEOFFSET)
									,fr_escalated=JSON_VALUE(@jsonInput, '$.tickets['+CAST(@LoopIndex AS VARCHAR)+'].fr_escalated') 
									,deleted=JSON_VALUE(@jsonInput, '$.tickets['+CAST(@LoopIndex AS VARCHAR)+'].deleted') 
									,is_escalated=JSON_VALUE(@jsonInput, '$.tickets['+CAST(@LoopIndex AS VARCHAR)+'].is_escalated')
									,fr_due_by=TRY_PARSE(JSON_VALUE(@jsonInput, '$.tickets['+CAST(@LoopIndex AS VARCHAR)+'].fr_due_by') AS DATETIMEOFFSET)
									,[priority]=TRY_PARSE(JSON_VALUE(@jsonInput, '$.tickets['+CAST(@LoopIndex AS VARCHAR)+'].priority') AS INT)
									,[status]=TRY_PARSE(JSON_VALUE(@jsonInput, '$.tickets['+CAST(@LoopIndex AS VARCHAR)+'].status') AS INT)
									,[source]=TRY_PARSE(JSON_VALUE(@jsonInput, '$.tickets['+CAST(@LoopIndex AS VARCHAR)+'].source') AS INT)
									,workspace_id=TRY_PARSE(JSON_VALUE(@jsonInput, '$.tickets['+CAST(@LoopIndex AS VARCHAR)+'].workspace_id') AS INT)
									,requested_for_id=TRY_PARSE(JSON_VALUE(@jsonInput, '$.tickets['+CAST(@LoopIndex AS VARCHAR)+'].requested_for_id') AS BIGINT)
									,[type]=JSON_VALUE(@jsonInput, '$.tickets['+CAST(@LoopIndex AS VARCHAR)+'].type')
									,description_text=JSON_VALUE(@jsonInput, '$.tickets['+CAST(@LoopIndex AS VARCHAR)+'].description_text')
									,department_name=JSON_VALUE(@jsonInput, '$.tickets['+CAST(@LoopIndex AS VARCHAR)+'].department_name')
									,updated_at=TRY_PARSE(JSON_VALUE(@jsonInput, '$.tickets['+CAST(@LoopIndex AS VARCHAR)+'].updated_at') AS DATETIMEOFFSET)
									,updated_on=GETUTCDATE()
								WHERE ID=@Id
							END
		
					--Custom Fields
							IF NOT EXISTS (SELECT * FROM FreshService_T_Ticket_CustomFields WITH(NOLOCK) WHERE ticket_id=@Id)
							BEGIN
								INSERT INTO FreshService_T_Ticket_CustomFields
								(
									ticket_id
									,[location]
									,major_incident_type
									,nsd_member_name
									,oem_case_id_logged
									,on_roaster_engineer
									,resolution_type
									,support_type
									,tenant
									,ticket_mode
									,ticket_monitoring_owner
									,time_track_mandate
									,user_type
									,parent_ticket_id
									,resolution_remarks
									,resource_name
									,problem_statement
									,oem_case_idif_any
									,sales_account_manager
									,sl_no
									,pid
									,model
									,product
								)
								SELECT @Id
									,JSON_VALUE(@jsonInput, '$.tickets['+CAST(@LoopIndex AS VARCHAR)+'].custom_fields.location')
									,JSON_VALUE(@jsonInput, '$.tickets['+CAST(@LoopIndex AS VARCHAR)+'].custom_fields.major_incident_type')
									,JSON_VALUE(@jsonInput, '$.tickets['+CAST(@LoopIndex AS VARCHAR)+'].custom_fields.nsd_member_name')
									,JSON_VALUE(@jsonInput, '$.tickets['+CAST(@LoopIndex AS VARCHAR)+'].custom_fields.oem_case_id_logged')
									,JSON_VALUE(@jsonInput, '$.tickets['+CAST(@LoopIndex AS VARCHAR)+'].custom_fields.on_roaster_engineer')
									,JSON_VALUE(@jsonInput, '$.tickets['+CAST(@LoopIndex AS VARCHAR)+'].custom_fields.resolution_type')
									,JSON_VALUE(@jsonInput, '$.tickets['+CAST(@LoopIndex AS VARCHAR)+'].custom_fields.support_type')
									,JSON_VALUE(@jsonInput, '$.tickets['+CAST(@LoopIndex AS VARCHAR)+'].custom_fields.tenant')
									,JSON_VALUE(@jsonInput, '$.tickets['+CAST(@LoopIndex AS VARCHAR)+'].custom_fields.ticket_mode')
									,JSON_VALUE(@jsonInput, '$.tickets['+CAST(@LoopIndex AS VARCHAR)+'].custom_fields.ticket_monitoring_owner')
									,JSON_VALUE(@jsonInput, '$.tickets['+CAST(@LoopIndex AS VARCHAR)+'].custom_fields.time_track_mandate')
									,JSON_VALUE(@jsonInput, '$.tickets['+CAST(@LoopIndex AS VARCHAR)+'].custom_fields.user_type')
									,JSON_VALUE(@jsonInput, '$.tickets['+CAST(@LoopIndex AS VARCHAR)+'].custom_fields.parent_ticket_id')
									,JSON_VALUE(@jsonInput, '$.tickets['+CAST(@LoopIndex AS VARCHAR)+'].custom_fields.resolution_remarks')
									,JSON_VALUE(@jsonInput, '$.tickets['+CAST(@LoopIndex AS VARCHAR)+'].custom_fields.resource_name')
									,JSON_VALUE(@jsonInput, '$.tickets['+CAST(@LoopIndex AS VARCHAR)+'].custom_fields.problem_statement')
									,JSON_VALUE(@jsonInput, '$.tickets['+CAST(@LoopIndex AS VARCHAR)+'].custom_fields.oem_case_idif_any')
									,JSON_VALUE(@jsonInput, '$.tickets['+CAST(@LoopIndex AS VARCHAR)+'].custom_fields.sales_account_manager')
									,JSON_VALUE(@jsonInput, '$.tickets['+CAST(@LoopIndex AS VARCHAR)+'].custom_fields.sl_no')
									,JSON_VALUE(@jsonInput, '$.tickets['+CAST(@LoopIndex AS VARCHAR)+'].custom_fields.pid')
									,JSON_VALUE(@jsonInput, '$.tickets['+CAST(@LoopIndex AS VARCHAR)+'].custom_fields.model')
									,JSON_VALUE(@jsonInput, '$.tickets['+CAST(@LoopIndex AS VARCHAR)+'].custom_fields.product')
								
							END
							ELSE
							BEGIN
								UPDATE FreshService_T_Ticket_CustomFields
								SET 
									[location]=JSON_VALUE(@jsonInput, '$.tickets['+CAST(@LoopIndex AS VARCHAR)+'].custom_fields.location')
									,major_incident_type=JSON_VALUE(@jsonInput, '$.tickets['+CAST(@LoopIndex AS VARCHAR)+'].custom_fields.major_incident_type')
									,nsd_member_name=JSON_VALUE(@jsonInput, '$.tickets['+CAST(@LoopIndex AS VARCHAR)+'].custom_fields.nsd_member_name')
									,oem_case_id_logged=JSON_VALUE(@jsonInput, '$.tickets['+CAST(@LoopIndex AS VARCHAR)+'].custom_fields.oem_case_id_logged')
									,on_roaster_engineer=JSON_VALUE(@jsonInput, '$.tickets['+CAST(@LoopIndex AS VARCHAR)+'].custom_fields.on_roaster_engineer')
									,resolution_type=JSON_VALUE(@jsonInput, '$.tickets['+CAST(@LoopIndex AS VARCHAR)+'].custom_fields.resolution_type')
									,support_type=JSON_VALUE(@jsonInput, '$.tickets['+CAST(@LoopIndex AS VARCHAR)+'].custom_fields.support_type')
									,tenant=JSON_VALUE(@jsonInput, '$.tickets['+CAST(@LoopIndex AS VARCHAR)+'].custom_fields.tenant')
									,ticket_mode=JSON_VALUE(@jsonInput, '$.tickets['+CAST(@LoopIndex AS VARCHAR)+'].custom_fields.ticket_mode')
									,ticket_monitoring_owner=JSON_VALUE(@jsonInput, '$.tickets['+CAST(@LoopIndex AS VARCHAR)+'].custom_fields.ticket_monitoring_owner')
									,time_track_mandate=JSON_VALUE(@jsonInput, '$.tickets['+CAST(@LoopIndex AS VARCHAR)+'].custom_fields.time_track_mandate')
									,user_type=JSON_VALUE(@jsonInput, '$.tickets['+CAST(@LoopIndex AS VARCHAR)+'].custom_fields.user_type')
									,parent_ticket_id=JSON_VALUE(@jsonInput, '$.tickets['+CAST(@LoopIndex AS VARCHAR)+'].custom_fields.parent_ticket_id')
									,resolution_remarks=JSON_VALUE(@jsonInput, '$.tickets['+CAST(@LoopIndex AS VARCHAR)+'].custom_fields.resolution_remarks')
									,resource_name=JSON_VALUE(@jsonInput, '$.tickets['+CAST(@LoopIndex AS VARCHAR)+'].custom_fields.resource_name')
									,problem_statement=JSON_VALUE(@jsonInput, '$.tickets['+CAST(@LoopIndex AS VARCHAR)+'].custom_fields.problem_statement')
									,oem_case_idif_any=JSON_VALUE(@jsonInput, '$.tickets['+CAST(@LoopIndex AS VARCHAR)+'].custom_fields.oem_case_idif_any')
									,sales_account_manager=JSON_VALUE(@jsonInput, '$.tickets['+CAST(@LoopIndex AS VARCHAR)+'].custom_fields.sales_account_manager')
									,sl_no=JSON_VALUE(@jsonInput, '$.tickets['+CAST(@LoopIndex AS VARCHAR)+'].custom_fields.sl_no')
									,pid=JSON_VALUE(@jsonInput, '$.tickets['+CAST(@LoopIndex AS VARCHAR)+'].custom_fields.pid')
									,model=JSON_VALUE(@jsonInput, '$.tickets['+CAST(@LoopIndex AS VARCHAR)+'].custom_fields.model')
									,product=JSON_VALUE(@jsonInput, '$.tickets['+CAST(@LoopIndex AS VARCHAR)+'].custom_fields.product')
								WHERE ticket_id =@Id
							END


						END

						COMMIT TRANSACTION
					END TRY
					BEGIN CATCH
						ROLLBACK TRANSACTION
					END CATCH
					
					SET @LoopIndex=@LoopIndex+1
				END

				SET @OutPutId = 1
				SET @OutPutMsg='Data saved successfully.' 

			END
			ELSE
			BEGIN
				SET @OutPutId = 0
				SET @OutPutMsg='Data already exists' 
			END
		
			
		
	END TRY
	BEGIN CATCH
	-----------------
		--ROLLBACK TRANSACTION
		DECLARE @error int, @message varchar(4000), @xstate int;  
		Select @error = ERROR_NUMBER(), @message = ERROR_MESSAGE(), @xstate = XACT_STATE();  
		RAISERROR ('usp_FS_T_Tickets_IU: %d: %s', 16, 1, @error, @message) ;  
		SET @OutPutId = 0
		SET @OutPutMsg=@message  

		SELECT '' AS [Message],
			@OutPutMsg AS ErrorMessage,
			@OutPutId AS [Status],
			'0' AS Id

		RETURN
	END CATCH
	-----------------
	
	SELECT @OutPutMsg AS [Message],
	'' AS ErrorMessage,
	@OutPutId AS [Status],
	'1' AS Id


	--COMMIT TRANSACTION
	
  
		
END
GO
/****** Object:  StoredProcedure [dbo].[USP_G_ConversationByUserId]    Script Date: 09/06/2025 10:37:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE   PROC [dbo].[USP_G_ConversationByUserId]
(
	@UserId uniqueidentifier
)
AS
BEGIN

 SELECT 
	TOP 1
 	[ConversationId],
	[UserId],
	[UserName],
	[UserEmail],
	[ActivityId],
	[TenantId],
	[ServiceUrl],
	[BotInstalledOn],
	[RecipientId],
	[RecipientName],
	[UserPrincipalName],
	[AppName],
	[Active],
	[BotRemovedOn],
	[ModifiedOn]
	FROM T_BotInstallUninstal_Log WITH (NOLOCK)
	WHERE UserId = @UserId				
END;

GO
/****** Object:  StoredProcedure [dbo].[usp_M_GetCustomerMapping]    Script Date: 09/06/2025 10:37:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE     PROC [dbo].[usp_M_GetCustomerMapping]
(
	@name VARCHAR(100)=null,
	@id bigint=null,
	@active bit =NULL	
)
AS
BEGIN

	SELECT 
		D.Id 
		,D.[zaaid_site24x7]
		,D.[name_site24x7]
		,D.[departmentid_freshservice]
		,D.[name_freshservice]
		,D.active
	FROM M_CustomerMapping D WITH(NOLOCK)
	WHERE (D.departmentid_freshservice=@id or ISNULL(@id,0) =0)
	AND  (D.[name_freshservice] LIKE ISNULL(@name,'')+'%' or ISNULL(@name,'') ='')
	AND (D.active=@active or ISNULL(@active,0) =0)
	ORDER BY D.[name_freshservice] ASC
	
END
GO
/****** Object:  StoredProcedure [dbo].[usp_M_GetReportUserAccess]    Script Date: 09/06/2025 10:37:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROC [dbo].[usp_M_GetReportUserAccess]
(
	@UserEmail VARCHAR(250),
	@TeamsTab BIT = NULL,
	@MonthlyReportTab BIT = NULL,
	@ContractTab BIT = NULL
)
AS
BEGIN
	IF @UserEmail IS NULL OR TRIM(@UserEmail) = ''
		RETURN;

	IF @TeamsTab IS NULL AND @MonthlyReportTab IS NULL AND @ContractTab IS NULL
		RETURN;

	SELECT
		U.UserName,
		U.UserEmail, 
		U.TeamsTab, 
		U.MonthlyReportTab,
		U.ContractTab,
		U.Active
	FROM M_Report_UserAccess U WITH(NOLOCK)
	WHERE 
		ISNULL(U.Active,0) = 1
		AND ISNULL(U.TeamsTab,0) = 1
		AND U.UserEmail = TRIM(@UserEmail)
		AND (
			(@MonthlyReportTab = 1 AND ISNULL(U.MonthlyReportTab,0) = 1)
			OR (@ContractTab = 1 AND ISNULL(U.ContractTab,0) = 1)
		)
END
GO
/****** Object:  StoredProcedure [dbo].[usp_R_GetDepartment]    Script Date: 09/06/2025 10:37:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO




CREATE     PROC [dbo].[usp_R_GetDepartment]
(
	@name VARCHAR(100)=null,
	@id bigint=null,
	@ReportType VARCHAR(100)=NULL	,
	@active bit =NULL	
)
AS
/*
Rev 1.0		Soumik		02-01-2025		ONLY MANAGED SERVER DELIVERY COMPANY
*/
BEGIN

	SELECT 
		D.departmentid as id
		,D.[name]
		,D.[reporttype]
		,D.active
	FROM [M_ReportTypeWiseCustomer] D WITH(NOLOCK)
	--Rev 1.0
	INNER JOIN [FreshService_M_Department_CustomFields] DC WITH(NOLOCK) ON D.departmentid = DC.department_id
	WHERE DC.tenant = 'Embee-MSD Managed Service Delivery'
	--End of Rev 1.0
	AND (D.departmentid=@id or ISNULL(@id,0) =0)
	AND  (D.[name] LIKE ISNULL(@name,'')+'%' or ISNULL(@name,'') ='')
	AND (D.active=@active or ISNULL(@active,0) =0)
	ORDER BY D.[NAME] ASC
	
END

GO
/****** Object:  StoredProcedure [dbo].[usp_R_GetReportSection]    Script Date: 09/06/2025 10:37:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROC [dbo].[usp_R_GetReportSection]
(
	@active bit =NULL	
)
AS
BEGIN

	SELECT [Id]
		,[Code]
		,[Name]
		,[SortOrder]
		,[Active]
		,IsOptional
	FROM [dbo].[M_Report_Sections] R WITH(NOLOCK)
	WHERE (R.Active=@active or ISNULL(@active,0) =0)
	ORDER BY R.[SortOrder] ASC
	
END
GO
/****** Object:  StoredProcedure [dbo].[Usp_Report_UserAccess_InsUp]    Script Date: 09/06/2025 10:37:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE   PROC [dbo].[Usp_Report_UserAccess_InsUp]
(
	@UserName VARCHAR(100),
	@UserEmail VARCHAR(100),
	@Active BIT,
	@CreatedBy VARCHAR(100),
	@UserId	NVARCHAR(100)
)
AS
BEGIN

	IF EXISTS (SELECT 1 FROM M_Report_UserAccess WITH (NOLOCK) WHERE UserId = @UserId AND UserEmail = @UserEmail)
	BEGIN
			UPDATE M_Report_UserAccess
			SET
			UserName	= @UserName,
			UserEmail	= @UserEmail,
			UserId		= @UserId,
			Active		= @Active,
			ModifiedBy	= @UserEmail,
			ModifiedOn = GETDATE()
			WHERE UserEmail = @UserEmail AND UserId = @UserId
	END
	
	ELSE
	BEGIN
		INSERT INTO M_Report_UserAccess
		(
			UserName,UserEmail,Active,CreatedBy,CreatedOn,UserId
		)
		VALUES
		(
			@UserName,@UserEmail,@Active,@UserEmail,GETDATE(),@UserId
		)
	END

	
	IF @@ERROR<>0
	BEGIN
		SELECT 
			'Something went wrong, unable to insert Conversation data'	AS [Message],
			''						AS ErrorMessage,
			0						AS [Status],
			0						AS Id,
			''						AS ReferenceNo
		RETURN 
	END

	SELECT 
		'Conversation data saved successfully!'			AS	[Message],
		''								AS ErrorMessage,
		1								AS [Status],
		1						AS Id,
		@UserId					AS ReferenceNo			
END;

GO
/****** Object:  StoredProcedure [dbo].[usp_Site24x7_AccessToken_Get]    Script Date: 09/06/2025 10:37:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROC [dbo].[usp_Site24x7_AccessToken_Get]
(
@ClientId VARCHAR(100)
)
AS
BEGIN

	SELECT 
		Id
		,ClientId as client_id
		,ClientSecret as  client_secret
		,AccessToken as access_token
		,RefreshToken as refresh_token
		,Scope
		,API_Domain
		,TokenType as api_domain
		,ExpiresIn as ExpiresIn
		,ExpiresStarts
		,ExpiresOn
		,CreatedOn
		,UpdatedOn
		,CASE WHEN ExpiresOn <GETUTCDATE() THEN 1 ELSE 0 END AS ExpiryFlag
	FROM Site24x7_M_AccessToken
	WHERE ClientId=@ClientId
END
GO
/****** Object:  StoredProcedure [dbo].[usp_Site24x7_AccessToken_Update]    Script Date: 09/06/2025 10:37:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROC [dbo].[usp_Site24x7_AccessToken_Update]
(
@ClientId VARCHAR(100)=null,
@AccessToken VARCHAR(200)=null,
@RefreshToken VARCHAR(200)=null,
@ExpiresIn INT=null,
@ExpiresStarts DATETIME NULL=null,
@ExpiresOn DATETIME =null
)
AS
BEGIN


	UPDATE Site24x7_M_AccessToken
	SET 
		AccessToken=@AccessToken,
		ExpiresIn=@ExpiresIn,
		ExpiresStarts=@ExpiresStarts,
		ExpiresOn=@ExpiresOn,
		UpdatedOn=GETUTCDATE()
	FROM Site24x7_M_AccessToken
	WHERE ClientId=@ClientId

	SELECT '' AS [Message],
	'' AS ErrorMessage,
	1 AS [Status],
	'1'AS Id


END
GO
/****** Object:  StoredProcedure [dbo].[usp_Site24x7_MSP_Customer_Get]    Script Date: 09/06/2025 10:37:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE   PROC [dbo].[usp_Site24x7_MSP_Customer_Get]
(
	@zaaid varchar(50)=null,
	@name VARCHAR(100)=null
)
AS
BEGIN

	SELECT 
		zaaid
		,[user_id]
		,[name]
		,encodedZaaid 
		,CreatedOn
		,UpdatedOn
	FROM Site24x7_M_MSP_Customer
	WHERE (zaaid=@zaaid or ISNULL(@zaaid,'') ='')
	AND  ([name] LIKE ISNULL(@name,'')+'%' or ISNULL(@name,'') ='')
	--soumik rev
	AND ACTIVE = 1
	--soumik rev end

	ORDER BY [NAME] ASC
END
GO
/****** Object:  StoredProcedure [dbo].[usp_Site24x7_R_ServerPerformanceReport]    Script Date: 09/06/2025 10:37:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROC [dbo].[usp_Site24x7_R_ServerPerformanceReport] (
	@departmentid bigint=27000180437,
	@zaaid VARCHAR(50)=null,
	@start_date varchar(10)='01/02/2025',
	@end_date varchar(10)='28/02/2025'
) AS
BEGIN
	DECLARE @header as TABLE
	(
		zaaid VARCHAR(100),
		[name] VARCHAR(100),
		ServerName VARCHAR(200),
		RowIndex INT
	)

	DECLARE @header_rowid as TABLE
	(
		RowId VARCHAR(100)
	)

	IF ISNULL(@zaaid,'')=''
	BEGIN
		SELECT @zaaid=zaaid_site24x7
		FROM [dbo].[M_CustomerMapping] WITH(NOLOCK)
		WHERE departmentid_freshservice=@departmentid
	END


	DECLARE @LastDateTobeConsidered DATE = (SELECT TOP 1 LastDate FROM LastDateTobeConsidered WHERE ACTIVE =1);

	INSERT INTO @header_rowid(RowId)
	SELECT RowId FROM [Site24x7_T_Per_Report_Server_Hdr_Monthly] H WITH(NOLOCK)
	WHERE H.zaaid=@zaaid
	--Soumik Rev 1.0 
	AND convert(date,dtStartDate,103)  between convert(date,@start_date,103) and convert(date,@end_date,103)
	--AND CAST(SWITCHOFFSET(CAST(dtStartDate AS DATETIMEOFFSET), '+05:30') AS DATE) 
 --     BETWEEN CONVERT(DATE, @Start_Date, 103) AND CONVERT(DATE, @End_Date, 103)
	  AND (CAST(SWITCHOFFSET(CAST(dtStartDate AS DATETIMEOFFSET), '+05:30') AS DATE) > @LastDateTobeConsidered OR ISNULL(@LastDateTobeConsidered,'')='')
	--Soumik End of Rev 1.0
	

	--INSERT INTO @header (zaaid,[name],ServerName,RowIndex)
	--SELECT C.zaaid,C.[name],N.ServerName,n.RowIndex
	INSERT INTO @header (zaaid,[name],ServerName)
	SELECT C.zaaid,C.[name],N.ServerName

	FROM [Site24x7_M_MSP_Customer] C WITH(NOLOCK) 
	INNER JOIN [Site24x7_T_Per_Report_Server_Hdr_Monthly] H WITH(NOLOCK) ON C.zaaid= H.zaaid AND H.zaaid=@zaaid
	AND RowId IN (SELECT RowId FROM @header_rowid)
	INNER JOIN [Site24x7_T_Per_Report_Server_Names_Monthly] N WITH(NOLOCK) ON H.RowId=N.RowId 
	group by C.zaaid,C.[name],N.ServerName
	ORDER BY N.ServerName


	
	SELECT *,0 AS RowIndex FROM 
	(
	-- Disk Utilization
	SELECT H.zaaid, H.[name], H.ServerName,A.Average,L.Minimum,U.Maximum,'Disk' AS UtilizationType
	FROM 
		@header H LEFT OUTER JOIN 
		(SELECT ServerName
		,CAST(ROUND(AVG(ISNULL(DISKUSEDPERCENT,0)),2)AS float) AS Average

		FROM VW_Site24x7_ServerPerformance_Monthly M WITH(NOLOCK)

		WHERE param_metric_aggregation=0 
		AND ROWID IN (SELECT RowId FROM @header_rowid )
		GROUP BY ServerName
		) A ON  H.ServerName=A.ServerName
		LEFT OUTER JOIN 
		(
		SELECT ServerName
		,CAST(ROUND(MIN(ISNULL(DISKUSEDPERCENT,0)),2)AS float) AS Minimum
		FROM VW_Site24x7_ServerPerformance_Monthly M WITH(NOLOCK)
		WHERE param_metric_aggregation IN (4,5)  
		AND ROWID IN (SELECT RowId FROM @header_rowid )
		GROUP BY ServerName
		) L ON H.ServerName=L.ServerName
		LEFT OUTER  JOIN 
		(
		SELECT ServerName
		,CAST(ROUND(MAX(ISNULL(DISKUSEDPERCENT,0)),2)AS float) AS Maximum
		FROM VW_Site24x7_ServerPerformance_Monthly M WITH(NOLOCK)
		WHERE param_metric_aggregation IN (4,5)  
		AND ROWID IN (SELECT RowId FROM @header_rowid )
		GROUP BY ServerName
		) U ON H.ServerName=U.ServerName
	UNION
	-- Memory Utilization
	SELECT H.zaaid, H.[name], H.ServerName,A.Average,L.Minimum,U.Maximum,'Memory' AS UtilizationType
	FROM 
		@header H LEFT OUTER JOIN 
		(SELECT ServerName
		,CAST(ROUND(AVG(ISNULL(MEMUSEDPERCENT,0)),2)AS float) AS Average
		FROM VW_Site24x7_ServerPerformance_Monthly M WITH(NOLOCK)
		WHERE param_metric_aggregation=0 
		AND ROWID IN (SELECT RowId FROM @header_rowid )
		GROUP BY ServerName
		) A ON  H.ServerName=A.ServerName
		LEFT OUTER JOIN 
		(
		SELECT ServerName
		,CAST(ROUND(MIN(ISNULL(MEMUSEDPERCENT,0)),2)AS float) AS Minimum
		FROM VW_Site24x7_ServerPerformance_Monthly M WITH(NOLOCK)
		WHERE param_metric_aggregation IN (4,5) 
		AND ROWID IN (SELECT RowId FROM @header_rowid )
		GROUP BY ServerName
		) L ON H.ServerName=L.ServerName
		LEFT OUTER  JOIN 
		(
		SELECT ServerName
		,CAST(ROUND(MAX(ISNULL(MEMUSEDPERCENT,0)),2)AS float) AS Maximum
		FROM VW_Site24x7_ServerPerformance_Monthly M WITH(NOLOCK)
		WHERE param_metric_aggregation IN (4,5) 
		AND ROWID IN (SELECT RowId FROM @header_rowid )
		GROUP BY ServerName
		) U ON H.ServerName=U.ServerName
	UNION
	-- CPU Utilization
	SELECT H.zaaid, H.[name], H.ServerName,A.Average,L.Minimum,U.Maximum,'CPU' AS UtilizationType
	FROM 
		@header H LEFT OUTER JOIN 
		(SELECT ServerName
		,CAST(ROUND(AVG(ISNULL(CPUUSEDPERCENT,0)),2)AS float) AS Average
		FROM VW_Site24x7_ServerPerformance_Monthly M WITH(NOLOCK)
		WHERE param_metric_aggregation=0 
		AND ROWID IN (SELECT RowId FROM @header_rowid )
		GROUP BY ServerName
		) A ON  H.ServerName=A.ServerName
		LEFT OUTER JOIN 
		(
		SELECT ServerName
		,CAST(ROUND(MIN(ISNULL(CPUUSEDPERCENT,0)),2) AS float) AS Minimum
		FROM VW_Site24x7_ServerPerformance_Monthly M WITH(NOLOCK)
		WHERE param_metric_aggregation IN (4,5)
		AND ROWID IN (SELECT RowId FROM @header_rowid )
		GROUP BY ServerName
		) L ON H.ServerName=L.ServerName
		LEFT OUTER  JOIN 
		(
		SELECT ServerName
		,CAST(ROUND(MAX(ISNULL(CPUUSEDPERCENT,0)),2) AS float) AS Maximum
		FROM VW_Site24x7_ServerPerformance_Monthly M WITH(NOLOCK)
		WHERE param_metric_aggregation IN (4,5) 
		AND ROWID IN (SELECT RowId FROM @header_rowid )
		GROUP BY ServerName
		) U ON H.ServerName=U.ServerName
		--ORDER BY A.ServerName
	) X
	ORDER BY X.UtilizationType, X.ServerName
END

GO
/****** Object:  StoredProcedure [dbo].[usp_Site24x7_R_ServerPerformanceReport_BAK_SOUMIK_22032025]    Script Date: 09/06/2025 10:37:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE   PROC [dbo].[usp_Site24x7_R_ServerPerformanceReport_BAK_SOUMIK_22032025]
(
	@departmentid bigint=27000111932,
	@zaaid VARCHAR(50)=null,
	@start_date varchar(10)='01/02/2025',
	@end_date varchar(10)='20/02/2025'
)
AS
BEGIN
	
	DECLARE @header as TABLE
	(
		zaaid VARCHAR(100),
		[name] VARCHAR(100),
		ServerName VARCHAR(200),
		RowIndex INT
	)

	DECLARE @header_rowid as TABLE
	(
		RowId VARCHAR(100)
	)

	IF ISNULL(@zaaid,'')=''
	BEGIN
		SELECT @zaaid=zaaid_site24x7
		FROM [dbo].[M_CustomerMapping] WITH(NOLOCK)
		WHERE departmentid_freshservice=@departmentid
	END

	DECLARE @LastDateTobeConsidered DATE = (SELECT TOP 1 LastDate FROM LastDateTobeConsidered WHERE ACTIVE =1);

	INSERT INTO @header_rowid(RowId)
	SELECT RowId FROM [Site24x7_T_Per_Report_Server_Hdr] H WITH(NOLOCK)
	WHERE H.zaaid=@zaaid
	--Soumik Rev 1.0 
	--AND dtStartDate between convert(date,@start_date,103) and convert(date,@end_date,103)
	AND CAST(SWITCHOFFSET(CAST(dtStartDate AS DATETIMEOFFSET), '+05:30') AS DATE) 
      BETWEEN CONVERT(DATE, @Start_Date, 103) AND CONVERT(DATE, @End_Date, 103)
	  AND (CAST(SWITCHOFFSET(CAST(dtStartDate AS DATETIMEOFFSET), '+05:30') AS DATE) > @LastDateTobeConsidered OR ISNULL(@LastDateTobeConsidered,'')='')
	--Soumik End of Rev 1.0
	

	INSERT INTO @header (zaaid,[name],ServerName,RowIndex)
	SELECT C.zaaid,C.[name],N.ServerName,n.RowIndex
	FROM [Site24x7_M_MSP_Customer] C WITH(NOLOCK) 
	INNER JOIN [Site24x7_T_Per_Report_Server_Hdr] H WITH(NOLOCK) ON C.zaaid= H.zaaid AND H.zaaid=@zaaid
	AND RowId IN (SELECT RowId FROM @header_rowid)
	INNER JOIN [Site24x7_T_Per_Report_Server_Names] N WITH(NOLOCK) ON H.RowId=N.RowId 
	group by C.zaaid,C.[name],N.ServerName,n.RowIndex
	ORDER BY N.ServerName


	
	SELECT * FROM 
	(
	-- Disk Utilization
	SELECT H.zaaid, H.[name], H.ServerName, A.RowIndex,A.Average,L.Minimum,U.Maximum,'Disk' AS UtilizationType
	FROM 
		@header H LEFT OUTER JOIN 
		(SELECT RowIndex
		,CAST(ROUND(AVG(NULLIF(DISKUSEDPERCENT,0)),0)AS INT) AS Average
		FROM [Site24x7_T_Per_Report_Server_Metrics] M WITH(NOLOCK)
		WHERE param_metric_aggregation=0 
		AND ROWID IN (SELECT RowId FROM @header_rowid )
		GROUP BY RowIndex
		) A ON  H.RowIndex=A.RowIndex
		LEFT OUTER JOIN 
		(
		SELECT RowIndex
		,CAST(ROUND(MIN(ISNULL(DISKUSEDPERCENT,0)),0)AS INT) AS Minimum
		FROM [Site24x7_T_Per_Report_Server_Metrics] M WITH(NOLOCK)
		WHERE param_metric_aggregation IN (4,5)  
		AND ROWID IN (SELECT RowId FROM @header_rowid )
		GROUP BY RowIndex
		) L ON H.RowIndex=L.RowIndex
		LEFT OUTER  JOIN 
		(
		SELECT RowIndex
		,CAST(ROUND(MAX(ISNULL(DISKUSEDPERCENT,0)),0)AS INT) AS Maximum
		FROM [Site24x7_T_Per_Report_Server_Metrics] M WITH(NOLOCK)
		WHERE param_metric_aggregation IN (4,5)  
		AND ROWID IN (SELECT RowId FROM @header_rowid )
		GROUP BY RowIndex
		) U ON H.RowIndex=U.RowIndex
	UNION
	-- Memory Utilization
	SELECT H.zaaid, H.[name], H.ServerName,A.RowIndex,A.Average,L.Minimum,U.Maximum,'Memory' AS UtilizationType
	FROM 
		@header H LEFT OUTER JOIN 
		(SELECT RowIndex
		,CAST(ROUND(AVG(NULLIF(MEMUSEDPERCENT,0)),0)AS INT) AS Average
		FROM [Site24x7_T_Per_Report_Server_Metrics] M WITH(NOLOCK)
		WHERE param_metric_aggregation=0 
		AND ROWID IN (SELECT RowId FROM @header_rowid )
		GROUP BY RowIndex
		) A ON  H.RowIndex=A.RowIndex
		LEFT OUTER JOIN 
		(
		SELECT RowIndex
		,CAST(ROUND(MIN(ISNULL(MEMUSEDPERCENT,0)),0)AS INT) AS Minimum
		FROM [Site24x7_T_Per_Report_Server_Metrics] M WITH(NOLOCK)
		WHERE param_metric_aggregation IN (4,5) 
		AND ROWID IN (SELECT RowId FROM @header_rowid )
		GROUP BY RowIndex
		) L ON H.RowIndex=L.RowIndex
		LEFT OUTER  JOIN 
		(
		SELECT RowIndex
		,CAST(ROUND(MAX(ISNULL(MEMUSEDPERCENT,0)),0)AS INT) AS Maximum
		FROM [Site24x7_T_Per_Report_Server_Metrics] M WITH(NOLOCK)
		WHERE param_metric_aggregation IN (4,5) 
		AND ROWID IN (SELECT RowId FROM @header_rowid )
		GROUP BY RowIndex
		) U ON H.RowIndex=U.RowIndex
	UNION
	-- CPU Utilization
	SELECT H.zaaid, H.[name], H.ServerName,A.RowIndex,A.Average,L.Minimum,U.Maximum,'CPU' AS UtilizationType
	FROM 
		@header H LEFT OUTER JOIN 
		(SELECT RowIndex
		,CAST(ROUND(AVG(NULLIF(CPUUSEDPERCENT,0)),0)AS INT) AS Average
		FROM [Site24x7_T_Per_Report_Server_Metrics] M WITH(NOLOCK)
		WHERE param_metric_aggregation=0 
		AND ROWID IN (SELECT RowId FROM @header_rowid )
		GROUP BY RowIndex
		) A ON  H.RowIndex=A.RowIndex
		LEFT OUTER JOIN 
		(
		SELECT RowIndex
		,CAST(ROUND(MIN(ISNULL(CPUUSEDPERCENT,0)),0) AS INT) AS Minimum
		FROM [Site24x7_T_Per_Report_Server_Metrics] M WITH(NOLOCK)
		WHERE param_metric_aggregation IN (4,5)
		AND ROWID IN (SELECT RowId FROM @header_rowid )
		GROUP BY RowIndex
		) L ON H.RowIndex=L.RowIndex
		LEFT OUTER  JOIN 
		(
		SELECT RowIndex
		,CAST(ROUND(MAX(ISNULL(CPUUSEDPERCENT,0)),0) AS INT) AS Maximum
		FROM [Site24x7_T_Per_Report_Server_Metrics] M WITH(NOLOCK)
		WHERE param_metric_aggregation IN (4,5) 
		AND ROWID IN (SELECT RowId FROM @header_rowid )
		GROUP BY RowIndex
		) U ON H.RowIndex=U.RowIndex
		--ORDER BY A.RowIndex
	) X
	ORDER BY X.UtilizationType, X.ServerName
END
GO
/****** Object:  StoredProcedure [dbo].[usp_Site24x7_R_ServerPerformanceReport_sam]    Script Date: 09/06/2025 10:37:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--EXEC [usp_Site24x7_R_ServerPerformanceReport_sam] @departmentid =27000180437, @zaaid =null, @start_date ='01/02/2025', @end_date ='28/02/2025'



CREATE   PROC [dbo].[usp_Site24x7_R_ServerPerformanceReport_sam]
(
	@departmentid bigint=27000111932,
	@zaaid VARCHAR(50)=null,
	@start_date varchar(10)='01/02/2025',
	@end_date varchar(10)='28/02/2025'
)
AS
BEGIN
	
	DECLARE @header as TABLE
	(
		zaaid VARCHAR(100),
		[name] VARCHAR(100),
		ServerName VARCHAR(200),
		RowIndex INT
	)

	DECLARE @header_rowid as TABLE
	(
		RowId VARCHAR(100)
	)

	IF ISNULL(@zaaid,'')=''
	BEGIN
		SELECT @zaaid=zaaid_site24x7
		FROM [dbo].[M_CustomerMapping] WITH(NOLOCK)
		WHERE departmentid_freshservice=@departmentid
	END

	DECLARE @LastDateTobeConsidered DATE = (SELECT TOP 1 LastDate FROM LastDateTobeConsidered WHERE ACTIVE =1);

	INSERT INTO @header_rowid(RowId)
	SELECT RowId FROM [Site24x7_T_Per_Report_Server_Hdr] H WITH(NOLOCK)
	WHERE H.zaaid=@zaaid
	--Soumik Rev 1.0 
	--AND dtStartDate between convert(date,@start_date,103) and convert(date,@end_date,103)
	AND CAST(SWITCHOFFSET(CAST(dtStartDate AS DATETIMEOFFSET), '+05:30') AS DATE) 
      BETWEEN CONVERT(DATE, @Start_Date, 103) AND CONVERT(DATE, @End_Date, 103)
	  AND (CAST(SWITCHOFFSET(CAST(dtStartDate AS DATETIMEOFFSET), '+05:30') AS DATE) > @LastDateTobeConsidered OR ISNULL(@LastDateTobeConsidered,'')='')
	--Soumik End of Rev 1.0
	

	--INSERT INTO @header (zaaid,[name],ServerName,RowIndex)
	--SELECT C.zaaid,C.[name],N.ServerName,n.RowIndex
	INSERT INTO @header (zaaid,[name],ServerName)
	SELECT C.zaaid,C.[name],N.ServerName

	FROM [Site24x7_M_MSP_Customer] C WITH(NOLOCK) 
	INNER JOIN [Site24x7_T_Per_Report_Server_Hdr] H WITH(NOLOCK) ON C.zaaid= H.zaaid AND H.zaaid=@zaaid
	AND RowId IN (SELECT RowId FROM @header_rowid)
	INNER JOIN [Site24x7_T_Per_Report_Server_Names] N WITH(NOLOCK) ON H.RowId=N.RowId 
	group by C.zaaid,C.[name],N.ServerName
	ORDER BY N.ServerName


	
	SELECT *,0 AS RowIndex FROM 
	(
	-- Disk Utilization
	SELECT H.zaaid, H.[name], H.ServerName,A.Average,L.Minimum,U.Maximum,'Disk' AS UtilizationType
	FROM 
		@header H LEFT OUTER JOIN 
		(SELECT ServerName
		,CAST(ROUND(AVG(NULLIF(DISKUSEDPERCENT,0)),2)AS float) AS Average

		FROM VW_Site24x7_ServerPerformance M WITH(NOLOCK)

		WHERE param_metric_aggregation=0 
		AND ROWID IN (SELECT RowId FROM @header_rowid )
		GROUP BY ServerName
		) A ON  H.ServerName=A.ServerName
		LEFT OUTER JOIN 
		(
		SELECT ServerName
		,CAST(ROUND(MIN(ISNULL(DISKUSEDPERCENT,0)),2)AS float) AS Minimum
		FROM VW_Site24x7_ServerPerformance M WITH(NOLOCK)
		WHERE param_metric_aggregation IN (4,5)  
		AND ROWID IN (SELECT RowId FROM @header_rowid )
		GROUP BY ServerName
		) L ON H.ServerName=L.ServerName
		LEFT OUTER  JOIN 
		(
		SELECT ServerName
		,CAST(ROUND(MAX(ISNULL(DISKUSEDPERCENT,0)),2)AS float) AS Maximum
		FROM VW_Site24x7_ServerPerformance M WITH(NOLOCK)
		WHERE param_metric_aggregation IN (4,5)  
		AND ROWID IN (SELECT RowId FROM @header_rowid )
		GROUP BY ServerName
		) U ON H.ServerName=U.ServerName
	UNION
	-- Memory Utilization
	SELECT H.zaaid, H.[name], H.ServerName,A.Average,L.Minimum,U.Maximum,'Memory' AS UtilizationType
	FROM 
		@header H LEFT OUTER JOIN 
		(SELECT ServerName
		,CAST(ROUND(AVG(NULLIF(MEMUSEDPERCENT,0)),2)AS float) AS Average
		FROM VW_Site24x7_ServerPerformance M WITH(NOLOCK)
		WHERE param_metric_aggregation=0 
		AND ROWID IN (SELECT RowId FROM @header_rowid )
		GROUP BY ServerName
		) A ON  H.ServerName=A.ServerName
		LEFT OUTER JOIN 
		(
		SELECT ServerName
		,CAST(ROUND(MIN(ISNULL(MEMUSEDPERCENT,0)),2)AS float) AS Minimum
		FROM VW_Site24x7_ServerPerformance M WITH(NOLOCK)
		WHERE param_metric_aggregation IN (4,5) 
		AND ROWID IN (SELECT RowId FROM @header_rowid )
		GROUP BY ServerName
		) L ON H.ServerName=L.ServerName
		LEFT OUTER  JOIN 
		(
		SELECT ServerName
		,CAST(ROUND(MAX(ISNULL(MEMUSEDPERCENT,0)),2)AS float) AS Maximum
		FROM VW_Site24x7_ServerPerformance M WITH(NOLOCK)
		WHERE param_metric_aggregation IN (4,5) 
		AND ROWID IN (SELECT RowId FROM @header_rowid )
		GROUP BY ServerName
		) U ON H.ServerName=U.ServerName
	UNION
	-- CPU Utilization
	SELECT H.zaaid, H.[name], H.ServerName,A.Average,L.Minimum,U.Maximum,'CPU' AS UtilizationType
	FROM 
		@header H LEFT OUTER JOIN 
		(SELECT ServerName
		,CAST(ROUND(AVG(NULLIF(CPUUSEDPERCENT,0)),2)AS float) AS Average
		FROM VW_Site24x7_ServerPerformance M WITH(NOLOCK)
		WHERE param_metric_aggregation=0 
		AND ROWID IN (SELECT RowId FROM @header_rowid )
		GROUP BY ServerName
		) A ON  H.ServerName=A.ServerName
		LEFT OUTER JOIN 
		(
		SELECT ServerName
		,CAST(ROUND(MIN(ISNULL(CPUUSEDPERCENT,0)),2) AS float) AS Minimum
		FROM VW_Site24x7_ServerPerformance M WITH(NOLOCK)
		WHERE param_metric_aggregation IN (4,5)
		AND ROWID IN (SELECT RowId FROM @header_rowid )
		GROUP BY ServerName
		) L ON H.ServerName=L.ServerName
		LEFT OUTER  JOIN 
		(
		SELECT ServerName
		,CAST(ROUND(MAX(ISNULL(CPUUSEDPERCENT,0)),2) AS float) AS Maximum
		FROM VW_Site24x7_ServerPerformance M WITH(NOLOCK)
		WHERE param_metric_aggregation IN (4,5) 
		AND ROWID IN (SELECT RowId FROM @header_rowid )
		GROUP BY ServerName
		) U ON H.ServerName=U.ServerName
		--ORDER BY A.ServerName
	) X
	ORDER BY X.UtilizationType, X.ServerName
END
GO
/****** Object:  StoredProcedure [dbo].[usp_Site24x7_T_Per_Report_Server_InsertUpdate]    Script Date: 09/06/2025 10:37:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE   PROCEDURE [dbo].[usp_Site24x7_T_Per_Report_Server_InsertUpdate]
(
	@jsonInput			VARCHAR(MAX),
	@zaaid				VARCHAR(100),
	@param_period INT,
	@param_metric_aggregation int,
	@param_start_date varchar(50),
	@param_end_date varchar(50)
)  
AS
BEGIN
    
    SET NOCOUNT ON;  
    
	declare @OutPutId			BIGINT=NULL
	declare @OutPutMsg			VARCHAR(255)=NULL

	SET @OutPutId = 0
	SET @OutPutMsg='Sorry something went wrong.'

	
	IF ISNULL(@jsonInput,'')=''
	BEGIN
		SET @OutPutId = 0
		SET @OutPutMsg='Invalid details, please check your entry.' 
		RETURN
	END

	BEGIN TRY 
		
		BEGIN TRANSACTION 
			
			DECLARE @RowId BIGINT

			set @jsonInput=replace(@jsonInput,'"0":','"metrics":');


			IF  (@jsonInput IS NOT NULL AND iSNULL(@jsonInput,'') <> '') 
			BEGIN

				SELECT @RowId=RowId FROM Site24x7_T_Per_Report_Server_Hdr H WITH(NOLOCK)
				WHERE zaaid=@zaaid 
				AND param_period=@param_period
				--AND param_metric_aggregation=@param_metric_aggregation
				AND param_start_date=@param_start_date

				DECLARE @period INT							=	JSON_VALUE(@jsonInput, '$.data.info.period')
						,@resource_type_name VARCHAR(50)	=	JSON_VALUE(@jsonInput, '$.data.info.resource_type_name')
						,@resource_type INT					=	JSON_VALUE(@jsonInput, '$.data.info.resource_type')
						,@end_time VARCHAR(50)				=	JSON_VALUE(@jsonInput, '$.data.info.end_time')
						,@period_name VARCHAR(50)			=	JSON_VALUE(@jsonInput, '$.data.info.period_name')
						,@report_type INT					=	JSON_VALUE(@jsonInput, '$.data.info.report_type')
						,@start_time VARCHAR(50)			=	JSON_VALUE(@jsonInput, '$.data.info.start_time')
						,@metric_aggregation INT			=	JSON_VALUE(@jsonInput, '$.data.info.metric_aggregation')
						,@resource_name VARCHAR(50)			=	JSON_VALUE(@jsonInput, '$.data.info.resource_name')
						,@report_name VARCHAR(50)			=	JSON_VALUE(@jsonInput, '$.data.info.report_name')
						,@monitor_type VARCHAR(50)			=	JSON_VALUE(@jsonInput, '$.data.info.monitor_type')


				IF ISNULL(@RowId,0)=0
				BEGIN
					
					INSERT INTO Site24x7_T_Per_Report_Server_Hdr
					(
						zaaid
						,param_period
						--,param_metric_aggregation
						,param_start_date
						,param_end_date
						,[period]
						,resource_type_name
						,resource_type
						,end_time
						,period_name
						,report_type
						,start_time
						,metric_aggregation
						,resource_name
						,report_name
						,monitor_type
						,CreatedOn
						,UpdatedOn
						,dtStartDate
						,dtEndDate
					)
					VALUES
					(
						@zaaid
						,@param_period
						--,@param_metric_aggregation
						,@param_start_date
						,@param_end_date
						,@period
						,@resource_type_name
						,@resource_type
						,@end_time
						,@period_name
						,@report_type
						,@start_time
						,@metric_aggregation
						,@resource_name
						,@report_name
						,@monitor_type
						,GETUTCDATE()
						,GETUTCDATE()
						,CONVERT(DATETIME, CONVERT(DATETIMEOFFSET, replace(@param_start_date,'%2B0530','+05:30'), 126))
						,CONVERT(DATETIME, CONVERT(DATETIMEOFFSET, replace(@param_end_date,'%2B0530','+05:30'), 126))
					)
					
					SET @RowId=@@IDENTITY
				END

				IF NOT EXISTS(SELECT * FROM Site24x7_T_Per_Report_Server_Names WITH(NOLOCK) WHERE RowId=@RowId)
				BEGIN				
					INSERT INTO Site24x7_T_Per_Report_Server_Names
					(
						RowId
						,RowIndex
						,ServerName
					)
					SELECT @RowId,[key],[value] 
					FROM OPENJSON(@jsonInput, '$.data.group_data.SERVER.name')
					order by [key] asc
				END
				IF NOT EXISTS(SELECT * FROM Site24x7_T_Per_Report_Server_Availability WITH(NOLOCK) WHERE RowId=@RowId)
				BEGIN	
					INSERT INTO Site24x7_T_Per_Report_Server_Availability
				(
					RowId
					,RowIndex
					,[Availability]
				)
				SELECT @RowId,[key],[value] 
				FROM OPENJSON(@jsonInput, '$.data.group_data.SERVER.availability')
				order by [key] asc
				END

				
				DELETE FROM Site24x7_T_Per_Report_Server_Metrics WHERE RowId=@RowId and param_metric_aggregation=@param_metric_aggregation
				
				INSERT INTO Site24x7_T_Per_Report_Server_Metrics
				(
					RowId
					,RowIndex
					,param_metric_aggregation
					--,DISKUSEDPERCENT
					--,MEMUSEDPERCENT
					--,CPUUSEDPERCENT
				)
				SELECT @RowId,[key] ,@param_metric_aggregation
				FROM OPENJSON(@jsonInput, '$.data.group_data.SERVER.attribute_data')
				order by [key] asc

				DECLARE @MaxRowIndex INT, @LoopIndex INT=0
				DECLARE @diskUsedPercent NUMERIC(10,2),@memUsedPercent NUMERIC(10,2),@cpuUsedPercent NUMERIC(10,2)

				SELECT @MaxRowIndex=MAX(RowIndex) FROM Site24x7_T_Per_Report_Server_Metrics WHERE RowId=@RowId

				WHILE @LoopIndex <= ISNULL(@MaxRowIndex,-1)
				BEGIN					
					
					SET @diskUsedPercent = TRY_PARSE(JSON_VALUE(@jsonInput, '$.data.group_data.SERVER.attribute_data['+CAST(@LoopIndex AS VARCHAR)+'].metrics.DISKUSEDPERCENT') AS NUMERIC(10,2) USING 'en-US');
					SET @memUsedPercent =  TRY_PARSE(JSON_VALUE(@jsonInput, '$.data.group_data.SERVER.attribute_data['+CAST(@LoopIndex AS VARCHAR)+'].metrics.MEMUSEDPERCENT') AS NUMERIC(10,2) USING 'en-US');
					SET @cpuUsedPercent =  TRY_PARSE(JSON_VALUE(@jsonInput, '$.data.group_data.SERVER.attribute_data['+CAST(@LoopIndex AS VARCHAR)+'].metrics.CPUUSEDPERCENT') AS NUMERIC(10,2) USING 'en-US');
					
					UPDATE Site24x7_T_Per_Report_Server_Metrics 
					SET DISKUSEDPERCENT=@diskUsedPercent,
						MEMUSEDPERCENT=@memUsedPercent,
						CPUUSEDPERCENT=@cpuUsedPercent
					WHERE RowId=@RowId
					AND RowIndex=@LoopIndex
					AND param_metric_aggregation=@param_metric_aggregation

					SET @LoopIndex=@LoopIndex+1
				END

				SET @OutPutId = 1
				SET @OutPutMsg='Data saved successfully.' 

			END
			ELSE
			BEGIN
				SET @OutPutId = 0
				SET @OutPutMsg='Data already exists' 
			END
		
			
		
	END TRY
	BEGIN CATCH
	-----------------
		ROLLBACK TRANSACTION
		DECLARE @error int, @message varchar(4000), @xstate int;  
		Select @error = ERROR_NUMBER(), @message = ERROR_MESSAGE(), @xstate = XACT_STATE();  
		RAISERROR ('Site24x7_T_Per_Report_Server_IU: %d: %s', 16, 1, @error, @message) ;  
		SET @OutPutId = 0
		SET @OutPutMsg=@message  

		SELECT '' AS [Message],
			@OutPutMsg AS ErrorMessage,
			@OutPutId AS [Status],
			'0' AS Id

		RETURN
	END CATCH
	-----------------
	
	SELECT @OutPutMsg AS [Message],
	'' AS ErrorMessage,
	@OutPutId AS [Status],
	'1' AS Id


	COMMIT TRANSACTION
	
  
		
END
GO
/****** Object:  StoredProcedure [dbo].[usp_Site24x7_T_Per_Report_Server_Monthly_InsertUpdate]    Script Date: 09/06/2025 10:37:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[usp_Site24x7_T_Per_Report_Server_Monthly_InsertUpdate]
(
	@jsonInput			VARCHAR(MAX),
	@zaaid				VARCHAR(100),
	@param_period INT,
	@param_metric_aggregation int,
	@param_start_date varchar(50),
	@param_end_date varchar(50)
)  
AS
BEGIN
    
    SET NOCOUNT ON;  
    
	declare @OutPutId			BIGINT=NULL
	declare @OutPutMsg			VARCHAR(255)=NULL

	SET @OutPutId = 0
	SET @OutPutMsg='Sorry something went wrong.'

	
	IF ISNULL(@jsonInput,'')=''
	BEGIN
		SET @OutPutId = 0
		SET @OutPutMsg='Invalid details, please check your entry.' 
		RETURN
	END

	BEGIN TRY 
		
		BEGIN TRANSACTION 
			
			DECLARE @RowId BIGINT

			set @jsonInput=replace(@jsonInput,'"0":','"metrics":');


			IF  (@jsonInput IS NOT NULL AND iSNULL(@jsonInput,'') <> '') 
			BEGIN

				SELECT @RowId=RowId FROM Site24x7_T_Per_Report_Server_Hdr_Monthly H WITH(NOLOCK)
				WHERE zaaid=@zaaid 
				AND param_period=@param_period
				--AND param_metric_aggregation=@param_metric_aggregation
				AND param_start_date=@param_start_date

				DECLARE @period INT							=	JSON_VALUE(@jsonInput, '$.data.info.period')
						,@resource_type_name VARCHAR(50)	=	JSON_VALUE(@jsonInput, '$.data.info.resource_type_name')
						,@resource_type INT					=	JSON_VALUE(@jsonInput, '$.data.info.resource_type')
						,@end_time VARCHAR(50)				=	JSON_VALUE(@jsonInput, '$.data.info.end_time')
						,@period_name VARCHAR(50)			=	JSON_VALUE(@jsonInput, '$.data.info.period_name')
						,@report_type INT					=	JSON_VALUE(@jsonInput, '$.data.info.report_type')
						,@start_time VARCHAR(50)			=	JSON_VALUE(@jsonInput, '$.data.info.start_time')
						,@metric_aggregation INT			=	JSON_VALUE(@jsonInput, '$.data.info.metric_aggregation')
						,@resource_name VARCHAR(50)			=	JSON_VALUE(@jsonInput, '$.data.info.resource_name')
						,@report_name VARCHAR(50)			=	JSON_VALUE(@jsonInput, '$.data.info.report_name')
						,@monitor_type VARCHAR(50)			=	JSON_VALUE(@jsonInput, '$.data.info.monitor_type')


				IF ISNULL(@RowId,0)=0
				BEGIN
					
					INSERT INTO Site24x7_T_Per_Report_Server_Hdr_Monthly
					(
						zaaid
						,param_period
						--,param_metric_aggregation
						,param_start_date
						,param_end_date
						,[period]
						,resource_type_name
						,resource_type
						,end_time
						,period_name
						,report_type
						,start_time
						,metric_aggregation
						,resource_name
						,report_name
						,monitor_type
						,CreatedOn
						,UpdatedOn
						,dtStartDate
						,dtEndDate
					)
					VALUES
					(
						@zaaid
						,@param_period
						--,@param_metric_aggregation
						,@param_start_date
						,@param_end_date
						,@period
						,@resource_type_name
						,@resource_type
						,@end_time
						,@period_name
						,@report_type
						,@start_time
						,@metric_aggregation
						,@resource_name
						,@report_name
						,@monitor_type
						,GETUTCDATE()
						,GETUTCDATE()
						,CONVERT(DATETIME, CONVERT(DATETIMEOFFSET, replace(@param_start_date,'%2B0530','+05:30'), 126))
						,CONVERT(DATETIME, CONVERT(DATETIMEOFFSET, replace(@param_end_date,'%2B0530','+05:30'), 126))
					)
					
					SET @RowId=@@IDENTITY
				END


				IF NOT EXISTS(SELECT * FROM Site24x7_T_Per_Report_Server_Names_Monthly WITH(NOLOCK) WHERE RowId=@RowId)
				BEGIN				
					INSERT INTO Site24x7_T_Per_Report_Server_Names_Monthly
					(
						RowId
						,RowIndex
						,ServerName
					)
					SELECT @RowId,[key],[value] 
					FROM OPENJSON(@jsonInput, '$.data.group_data.SERVER.name')
					order by [key] asc
				END
				IF NOT EXISTS(SELECT * FROM Site24x7_T_Per_Report_Server_Availability_Monthly WITH(NOLOCK) WHERE RowId=@RowId)
				BEGIN	
					INSERT INTO Site24x7_T_Per_Report_Server_Availability_Monthly
				(
					RowId
					,RowIndex
					,[Availability]
				)
				SELECT @RowId,[key],[value] 
				FROM OPENJSON(@jsonInput, '$.data.group_data.SERVER.availability')
				order by [key] asc
				END

				
				DELETE FROM Site24x7_T_Per_Report_Server_Metrics_Monthly WHERE RowId=@RowId and param_metric_aggregation=@param_metric_aggregation
				
				INSERT INTO Site24x7_T_Per_Report_Server_Metrics_Monthly
				(
					RowId
					,RowIndex
					,param_metric_aggregation
					--,DISKUSEDPERCENT
					--,MEMUSEDPERCENT
					--,CPUUSEDPERCENT
				)
				SELECT @RowId,[key] ,@param_metric_aggregation
				FROM OPENJSON(@jsonInput, '$.data.group_data.SERVER.attribute_data')
				order by [key] asc

				DECLARE @MaxRowIndex INT, @LoopIndex INT=0
				DECLARE @diskUsedPercent NUMERIC(10,2),@memUsedPercent NUMERIC(10,2),@cpuUsedPercent NUMERIC(10,2)

				SELECT @MaxRowIndex=MAX(RowIndex) FROM Site24x7_T_Per_Report_Server_Metrics_Monthly WHERE RowId=@RowId

				WHILE @LoopIndex <= ISNULL(@MaxRowIndex,-1)
				BEGIN					
					
					SET @diskUsedPercent = TRY_PARSE(JSON_VALUE(@jsonInput, '$.data.group_data.SERVER.attribute_data['+CAST(@LoopIndex AS VARCHAR)+'].metrics.DISKUSEDPERCENT') AS NUMERIC(10,2) USING 'en-US');
					SET @memUsedPercent =  TRY_PARSE(JSON_VALUE(@jsonInput, '$.data.group_data.SERVER.attribute_data['+CAST(@LoopIndex AS VARCHAR)+'].metrics.MEMUSEDPERCENT') AS NUMERIC(10,2) USING 'en-US');
					SET @cpuUsedPercent =  TRY_PARSE(JSON_VALUE(@jsonInput, '$.data.group_data.SERVER.attribute_data['+CAST(@LoopIndex AS VARCHAR)+'].metrics.CPUUSEDPERCENT') AS NUMERIC(10,2) USING 'en-US');
					
					UPDATE Site24x7_T_Per_Report_Server_Metrics_Monthly 
					SET DISKUSEDPERCENT=@diskUsedPercent,
						MEMUSEDPERCENT=@memUsedPercent,
						CPUUSEDPERCENT=@cpuUsedPercent
					WHERE RowId=@RowId
					AND RowIndex=@LoopIndex
					AND param_metric_aggregation=@param_metric_aggregation

					SET @LoopIndex=@LoopIndex+1
				END

				SET @OutPutId = 1
				SET @OutPutMsg='Data saved successfully.' 

			END
			ELSE
			BEGIN
				SET @OutPutId = 0
				SET @OutPutMsg='Data already exists' 
			END
		
			
		
	END TRY
	BEGIN CATCH
	-----------------
		ROLLBACK TRANSACTION
		DECLARE @error int, @message varchar(4000), @xstate int;  
		Select @error = ERROR_NUMBER(), @message = ERROR_MESSAGE(), @xstate = XACT_STATE();  
		RAISERROR ('Site24x7_T_Per_Report_Server_Monthly_IU: %d: %s', 16, 1, @error, @message) ;  
		SET @OutPutId = 0
		SET @OutPutMsg=@message  

		SELECT '' AS [Message],
			@OutPutMsg AS ErrorMessage,
			@OutPutId AS [Status],
			'0' AS Id

		RETURN
	END CATCH
	-----------------
	
	SELECT @OutPutMsg AS [Message],
	'' AS ErrorMessage,
	@OutPutId AS [Status],
	'1' AS Id


	COMMIT TRANSACTION
	
  
		
END
GO
/****** Object:  StoredProcedure [dbo].[usp_TeamsBot_T_UserSearch_InsertUpdate]    Script Date: 09/06/2025 10:37:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_TeamsBot_T_UserSearch_InsertUpdate]
(
    @Name NVARCHAR(100) = NULL,
    @Email NVARCHAR(50) = NULL,
    @UPN NVARCHAR(50) = NULL,
    @ADID NVARCHAR(50) = NULL,
    @ChannelId NVARCHAR(50) = NULL,
    @ConversationType NVARCHAR(50) = NULL,
    @ConversationId NVARCHAR(500) = NULL,
    @TenantId NVARCHAR(50) = NULL,
    @ChatId NVARCHAR(50) = NULL,
    @LocalTimestamp DATETIMEOFFSET = NULL,
    @Locale NVARCHAR(50) = NULL,
    @ServiceUrl NVARCHAR(50) = NULL,
    @Text NVARCHAR(MAX) = NULL,
    @TextFormat NVARCHAR(50) = NULL,
    @Timestamp DATETIMEOFFSET = NULL,
    @Response NVARCHAR(MAX) = NULL,
    @Intent NVARCHAR(MAX) = NULL,
	@QuerySucceed BIT = NULL,
    @FileJSONInput VARCHAR(MAX)
)
AS
BEGIN
    DECLARE @MessageId INT;

    BEGIN TRANSACTION;

    BEGIN TRY

        INSERT INTO [dbo].[TeamsBot_T_UserSearch] (
            [UserName],
            [UserEmail],
            [UserUPN],
            [UserADID],
            [ChannelId],
            [ConversationType],
            [ConversationId],
            [TenantId],
            [ChatId],
            [LocalTimestamp],
            [Locale],
            [ServiceUrl],
            [Text],
            [TextFormat],
            [Timestamp],
            [Response],
            [Intent],
            CreatedOnIST,
            CreatedOnUTC,
			QuerySucceed
        )
        VALUES (
            @Name,
            @Email,
            @UPN,
            @ADID,
            @ChannelId,
            @ConversationType,
            @ConversationId,
            @TenantId,
            @ChatId,
            @LocalTimestamp,
            @Locale,
            @ServiceUrl,
            @Text,
            @TextFormat,
            @Timestamp,
            @Response,
            @Intent,
            CAST(SWITCHOFFSET(GETUTCDATE(), '+05:30') AS DATETIME),
            GETUTCDATE(),
			@QuerySucceed
        );

        SET @MessageId = SCOPE_IDENTITY();

		IF(@FileJSONInput IS NOT NULL AND @FileJSONInput != 'null')
		BEGIN
			INSERT INTO [TeamsBot_T_UserSearchFiles] (
				[MessageId],
				[FileName],
				[FileURL],
				[FileContent]
			)
			SELECT 
				@MessageId,
				JSON_VALUE([File].value, '$.FileName') AS [FileName],
				JSON_VALUE([File].value, '$.FileURL') AS FileURL,
				JSON_VALUE([File].value, '$.FileContent') AS FileContent
			FROM OPENJSON(@FileJSONInput) AS [File];
		END


    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;

        SELECT 
			'Something went wrong, Unable to save data in db'	AS [Message],
			''						AS ErrorMessage,
			0						AS [Status],
			0						AS Id,
			''						AS ReferenceNo
		RETURN
    END CATCH

    IF @@TRANCOUNT > 0
        COMMIT TRANSACTION;

    SELECT 
        'DB execution successful - Saved data in db'               AS [Message],
        ''																AS ErrorMessage,
		1																AS [Status],
		@MessageId														AS Id,
		@MessageId														AS ReferenceNo		
END;
GO
USE [master]
GO
ALTER DATABASE [AI-Portal-Apps] SET  READ_WRITE 
GO
