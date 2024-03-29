USE [MESWorkLoad]
GO
/****** Object:  Table [dbo].[ListWorkLoad]    Script Date: 11/12/2019 1:32:47 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ListWorkLoad](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[EmployeeName] [nvarchar](50) NULL,
	[EmployeeId] [nvarchar](4) NULL,
	[WorkingHour] [float] NULL,
	[Description] [nvarchar](max) NULL,
	[DateWork] [datetime] NULL,
	[TotalWorking] [float] NULL,
	[Percen] [float] NULL,
	[DateAdd] [datetime] NULL,
	[UserAdd] [nvarchar](50) NULL,
	[Active] [bit] NULL,
	[DepartmentId] [nvarchar](10) NULL,
	[DepartmentName] [nvarchar](255) NULL,
 CONSTRAINT [PK_ListWorkLoad] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[sysLogfileAction]    Script Date: 11/12/2019 1:32:47 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[sysLogfileAction](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[ActionName] [varchar](50) NULL,
	[ControllerName] [varchar](50) NULL,
	[Method] [varchar](25) NULL,
	[Content] [nvarchar](max) NULL,
	[Detail] [nvarchar](max) NULL,
	[UserName] [varchar](50) NULL,
	[Date] [datetime] NULL,
 CONSTRAINT [PK__sysLogfi__3214EC27AD577D97] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
ALTER TABLE [dbo].[ListWorkLoad] ADD  CONSTRAINT [DF_ListWorkLoad_DateAdd]  DEFAULT (getdate()) FOR [DateAdd]
GO
ALTER TABLE [dbo].[sysLogfileAction] ADD  CONSTRAINT [DF__sysLogfile__Date__36B12243]  DEFAULT (getdate()) FOR [Date]
GO
