-- 创建考试系统数据库
if db_id('ExaminationSystemDb') is null
	create database ExaminationSystemDb;

go
use ExaminationSystemDb;

go
-- 创建权限表
if not exists (select * from sysobjects where name='ES_Permission')
	create table ES_Permission (
		PmId int primary key identity(1,1),
		PmName nvarchar(8) not null,
		PmDescribe nvarchar(128) not null,
		IsDel bit not null default(0)
	);

go
-- 创建角色表
if not exists (select * from sysobjects where name='ES_Role')
	create table ES_Role (
		RoleId int primary key identity(1,1),
		RoleName nvarchar(8) not null,
		RoleDescribe nvarchar(128) not null,
		IsDel bit not null default(0)
	);

go
-- 创建角色权限中间表
if not exists (select * from sysobjects where name='ES_Role_Permission')
	create table ES_Role_Permission (
		RlPmId int primary key identity(1,1),
		RoleId int foreign key references ES_Role(RoleId) not null,
		PmId int foreign key references ES_Permission(PmId) not null,
		IsDel bit not null default(0)
	);

go
-- 创建用户表
if not exists (select * from sysobjects where name='ES_User')
	create table ES_User (
		UserId int primary key identity(1,1),
		UserAccount varchar(16) unique not null,
		UserPassword varchar(32) not null,
		UserName nvarchar(8) not null,
		RoleId int foreign key references ES_Role(RoleId),
		IsDel bit not null default(0)
	);

go
-- 创建场次表
if not exists (select * from sysobjects where name='ES_ExamPart')
	create table ES_ExamPart (
		EmPtId int primary key identity(1,1),
		EmPtStart datetime not null,
		EmPtEnd datetime not null,
		IsDel bit not null default(0)
	);

go
-- 创建考生（用户）场次中间表
if not exists (select * from sysobjects where name='ES_User_ExamPart')
	create table ES_User_ExamPart (
		UEmPt int primary key identity(1,1),
		UserId int foreign key references ES_User(UserId),
		EmPtId int foreign key references ES_ExamPart(EmPtId),
		IsJoin bit not null default(0),
		IsDel bit not null default(0)
	);

go
-- 创建试卷表
if not exists (select * from sysobjects where name='ES_ExamPaper')
	create table ES_ExamPaper (
		EmPaperId int primary key identity(1,1),
		EmPaperName nvarchar(32) not null,
		EmPaperSelectNum int not null, -- 选择题数量
		EmPaperFillNum int not null, -- 填空题数量
		EmPaperJudgeNum int not null, -- 判断题数量
		EmPaperMultipleNum int not null, -- 多选题数量
		EmPaperScore int not null default(100), -- 总分值
		EmPaperTrueScore int default(0), -- 得分
		IsDel bit not null default(0)
	);

go
-- 创建考生（用户）试卷表
if not exists (select * from sysobjects where name='ES_User_ExamPaper')
	create table ES_User_ExamPaper (
		UEmPaper int primary key identity(1,1),
		UserId int foreign key references ES_User(UserId),
		EmPaperId int foreign key references ES_ExamPaper(EmPaperId),
		IsDel bit not null default(0)
	);

go
-- 创建标签表
if not exists (select * from sysobjects where name='ES_Tag')
	create table ES_Tag (
		TagId int primary key identity(1,1),
		TagName nvarchar(8) unique not null,
		TagDescribe nvarchar(64),
		IsDel bit not null default(0)
	);

go
-- 创建试卷标签中间表
if not exists (select * from sysobjects where name='ES_Paper_Tag')
	create table ES_Paper_Tag (
		PrTgId int primary key identity(1,1),
		PaperId int foreign key references ES_ExamPaper(EmPaperId),
		TagId int foreign key references ES_Tag(TagId),
		ExerciseNum int not null, -- 所需要的选定标签的题目数量
		IsNotSelect bit not null default(0), -- 是否排除这个标签下的题
		IsDel bit not null default(0)
	);

go
-- 创建题目表
if not exists (select * from sysobjects where name='ES_Exercise')
	create table ES_Exercise (
		EsId int primary key identity(1,1),
		EsType nchar(3) not null, -- 题目类型
		EsSubExerciseId int not null -- 子分类下题目Id，根据EsType确定哪个分类
	);

go
-- 创建试卷题目表
if not exists (select * from sysobjects where name='ES_ExamPaper_Exercise')
	create table ES_ExamPaper_Exercise (
		EEId int primary key identity(1,1),
		EmPaperId int foreign key references ES_ExamPaper(EmPaperId),
		EsId int foreign key references ES_Exercise(EsId),
		IsDel bit not null default(0)
	);

go
-- 创建标签题目中间表
if not exists (select * from sysobjects where name='ES_Tag_Exercise')
	create table ES_Tag_Exercise(
		TgEsId int primary key identity(1,1),
		TagId int foreign key references ES_Tag(TagId),
		EsId int foreign key references ES_Exercise(EsId),
		IsDel bit not null default(0)
	);

