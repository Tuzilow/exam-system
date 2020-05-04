-- ��������ϵͳ���ݿ�
if db_id('ExaminationSystemDb') is null
	create database ExaminationSystemDb;

go
use ExaminationSystemDb;

go
-- ����Ȩ�ޱ�
if not exists (select * from sysobjects where name='ES_Permission')
	create table ES_Permission (
		PmId int primary key identity(1,1),
		PmName nvarchar(8) not null,
		PmDescribe nvarchar(128) not null,
		IsDel bit not null default(0)
	);

go
-- ������ɫ��
if not exists (select * from sysobjects where name='ES_Role')
	create table ES_Role (
		RoleId int primary key identity(1,1),
		RoleName nvarchar(8) not null,
		RoleDescribe nvarchar(128) not null,
		IsDel bit not null default(0)
	);

go
-- ������ɫȨ���м��
if not exists (select * from sysobjects where name='ES_Role_Permission')
	create table ES_Role_Permission (
		RlPmId int primary key identity(1,1),
		RoleId int foreign key references ES_Role(RoleId) not null,
		PmId int foreign key references ES_Permission(PmId) not null,
		IsDel bit not null default(0)
	);

go
-- �����û���
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
-- �������α�
if not exists (select * from sysobjects where name='ES_ExamPart')
	create table ES_ExamPart (
		EmPtId int primary key identity(1,1),
		EmPtStart datetime not null,
		EmPtEnd datetime not null,
		IsDel bit not null default(0)
	);

go
-- �����������û��������м��
if not exists (select * from sysobjects where name='ES_User_ExamPart')
	create table ES_User_ExamPart (
		UEmPt int primary key identity(1,1),
		UserId int foreign key references ES_User(UserId),
		EmPtId int foreign key references ES_ExamPart(EmPtId),
		IsDel bit not null default(0)
	);

go
-- �����Ծ��
if not exists (select * from sysobjects where name='ES_ExamPaper')
	create table ES_ExamPaper (
		EmPaperId int primary key identity(1,1),
		EmPaperName nvarchar(32) not null,
		EmPaperSelectNum int not null, -- ѡ��������
		EmPaperFillNum int not null, -- ���������
		EmPaperJudgeNum int not null, -- �ж�������
		EmPaperMultipleNum int not null, -- ��ѡ������
		EmPaperScore int not null default(100), -- �ܷ�ֵ
		EmPaperTrueScore int default(0), -- �÷�
		IsDel bit not null default(0)
	);

go
-- �����������û����Ծ��
if not exists (select * from sysobjects where name='ES_User_ExamPaper')
	create table ES_User_ExamPaper (
		UEmPaper int primary key identity(1,1),
		UserId int foreign key references ES_User(UserId),
		EmPaperId int foreign key references ES_ExamPaper(EmPaperId),
		IsDel bit not null default(0)
	);

go
-- ������ǩ��
if not exists (select * from sysobjects where name='ES_Tag')
	create table ES_Tag (
		TagId int primary key identity(1,1),
		TagName nvarchar(8) unique not null,
		TagDescribe nvarchar(64),
		IsDel bit not null default(0)
	);

go
-- �����Ծ��ǩ�м��
if not exists (select * from sysobjects where name='ES_Paper_Tag')
	create table ES_Paper_Tag (
		PrTgId int primary key identity(1,1),
		PaperId int foreign key references ES_ExamPaper(EmPaperId),
		TagId int foreign key references ES_Tag(TagId),
		ExerciseNum int not null, -- ����Ҫ��ѡ����ǩ����Ŀ����
		IsNotSelect bit not null default(0), -- �Ƿ��ų������ǩ�µ���
		IsDel bit not null default(0)
	);

go
-- ������Ŀ��
if not exists (select * from sysobjects where name='ES_Exercise')
	create table ES_Exercise (
		EsId int primary key identity(1,1),
		EsType nchar(3) not null, -- ��Ŀ����
		EsSubExerciseId int not null -- �ӷ�������ĿId������EsTypeȷ���ĸ�����
	);

go
-- ������ǩ��Ŀ�м��
if not exists (select * from sysobjects where name='ES_Tag_Exercise')
	create table ES_Tag_Exercise(
		TgEsId int primary key identity(1,1),
		TagId int foreign key references ES_Tag(TagId),
		EsId int foreign key references ES_Exercise(EsId),
		IsDel bit not null default(0)
	);

go
-- ����ѡ�����
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
-- �����ж����
if not exists (select * from sysobjects where name='ES_JudgeQuestion')
	create table ES_JudgeQuestion (
		JQId int primary key identity(1,1),
		JQTitle nvarchar(128) not null,
		JQTrueAns nvarchar(64) not null,
		JQFalseAns nvarchar(64) not null,
		JQScore int not null
	);

