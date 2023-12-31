USE [1806aa]
GO
/****** Object:  Table [dbo].[IM_Content]    Script Date: 2021/4/1 16:34:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[IM_Content](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[To_empid] [int] NOT NULL,
	[form_empid] [int] NOT NULL,
	[mes_content] [nvarchar](1000) NOT NULL,
	[addtime] [datetime] NOT NULL,
 CONSTRAINT [PK_IM_Content] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[IM_Emp]    Script Date: 2021/4/1 16:34:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[IM_Emp](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[ename] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_IM_Emp] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET IDENTITY_INSERT [dbo].[IM_Content] ON 

INSERT [dbo].[IM_Content] ([id], [To_empid], [form_empid], [mes_content], [addtime]) VALUES (1, 1, 2, N'Hello', CAST(0x0000ACFD00FABEA0 AS DateTime))
INSERT [dbo].[IM_Content] ([id], [To_empid], [form_empid], [mes_content], [addtime]) VALUES (2, 2, 1, N'Hi', CAST(0x0000ACFD00FAE8D0 AS DateTime))
INSERT [dbo].[IM_Content] ([id], [To_empid], [form_empid], [mes_content], [addtime]) VALUES (3, 1, 2, N'还在听么？', CAST(0x0000ACFD010C4AD1 AS DateTime))
INSERT [dbo].[IM_Content] ([id], [To_empid], [form_empid], [mes_content], [addtime]) VALUES (4, 1, 2, N'是的，我还在听课！', CAST(0x0000ACFD010E6881 AS DateTime))
INSERT [dbo].[IM_Content] ([id], [To_empid], [form_empid], [mes_content], [addtime]) VALUES (5, 1, 2, N'还在么？', CAST(0x0000ACFD010F8D5B AS DateTime))
INSERT [dbo].[IM_Content] ([id], [To_empid], [form_empid], [mes_content], [addtime]) VALUES (6, 2, 1, N'我还在呢？', CAST(0x0000ACFD010FC73D AS DateTime))
INSERT [dbo].[IM_Content] ([id], [To_empid], [form_empid], [mes_content], [addtime]) VALUES (7, 1, 2, N'好，今天的代码能理解么？', CAST(0x0000ACFD010FF0E5 AS DateTime))
INSERT [dbo].[IM_Content] ([id], [To_empid], [form_empid], [mes_content], [addtime]) VALUES (8, 2, 1, N'还可以，还有即时显示没有看到怎么实现', CAST(0x0000ACFD01100ED9 AS DateTime))
INSERT [dbo].[IM_Content] ([id], [To_empid], [form_empid], [mes_content], [addtime]) VALUES (9, 1, 2, N'', CAST(0x0000ACFD01104934 AS DateTime))
SET IDENTITY_INSERT [dbo].[IM_Content] OFF
SET IDENTITY_INSERT [dbo].[IM_Emp] ON 

INSERT [dbo].[IM_Emp] ([id], [ename]) VALUES (1, N'Tom')
INSERT [dbo].[IM_Emp] ([id], [ename]) VALUES (2, N'Jerry')
INSERT [dbo].[IM_Emp] ([id], [ename]) VALUES (3, N'John')
SET IDENTITY_INSERT [dbo].[IM_Emp] OFF
