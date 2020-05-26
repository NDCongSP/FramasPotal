
create database MealControl
go

use MealControl
go

MealControl

drop table MaterialType
go
CREATE TABLE MaterialType
(
	ID int identity(1,1) not null,
	Code varchar(25) not null,
	Name nvarchar(100) not null,
	[Description] nvarchar(500),	
	TableName varchar(50) default 'MaterialType',
	[Status] int DEFAULT 1,
	PCIDAdd varchar(25) default host_name(),
	[DateAdd] datetime default getdate(),
	PCIDUpd varchar(25),
	DateUpd datetime,
	PRIMARY KEY (ID)
)
go
insert into MaterialType (Code, Name, [Description]) values ('TrangMieng', N'Tráng miệng',N'Tráng miệng')
insert into MaterialType (Code, Name, [Description]) values ('ThucAn', N'Thức ăn',N'Thức ăn')
insert into MaterialType (Code, Name, [Description]) values ('Com', N'Cơm',N'Cơm')
insert into MaterialType (Code, Name, [Description]) values ('Rau', N'Rau',N'Rau')
insert into MaterialType (Code, Name, [Description]) values ('Canh', N'Canh',N'Canh')

drop table Material
go
CREATE TABLE Material
(
	ID int identity(1,1) not null,
	Code varchar(25) not null,
	Name nvarchar(100) not null,
	[Description] nvarchar(500),
	MaterialTypeID int not null,
	Price decimal(15,4),
	TableName varchar(50) default 'Material',
	[Image] varchar(100),
	[Status] int DEFAULT 1,
	PCIDAdd varchar(25) default host_name(),
	[DateAdd] datetime default getdate(),
	PCIDUpd varchar(25),
	DateUpd datetime,
	PRIMARY KEY (ID)
)
go
insert into Material (Code, Name, [Description], MaterialTypeID, [Image]) values ('Ml1', N'Cơm trắng',N'Cơm trắng',3,'/Content/adminlte/dist/img/RiceChicken.jpg')
insert into Material (Code, Name, [Description], MaterialTypeID, [Image]) values ('Ml2', N'Canh cua',N'Canh cua',5,'/Content/adminlte/dist/img/RiceChicken.jpg')
insert into Material (Code, Name, [Description], MaterialTypeID, [Image]) values ('Ml3', N'Rau xào',N'Rau xào',4,'/Content/adminlte/dist/img/RiceChicken.jpg')
insert into Material (Code, Name, [Description], MaterialTypeID, [Image]) values ('Ml4', N'Chuối',N'Chuối',1,'/Content/adminlte/dist/img/RiceChicken.jpg')
insert into Material (Code, Name, [Description], MaterialTypeID, [Image]) values ('Ml5', N'Sữa chua',N'Sữa chua',1,'/Content/adminlte/dist/img/RiceChicken.jpg')
insert into Material (Code, Name, [Description], MaterialTypeID, [Image]) values ('Ml6', N'Thịt gà',N'Thịt gà',2,'/Content/adminlte/dist/img/RiceChicken.jpg')
insert into Material (Code, Name, [Description], MaterialTypeID, [Image]) values ('Ml7', N'Thịt vịt',N'Thịt vịt',2,'/Content/adminlte/dist/img/RiceChicken.jpg')
insert into Material (Code, Name, [Description], MaterialTypeID, [Image]) values ('Ml8', N'Cá chiên',N'Cá chiên',2,'/Content/adminlte/dist/img/RiceChicken.jpg')
insert into Material (Code, Name, [Description], MaterialTypeID, [Image]) values ('Ml9', N'Thịt heo rang',N'Thịt heo rang',2,'/Content/adminlte/dist/img/RiceChicken.jpg')
insert into Material (Code, Name, [Description], MaterialTypeID, [Image]) values ('Ml10', N'Tôm rang',N'Tôm rang',2,'/Content/adminlte/dist/img/RiceChicken.jpg')
insert into Material (Code, Name, [Description], MaterialTypeID, [Image]) values ('Ml11', N'Trứng chiên',N'Trứng chiên',2,'/Content/adminlte/dist/img/RiceChicken.jpg')
insert into Material (Code, Name, [Description], MaterialTypeID, [Image]) values ('Ml12', N'Đậu hũ',N'Đậu hũ',2,'/Content/adminlte/dist/img/RiceChicken.jpg')
insert into Material (Code, Name, [Description], MaterialTypeID, [Image]) values ('Ml13', N'Thịt bò',N'Thịt bò',2,'/Content/adminlte/dist/img/RiceChicken.jpg')




drop table MenuMeal
go
CREATE TABLE MenuMeal
(
	ID int not null,
	Code varchar(25) not null,
	Name nvarchar(100) not null,
	[Description] nvarchar(500),
	Price decimal(15,4),
	TableName varchar(50) default 'MenuMeal',
	[Image] varchar(100),
	[Status] int DEFAULT 1,
	PCIDAdd varchar(25) default host_name(),
	UserAdd varchar(25),
	[DateAdd] datetime default getdate(),
	PCIDUpd varchar(25),
	UserUpd varchar(25),
	DateUpd datetime,
	PRIMARY KEY (ID)
)
go

insert into MenuMeal (ID, Code, Name, [Description], [Image]) values (1, 'Item1', N'Cơm gà',N'Cơm thịt gà','/Content/adminlte/dist/img/RiceChicken.jpg')
insert into MenuMeal (ID, Code, Name, [Description], [Image]) values (2, 'Item2', N'Cơm bò',N'Cơm thịt bò','/Content/adminlte/dist/img/ComBo.jpg')
insert into MenuMeal (ID, Code, Name, [Description], [Image]) values (3, 'Item3', N'Cơm cá',N'Cơm cá chiên','/Content/adminlte/dist/img/ComCa.jpg')
insert into MenuMeal (ID, Code, Name, [Description], [Image]) values (4, 'Item4', N'Cơm trứng',N'Cơm Trứng chiên','/Content/adminlte/dist/img/ComTrung.jpg')
insert into MenuMeal (ID, Code, Name, [Description], [Image]) values (5, 'Item5', N'Cơm heo rang',N'Cơm heo rang','/Content/adminlte/dist/img/RiceChicken.jpg')

select * from MenuItems where ID = 7
select * from MenuItemsDetail where ID = 6



drop table MenuMealDetail
go
CREATE TABLE MenuMealDetail
(
	ID int not null,
	MaterialID int not null,
	MaterialCode varchar(25) not null,
	MaterialName nvarchar(100) not null,
	Price decimal(15,4),
	TableName varchar(50) default 'MenuMealDetail',
	[Status] int DEFAULT 1,
	PRIMARY KEY (ID,MaterialID)
)
go

insert into MenuMealDetail (ID, MaterialID, MaterialCode, MaterialName) values (1, 1,'Ml1',N'Cơm trắng')
insert into MenuMealDetail (ID, MaterialID, MaterialCode, MaterialName) values (1, 2,'Ml2',N'Canh cua')
insert into MenuMealDetail (ID, MaterialID, MaterialCode, MaterialName) values (1, 3,'Ml3',N'Rau xào')
insert into MenuMealDetail (ID, MaterialID, MaterialCode, MaterialName) values (1, 6,'Ml6',N'Thịt gà')
insert into MenuMealDetail (ID, MaterialID, MaterialCode, MaterialName) values (1, 10,'Ml10',N'Tôm rang')
insert into MenuMealDetail (ID, MaterialID, MaterialCode, MaterialName) values (1, 5,'Ml5',N'Sữa chua')
			
insert into MenuMealDetail (ID, MaterialID, MaterialCode, MaterialName) values (2, 1,'Ml1',N'Cơm trắng')
insert into MenuMealDetail (ID, MaterialID, MaterialCode, MaterialName) values (2, 2,'Ml2',N'Canh cua')
insert into MenuMealDetail (ID, MaterialID, MaterialCode, MaterialName) values (2, 3,'Ml3',N'Rau xào')
insert into MenuMealDetail (ID, MaterialID, MaterialCode, MaterialName) values (2, 13,'Ml13',N'Thịt bò')
insert into MenuMealDetail (ID, MaterialID, MaterialCode, MaterialName) values (2, 10,'Ml10',N'Tôm rang')
insert into MenuMealDetail (ID, MaterialID, MaterialCode, MaterialName) values (2, 5,'Ml5',N'Sữa chua')
			
insert into MenuMealDetail (ID, MaterialID, MaterialCode, MaterialName) values (3, 1,'Ml1',N'Cơm trắng')
insert into MenuMealDetail (ID, MaterialID, MaterialCode, MaterialName) values (3, 2,'Ml2',N'Canh cua')
insert into MenuMealDetail (ID, MaterialID, MaterialCode, MaterialName) values (3, 3,'Ml3',N'Rau xào')
insert into MenuMealDetail (ID, MaterialID, MaterialCode, MaterialName) values (3, 8,'Ml8',N'Cá chiên')
insert into MenuMealDetail (ID, MaterialID, MaterialCode, MaterialName) values (3, 10,'Ml10',N'Tôm rang')
insert into MenuMealDetail (ID, MaterialID, MaterialCode, MaterialName) values (3, 4,'Ml4',N'Chuối')
			
insert into MenuMealDetail (ID, MaterialID, MaterialCode, MaterialName) values (4, 1,'Ml1',N'Cơm trắng')
insert into MenuMealDetail (ID, MaterialID, MaterialCode, MaterialName) values (4, 2,'Ml2',N'Canh cua')
insert into MenuMealDetail (ID, MaterialID, MaterialCode, MaterialName) values (4, 3,'Ml3',N'Rau xào')
insert into MenuMealDetail (ID, MaterialID, MaterialCode, MaterialName) values (4, 8,'Ml8',N'Cá chiên')
insert into MenuMealDetail (ID, MaterialID, MaterialCode, MaterialName) values (4, 11,'Ml11',N'Trứng chiên')
insert into MenuMealDetail (ID, MaterialID, MaterialCode, MaterialName) values (4, 5,'Ml5',N'Sữa chua')
			
insert into MenuMealDetail (ID, MaterialID, MaterialCode, MaterialName) values (5, 1,'Ml1',N'Cơm trắng')
insert into MenuMealDetail (ID, MaterialID, MaterialCode, MaterialName) values (5, 2,'Ml2',N'Canh cua')
insert into MenuMealDetail (ID, MaterialID, MaterialCode, MaterialName) values (5, 3,'Ml3',N'Rau xào')
insert into MenuMealDetail (ID, MaterialID, MaterialCode, MaterialName) values (5, 9,'Ml9',N'Thịt heo rang')
insert into MenuMealDetail (ID, MaterialID, MaterialCode, MaterialName) values (5, 11,'Ml11',N'Trứng chiên')
insert into MenuMealDetail (ID, MaterialID, MaterialCode, MaterialName) values (5, 4,'Ml4',N'Chuối')




drop table ShiftWork
go
CREATE TABLE ShiftWork
(
	ID int identity(1,1) not null,
	Code varchar(25) not null,
	Name nvarchar(100) not null,
	[Description] varchar(500),
	TableName varchar(50) default 'ShiftWork',
	[Status] int DEFAULT 1,
	UserAdd varchar(25) default host_name(),
	[DateAdd] datetime default getdate(),
	UserUpd varchar(25) default host_name(),
	[DateUpd] datetime default getdate(),
	PRIMARY KEY (ID)
)
go


insert into ShiftWork (Code, Name, [Description], UserUpd, DateUpd) values ('Shift_HC',N'Ca hành chính',N'Làm ca hành chính',null,null)
insert into ShiftWork (Code, Name, [Description], UserUpd, DateUpd) values ('Shift_HC_OT',N'Ca hành chính OT',N'Làm ca hành chính OT',null,null)
insert into ShiftWork (Code, Name, [Description], UserUpd, DateUpd) values ('Shift_1',N'Ca 1',N'Làm ca 1',null,null)
insert into ShiftWork (Code, Name, [Description], UserUpd, DateUpd) values ('Shift_2',N'Ca 2',N'Làm ca 2',null,null)
insert into ShiftWork (Code, Name, [Description], UserUpd, DateUpd) values ('Shift_3',N'Ca 3',N'Làm ca 3',null,null)
go

select * from ShiftWork
select * from MenuMeal
go


drop table MealSchedule
go
CREATE TABLE MealSchedule
(
	[Day] varchar(8) not null,
	ShiftWorkID int not null,
	ShiftWorkCode varchar(25) not null,
	MenuMealID int not null,
	MenuMealCode varchar(25) not null,
	[Description] varchar(500),
	TableName varchar(50) default 'MealSchedule',
	[Status] int DEFAULT 1,
	UserAdd varchar(25) default host_name(),
	[DateAdd] datetime default getdate(),
	UserUpd varchar(25) default host_name(),
	[DateUpd] datetime default getdate(),
	PRIMARY KEY ([Day],ShiftWorkID,MenuMealID)
)
go

