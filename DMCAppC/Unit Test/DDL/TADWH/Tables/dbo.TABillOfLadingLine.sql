USE [TADWH]
GO

/****** Object:  Table [dbo].[TABillOfLadingLine]    Script Date: 13.01.2026 10:57:53 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[TABillOfLadingLine](
	[TABillOfLadingLineId] [bigint] IDENTITY(1,1) NOT NULL,
	[CompanyId] [int] NOT NULL,
	[TABillOfLadingNo] [nvarchar](40) NOT NULL,
	[TALineNo] [int] NOT NULL,
	[OrigTAContainerID] [nvarchar](40) NOT NULL,
	[TADestinationLocationCode] [nvarchar](20) NOT NULL,
	[TAContainerTypeCode] [nvarchar](40) NOT NULL,
	[TAStatus] [int] NOT NULL,
	[ModifiedAt] [datetime] NULL,
	[ModifiedBy] [sysname] NOT NULL,
	[Deleted] [tinyint] NULL,
	[systemId] [uniqueidentifier] NULL,
	[systemCreatedAt] [datetime] NULL,
	[systemCreatedBy] [uniqueidentifier] NULL,
	[systemModifiedAt] [datetime] NULL,
	[systemModifiedBy] [uniqueidentifier] NULL,
 CONSTRAINT [pk_dbo_TABillOfLadingLine_TABillOfLadingLineId] PRIMARY KEY NONCLUSTERED 
(
	[TABillOfLadingLineId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[TABillOfLadingLine] ADD  CONSTRAINT [df_dbo_TABillOfLadingLine_ModifiedAt]  DEFAULT (getdate()) FOR [ModifiedAt]
GO

ALTER TABLE [dbo].[TABillOfLadingLine] ADD  CONSTRAINT [df_dbo_TABillOfLadingLine_ModifiedBy]  DEFAULT ('SYSTEM') FOR [ModifiedBy]
GO

ALTER TABLE [dbo].[TABillOfLadingLine] ADD  CONSTRAINT [df_dbo_TABillOfLadingLine_Deleted]  DEFAULT ((0)) FOR [Deleted]
GO