go
-- 创建选择题表
if not exists (select * from sysobjects where name='ES_SelectQuestion')
	create table ES_SelectQuestion (
		SQId int primary key identity(1,1),
		SQTitle nvarchar(128) not null,
		SQTrueAns nvarchar(64) not null,
		SQAns1 nvarchar(64) not null,
		SQAns2 nvarchar(64) not null,
		SQAns3 nvarchar(64) not null,
		SQAns4 nvarchar(64) not null,
		SQScore int not null
	);

go
-- 创建判断题表
if not exists (select * from sysobjects where name='ES_JudgeQuestion')
	create table ES_JudgeQuestion (
		JQId int primary key identity(1,1),
		JQTitle nvarchar(128) not null,
		JQTrueAns nvarchar(64) not null,
		JQFalseAns nvarchar(64) not null,
		JQScore int not null
	);

go
-- 创建填空题表和答案表
if not exists (select * from sysobjects where name='ES_FillQuestion')
	create table ES_FillQuestion (
		FQId int primary key identity(1,1),
		FQTitle nvarchar(128) not null,
		FQScore int not null
	);
go
if not exists (select * from sysobjects where name='ES_FillAnswer')
	create table ES_FillAnswer (
		FAId int primary key identity(1,1),
		FQId int foreign key references ES_FillQuestion(FQId) not null,
		FAContent nvarchar(32) not null,
		FAScore int not null
	);

go
-- 创建多选题和答案表
if not exists (select * from sysobjects where name='ES_MultipleQuestion')
	create table ES_MultipleQuestion (
		MQId int primary key identity(1,1),
		MQTitle nvarchar(128) not null,
		MQAns1 nvarchar(64) not null,
		MQAns2 nvarchar(64) not null,
		MQAns3 nvarchar(64) not null,
		MQAns4 nvarchar(64) not null,
		MQAns5 nvarchar(64) not null,
		MQAns6 nvarchar(64) not null,
		MQAns7 nvarchar(64) not null,
		MQScore int not null
	);
go
if not exists (select * from sysobjects where name='ES_MultipleAnswer')
	create table ES_MultipleAnswer (
		MAId int primary key identity(1,1),
		MQId int foreign key references ES_MultipleQuestion(MQId) not null,
		MAContent char(6) not null -- 填写 MQAns1
	);

