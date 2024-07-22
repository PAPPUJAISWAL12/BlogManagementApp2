create database BlogManagementSystem
--drop database BolgManagementSystem
use BlogManagementSystem

create table UserList(
UserId smallint primary key not null,
FullName Nvarchar(50) not null,
EmailAddress Nvarchar(50) not null,
UserPhoto Nvarchar(max) not null,
UserRole Nvarchar(40) not null check(UserRole='Admin' or UserRole='User' or UserRole='Editor'),
UserPassword Nvarchar(max) not null,
CurrentAddress Nvarchar(40) not null,
);

Create Table BlogPost(
BId smallint Primary key,
SectionHedding Nvarchar(max) not null,
SectionImage Nvarchar(max) null,
SectionDescription Nvarchar(max) null,
PostDate Date not null,
UploadUserId smallint foreign key references UserList(UserId) not null,
CancelUserId smallint foreign key references UserList(UserId) null,
CancelDate date  null,
ReasonForCancel Nvarchar(max) null
);

