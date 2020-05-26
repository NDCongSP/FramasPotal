
create database FingerScan
use FingerScan

drop table Products
create table Products(
id int IDENTITY(1,1) NOT NULL,
Name nvarchar(25),
PricDecimal decimal,
QuantDecimal decimal
)



alter database MealControl set enable_broker




select * from tbl_LogFinger



alter database FingerScan set enable_broker with rollback immediate

select is_broker_enabled, * from sys.databases



CREATE TABLE dbo.tbl_Notifications(
	NotificaionId int,
	Status nvarchar(50),
	Message nvarchar(50),
	ExtraColumn nvarchar(50)
)
GO

SELECT [NotificaionId],[Status],[Message],[ExtraColumn] FROM [dbo].[tbl_Notifications]

SELECT [NotificaionId],[Status],[Message],[ExtraColumn] FROM [dbo].[tbl_Notifications]

select * from MealControl.dbo.MealDelivery b where cast(b.DateAdd as date) >= dateadd(day,-1, cast(getdate() as date)) and cast(b.DateAdd as date) <= cast(getdate() as date) 

select * from tbl_LogFinger a where format(DateTimeRecord,'yyyyMMdd') = FORMAT(GETDATE(),'yyyyMMdd') and 
cast(b.DateAdd as date) >= dateadd(day,-1, cast(getdate() as date)) and cast(b.DateAdd as date) <= cast(getdate() as date) 

ALTER proc [dbo].[sp_DeliveryMealShow]
as
begin
	SET NOCOUNT ON; 
	-- table finger scan
	WITH FingerScan_CTE 
	AS  
	-- Define the CTE query.  
	(   
		SELECT a.ID,a.MachineNumber,a.IndRegID, 
		case when isnull(b.UserName,'') = '' then cast(0 as bit) else  cast(1 as bit) end isCheck
		FROM dbo.[tbl_LogFinger] a
		left join 
		(
			select * from MealControl.dbo.MealDelivery  where cast(DateAdd as date) >= dateadd(day,-1, cast(getdate() as date)) and cast(DateAdd as date) <= cast(getdate() as date) 
		) b on a.DateTimeRecord = b.DateAdd and a.IndRegID = b.UserName
		where format(DateTimeRecord,'yyyyMMdd') = FORMAT(GETDATE(),'yyyyMMdd') 
	),
	-- table order meal
	OrderMeal_CTE 
	AS  
	-- Define the CTE query.  
	(   
		--get data shift 1,2, office, OT
		SELECT a.Day, a.MenuMealID, b.Name MenuMealName, a.ShiftWorkID, c.Name ShiftWorkName, a.IsTakeMeal, CONVERT(INT,a.UserName) UserName, d.HC
		from MealControl.dbo.OrderMeal a
		inner join MealControl.dbo.MenuMeal b on a.MenuMealID = b.ID
		inner join MealControl.dbo.ShiftWork c on a.ShiftWorkID = c.ID
		inner join HumanResource.dbo.tblHoSoLyLichNhanVien d on CONVERT(INT,a.UserName) = d.MaSoNhanVien
		inner join MealControl.dbo.TimeLimitOrder e on a.ShiftWorkID = e.ShiftWorkID and d.HC = e.IsOffice
		where a.ShiftWorkID <> 5 and a.[Day] = FORMAT(GETDATE(),'yyyyMMdd') and e.BeginTime <= CONVERT(TIME(0),GETDATE()) and e.EndTime >= CONVERT(TIME(0),GETDATE())
		union all
		--get data shift 3
		SELECT a.Day, a.MenuMealID, b.Name MenuMealName, a.ShiftWorkID, c.Name ShiftWorkName, a.IsTakeMeal, CONVERT(INT,a.UserName) UserName, d.HC
		from MealControl.dbo.OrderMeal a
		inner join MealControl.dbo.MenuMeal b on a.MenuMealID = b.ID
		inner join MealControl.dbo.ShiftWork c on a.ShiftWorkID = c.ID
		inner join HumanResource.dbo.tblHoSoLyLichNhanVien d on CONVERT(INT,a.UserName) = d.MaSoNhanVien
		inner join MealControl.dbo.TimeLimitOrder e on a.ShiftWorkID = e.ShiftWorkID and d.HC = e.IsOffice
		where a.ShiftWorkID = 5 and a.[Day] >= FORMAT(dateadd(day,-1, cast(getdate() as date)),'yyyyMMdd') 
		and a.[Day] <= FORMAT(GETDATE(),'yyyyMMdd') 
		and ( (e.BeginTime <= CONVERT(TIME(0),GETDATE()) and e.EndTime >= '23:59:59') or (e.BeginTime <= '00:00:00' and e.EndTime >= CONVERT(TIME(0),GETDATE())) )
	)
	--Select data
	select a.ID,a.MachineNumber,a.IndRegID,b.HoNhanVien + b.TenNhanVien TenNhanVien,a.isCheck,
	case 
		when isnull(c.MenuMealName,'') = '' then N'Chưa đăng ký xuất ăn'
		when isnull(c.MenuMealName,'') <> '' and isCheck = 0 then N'Đã lấy xuất ăn'
		else 'OK'
	end MsgCheck,
	isnull(c.MenuMealID,0) MenuMealID, isnull(c.MenuMealName,N'False') MenuMealName, 
	isnull(c.ShiftWorkID,0) ShiftWorkID, isnull(c.ShiftWorkName,N'False') ShiftWorkName, isnull(c.IsTakeMeal,0) IsTakeMeal
	from FingerScan_CTE a
	inner join HumanResource.dbo.tblDanhSachNhanVien b on a.IndRegID = b.MaSoNhanVien	
	left join OrderMeal_CTE c on a.IndRegID = c.UserName
	ORDER BY a.ID desc
				
end

select * from MealControl.[dbo].OrderMeal

select * from HumanResource.dbo.tblDanhSachNhanVien where MaSoNhanVien = 6978

EXEC sp_DeliveryMealShow


select is_broker_enabled, * from sys.databases

update tbl_LogFinger set IndRegID = 1 where ID = 21


select * from tbl_LogFinger where format(DateTimeRecord,'yyyyMMdd') = format(getdate(),'yyyyMMdd')



SELECT ID, IndRegID FROM [dbo].[tbl_LogFinger] where format(DateTimeRecord,'yyyyMMdd') = format(getdate(),'yyyyMMdd')







--drop proc sp_MealDelivery_Add


select * from MealControl.dbo.MealDelivery a		 

exec sp_MealDelivery_Add 195,6979,'2019-05-10 09:46:34.000'

select * from MealControl.dbo.MealDelivery a		 