go
-- 权限表数据
-- 1.获取试卷及答题权限
-- 2.进入后台权限
-- 3.注册、删除考生账号权限
-- 4.添加试题权限
-- 5.生成试卷权限
-- 6.授权答题权限的权限
-- 7.添加、修改、删除标签权限
-- 8.添加、修改、删除考试信息权限
-- 9.注册、删除出卷人账号权限
-- 10.查看私密题库权限
-- insert into ES_Permission(PmName, PmDescribe) values('DOEXAM', '获取试卷及答题权限');
-- insert into ES_Permission(PmName, PmDescribe) values('BACKEND', '进入后台权限');
-- insert into ES_Permission(PmName, PmDescribe) values('REGISTER', '注册、删除考生账号权限');
-- insert into ES_Permission(PmName, PmDescribe) values('ISSUE', '添加试题权限');
-- insert into ES_Permission(PmName, PmDescribe) values('PAPER', '生成试卷权限');
-- insert into ES_Permission(PmName, PmDescribe) values('CANEXAM', '授权答题权限的权限');
-- insert into ES_Permission(PmName, PmDescribe) values('OPTAG', '添加、修改、删除标签权限');
-- insert into ES_Permission(PmName, PmDescribe) values('OPEMMSG', '添加、修改、删除考试信息权限');
-- insert into ES_Permission(PmName, PmDescribe) values('AUTHOR', '注册、删除出卷人账号权限');
-- insert into ES_Permission(PmName, PmDescribe) values('SECRET', '查看私密题库权限');
select * from ES_Permission;
go
-- 角色表数据
-- 1.考生
-- 2.出卷人
-- 3.管理员
-- insert into ES_Role(RoleName, RoleDescribe) values('考生', '只有答题权限');
-- insert into ES_Role(RoleName, RoleDescribe) values('出卷人', '无答题权限，默认无查看私密题库权限，无注册、删除出卷人账号权限');
-- insert into ES_Role(RoleName, RoleDescribe) values('管理员', '有全部权限');
select * from ES_Role;
go
-- 权限角色表数据
-- insert into ES_Role_Permission(RoleId, PmId) values(1, 1); -- 考生
-- insert into ES_Role_Permission(RoleId, PmId) values(2, 2); -- 出卷人
-- insert into ES_Role_Permission(RoleId, PmId) values(2, 3); -- 出卷人
-- insert into ES_Role_Permission(RoleId, PmId) values(2, 4); -- 出卷人
-- insert into ES_Role_Permission(RoleId, PmId) values(2, 5); -- 出卷人
-- insert into ES_Role_Permission(RoleId, PmId) values(2, 6); -- 出卷人
-- insert into ES_Role_Permission(RoleId, PmId) values(2, 8); -- 出卷人
-- insert into ES_Role_Permission(RoleId, PmId) values(3, 1); -- 管理员
-- insert into ES_Role_Permission(RoleId, PmId) values(3, 2); -- 管理员
-- insert into ES_Role_Permission(RoleId, PmId) values(3, 3); -- 管理员
-- insert into ES_Role_Permission(RoleId, PmId) values(3, 4); -- 管理员
-- insert into ES_Role_Permission(RoleId, PmId) values(3, 5); -- 管理员
-- insert into ES_Role_Permission(RoleId, PmId) values(3, 6); -- 管理员
-- insert into ES_Role_Permission(RoleId, PmId) values(3, 8); -- 管理员
-- insert into ES_Role_Permission(RoleId, PmId) values(3, 10); -- 管理员
-- insert into ES_Role_Permission(RoleId, PmId) values(3, 11); -- 管理员
-- insert into ES_Role_Permission(RoleId, PmId) values(3, 12); -- 管理员
select * from ES_Role_Permission;
go
-- 添加用户
-- insert into ES_User(UserAccount, UserName, UserPassword, RoleId) values('admin', '管理员', '123456', 3);
-- insert into ES_User(UserAccount, UserName, UserPassword, RoleId) values('xmy', '徐my', '123456', 1);
-- insert into ES_User(UserAccount, UserName, UserPassword, RoleId) values('test01', 'test01', '123456', 1);
-- insert into ES_User(UserAccount, UserName, UserPassword, RoleId) values('test02', 'test02', '123456', 1);
-- insert into ES_User(UserAccount, UserName, UserPassword, RoleId) values('test03', 'test03', '123456', 1);
-- insert into ES_User(UserAccount, UserName, UserPassword, RoleId) values('test04', 'test04', '123456', 1);
-- insert into ES_User(UserAccount, UserName, UserPassword, RoleId) values('test05', 'test05', '123456', 1);
-- insert into ES_User(UserAccount, UserName, UserPassword, RoleId) values('test06', 'test06', '123456', 1);
-- insert into ES_User(UserAccount, UserName, UserPassword, RoleId) values('test07', 'test07', '123456', 1);
-- insert into ES_User(UserAccount, UserName, UserPassword, RoleId) values('test08', 'test08', '123456', 1);
-- insert into ES_User(UserAccount, UserName, UserPassword, RoleId) values('test09', 'test09', '123456', 1);
-- insert into ES_User(UserAccount, UserName, UserPassword, RoleId) values('test10', 'test10', '123456', 1);
-- insert into ES_User(UserAccount, UserName, UserPassword, RoleId) values('test11', 'test11', '123456', 1);
-- insert into ES_User(UserAccount, UserName, UserPassword, RoleId) values('test12', 'test12', '123456', 1);
-- insert into ES_User(UserAccount, UserName, UserPassword, RoleId) values('test13', 'test13', '123456', 1);
-- insert into ES_User(UserAccount, UserName, UserPassword, RoleId) values('test14', 'test14', '123456', 1);
-- insert into ES_User(UserAccount, UserName, UserPassword, RoleId) values('test15', 'test15', '123456', 1);
select * from ES_User;
go
-- 添加场次
-- insert into ES_ExamPart(EmPtStart, EmPtEnd) values('2020-05-14 08:00:00', '2020-05-14 10:00:00')
-- insert into ES_ExamPart(EmPtStart, EmPtEnd) values('2020-05-16 08:00:00', '2020-05-16 10:00:00')
select * from ES_ExamPart;
go
-- 添加场次关联
insert into ES_User_ExamPart(UserId ,EmPtId) values(2, 1);
select * from ES_User_ExamPart;
go
-- 创建视图
-- 考生信息视图
--create view User_ExamPart_ExamPaper_v as
--select 
--	u.UserId 考生ID,
--	u.UserAccount 账号,
--	u.UserPassword 密码,
--	u.UserName 姓名,
--	epart.EmPtStart 开始时间,
--	epart.EmPtEnd 结束时间,
--	epaper.EmPaperId 试卷ID,
--	epaper.EmPaperName 试卷标题,
--	epaper.EmPaperSelectNum 选择题数量,
--	epaper.EmPaperFillNum 填空题数量,
--	epaper.EmPaperJudgeNum 判断题数量,
--	epaper.EmPaperMultipleNum 多选题数量,
--	epaper.EmPaperScore 总分值,
--	epaper.EmPaperTrueScore 得分
--from ES_User u
--left join ES_User_ExamPart uepart on u.UserId = uepart.UserId and uepart.IsDel = 0
--left join ES_ExamPart epart on uepart.EmPtId = epart.EmPtId and epart.IsDel = 0
--left join ES_User_ExamPaper uepaper on uepaper.UserId = u.UserId and uepaper.IsDel = 0
--left join ES_ExamPaper epaper on epaper.EmPaperId = uepaper.EmPaperId and epaper.IsDel = 0
--where u.RoleId = 1 and u.IsDel = 0;
select * from User_ExamPart_ExamPaper_v;
go