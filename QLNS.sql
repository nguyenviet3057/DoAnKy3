--Delete old database
go
use master
declare @oldDB Nvarchar(max) = 'QLNS'
if exists (select name from sys.databases where name = @oldDB)
begin
	alter database QLNS set single_user with rollback immediate;
	drop database QLNS;
end

--Create database
go
create database QLNS;

--Create tables
go
use QLNS;
create table USERS(
	USER_ID int primary key,
	USER_PSWD varchar(256) not null,
	USER_TOKEN varchar(256) null,
	USER_STATE bit not null
);
create table EMPLOYEE(
	EMP_NUM int primary key,
	EMP_NAME Nvarchar(50) not null,
	EMP_GENDER Nvarchar(3) not null,
	EMP_DOB date not null,
	EMP_ADDRESS Nvarchar(100) not null,
	EMP_ID varchar(20) not null,
	EMP_PLACE_OG Nvarchar(20) null,
	EMP_PHONE varchar(10) null,
	EMP_EMAIL varchar(30) not null,
	EMP_REL Nvarchar(20) null,
	EMP_NAT varchar(20) not null default 'Vietnamese',
	EMP_POS Nvarchar(20) not null,
	DEPT_CODE varchar(5) not null
);
create table PROJECT(
	PROJ_CODE varchar(5) primary key,
	PROJ_NAME Nvarchar(30) not null,
	EMP_NUM int not null,
	PROJ_DESC Nvarchar(200) null
);
create table PROJECT_TASK(
	PROJTSK_NUM int,
	EMP_NUM int,
	primary key (PROJTSK_NUM, EMP_NUM),
	PROJTSK_PROG float not null default 0,
	PROJTSK_DOC Nvarchar(100) null,
	PROJTSK_IMG Nvarchar(100) null,
	PROJTSK_DESC Nvarchar(200) null,
	PROJ_CODE varchar(5) not null
);
create table CONTRACT(
	CONTR_NUM int primary key,
	EMP_NUM int not null,
	CONTR_SIGN_DATE date not null default getdate(),
	CONTR_END_DATE date not null default dateadd(year, 4, getdate()),
	CONTR_SAL_BASE money null
);
create table SALARY(
	CONTR_NUM int,
	SAL_DATE date default getdate(),
	primary key (CONTR_NUM, SAL_DATE),
	SAL_INC money null,
	SAL_BONUS money null,
	SAL_ALLW money null
);
create table DEGREE(
	DEG_CODE varchar(5) primary key,
	DEG_NAME Nvarchar(50) not null,
	DEG_DESC Nvarchar(100) null
);
create table DEGREE_CONFIRM(
	DEG_CODE varchar(5),
	EMP_NUM int,
	primary key (DEG_CODE, EMP_NUM)
);
create table DEPARTMENT(
	DEPT_CODE varchar(5) primary key,
	DEPT_NAME Nvarchar(20) not null,
	EMP_NUM int not null,
	DEPT_ADDRESS Nvarchar(50) null,
	DEPT_DESC Nvarchar(200) null
);
create table NOTIFICATION(
	NOTIF_NUM int identity primary key,
	EMP_NUM int not null,
	NOTIF_DATE date not null default getdate(),
	NOTIF_DETAIL Nvarchar(100) not null
);
create table TIME_KEEP(
	TIME_CODE varchar(20) not null primary key,
	EMP_NUM int not null,
	TIME_DATE date not null default getdate(),
	TIME_CLK_IN time null default convert(time, getdate()),
	TIME_CLK_OUT time null default convert(time, getdate()),
	TIME_ABST real not null default -1
);
create table FEEDBACK(
	FDBK_NUM int identity primary key,
	FDBK_DATE date not null default getdate(),
	EMP_NUM int not null,
	FDBK_DESC Nvarchar(200) null,
	FDBK_SCALE float not null
);
create table EVALUATION(
	EVAL_NUM int,
	EMP_NUM int,
	PROJ_CODE varchar(5)
	primary key (EVAL_NUM, EMP_NUM, PROJ_CODE),
	EVAL_HRDWRK int not null,
	EVAL_FRDLY int not null,
	EVAL_CRTV int not null,
	EVAL_CMT Nvarchar(200) null
);

--Insert data
go
insert into USERS
values
(10001, 12345, null, 1),
(10002, 12345, null, 1),
(10003, 12345, null, 1),
(10004, 12345, null, 1)

insert into EMPLOYEE
values
(10001, N'Phạm Hoàng Dương', N'Nam', '2003-03-14', N'Lmao', '1234567890', N'Hà Nội', '0983581617', 'duong0185266@huce.edu.vn', N'Phật giáo', default, 'ADMIN', 'TECH'),
(10002, N'Nguyễn Quốc Việt', N'Nam', '2003-03-14', N'Lmao', '1234567890', N'Hà Nội', '0983581617', 'duong0185266@huce.edu.vn', null, default, 'ADMIN', 'TECH'),
(10003, N'Trần Quang Nam', N'Nam', '2003-03-14', N'Lmao', '1234567890', N'Hà Nội', '0983581617', 'duong0185266@huce.edu.vn', null, default, 'ADMIN', 'TECH'),
(10004, N'Nguyễn Hoàng Dương', N'Nam', '2003-03-14', N'Lmao', '1234567890', N'Hà Nội', '0983581617', 'duong0185266@huce.edu.vn', null, default, 'ADMIN', 'TECH')

