USE [TADWH]
GO

/****** Object:  Table [dbo].[Manufacturer]    Script Date: 20.01.2026 11:02:29 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Manufacturer](
	[ManufacturerId] [bigint] IDENTITY(1,1) NOT NULL,
	[CompanyId] [int] NOT NULL,
	[Code] [nvarchar](20) NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
	[systemId] [uniqueidentifier] NOT NULL,
	[systemCreatedAt] [datetime] NOT NULL,
	[systemCreatedBy] [uniqueidentifier] NOT NULL,
	[systemModifiedAt] [datetime] NOT NULL,
	[systemModifiedBy] [uniqueidentifier] NOT NULL,
	[TAManufacturerType] [int] NULL,
	[TAModelNos] [nvarchar](40) NULL,
	[TACockpitRelevant] [tinyint] NULL,
	[ModifiedAt] [datetime] NULL,
	[ModifiedBy] [sysname] NOT NULL,
	[Deleted] [tinyint] NULL,
 CONSTRAINT [pk_dbo_Manufacturer_ManufacturerId] PRIMARY KEY NONCLUSTERED 
(
	[ManufacturerId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[Manufacturer] ADD  CONSTRAINT [df_dbo_Manufacturer_ModifiedAt]  DEFAULT (getdate()) FOR [ModifiedAt]
GO

ALTER TABLE [dbo].[Manufacturer] ADD  CONSTRAINT [df_dbo_Manufacturer_ModifiedBy]  DEFAULT ('SYSTEM') FOR [ModifiedBy]
GO

ALTER TABLE [dbo].[Manufacturer] ADD  CONSTRAINT [df_dbo_Manufacturer_Deleted]  DEFAULT ((0)) FOR [Deleted]
GO