update MealSchedule set [Day] = '20190426'

insert into MealSchedule ([Day],ShiftWorkID,ShiftWorkCode,MenuMealID,MenuMealCode,UserUpd,DateUpd) values ('20190422','1','Shift_HC','1','Item1',null,null)
insert into MealSchedule ([Day],ShiftWorkID,ShiftWorkCode,MenuMealID,MenuMealCode,UserUpd,DateUpd) values ('20190422','1','Shift_HC','2','Item2',null,null)
insert into MealSchedule ([Day],ShiftWorkID,ShiftWorkCode,MenuMealID,MenuMealCode,UserUpd,DateUpd) values ('20190422','2','Shift_HC_OT','2','Item2',null,null)
insert into MealSchedule ([Day],ShiftWorkID,ShiftWorkCode,MenuMealID,MenuMealCode,UserUpd,DateUpd) values ('20190422','2','Shift_HC_OT','4','Item4',null,null)
insert into MealSchedule ([Day],ShiftWorkID,ShiftWorkCode,MenuMealID,MenuMealCode,UserUpd,DateUpd) values ('20190422','3','Shift_1','1','Item1',null,null)
insert into MealSchedule ([Day],ShiftWorkID,ShiftWorkCode,MenuMealID,MenuMealCode,UserUpd,DateUpd) values ('20190422','3','Shift_1','3','Item3',null,null)
insert into MealSchedule ([Day],ShiftWorkID,ShiftWorkCode,MenuMealID,MenuMealCode,UserUpd,DateUpd) values ('20190422','4','Shift_2','2','Item2',null,null)
insert into MealSchedule ([Day],ShiftWorkID,ShiftWorkCode,MenuMealID,MenuMealCode,UserUpd,DateUpd) values ('20190422','4','Shift_2','4','Item4',null,null)
insert into MealSchedule ([Day],ShiftWorkID,ShiftWorkCode,MenuMealID,MenuMealCode,UserUpd,DateUpd) values ('20190422','5','Shift_3','5','Item5',null,null)
insert into MealSchedule ([Day],ShiftWorkID,ShiftWorkCode,MenuMealID,MenuMealCode,UserUpd,DateUpd) values ('20190422','5','Shift_3','6','Item6',null,null)



select * from MealSchedule




exec sp_MealSchedule_GetByDay '20190427', '20190427'

alter proc sp_MealSchedule_GetByDay @from varchar(8), @to varchar(8)
as
begin
	
	SET NOCOUNT ON; 
	select a.Day, a.DateAdd, a.ShiftWorkID, a.ShiftWorkCode, c.Name ShiftWorkName, a.MenuMealID, a.MenuMealCode, b.Name MenuMealName
	from MealSchedule a
	inner join MenuMeal b on a.MenuMealID = b.ID
	inner join ShiftWork c on a.ShiftWorkID = c.ID
	where [Day] >= @from and [Day] <= @to
		
end





alter proc sp_GetMealSchedule @ShiftWork int, @Date date
as
begin
	
	SET NOCOUNT ON; 
	select b.ID, b.Code, b.Name, b.Description, b.Price, b.TableName, b.Image, 
	b.Status, b.PCIDAdd, b.UserAdd, b.DateAdd, b.PCIDUpd, b.UserUpd, b.DateUpd
	from MealSchedule a
	inner join MenuMeal b on a.MenuMealID = b.ID
	where [Day] = format(@Date,'yyyyMMdd') and a.ShiftWorkID = @ShiftWork
	
	select b.ID, b.MaterialID, b.MaterialCode, b.MaterialName, b.Price, b.TableName, b.Status
	from MealSchedule a
	inner join MenuMealDetail b on a.MenuMealID = b.ID
	where [Day] = format(@Date,'yyyyMMdd') and a.ShiftWorkID = @ShiftWork

end


exec sp_GetMealSchedule 2,'2019-04-24'



alter proc sp_MealSchedule_Parent @ShiftWork int, @Date date
as
begin
	
	SET NOCOUNT ON; 
	select b.ID, b.Code, b.Name, b.Description, b.Price, b.TableName, b.Image, 
	b.Status, b.PCIDAdd, b.UserAdd, b.DateAdd, b.PCIDUpd, b.UserUpd, b.DateUpd
	from MealSchedule a
	inner join MenuMeal b on a.MenuMealID = b.ID
	where [Day] = format(@Date,'yyyyMMdd') and a.ShiftWorkID = @ShiftWork

