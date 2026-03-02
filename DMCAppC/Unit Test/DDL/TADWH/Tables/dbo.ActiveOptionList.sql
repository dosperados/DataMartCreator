USE [TADWH]
GO

/****** Object:  Table [dbo].[ActiveOptionList]    Script Date: 13.01.2026 10:59:31 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[ActiveOptionList](
	[OptionListId] [int] NOT NULL,
	[VersionId] [int] NOT NULL,
	[TableNo] [int] NOT NULL,
	[Tablename] [sysname] NOT NULL,
	[FieldNo] [int] NOT NULL,
	[FieldName] [nvarchar](250) NULL,
	[FieldCaptionENU] [nvarchar](250) NULL,
	[FieldCaptionDE] [nvarchar](250) NULL,
	[TableNameDWH] [nvarchar](255) NULL,
	[FieldNameDWH] [nvarchar](255) NULL,
	[FieldCaptionDeDWH] [nvarchar](255) NULL,
	[FieldCaptionEnuDWH] [nvarchar](255) NULL,
	[OptionNo] [int] NOT NULL,
	[OptionENU] [nvarchar](128) NULL,
	[OptionDE] [nvarchar](128) NULL,
	[ModifiedAt] [datetime] NOT NULL,
	[ModifiedBy] [sysname] NOT NULL,
	[OptionId]  AS ((((CONVERT([nvarchar](128),[TableNo])+'_')+CONVERT([nvarchar](16),[FieldNo]))+'_')+CONVERT([nvarchar](16),[OptionNo])),
 CONSTRAINT [pk_dbo_ActiveOptionList_TableNo_FieldNo_OptionNo] PRIMARY KEY CLUSTERED 
(
	[TableNo] ASC,
	[FieldNo] ASC,
	[OptionNo] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [UC_dbo_OptionList] UNIQUE NONCLUSTERED 
(
	[TableNameDWH] ASC,
	[FieldNameDWH] ASC,
	[OptionNo] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO


