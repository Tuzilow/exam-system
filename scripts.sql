create database ExaminationSystemDb;
GO
USE [ExaminationSystemDb]
GO
/****** Object:  Table [dbo].[ES_ExamPaper]    Script Date: 20/7/14 下午 6:25:03 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ES_ExamPaper](
	[EmPaperId] [int] IDENTITY(1,1) NOT NULL,
	[EmPaperName] [nvarchar](512) NULL,
	[EmPaperSelectNum] [int] NOT NULL,
	[EmPaperFillNum] [int] NOT NULL,
	[EmPaperJudgeNum] [int] NOT NULL,
	[EmPaperMultipleNum] [int] NOT NULL,
	[EmPaperScore] [int] NOT NULL,
	[EmPaperTrueScore] [float] NULL,
	[IsDel] [bit] NOT NULL,
	[EmPaperSelectScore] [int] NOT NULL,
	[EmPaperFillScore] [int] NOT NULL,
	[EmPaperJudgeScore] [int] NOT NULL,
	[EmPaperMultipleScore] [int] NOT NULL,
	[EmPtId] [int] NULL,
	[EmTagPercent] [nvarchar](max) NULL,
 CONSTRAINT [PK__ES_ExamP__A6CA12AD08719CE6] PRIMARY KEY CLUSTERED 
(
	[EmPaperId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ES_User]    Script Date: 20/7/14 下午 6:25:03 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ES_User](
	[UserId] [int] IDENTITY(1,1) NOT NULL,
	[UserAccount] [varchar](16) NOT NULL,
	[UserPassword] [varchar](32) NOT NULL,
	[UserName] [nvarchar](8) NOT NULL,
	[RoleId] [int] NULL,
	[IsDel] [bit] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
UNIQUE NONCLUSTERED 
(
	[UserAccount] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ES_ExamPart]    Script Date: 20/7/14 下午 6:25:03 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ES_ExamPart](
	[EmPtId] [int] IDENTITY(1,1) NOT NULL,
	[EmPtStart] [datetime] NOT NULL,
	[EmPtEnd] [datetime] NOT NULL,
	[IsDel] [bit] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[EmPtId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ES_User_ExamPart]    Script Date: 20/7/14 下午 6:25:03 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ES_User_ExamPart](
	[UEmPt] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [int] NULL,
	[EmPtId] [int] NULL,
	[IsJoin] [bit] NOT NULL,
	[IsDel] [bit] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[UEmPt] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ES_User_ExamPaper]    Script Date: 20/7/14 下午 6:25:03 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ES_User_ExamPaper](
	[UEmPaper] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [int] NULL,
	[EmPaperId] [int] NULL,
	[IsDel] [bit] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[UEmPaper] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  View [dbo].[User_ExamPart_ExamPaper_v]    Script Date: 20/7/14 下午 6:25:03 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create view [dbo].[User_ExamPart_ExamPaper_v] as
select 
	u.UserId 考生ID,
	u.UserAccount 账号,
	u.UserPassword 密码,
	u.UserName 姓名,
	epart.EmPtStart 开始时间,
	epart.EmPtEnd 结束时间,
	epaper.EmPaperId 试卷ID,
	epaper.EmPaperName 试卷标题,
	epaper.EmPaperSelectNum 选择题数量,
	epaper.EmPaperFillNum 填空题数量,
	epaper.EmPaperJudgeNum 判断题数量,
	epaper.EmPaperMultipleNum 多选题数量,
	epaper.EmPaperScore 总分值,
	epaper.EmPaperTrueScore 得分
from ES_User u
left join ES_User_ExamPart uepart on u.UserId = uepart.UserId and uepart.IsDel = 0
left join ES_ExamPart epart on uepart.EmPtId = epart.EmPtId and epart.IsDel = 0
left join ES_User_ExamPaper uepaper on uepaper.UserId = u.UserId and uepaper.IsDel = 0
left join ES_ExamPaper epaper on epaper.EmPaperId = uepaper.EmPaperId and epaper.IsDel = 0
where u.RoleId = 1 and u.IsDel = 0;
GO
/****** Object:  Table [dbo].[ES_ExamLog]    Script Date: 20/7/14 下午 6:25:03 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ES_ExamLog](
	[LogId] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [int] NULL,
	[EmPaperId] [int] NULL,
	[ExercisesId] [varchar](512) NOT NULL,
	[EmPtId] [int] NULL,
	[IsStart] [bit] NOT NULL,
	[IsSubmit] [bit] NOT NULL,
	[Answers] [nvarchar](max) NULL,
	[IsDel] [bit] NOT NULL,
	[ExamScore] [float] NOT NULL,
	[StartTime] [varchar](max) NULL,
PRIMARY KEY CLUSTERED 
(
	[LogId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ES_ExamPaper_Exercise]    Script Date: 20/7/14 下午 6:25:03 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ES_ExamPaper_Exercise](
	[EEId] [int] IDENTITY(1,1) NOT NULL,
	[EmPaperId] [int] NULL,
	[EsId] [int] NULL,
	[IsDel] [bit] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[EEId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ES_Exercise]    Script Date: 20/7/14 下午 6:25:03 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ES_Exercise](
	[EsId] [int] IDENTITY(1,1) NOT NULL,
	[EsType] [nchar](3) NOT NULL,
	[EsSubExerciseId] [int] NOT NULL,
	[ImgId] [int] NULL,
	[IsDel] [bit] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[EsId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ES_FillAnswer]    Script Date: 20/7/14 下午 6:25:03 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ES_FillAnswer](
	[FAId] [int] IDENTITY(1,1) NOT NULL,
	[FQId] [int] NOT NULL,
	[FAContent] [nvarchar](32) NOT NULL,
	[FAScore] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[FAId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ES_FillQuestion]    Script Date: 20/7/14 下午 6:25:03 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ES_FillQuestion](
	[FQId] [int] IDENTITY(1,1) NOT NULL,
	[FQTitle] [nvarchar](512) NULL,
	[FQScore] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[FQId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ES_Image]    Script Date: 20/7/14 下午 6:25:03 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ES_Image](
	[ImgId] [int] IDENTITY(1,1) NOT NULL,
	[ImgTitle] [nvarchar](128) NOT NULL,
	[ImgUrl] [varchar](256) NOT NULL,
	[ImgOther] [varchar](256) NULL,
	[IsDel] [bit] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[ImgId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [UQ_Title] UNIQUE NONCLUSTERED 
(
	[ImgTitle] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ES_JudgeQuestion]    Script Date: 20/7/14 下午 6:25:03 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ES_JudgeQuestion](
	[JQId] [int] IDENTITY(1,1) NOT NULL,
	[JQTitle] [nvarchar](512) NULL,
	[JQScore] [int] NOT NULL,
	[JQIsTrue] [bit] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[JQId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ES_MultipleAnswer]    Script Date: 20/7/14 下午 6:25:03 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ES_MultipleAnswer](
	[MAId] [int] IDENTITY(1,1) NOT NULL,
	[MQId] [int] NOT NULL,
	[MAContent] [char](6) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[MAId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ES_MultipleQuestion]    Script Date: 20/7/14 下午 6:25:03 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ES_MultipleQuestion](
	[MQId] [int] IDENTITY(1,1) NOT NULL,
	[MQTitle] [nvarchar](512) NULL,
	[MQAns1] [nvarchar](64) NOT NULL,
	[MQAns2] [nvarchar](64) NOT NULL,
	[MQAns3] [nvarchar](64) NOT NULL,
	[MQAns4] [nvarchar](64) NOT NULL,
	[MQAns5] [nvarchar](64) NULL,
	[MQAns6] [nvarchar](64) NULL,
	[MQAns7] [nvarchar](64) NULL,
	[MQScore] [int] NOT NULL,
 CONSTRAINT [PK__ES_Multi__6C3F3491F1D73E55] PRIMARY KEY CLUSTERED 
(
	[MQId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ES_Paper_Tag]    Script Date: 20/7/14 下午 6:25:03 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ES_Paper_Tag](
	[PrTgId] [int] IDENTITY(1,1) NOT NULL,
	[PaperId] [int] NULL,
	[TagId] [int] NULL,
	[ExerciseNum] [int] NOT NULL,
	[IsNotSelect] [bit] NOT NULL,
	[IsDel] [bit] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[PrTgId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ES_Permission]    Script Date: 20/7/14 下午 6:25:03 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ES_Permission](
	[PmId] [int] IDENTITY(1,1) NOT NULL,
	[PmName] [nvarchar](8) NOT NULL,
	[PmDescribe] [nvarchar](128) NOT NULL,
	[IsDel] [bit] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[PmId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ES_Role]    Script Date: 20/7/14 下午 6:25:03 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ES_Role](
	[RoleId] [int] IDENTITY(1,1) NOT NULL,
	[RoleName] [nvarchar](8) NOT NULL,
	[RoleDescribe] [nvarchar](128) NOT NULL,
	[IsDel] [bit] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[RoleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ES_Role_Permission]    Script Date: 20/7/14 下午 6:25:03 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ES_Role_Permission](
	[RlPmId] [int] IDENTITY(1,1) NOT NULL,
	[RoleId] [int] NOT NULL,
	[PmId] [int] NOT NULL,
	[IsDel] [bit] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[RlPmId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ES_SelectQuestion]    Script Date: 20/7/14 下午 6:25:03 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ES_SelectQuestion](
	[SQId] [int] IDENTITY(1,1) NOT NULL,
	[SQTitle] [nvarchar](512) NULL,
	[SQTrueAns] [nvarchar](64) NOT NULL,
	[SQAns1] [nvarchar](64) NOT NULL,
	[SQAns2] [nvarchar](64) NOT NULL,
	[SQAns3] [nvarchar](64) NOT NULL,
	[SQAns4] [nvarchar](64) NULL,
	[SQScore] [int] NOT NULL,
 CONSTRAINT [PK__ES_Selec__F47276B08BBA72F1] PRIMARY KEY CLUSTERED 
(
	[SQId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ES_Tag]    Script Date: 20/7/14 下午 6:25:03 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ES_Tag](
	[TagId] [int] IDENTITY(1,1) NOT NULL,
	[TagName] [nvarchar](8) NOT NULL,
	[TagDescribe] [nvarchar](64) NULL,
	[IsDel] [bit] NOT NULL,
	[IsPrivate] [bit] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[TagId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
UNIQUE NONCLUSTERED 
(
	[TagName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ES_Tag_Exercise]    Script Date: 20/7/14 下午 6:25:03 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ES_Tag_Exercise](
	[TgEsId] [int] IDENTITY(1,1) NOT NULL,
	[TagId] [int] NULL,
	[EsId] [int] NULL,
	[IsDel] [bit] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[TgEsId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[ES_ExamLog] ADD  DEFAULT ((0)) FOR [IsStart]
GO
ALTER TABLE [dbo].[ES_ExamLog] ADD  DEFAULT ((0)) FOR [IsSubmit]
GO
ALTER TABLE [dbo].[ES_ExamLog] ADD  DEFAULT ((0)) FOR [IsDel]
GO
ALTER TABLE [dbo].[ES_ExamLog] ADD  DEFAULT ((0)) FOR [ExamScore]
GO
ALTER TABLE [dbo].[ES_ExamPaper] ADD  CONSTRAINT [DF__ES_ExamPa__EmPap__4F7CD00D]  DEFAULT ((100)) FOR [EmPaperScore]
GO
ALTER TABLE [dbo].[ES_ExamPaper] ADD  CONSTRAINT [DF__ES_ExamPa__EmPap__5070F446]  DEFAULT ((0)) FOR [EmPaperTrueScore]
GO
ALTER TABLE [dbo].[ES_ExamPaper] ADD  CONSTRAINT [DF__ES_ExamPa__IsDel__5165187F]  DEFAULT ((0)) FOR [IsDel]
GO
ALTER TABLE [dbo].[ES_ExamPaper_Exercise] ADD  DEFAULT ((0)) FOR [IsDel]
GO
ALTER TABLE [dbo].[ES_ExamPart] ADD  DEFAULT ((0)) FOR [IsDel]
GO
ALTER TABLE [dbo].[ES_Exercise] ADD  DEFAULT ((0)) FOR [IsDel]
GO
ALTER TABLE [dbo].[ES_Image] ADD  DEFAULT ((0)) FOR [IsDel]
GO
ALTER TABLE [dbo].[ES_JudgeQuestion] ADD  DEFAULT ((1)) FOR [JQIsTrue]
GO
ALTER TABLE [dbo].[ES_Paper_Tag] ADD  DEFAULT ((0)) FOR [IsNotSelect]
GO
ALTER TABLE [dbo].[ES_Paper_Tag] ADD  DEFAULT ((0)) FOR [IsDel]
GO
ALTER TABLE [dbo].[ES_Permission] ADD  DEFAULT ((0)) FOR [IsDel]
GO
ALTER TABLE [dbo].[ES_Role] ADD  DEFAULT ((0)) FOR [IsDel]
GO
ALTER TABLE [dbo].[ES_Role_Permission] ADD  DEFAULT ((0)) FOR [IsDel]
GO
ALTER TABLE [dbo].[ES_Tag] ADD  DEFAULT ((0)) FOR [IsDel]
GO
ALTER TABLE [dbo].[ES_Tag] ADD  DEFAULT ((0)) FOR [IsPrivate]
GO
ALTER TABLE [dbo].[ES_Tag_Exercise] ADD  DEFAULT ((0)) FOR [IsDel]
GO
ALTER TABLE [dbo].[ES_User] ADD  DEFAULT ((0)) FOR [IsDel]
GO
ALTER TABLE [dbo].[ES_User_ExamPaper] ADD  DEFAULT ((0)) FOR [IsDel]
GO
ALTER TABLE [dbo].[ES_User_ExamPart] ADD  DEFAULT ((0)) FOR [IsJoin]
GO
ALTER TABLE [dbo].[ES_User_ExamPart] ADD  DEFAULT ((0)) FOR [IsDel]
GO
ALTER TABLE [dbo].[ES_ExamLog]  WITH CHECK ADD  CONSTRAINT [FK__ES_ExamLo__EmPap__503BEA1C] FOREIGN KEY([EmPaperId])
REFERENCES [dbo].[ES_ExamPaper] ([EmPaperId])
GO
ALTER TABLE [dbo].[ES_ExamLog] CHECK CONSTRAINT [FK__ES_ExamLo__EmPap__503BEA1C]
GO
ALTER TABLE [dbo].[ES_ExamLog]  WITH CHECK ADD FOREIGN KEY([EmPtId])
REFERENCES [dbo].[ES_ExamPart] ([EmPtId])
GO
ALTER TABLE [dbo].[ES_ExamLog]  WITH CHECK ADD FOREIGN KEY([UserId])
REFERENCES [dbo].[ES_User] ([UserId])
GO
ALTER TABLE [dbo].[ES_ExamPaper]  WITH CHECK ADD  CONSTRAINT [FK__ES_ExamPa__EmPtI__607251E5] FOREIGN KEY([EmPtId])
REFERENCES [dbo].[ES_ExamPart] ([EmPtId])
GO
ALTER TABLE [dbo].[ES_ExamPaper] CHECK CONSTRAINT [FK__ES_ExamPa__EmPtI__607251E5]
GO
ALTER TABLE [dbo].[ES_ExamPaper_Exercise]  WITH CHECK ADD  CONSTRAINT [FK__ES_ExamPa__EmPap__6477ECF3] FOREIGN KEY([EmPaperId])
REFERENCES [dbo].[ES_ExamPaper] ([EmPaperId])
GO
ALTER TABLE [dbo].[ES_ExamPaper_Exercise] CHECK CONSTRAINT [FK__ES_ExamPa__EmPap__6477ECF3]
GO
ALTER TABLE [dbo].[ES_ExamPaper_Exercise]  WITH CHECK ADD FOREIGN KEY([EsId])
REFERENCES [dbo].[ES_Exercise] ([EsId])
GO
ALTER TABLE [dbo].[ES_Exercise]  WITH CHECK ADD FOREIGN KEY([ImgId])
REFERENCES [dbo].[ES_Image] ([ImgId])
GO
ALTER TABLE [dbo].[ES_FillAnswer]  WITH CHECK ADD FOREIGN KEY([FQId])
REFERENCES [dbo].[ES_FillQuestion] ([FQId])
GO
ALTER TABLE [dbo].[ES_MultipleAnswer]  WITH CHECK ADD  CONSTRAINT [FK__ES_Multipl__MQId__787EE5A0] FOREIGN KEY([MQId])
REFERENCES [dbo].[ES_MultipleQuestion] ([MQId])
GO
ALTER TABLE [dbo].[ES_MultipleAnswer] CHECK CONSTRAINT [FK__ES_Multipl__MQId__787EE5A0]
GO
ALTER TABLE [dbo].[ES_Paper_Tag]  WITH CHECK ADD  CONSTRAINT [FK__ES_Paper___Paper__5CD6CB2B] FOREIGN KEY([PaperId])
REFERENCES [dbo].[ES_ExamPaper] ([EmPaperId])
GO
ALTER TABLE [dbo].[ES_Paper_Tag] CHECK CONSTRAINT [FK__ES_Paper___Paper__5CD6CB2B]
GO
ALTER TABLE [dbo].[ES_Paper_Tag]  WITH CHECK ADD FOREIGN KEY([TagId])
REFERENCES [dbo].[ES_Tag] ([TagId])
GO
ALTER TABLE [dbo].[ES_Role_Permission]  WITH CHECK ADD FOREIGN KEY([RoleId])
REFERENCES [dbo].[ES_Role] ([RoleId])
GO
ALTER TABLE [dbo].[ES_Role_Permission]  WITH CHECK ADD FOREIGN KEY([PmId])
REFERENCES [dbo].[ES_Permission] ([PmId])
GO
ALTER TABLE [dbo].[ES_Tag_Exercise]  WITH CHECK ADD FOREIGN KEY([TagId])
REFERENCES [dbo].[ES_Tag] ([TagId])
GO
ALTER TABLE [dbo].[ES_Tag_Exercise]  WITH CHECK ADD FOREIGN KEY([EsId])
REFERENCES [dbo].[ES_Exercise] ([EsId])
GO
ALTER TABLE [dbo].[ES_User]  WITH CHECK ADD FOREIGN KEY([RoleId])
REFERENCES [dbo].[ES_Role] ([RoleId])
GO
ALTER TABLE [dbo].[ES_User_ExamPaper]  WITH CHECK ADD  CONSTRAINT [FK__ES_User_E__EmPap__5535A963] FOREIGN KEY([EmPaperId])
REFERENCES [dbo].[ES_ExamPaper] ([EmPaperId])
GO
ALTER TABLE [dbo].[ES_User_ExamPaper] CHECK CONSTRAINT [FK__ES_User_E__EmPap__5535A963]
GO
ALTER TABLE [dbo].[ES_User_ExamPaper]  WITH CHECK ADD FOREIGN KEY([UserId])
REFERENCES [dbo].[ES_User] ([UserId])
GO
ALTER TABLE [dbo].[ES_User_ExamPart]  WITH CHECK ADD FOREIGN KEY([EmPtId])
REFERENCES [dbo].[ES_ExamPart] ([EmPtId])
GO
ALTER TABLE [dbo].[ES_User_ExamPart]  WITH CHECK ADD FOREIGN KEY([UserId])
REFERENCES [dbo].[ES_User] ([UserId])
GO
USE [master]
GO
ALTER DATABASE [ExaminationSystemDb] SET  READ_WRITE 
GO
use [ExaminationSystemDb];
GO
insert into ES_Permission(PmName, PmDescribe) values('DOEXAM', '获取试卷及答题权限');
insert into ES_Permission(PmName, PmDescribe) values('BACKEND', '进入后台权限');
insert into ES_Permission(PmName, PmDescribe) values('REGISTER', '注册、删除考生账号权限');
insert into ES_Permission(PmName, PmDescribe) values('ISSUE', '添加试题权限');
insert into ES_Permission(PmName, PmDescribe) values('PAPER', '生成试卷权限');
insert into ES_Permission(PmName, PmDescribe) values('CANEXAM', '授权答题权限的权限');
insert into ES_Permission(PmName, PmDescribe) values('OPTAG', '添加、修改、删除标签权限');
insert into ES_Permission(PmName, PmDescribe) values('OPEMMSG', '添加、修改、删除考试信息权限');
insert into ES_Permission(PmName, PmDescribe) values('AUTHOR', '注册、删除出卷人账号权限');
insert into ES_Permission(PmName, PmDescribe) values('SECRET', '查看私密题库权限');
GO
insert into ES_Role(RoleName, RoleDescribe) values('考生', '只有答题权限');
insert into ES_Role(RoleName, RoleDescribe) values('出卷人', '无答题权限，默认无查看私密题库权限，无注册、删除出卷人账号权限');
insert into ES_Role(RoleName, RoleDescribe) values('管理员', '有全部权限');
GO
insert into ES_Role_Permission(RoleId, PmId) values(1, 1); -- 考生
insert into ES_Role_Permission(RoleId, PmId) values(2, 2); -- 出卷人
insert into ES_Role_Permission(RoleId, PmId) values(2, 3); -- 出卷人
insert into ES_Role_Permission(RoleId, PmId) values(2, 4); -- 出卷人
insert into ES_Role_Permission(RoleId, PmId) values(2, 5); -- 出卷人
insert into ES_Role_Permission(RoleId, PmId) values(2, 6); -- 出卷人
insert into ES_Role_Permission(RoleId, PmId) values(2, 8); -- 出卷人
insert into ES_Role_Permission(RoleId, PmId) values(3, 1); -- 管理员
insert into ES_Role_Permission(RoleId, PmId) values(3, 2); -- 管理员
insert into ES_Role_Permission(RoleId, PmId) values(3, 3); -- 管理员
insert into ES_Role_Permission(RoleId, PmId) values(3, 4); -- 管理员
insert into ES_Role_Permission(RoleId, PmId) values(3, 5); -- 管理员
insert into ES_Role_Permission(RoleId, PmId) values(3, 6); -- 管理员
insert into ES_Role_Permission(RoleId, PmId) values(3, 8); -- 管理员
insert into ES_Role_Permission(RoleId, PmId) values(3, 10); -- 管理员
GO
insert into ES_User(UserAccount, UserName, UserPassword, RoleId) values('admin', '管理员', '123456', 3);