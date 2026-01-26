USE [TADWH]
GO

/****** Object:  Table [dbo].[TABillOfLadingHeader]    Script Date: 13.01.2026 10:57:30 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[TABillOfLadingHeader](
	[TABillOfLadingHeaderId] [bigint] IDENTITY(1,1) NOT NULL,
	[CompanyId] [int] NOT NULL,
	[TANo] [nvarchar](40) NOT NULL,
	[TAType] [int] NOT NULL,
	[TABLNo] [nvarchar](60) NOT NULL,
	[TADepartureLocationCode] [nvarchar](20) NOT NULL,
	[TADeparturePortCode] [nvarchar](40) NOT NULL,
	[TADepartureCountryRegionCode] [nvarchar](20) NOT NULL,
	[TAShip] [nvarchar](100) NOT NULL,
	[TAArrivalTerminal] [nvarchar](100) NOT NULL,
	[TAShippingAgentCode] [nvarchar](20) NOT NULL,
	[TAExpectedReceiptDate] [datetime] NOT NULL,
	[TAReceiptDate] [datetime] NOT NULL,
	[TAETD] [datetime] NOT NULL,
	[TAETA] [datetime] NOT NULL,
	[TACurrentETA] [datetime] NOT NULL,
	[TACertificateOfOriginReceived] [tinyint] NOT NULL,
	[TACustomsDocumentsReceived] [tinyint] NOT NULL,
	[TAArrivalPortCode] [nvarchar](20) NOT NULL,
	[TAArrivingShip] [nvarchar](100) NOT NULL,
	[TADocumentsSentAt] [datetime] NOT NULL,
	[TACorrection] [tinyint] NOT NULL,
	[systemId] [uniqueidentifier] NOT NULL,
	[systemCreatedAt] [datetime] NOT NULL,
	[systemCreatedBy] [uniqueidentifier] NOT NULL,
	[systemModifiedAt] [datetime] NOT NULL,
	[systemModifiedBy] [uniqueidentifier] NOT NULL,
	[ModifiedAt] [datetime] NULL,
	[ModifiedBy] [sysname] NOT NULL,
	[Deleted] [tinyint] NULL,
 CONSTRAINT [pk_dbo_TABillOfLadingHeader_TABillOfLadingHeaderId] PRIMARY KEY NONCLUSTERED 
(
	[TABillOfLadingHeaderId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[TABillOfLadingHeader] ADD  CONSTRAINT [df_dbo_TABillOfLadingHeader_ModifiedAt]  DEFAULT (getdate()) FOR [ModifiedAt]
GO

ALTER TABLE [dbo].[TABillOfLadingHeader] ADD  CONSTRAINT [df_dbo_TABillOfLadingHeader_ModifiedBy]  DEFAULT ('SYSTEM') FOR [ModifiedBy]
GO

ALTER TABLE [dbo].[TABillOfLadingHeader] ADD  CONSTRAINT [df_dbo_TABillOfLadingHeader_Deleted]  DEFAULT ((0)) FOR [Deleted]
GO