go
-- ����������ʹ𰸱�
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
-- ������ѡ��ʹ𰸱�
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
		MAContent char(6) not null -- ��д MQAns1
	);

go
-- Ȩ�ޱ�����
-- 1.��ȡ�Ծ�����Ȩ��
-- 2.�����̨Ȩ��
-- 3.ע�ᡢɾ�������˺�Ȩ��
-- 4.�������Ȩ��
-- 5.�����Ծ�Ȩ��
-- 6.��Ȩ����Ȩ�޵�Ȩ��
-- 7.��ӡ��޸ġ�ɾ����ǩȨ��
-- 8.��ӡ��޸ġ�ɾ��������ϢȨ��
-- 9.ע�ᡢɾ���������˺�Ȩ��
-- 10.�鿴˽�����Ȩ��
-- insert into ES_Permission(PmName, PmDescribe) values('DOEXAM', '��ȡ�Ծ�����Ȩ��');
-- insert into ES_Permission(PmName, PmDescribe) values('BACKEND', '�����̨Ȩ��');
-- insert into ES_Permission(PmName, PmDescribe) values('REGISTER', 'ע�ᡢɾ�������˺�Ȩ��');
-- insert into ES_Permission(PmName, PmDescribe) values('ISSUE', '�������Ȩ��');
-- insert into ES_Permission(PmName, PmDescribe) values('PAPER', '�����Ծ�Ȩ��');
-- insert into ES_Permission(PmName, PmDescribe) values('CANEXAM', '��Ȩ����Ȩ�޵�Ȩ��');
-- insert into ES_Permission(PmName, PmDescribe) values('OPTAG', '��ӡ��޸ġ�ɾ����ǩȨ��');
-- insert into ES_Permission(PmName, PmDescribe) values('OPEMMSG', '��ӡ��޸ġ�ɾ��������ϢȨ��');
-- insert into ES_Permission(PmName, PmDescribe) values('AUTHOR', 'ע�ᡢɾ���������˺�Ȩ��');
-- insert into ES_Permission(PmName, PmDescribe) values('SECRET', '�鿴˽�����Ȩ��');
select * from ES_Permission;
go
-- ��ɫ������
-- 1.����
-- 2.������
-- 3.����Ա
-- insert into ES_Role(RoleName, RoleDescribe) values('����', 'ֻ�д���Ȩ��');
-- insert into ES_Role(RoleName, RoleDescribe) values('������', '�޴���Ȩ�ޣ�Ĭ���޲鿴˽�����Ȩ�ޣ���ע�ᡢɾ���������˺�Ȩ��');
-- insert into ES_Role(RoleName, RoleDescribe) values('����Ա', '��ȫ��Ȩ��');
select * from ES_Role;
go
-- Ȩ�޽�ɫ������
-- insert into ES_Role_Permission(RoleId, PmId) values(1, 1); -- ����
-- insert into ES_Role_Permission(RoleId, PmId) values(2, 2); -- ������
-- insert into ES_Role_Permission(RoleId, PmId) values(2, 3); -- ������
-- insert into ES_Role_Permission(RoleId, PmId) values(2, 4); -- ������
-- insert into ES_Role_Permission(RoleId, PmId) values(2, 5); -- ������
-- insert into ES_Role_Permission(RoleId, PmId) values(2, 6); -- ������
-- insert into ES_Role_Permission(RoleId, PmId) values(2, 8); -- ������
-- insert into ES_Role_Permission(RoleId, PmId) values(3, 1); -- ����Ա
-- insert into ES_Role_Permission(RoleId, PmId) values(3, 2); -- ����Ա
-- insert into ES_Role_Permission(RoleId, PmId) values(3, 3); -- ����Ա
-- insert into ES_Role_Permission(RoleId, PmId) values(3, 4); -- ����Ա
-- insert into ES_Role_Permission(RoleId, PmId) values(3, 5); -- ����Ա
-- insert into ES_Role_Permission(RoleId, PmId) values(3, 6); -- ����Ա
-- insert into ES_Role_Permission(RoleId, PmId) values(3, 8); -- ����Ա
-- insert into ES_Role_Permission(RoleId, PmId) values(3, 10); -- ����Ա
-- insert into ES_Role_Permission(RoleId, PmId) values(3, 11); -- ����Ա
-- insert into ES_Role_Permission(RoleId, PmId) values(3, 12); -- ����Ա
select * from ES_Role_Permission;
go
-- ��ӹ���Ա
insert into ES_User(UserAccount, UserName, UserPassword, RoleId) values('admin', '����Ա', '123456', 3);