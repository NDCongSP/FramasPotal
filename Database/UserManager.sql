

drop table AspPermission
go
CREATE TABLE AspPermission
(
	RoleID varchar(25) not null,
	[Description] nvarchar(500),
	[ActionID] varchar(50),
	UserAdd varchar(25) default host_name(),
	[DateAdd] datetime default getdate(),
	PRIMARY KEY (RoleID,[ActionID])
)
go
insert into AspPermission (RoleID,ActionID) values ('Administrator',N'Add')
insert into AspPermission (RoleID,ActionID) values ('Administrator',N'Delete')
insert into AspPermission (RoleID,ActionID) values ('Administrator',N'Edit')
insert into AspPermission (RoleID,ActionID) values ('Administrator',N'Save')
insert into AspPermission (RoleID,ActionID) values ('Administrator',N'Print')
insert into AspPermission (RoleID,ActionID) values ('Administrator',N'Search')
insert into AspPermission (RoleID,ActionID) values ('Administrator',N'Statical')
			 				
insert into AspPermission (RoleID,ActionID) values ('Manager',N'Add')
insert into AspPermission (RoleID,ActionID) values ('Manager',N'Delete')
insert into AspPermission (RoleID,ActionID) values ('Manager',N'Edit')
insert into AspPermission (RoleID,ActionID) values ('Manager',N'Save')
insert into AspPermission (RoleID,ActionID) values ('Manager',N'Print')
			 				
insert into AspPermission (RoleID,ActionID) values ('Employee',N'Add')
insert into AspPermission (RoleID,ActionID) values ('Employee',N'Save')
insert into AspPermission (RoleID,ActionID) values ('Employee',N'Print')
insert into AspPermission (RoleID,ActionID) values ('Employee',N'Search')
go

select * from AspPermission


create proc spCheckRoles @UserName nvarchar(128), @Action nvarchar(128)
as
begin
	select * 
	from AspNetUserRoles a
	inner join AspPermission b on a.RoleId = b.RoleID
	where a.UserId = @UserName and b.ActionID = @Action
end