insert into PROJECT
values
('TEST', N'Đồ án môn học', 10001, N'Đồ án môn học của học kỳ 1 năm học 2023-2024')

insert into PROJECT_TASK
values
(1, 10001, default, null, null, N'Làm đồ án', 'TEST'),
(1, 10002, default, null, null, N'Làm đồ án', 'TEST'),
(1, 10003, default, null, null, N'Làm đồ án', 'TEST'),
(1, 10004, default, null, null, N'Làm đồ án', 'TEST')

insert into CONTRACT
values
(10001, 10001, default, default, 10000000),
(10002, 10002, default, default, 10000000),
(10003, 10003, default, default, 10000000),
(10004, 10004, default, default, 10000000)

insert into SALARY
values
(10001, default, null, null, null),
(10002, default, null, null, null),
(10003, default, null, null, null),
(10004, default, null, null, null)

insert into DEGREE
values
('TS', N'bằng tiến sĩ', null),
('KS', N'Kỹ sư', null)

insert into DEGREE_CONFIRM
values
('TS', 10001),
('TS', 10002),
('TS', 10003),
('TS', 10004)

insert into DEPARTMENT
values
('TECH', N'Kỹ thuật', 10001, N'Ở nhà', N'Bọn siêu nhân có mọi chức năng thao tác với hệ thống')

insert into NOTIFICATION
values
(10001, default, N'Đây là thông báo 1'),
(10001, default, N'Đây là thông báo 2')

insert into TIME_KEEP
values
('231128-10001', 10001, default, default, default, 1),
('231128-10002', 10002, default, default, default, 1),
('231128-10003', 10003, default, default, default, 1),
('231128-10004', 10004, default, default, default, 1)

insert into FEEDBACK
values
(default, 10001, N'Đáng khen', 2),
(default, 10002, N'Đáng khen', 2),
(default, 10003, N'Đáng khen', 2),
(default, 10004, N'Đáng khen', 2)

insert into EVALUATION
values
(10001, 10001, 'TEST', 100, 100, 100, N'Giỏi'),
(10001, 10002, 'TEST', 100, 100, 100, N'Giỏi'),
(10001, 10003, 'TEST', 100, 100, 100, N'Giỏi'),
(10001, 10004, 'TEST', 100, 100, 100, N'Giỏi')

--Alter tables, add constraints
alter table USERS
add constraint USER_FK_EMP foreign key (USER_ID) references EMPLOYEE (EMP_NUM) on update cascade;

alter table EMPLOYEE
add constraint EMP_FK_DEPT foreign key (DEPT_CODE) references DEPARTMENT (DEPT_CODE) on update no action on delete no action;

alter table PROJECT_TASK
add constraint PROJTSK_FK_EMP foreign key (EMP_NUM) references EMPLOYEE (EMP_NUM) on update cascade;
alter table PROJECT_TASK
add constraint PROJTSK_FK_PROJ foreign key (PROJ_CODE) references PROJECT (PROJ_CODE) on update cascade;

alter table EVALUATION
add constraint EVAL_FK_EMP_1 foreign key (EVAL_NUM) references EMPLOYEE (EMP_NUM) on update no action on delete no action;
alter table EVALUATION
add constraint EVAL_FK_EMP_2 foreign key (EMP_NUM) references EMPLOYEE (EMP_NUM) on update no action on delete no action;
alter table EVALUATION
add constraint EVAL_FK_PROJ foreign key (PROJ_CODE) references PROJECT (PROJ_CODE) on update cascade;

alter table FEEDBACK
add constraint FDBK_FK_EMP foreign key (EMP_NUM) references EMPLOYEE (EMP_NUM) on update cascade;

alter table TIME_KEEP
add constraint TIME_FK_EMP foreign key (EMP_NUM) references EMPLOYEE (EMP_NUM) on update cascade;

alter table NOTIFICATION
add constraint NOTIF_FK_EMP foreign key (EMP_NUM) references EMPLOYEE (EMP_NUM) on update cascade;

alter table DEPARTMENT
add constraint DEPT_FK_EMP foreign key (EMP_NUM) references EMPLOYEE (EMP_NUM) on update no action on delete no action;

alter table DEGREE_CONFIRM
add constraint DEGCON_FK_EMP foreign key (EMP_NUM) references EMPLOYEE (EMP_NUM) on update cascade;
alter table DEGREE_CONFIRM
add constraint DEGCON_FK_DEG foreign key (DEG_CODE) references DEGREE (DEG_CODE) on update cascade;

alter table CONTRACT
add constraint CONTR_FK_EMP foreign key (EMP_NUM) references EMPLOYEE (EMP_NUM) on update cascade;

alter table SALARY
add constraint SAL_FK_CONTR foreign key (CONTR_NUM) references CONTRACT (CONTR_NUM) on update cascade;

alter table PROJECT
add constraint PROJ_FK_EMP foreign key (EMP_NUM) references EMPLOYEE (EMP_NUM) on update no action on delete no action;