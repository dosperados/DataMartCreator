USE [TADWH]
GO

/****** Object:  Table [dbo].[ItemCategory]    Script Date: 20.01.2026 11:00:11 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[ItemCategory](
	[ItemCategoryId] [bigint] IDENTITY(1,1) NOT NULL,
	[CompanyId] [int] NOT NULL,
	[Code] [nvarchar](40) NOT NULL,
	[ParentCategory] [nvarchar](40) NOT NULL,
	[Description] [nvarchar](200) NOT NULL,
	[Indentation] [int] NOT NULL,
	[PresentationOrder] [int] NOT NULL,
	[HasChildren] [tinyint] NOT NULL,
	[LastModifiedDateTime] [datetime] NOT NULL,
	[OrigId] [uniqueidentifier] NOT NULL,
	[systemId] [uniqueidentifier] NOT NULL,
	[systemCreatedAt] [datetime] NOT NULL,
	[systemCreatedBy] [uniqueidentifier] NOT NULL,
	[systemModifiedAt] [datetime] NOT NULL,
	[systemModifiedBy] [uniqueidentifier] NOT NULL,
	[TAMISProductGroup] [nvarchar](40) NULL,
	[TACreationWorkflowTeam] [nvarchar](20) NULL,
	[TAReleaseWorkflowTeam] [nvarchar](20) NULL,
	[TADefaultItemActivityCode] [nvarchar](40) NULL,
	[TAItemCatDimValue] [nvarchar](40) NULL,
	[TAFSMItemCategory] [int] NULL,
	[TANoServiceitemComponent] [tinyint] NULL,
	[ModifiedAt] [datetime] NULL,
	[ModifiedBy] [sysname] NOT NULL,
	[Deleted] [tinyint] NULL,
 CONSTRAINT [pk_dbo_ItemCategory_ItemCategoryId] PRIMARY KEY NONCLUSTERED 
(
	[ItemCategoryId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[ItemCategory] ADD  CONSTRAINT [df_dbo_ItemCategory_ModifiedAt]  DEFAULT (getdate()) FOR [ModifiedAt]
GO

ALTER TABLE [dbo].[ItemCategory] ADD  CONSTRAINT [df_dbo_ItemCategory_ModifiedBy]  DEFAULT ('SYSTEM') FOR [ModifiedBy]
GO

ALTER TABLE [dbo].[ItemCategory] ADD  CONSTRAINT [df_dbo_ItemCategory_Deleted]  DEFAULT ((0)) FOR [Deleted]
GO