end

alter proc sp_MealSchedule_Child @ShiftWork int, @Date date
as
begin
	
	SET NOCOUNT ON; 
	select b.ID, b.MaterialID, b.MaterialCode, b.MaterialName, b.Price, b.TableName, b.Status
	from MealSchedule a
	inner join MenuMealDetail b on a.MenuMealID = b.ID
	where [Day] = format(@Date,'yyyyMMdd') and a.ShiftWorkID = @ShiftWork

end




select * from OrderMeal



drop table TimeLimitOrder
go
CREATE TABLE TimeLimitOrder
(
	ID int identity(1,1),
	ShiftWorkID int not null,
	ShiftWorkCode varchar(25) not null,
	TimeLimit time not null,
	IsOffice bit,
	
	BeginTime time,
	EndTime time,
	UserAdd varchar(25) default host_name(),
	[DateAdd] datetime default getdate(),
	UserUpd varchar(25) default host_name(),
	DateUpd datetime default getdate(),
	PRIMARY KEY (ShiftWorkID)
)
go
insert into TimeLimitOrder (ShiftWorkID, ShiftWorkCode, TimeLimit, UserAdd, [DateAdd]) values (1, 'Shift_H',getdate(),host_name(),getdate())
insert into TimeLimitOrder (ShiftWorkID, ShiftWorkCode, TimeLimit, UserAdd, [DateAdd]) values (2, 'Shift_H',getdate(),host_name(),getdate())
insert into TimeLimitOrder (ShiftWorkID, ShiftWorkCode, TimeLimit, UserAdd, [DateAdd]) values (3, 'Shift_1',getdate(),host_name(),getdate())
insert into TimeLimitOrder (ShiftWorkID, ShiftWorkCode, TimeLimit, UserAdd, [DateAdd]) values (4, 'Shift_2',getdate(),host_name(),getdate())
insert into TimeLimitOrder (ShiftWorkID, ShiftWorkCode, TimeLimit, UserAdd, [DateAdd]) values (5, 'Shift_3',getdate(),host_name(),getdate())
go

select * from TimeLimitOrder



drop table OrderMeal
go
CREATE TABLE OrderMeal
(
	ID int identity(1,1),
	[Day] varchar(8) not null,
	ShiftWorkID int not null,
	ShiftWorkCode varchar(25) not null,
	UserName nvarchar(256) not null,
	MenuMealID int not null,
	MenuMealCode varchar(25) not null,

	IsTakeMeal bit,
	UserAdd varchar(25) default host_name(),
	[DateAdd] datetime default getdate(),
	UserUpd varchar(25) default host_name(),
	DateUpd datetime default getdate(),
	PRIMARY KEY ([Day],ShiftWorkID,UserName)
)
go


alter proc sp_OrderMeal_GetDay @day varchar(8)
as
begin
	SET NOCOUNT ON; 
	select a.ID, a.[Day], a.ShiftWorkID, a.ShiftWorkCode, b.Name ShiftWorkName, a.UserName, a.MenuMealID, a.MenuMealCode, c.Name MenuMealName
	from OrderMeal a
	inner join ShiftWork b on a.ShiftWorkID = b.ID
	inner join MenuMeal c on a.MenuMealID = c.ID
	where [Day] = @day
end

exec sp_OrderMeal_GetDay '20190424'




select * from Material
select * from MaterialType



MaterialTypeID



drop table Feedback
create table Feedback
(
	ID int identity(1,1),
	UserName nvarchar(256),
	TextContain nvarchar(max),
	[Status] int DEFAULT 1,
	UserAdd varchar(25) default host_name(),
	[DateAdd] datetime default getdate()
	PRIMARY KEY (ID)
)

drop table Feedback_Reply
create table Feedback_Reply
(
	ID int identity(1,1),
	ConversationsID int,
	UserName nvarchar(256),
	TextContain nvarchar(max),
	[Status] int DEFAULT 1,
	UserAdd varchar(25) default host_name(),
	[DateAdd] datetime default getdate()
	PRIMARY KEY (ID)
)



drop table MealDelivery
create table MealDelivery
(
	ID int identity(1,1), 
	UserName int not null,
	[DateAdd] datetime not null,
	MachineNumberID int null,
	ShiftWorkID int null,
	MenuMealID int null,
	PRIMARY KEY ([DateAdd])
)






