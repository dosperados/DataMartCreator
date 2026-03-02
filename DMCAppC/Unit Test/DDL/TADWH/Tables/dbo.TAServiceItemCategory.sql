USE [TADWH]
GO

/****** Object:  Table [dbo].[TAServiceItemCategory]    Script Date: 13.01.2026 10:56:32 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[TAServiceItemCategory](
	[TAServiceItemCategoryId] [bigint] IDENTITY(1,1) NOT NULL,
	[CompanyId] [int] NOT NULL,
	[TACode] [nvarchar](20) NOT NULL,
	[TADescription] [nvarchar](200) NOT NULL,
	[ModifiedAt] [datetime] NULL,
	[ModifiedBy] [sysname] NOT NULL,
	[Deleted] [tinyint] NULL,
	[systemId] [uniqueidentifier] NULL,
	[systemCreatedAt] [datetime] NULL,
	[systemCreatedBy] [uniqueidentifier] NULL,
	[systemModifiedAt] [datetime] NULL,
	[systemModifiedBy] [uniqueidentifier] NULL,
 CONSTRAINT [pk_dbo_TAServiceItemCategory_TAServiceItemCategoryId] PRIMARY KEY NONCLUSTERED 
(
	[TAServiceItemCategoryId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[TAServiceItemCategory] ADD  CONSTRAINT [df_dbo_TAServiceItemCategory_ModifiedAt]  DEFAULT (getdate()) FOR [ModifiedAt]
GO

ALTER TABLE [dbo].[TAServiceItemCategory] ADD  CONSTRAINT [df_dbo_TAServiceItemCategory_ModifiedBy]  DEFAULT ('SYSTEM') FOR [ModifiedBy]
GO

ALTER TABLE [dbo].[TAServiceItemCategory] ADD  CONSTRAINT [df_dbo_TAServiceItemCategory_Deleted]  DEFAULT ((0)) FOR [Deleted]
GO


